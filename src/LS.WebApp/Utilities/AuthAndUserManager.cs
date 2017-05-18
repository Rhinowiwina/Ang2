using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using Common.Logging;
using LS.Core;
using LS.Domain;
using LS.Services;
using LS.Utilities;
using LS.WebApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using Exceptionless;
using Exceptionless.Models;
using LS.ApiBindingModels;
namespace LS.WebApp.Utilities {
    public class AuthAndUserManager : IDisposable {

        private static readonly string UserCreationFailedUserMessage = "An error occurred while creating user.";

        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationSignInManager _signInManager;
        private readonly IAuthenticationManager _authentication;
        private readonly ApplicationRoleManager _roleManager;

        private ILog Logger { get; set; }

        private List<ApplicationRole> _cachedRoles;

        private List<ApplicationRole> CachedRoles {
            get { return _cachedRoles ?? (_cachedRoles = _roleManager.Roles.ToList()); }
        }
        protected ServerEnvironmentBindingModel ServerVars {
            get { return GetServerVars(); }
        }
        public AuthAndUserManager(HttpRequestMessage request) {
            if (request == null) {
                throw new ArgumentException("Http Request must exist in order to use AuthAndUserManager", "request");
            }
            _userManager = request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            _signInManager = request.GetOwinContext().Get<ApplicationSignInManager>();
            _roleManager = request.GetOwinContext().Get<ApplicationRoleManager>();
            _authentication = request.GetOwinContext().Authentication;
            Logger = LoggerFactory.GetLogger(GetType());
        }

        public List<ApplicationRole> GetAllRoles() {
            return CachedRoles;
        }

        public ApplicationRole GetRoleWithName(string roleName) {
            return CachedRoles.SingleOrDefault(r => r.Name == roleName);
        }

        public ApplicationRole GetRoleWithId(string roleId) {
            return CachedRoles.SingleOrDefault(r => r.Id == roleId);
        }
        protected ServerEnvironmentBindingModel GetServerVars() {
            var serv = new UtilityDataService();
            var result = serv.GetServerVars();
            if (!result.Result.IsSuccessful) {
                return null;
            }
            return result.Result.Data;
        }

        //TODO: Look in to how this could be accomplished at the same time as the Update user call
        public async Task<ServiceProcessingResult> UpdateUserRoleIfNecessary(string newRoleName, ApplicationRole oldRole, string userId) {
            var processingResult = new ServiceProcessingResult { IsSuccessful = true };
            if (newRoleName == oldRole.Name) {
                return processingResult;
            }

            if (GetRoleWithName(newRoleName) != null && await _userManager.IsInRoleAsync(userId, newRoleName) == false) {
                try {
                    var result = await _userManager.RemoveFromRoleAsync(userId, oldRole.Name);
                    if (!result.Succeeded) {
                        var identityErrors = String.Join("\n", result.Errors);
                        var logMessage = String.Format("Failed to remove user from role. Errors: {0}", identityErrors);
                        Logger.Error(logMessage);

                        processingResult.IsSuccessful = false;
                        processingResult.Error = ErrorValues.USER_ROLE_UPDATE_FAILED_ERROR;
                        return processingResult;
                    }

                    result = await _userManager.AddToRoleAsync(userId, newRoleName);
                    if (!result.Succeeded) {
                        var identityErrors = String.Join("\n", result.Errors);
                        var logMessage = String.Format("Failed to add user to role. Errors: {0}", identityErrors);
                        Logger.Error(logMessage);

                        processingResult.IsSuccessful = false;
                        processingResult.Error = ErrorValues.USER_ROLE_UPDATE_FAILED_ERROR;
                    }
                } catch (Exception ex) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.USER_ROLE_UPDATE_FAILED_ERROR;
                    Logger.Error("Failed to update user role.", ex);
                }
            }

            return processingResult;
        }

        public async Task AddPasswordAndLogin(SetPasswordForUserIdModel model) {
            await _userManager.AddPasswordAsync(model.UserId, model.Password);

            if (!_userManager.IsInRole(model.UserId, "Administrator")) {
                await AddUserToRoleAsync(model.UserId, "Administrator");
            }

            var userService = new ApplicationUserDataService();
            var user = userService.Get(model.UserId).Data;

            await _signInManager.SignInAsync(user, false, false);
        }

        public async Task<bool> UserNameAlreadyExists(string username) {
            return await _userManager.FindByNameAsync(username) != null;
        }


        public async Task<ServiceProcessingResult> CreateUserAndAssignToRoleAsync(ApplicationUser user, string roleName, string companyname) {
            var result = new ServiceProcessingResult();
            result.IsSuccessful = true;

            var password = RandomPasswordGenerator.Generate();

            var userCreate = await _userManager.CreateAsync(user, password);
            if (!userCreate.Succeeded) {
                result.IsSuccessful = false;
                var userJsonObject = JsonConvert.SerializeObject(user);
                Logger.Error(string.Format("Error creating user in user manager. CompanyName: {0} RoleName: {1} User Object: {2}", companyname, roleName, userJsonObject));
                result.Error = new ProcessingError("Error creating user in user manager. Please contact Support", "Error creating user in user manager. Please contact Support", false, false);
                return result;
            }

            var addUserRole = await AddUserToRoleAsync(user.Id, roleName);
            if (!addUserRole.Succeeded) {
                result.IsSuccessful = false;
                result.Error = new ProcessingError("Failed to add user role.", "Failed to add user role.", false, false);
                Logger.Error(string.Format("Failed to add user role {0} after email send failure", user.Id));
            }



            var emailHelper = new EmailHelper();
            var CCAddresses = "";

            if (ServerVars.IsDev) {
                CCAddresses = "randy@305spin.com";

            }


            var emailResult = emailHelper.SendEmail("An account has been created for you.", user.Email, CCAddresses, "You have been registered for access to the Lifeline Services portal " +
                "on behalf of " + companyname + ". To access the portal, please go to <a href='https://arrow.spinlifeserv.com'>SpinLifeServ.com</a> and enter the credentials below.<br/><br/>" +
                "<font size='2'><i>Remember that the password is case sensitive.  Do not share this information with anyone.</i></font> <br/><br/>" +
                "<strong>Username:</strong>" + user.Email + "<br/><strong>Password:</strong>" + password, null);
            if (!emailResult.Result.IsSuccessful) {
                ExceptionlessClient.Default.CreateLog("Create User Email")
               .SetMessage(emailResult.Result.Error.UserHelp)
               .AddObject(user)
                .Submit();
                Logger.Error(string.Format("Failure sending email to {0}.  Inner Exception: " + emailResult.Result.Error.UserHelp, user.Email));
                result.IsSuccessful = false;
                result.Error = emailResult.Result.Error;
                if (!ServerVars.IsDev) {
                    try {
                        try {
                            var rolesForUser = await _userManager.GetRolesAsync(user.Id);
                            if (rolesForUser.Count() > 0) {
                                foreach (var item in rolesForUser.ToList()) {
                                    var roleDeleteResult = await _userManager.RemoveFromRoleAsync(user.Id, item);
                                }
                            }
                        } catch (Exception) {
                            //Log and go ahead and delete the user.
                            Logger.Error(string.Format("Failed to delete user's (id: {0}) roles after email send failure", user.Id));
                        }
                        var deleteResult = await _userManager.DeleteAsync(user);
                        if (!deleteResult.Succeeded) {
                            result.Error = new ProcessingError("The email address entered was not valid, however we were unable to delete the user account.  Email failure: " + emailResult.Result.Error.UserHelp, "The email address entered was not valid, however we were unable to delete the user account.  Email failure: " + emailResult.Result.Error.UserHelp, false, false);
                            Logger.Error(string.Format("Failed to delete user but call succeed (id: {0}) after email send failure", user.Id));
                        } else {
                            result.Error = new ProcessingError("Failed to create user because an email could not be sent to it. Reason: " + emailResult.Result.Error.UserHelp, "Failed to create user because an email could not be sent to it.  Reason: " + emailResult.Result.Error.UserHelp, false, false);
                            result.IsSuccessful = false;
                        }
                    } catch (Exception ex1) {
                        result.IsSuccessful = false;
                        result.Error = new ProcessingError("The email address entered was not valid, however we were unable to delete the user account.  Email failure: " + emailResult.Result.Error.UserHelp, "The email address entered was not valid, however we were unable to delete the user account.  Email failure: " + emailResult.Result.Error.UserHelp, false, false);
                        Logger.Fatal(string.Format("Failed to delete user (id: {0}) after email send failure", user.Id));
                        return result;
                    }

                }//endif not dev


            }//endif email  not succesfull


            return result;
        }

        public async Task<IdentityResult> AddUserToRoleAsync(string userId, string roleName) {
            return await _userManager.AddToRoleAsync(userId, roleName);
        }

        public async Task RefreshAuthenticationCookie(ApplicationUser userToRefresh) {
            _authentication.SignOut();
            await _signInManager.SignInAsync(userToRefresh, false, false);
        }

        public void Dispose() {
            _userManager.Dispose();
            _roleManager.Dispose();
            _signInManager.Dispose();
        }

        internal void RefreshAuthenticationCookie() {
            throw new NotImplementedException();
        }
    }
}
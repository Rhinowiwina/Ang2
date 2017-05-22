using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using LS.ApiBindingModels;
using LS.Core;
using LS.Domain;
using LS.Services;
using System.Collections.Generic;
using LS.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using LS.WebApp.Utilities;
using System.Data.SqlClient;
using System.Data;
using LS.WebApp.Controllers.api;
using ApiBindingModels;
using Exceptionless;
using Exceptionless.Models;
namespace WebApp.Controllers.api {
    [RoutePrefix("webapi/userportal")]
    public class UserPortalController : BaseApiController {
        private static readonly string BasicAuthType = "Basic";
        private static readonly string AuthHeader = "Authorization";

        private bool RequestIsAuthorized() {
            var authHeader = HttpContext.Current.Request.Headers[AuthHeader];
            if (authHeader == null || !authHeader.StartsWith(BasicAuthType)) {
                return false;
            }
            var encodedUsernamePassword = authHeader.Substring((BasicAuthType + " ").Length).Trim();
            var usernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));

            var credsArray = usernamePassword.Split(':');
            var username = credsArray[0];
            var password = credsArray[1];

            var basicAuthIsValid = username == ApplicationConfig.UsersPortalUsername && password == ApplicationConfig.UsersPortalPassword;

            return basicAuthIsValid;
        }

        [HttpGet]
        [Route("getLevel1Groups")]
        public async Task<IHttpActionResult> GetLevel1Groups(string CompanyID) {
           var processingResult = new ServiceProcessingResult<List<UsersPortalLevel1Groups>>{ IsSuccessful = true};

           if (!RequestIsAuthorized()) {
                processingResult.IsSuccessful = false;
               processingResult.Error = new ProcessingError("Not authorized.", "Not authorized.", false);
               return Ok(processingResult);
            }

            var sqlQuery = new SQLQuery();

            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@CompanyID", CompanyID)
            };

            var sqlText = @"
                SELECT Id, Name
                FROM Level1SalesGroup 
                WHERE CompanyID=@CompanyID AND IsDeleted=0
            ";

            var getLevel1Result = await sqlQuery.ExecuteReaderAsync<UsersPortalLevel1Groups>(CommandType.Text, sqlText, parameters);
            if (!getLevel1Result.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error getting level 1 groups for users portal", "Error getting level 1 groups for users portal", false, false);
                return Ok(processingResult);
            }

            processingResult.Data = (List<UsersPortalLevel1Groups>) getLevel1Result.Data;

            return Ok(processingResult);
        }


        [HttpGet]
        [Route("getSalesTeams")]
        public async Task<IHttpActionResult> GetSalesTeams(string Level1ID) {
            var processingResult = new ServiceProcessingResult<List<UsersPortalSalesTeams>> { IsSuccessful = true };

            if (!RequestIsAuthorized()) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Not authorized.", "Not authorized.", false);
                return Ok(processingResult);
            }

            var sqlQuery = new SQLQuery();

            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@Level1ID", Level1ID)
            };

            var sqlText = @"
                SELECT Id, Name, ExternalPrimaryId, ExternalDisplayName
                FROM v_UserTeams
                WHERE Level1Id=@Level1ID AND Level1IsDeleted=0 AND IsActive=1
                GROUP BY Id, Name, ExternalPrimaryId, ExternalDisplayName
            ";

            var getSalesTeamResult = await sqlQuery.ExecuteReaderAsync<UsersPortalSalesTeams>(CommandType.Text, sqlText, parameters);
            if (!getSalesTeamResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error getting sales teams for users portal", "Error getting sales teams for users portal", false, false);
                return Ok(processingResult);
            }

            processingResult.Data = (List<UsersPortalSalesTeams>)getSalesTeamResult.Data;

            return Ok(processingResult);
        }

        [HttpPost]
        [Route("createUser")]
        public async Task<IHttpActionResult> CreateUser(UsersPortalCreateUsersRequest model) {
            var processingResult = new ServiceProcessingResult<string>();

            if (!RequestIsAuthorized()) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Not authorized.", "Not authorized.", false);
                return Ok(processingResult);
            }

            var roleBeingAssigned = AuthAndUserManager.GetRoleWithName("Sales Rep");
            if (roleBeingAssigned == null) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Couldnt find role", "Couldnt find role", true, false);
                return Ok(processingResult);
            }

            var companyDataService = new CompanyDataService();

            var lookupCompany = await companyDataService.GetCompanyAsync(model.CompanyID);
            if (!lookupCompany.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = lookupCompany.Error;
                return Ok(processingResult);
            }

            var userToCreate = new ApplicationUser {
                Id = Guid.NewGuid().ToString(),
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.EmailAddress,
                Role = roleBeingAssigned,
                Email = model.EmailAddress,
                PayPalEmail = model.EmailAddress,
                SalesTeamId = model.SalesTeamID,
                CompanyId = lookupCompany.Data.Id,
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false,
                Language = "en",
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                ExternalUserID = model.ExternalUserID,
                IsExternalUserIDActive = true,
                PermissionsLifelineCA = true,
                PermissionsAccountOrder = true,
                AdditionalDataNeeded = false,
                PermissionsBypassTpiv = true
            };

            try {
                var password = RandomPasswordGenerator.Generate();
                var user = await AppUserManager.CreateAsync(userToCreate, password);
                if (user.Succeeded == false) {
                    var fullError = "";
                    foreach (var error in user.Errors) {
                        if (fullError != "") { fullError += " || "; }
                        fullError += error;
                    }

                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError(fullError, fullError, true);
                    return Ok(processingResult);
                }

                var addRole = await AuthAndUserManager.AddUserToRoleAsync(userToCreate.Id, roleBeingAssigned.Name);
                if (!addRole.Succeeded) {
                    var fullError = "";
                    foreach (var error in user.Errors) {
                        if (fullError != "") { fullError += " || "; }
                        fullError += error;
                    }

                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError(fullError, fullError, true);
                    return Ok(processingResult);
                }

                try {
                    var sendEmail = AppUserManager.SendEmailAsync(userToCreate.Id, "An account has been created for you.", "You have been registered for access to the Lifeline Services portal " +
                        "on behalf of " + lookupCompany.Data.Name + ". To access the portal, please go to <a href='https://arrow.spinlifeserv.com'>arrow.spinlifeserv.com</a> and enter the credentials below.<br/><br/>" +
                        "<font size='2'><i>Remember that the password is case sensitive.  Do not share this information with anyone.</i></font> <br/><br/>" +
                        "<strong>Username:</strong>" + userToCreate.UserName + "<br/><strong>Password:</strong>" + password);
                } catch (Exception ex) {
                    var userErrMsg = ex.Message;
                    if (ex.InnerException != null) {
                        //userErrMsg = ex.InnerException.ToString();
                        ex.ToExceptionless()
                          .SetMessage(string.Format("Failure sending email to {0}.  Inner Exception: " + ex.InnerException.ToString(),userToCreate.Email))
                          .MarkAsCritical()
                          .Submit();
                     
                    }
                    if (System.Configuration.ConfigurationManager.AppSettings["Environment"] != "DEV") {
                        try {
                            try {
                                var rolesForUser = await AppUserManager.GetRolesAsync(userToCreate.Id);
                                if (rolesForUser.Count() > 0) {
                                    foreach (var item in rolesForUser.ToList()) {
                                        var roleDeleteResult = await AppUserManager.RemoveFromRoleAsync(userToCreate.Id, item);
                                    }
                                }
                            } catch (Exception ex1) {
                                //Log and go ahead and delete the user.
                                ex1.ToExceptionless()
                                  .SetMessage(string.Format("Failed to delete user's (id: {0}) roles                        after email send failure", userToCreate.Id))
                                  .Submit();
                            
                            }

                            var deleteResult = await AppUserManager.DeleteAsync(userToCreate);
                            if (!deleteResult.Succeeded) {
                                processingResult.Error = new ProcessingError("The email address entered was not valid, however we were unable to delete the user account.  Email failure: " + userErrMsg, "The email address entered was not valid, however we were unable to delete the user account.  Email failure: " + userErrMsg, false, false);
                              
                            } else {
                                processingResult.Error = new ProcessingError("Failed to create user because an email could not be sent to it. Reason: " + userErrMsg, "Failed to create user because an email could not be sent to it.  Reason: " + userErrMsg, false, false);
                                processingResult.IsSuccessful = false;
                            }

                        } catch (Exception ex2) {
                            ex2.ToExceptionless()
                              .SetMessage(string.Format("Failed to delete user (id: {0}) after email                   send failure",userToCreate.Id))
                              .MarkAsCritical()
                              .Submit();
                            processingResult.IsSuccessful = false;
                            processingResult.Error = new ProcessingError("The email address entered was not valid, however we were unable to delete the user account.  Email failure: " + userErrMsg, "The email address entered was not valid, however we were unable to delete the user account.  Email failure: " + userErrMsg, false, false);
                           
                            return Ok(processingResult);
                        }
                    }
                }

                processingResult.Data = userToCreate.Id;
                processingResult.IsSuccessful = true;
            } catch (Exception ex) {
                ex.ToExceptionless()
                .SetMessage("Error creating account")
                .MarkAsCritical()
                .Submit();
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error creating account", "Error creating account", true);
                return Ok(processingResult);
            }
            return Ok(processingResult);
        }
    }
}

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

namespace LS.WebApp.Controllers.api {
    [RoutePrefix("webapi/user")]
    public class UserController : BaseApiController {
        private static readonly string BasicAuthType = "Basic";
        private static readonly string AuthHeader = "Authorization";

        private bool RequestIsAuthorized() {
            string a = "test";
            var authHeader = HttpContext.Current.Request.Headers[AuthHeader];
            if (authHeader == null || !authHeader.StartsWith(BasicAuthType)) {
                return false;
            }
            var encodedUsernamePassword = authHeader.Substring((BasicAuthType + " ").Length).Trim();
            var usernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));

            var credsArray = usernamePassword.Split(':');
            var username = credsArray[0];
            var password = credsArray[1];

            var basicAuthIsValid = username == ApplicationConfig.WebAPIUsername && password == ApplicationConfig.WebAPIPassword || username == ApplicationConfig.TopUpAppUsername && password == ApplicationConfig.TopUpAppPassword;

            return basicAuthIsValid;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IHttpActionResult> CreateUser() {
            var processingResult = new ServiceProcessingResult<string>();

            if (!RequestIsAuthorized()) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Not authorized.", "Not authorized.", false);
                return Ok(processingResult);
            }

            if (!Request.Content.IsMimeMultipartContent()) {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);
            await Request.Content.ReadAsMultipartAsync(provider);


            var roleName = provider.FormData.GetValues("RoleName")[0];
            var companyName = provider.FormData.GetValues("CompanyName")[0];
            var firstName = provider.FormData.GetValues("FirstName")[0];
            var lastName = provider.FormData.GetValues("LastName")[0];
            var email = provider.FormData.GetValues("Email")[0];
            var payPalEmail = provider.FormData.GetValues("PayPalEmail")[0];
            var userName = provider.FormData.GetValues("Email")[0];
            var salesTeamId = provider.FormData.GetValues("SalesTeamId")[0];
            var permissionsNlad = provider.FormData.GetValues("permissionsNlad")[0];
            var permissionsCA = provider.FormData.GetValues("permissionsCA")[0];
            var permissionsTX = provider.FormData.GetValues("permissionsTX")[0];
            var permissionsByPassTPIV = provider.FormData.GetValues("permissionsByPassTPIV")[0];
            var permissionsAccountOrder = provider.FormData.GetValues("permissionsAccountOrder")[0];
            var active = provider.FormData.GetValues("Active")[0];
            var language = "en";

            var roleBeingAssigned = AuthAndUserManager.GetRoleWithName(roleName);
            if (roleBeingAssigned == null) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Couldnt find role", "Couldnt find role", true, false);
                return Ok(processingResult);
            }

            var companyDataService = new CompanyDataService();

            var lookupCompany = await companyDataService.GetCompanyByNameAsync(companyName);
            if (!lookupCompany.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = lookupCompany.Error;
                return Ok(processingResult);
            }

            var salesTeamDataService = new SalesTeamDataService(LoggedInUserId);

            var lookupSalesTeam = await salesTeamDataService.getSalesTeamsByExternalDisplayName(salesTeamId);
            if (!lookupSalesTeam.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = lookupSalesTeam.Error;
                return Ok(processingResult);
            }

            if (lookupSalesTeam.Data.Count == 0) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Couldnt find team", "Couldnt find team", true, false);
                return Ok(processingResult);
            }
            var userToCreate = new ApplicationUser {
                Id = Guid.NewGuid().ToString(),
                FirstName = firstName,
                LastName = lastName,
                UserName = userName,
                Role = roleBeingAssigned,
                Email = email,
                PayPalEmail = payPalEmail,
                SalesTeamId = lookupSalesTeam.Data[0].Id,
                CompanyId = lookupCompany.Data.Id,
                Language = "en",
                PermissionsAccountOrder = Convert.ToBoolean(permissionsAccountOrder),
                PermissionsBypassTpiv = Convert.ToBoolean(permissionsByPassTPIV),
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow,
                IsActive = Convert.ToBoolean(active),
                IsDeleted = false
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

                    var sendEmail = AppUserManager.SendEmailAsync(userToCreate.Id,"An account has been created for you.","You have been registered for access to the Lifeline Services portal " +
                        "on behalf of " + lookupCompany.Data.Name + ". To access the portal, please go to <a href='https://spinlifeserv.com'>SpinLifeServ.com</a> and enter the credentials below.<br/><br/>" +
                        "<font size='2'><i>Remember that the password is case sensitive.  Do not share this information with anyone.</i></font> <br/><br/>" +
                        "<strong>Username:</strong>" + userToCreate.UserName + "<br/><strong>Password:</strong>" + password);
                    } catch (Exception) {
                
                    if (!ServerVars.IsDev) {
                        var deleteResult = AppUserManager.DeleteAsync(userToCreate).Result;

                        if (!deleteResult.Succeeded) {
                            //Logger.Error(string.Format("Failed to delete user {0} after email send failure",userToCreate.Id));
                            }
                        throw;
                    }
                }

                processingResult.Data = "Account created successfully.";
                processingResult.IsSuccessful = true;
            } catch (Exception ex) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error creating account", "Error creating account", true);
                return Ok(processingResult);
            }

            try {
                var rootFiles = new DirectoryInfo(root);
                foreach (FileInfo file in rootFiles.GetFiles()) {
                    if (file.LastAccessTime < DateTime.UtcNow.AddDays(-1)) {
                        file.Delete();
                    }
                }
            } catch (Exception ex) {
                //Logger.Error(ex);
            }

            return Ok(processingResult);
        }

        [HttpPost]
        [Route("verify")]
        public async Task<IHttpActionResult> Verify() {
            var processingResult = new ServiceProcessingResult<string>();

            if (!RequestIsAuthorized()) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Not authorized.", "Not authorized.", false);
                return Ok(processingResult);
            }

            if (!Request.Content.IsMimeMultipartContent()) {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);
            await Request.Content.ReadAsMultipartAsync(provider);
            var username = provider.FormData.GetValues("Value1");
            var password = provider.FormData.GetValues("Value2");
            if (username == null) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Missing parameter Value 1", "Missing parameter Value 1", true);
                return Ok(processingResult);
            }
            if (password == null) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Missing parameter Value 2", "Missing parameter Value 2", true);
                return Ok(processingResult);
            }

            try {
                var user = await AppUserManager.FindAsync(username[0], password[0]);
                if (user == null) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("User not found", "User not found", true);
                    return Ok(processingResult);
                }
                processingResult.Data = user.Id;
                processingResult.IsSuccessful = true;
            } catch (Exception ex) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error retrieving credentials", "Error retrieving credentials", true);
                return Ok(processingResult);
            }

            try {
                var rootFiles = new DirectoryInfo(root);
                foreach (FileInfo file in rootFiles.GetFiles()) {
                    if (file.LastAccessTime < DateTime.UtcNow.AddDays(-1)) {
                        file.Delete();
                    }
                }
            } catch (Exception ex) {

            }

            return Ok(processingResult);
        }

        [HttpGet]
        [Route("GetUser")]
        public async Task<IHttpActionResult> GetUser(string userId) {
            var apiUserResult = new ApiGetUserBindingModel();

            if (!RequestIsAuthorized()) {
                apiUserResult.IsError = true;
                apiUserResult.ErrorMessage_User = "Not Authorized";
                apiUserResult.ErrorMessage_Developer = "Invalid Credentials";
                return Ok(apiUserResult);
            }

            if (string.IsNullOrEmpty(userId)) {
                apiUserResult.IsError = true;
                apiUserResult.ErrorMessage_User = "Please provide a user id and try again.";
                apiUserResult.ErrorMessage_Developer = "User id can not be null";
                return Ok(apiUserResult);
            }

            var dataService = new ApplicationUserDataService();
            var getUserResult = await dataService.GetAsync(userId);
            if (!getUserResult.IsSuccessful) {
                apiUserResult.IsError = true;
                apiUserResult.ErrorMessage_User = "Error retrieving user";
                apiUserResult.ErrorMessage_Developer = getUserResult.Error.UserHelp;

                return Ok(getUserResult.Error);
            }
            if (getUserResult.Data == null) {
                apiUserResult.IsError = true;
                apiUserResult.ErrorMessage_User = "User was not found";
                apiUserResult.ErrorMessage_Developer = "User was not found";

                return Ok(apiUserResult);
            }

            apiUserResult = getUserResult.Data.ToApiGetUserBindingModel();


            return Ok(apiUserResult);
        }


        [HttpGet]
        [Route("ValidateUsernameForAppAssociation")]
        public async Task<IHttpActionResult> ValidateUsernameForAppAssociation(string userName) {
            var apiUserResult = new ValidatedUserModel();

            if (!RequestIsAuthorized()) {
                apiUserResult.IsError = true;
                apiUserResult.ErrorMessage_User = "Not Authorized";
                apiUserResult.ErrorMessage_Developer = "Invalid Credentials";
                return Ok(apiUserResult);
            }

            if (string.IsNullOrEmpty(userName)) {
                apiUserResult.IsError = true;
                apiUserResult.ErrorMessage_User = "Please provide a user name and try again.";
                apiUserResult.ErrorMessage_Developer = "User name can not be null";
                return Ok(apiUserResult);
            }

            var dataService = new ApplicationUserDataService();
            var getUserResult = await dataService.GetUserByUsernameWithRole(userName);
            if (!getUserResult.IsSuccessful) {
                apiUserResult.IsError = true;
                apiUserResult.ErrorMessage_User = "Error retrieving user";
                apiUserResult.ErrorMessage_Developer = getUserResult.Error.UserHelp;

                return Ok(getUserResult.Error);
            }
            //User not found or Inactive
            if (getUserResult.Data == null || !getUserResult.Data.IsActive) {
                apiUserResult.IsError = true;
                apiUserResult.ErrorMessage_User = "User account not found, or is inactive";
                apiUserResult.ErrorMessage_Developer = "User account not found, or is inactive";

                return Ok(apiUserResult);
            }

            //Do checks here
            var user = getUserResult.Data;
            if (user.Role.Rank < 2)// Is sa or admin
                {
                apiUserResult.IsError = true;
                apiUserResult.ErrorMessage_User = "User account cannot be used";
                apiUserResult.ErrorMessage_Developer = "User account cannot be used";
                return Ok(apiUserResult);
            }

            //Get teams from vUserActiveTeams
            var getTeamsInUserResult = await dataService.GetTeamAssociatedWithUser(user.Id);

            if (getTeamsInUserResult.Data.Count < 1)// Check if access to team if not return 
                {
                apiUserResult.IsError = true;
                apiUserResult.ErrorMessage_User = "User account is not associated with a team";
                apiUserResult.ErrorMessage_Developer = "User account is not associated with a team";
                return Ok(apiUserResult);
            }
            string vTeam = getTeamsInUserResult.Data[0].ToString();//Use first in list if more than one
            apiUserResult.UserName = user.UserName;
            apiUserResult.FullName = user.FirstName + " " + user.LastName;
            apiUserResult.BMLocation = vTeam;
            return Ok(apiUserResult);
        }
        class RetVal {
            public bool IsError { get; set; } = false;
            public string ErrorMessage_Developer { get; set; }
            public string ErrorMessage_User { get; set; }
            public bool UpdateSuccessful { get; set; } = true;
        }
        class ValidatedUserModel {
            public string UserName { get; set; }
            public string FullName { get; set; }
            public string BMLocation { get; set; }
            public bool IsError { get; set; }
            public string ErrorMessage_Developer { get; set; }
            public string ErrorMessage_User { get; set; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity.Owin;
using LS.ApiBindingModels;
using LS.Core;
using LS.Domain;
using LS.Services;
using LS.WebApp.CustomAttributes;
using LS.WebApp.Models;
//using NLog.Internal;
using System.Net;
using System.Net.Mail;
using LS.Utilities;
using System.Security.Claims;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using Exceptionless;
using Exceptionless.Models;

namespace LS.WebApp.Controllers.api {
    //        [Authorize]
    [SingleSessionAuthorize]
    [RoutePrefix("api/appUser")]
    public class AppUserController : BaseAPIController {
        private static readonly string UserCreationFailedUserMessage = "An error occurred during user creation.";

        private static readonly string UserEditFailedUserMessage = "An error occurred while editing the user.";

        private static readonly string EditedUserDoesntExistUserHelp =
            "The user you are attempting to edit no longer exists.  Someone may have deleted this user while you were editting.  Please refresh the page and try again.";

        private static readonly string UserValidationFailedUserMessage =
            "User validation failed.";

        private static readonly string CannotAssignRoleToUserUserHelp =
            "You do not have permission to assign this role to a user.";

        private static readonly string UserNameAlreadyExistsUserHelp = "A user with this username already exists.";


        [HttpGet]
        public async Task<IHttpActionResult> SearchUsers(string searchString) {
            var processingResult = new ServiceProcessingResult<List<UserSearchViewBindingModel>>();
            if (searchString == null) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_SEARCH_WITHOUT_CRITERIA_ERROR;
                return Ok(processingResult);
            }

            var companyId = GetLoggedInUserCompanyId();
            var dataService = new ApplicationUserDataService();
            var getLoggedInUserResult = await dataService.GetAsync(LoggedInUserId);
            if (!getLoggedInUserResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_COULD_NOT_FIND_USER_ERROR;
                return Ok(processingResult);
            }
            var loggedInUser = getLoggedInUserResult.Data;
            processingResult = await dataService.SearchUsersInCompany(searchString, companyId, loggedInUser);

            if (processingResult.IsFatalFailure()) {
                ExceptionlessClient.Default.CreateLog(typeof(AppUserController)
                    .FullName, "A fatal error occurred while searching Users for Company with Id: " + companyId, "Error")
                    .AddTags("SearchUsers")
                     .Submit();


                //Logger.Fatal("A fatal error occurred while searching Users for Company with Id: " + companyId);
            }
            List<ApplicationUser> fullUserlist = new List<ApplicationUser>();

            //var sorteddata = processingResult.Data.OrderBy(x => x.FirstName).ToList();
            //processingResult.Data = sorteddata;
            return Ok(processingResult);
        }
        [Route("editOnBoardUser")]
        public async Task<IHttpActionResult> EditOnBoardUser() {
            var processingResult = new ServiceProcessingResult<UserOnBoardData>() { IsSuccessful = true };
            var dataService = new CompanyDataService();

            if (!Request.Content.IsMimeMultipartContent()) {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);
            try {
                var data = await Request.Content.ReadAsMultipartAsync(provider);
            } catch (Exception ex) {
                ex.ToExceptionless()
                      .SetMessage("Picture file size must be 4MB or smaller.")
                      .Submit();
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Picture file size must be 4MB or smaller.", "Picture file size must be 4MB or smaller.", true, false);
                return Ok(processingResult);
            }


            var model = new UserOnBoardData();

            try {
                // Show all the key-value pairs.
                foreach (var key in provider.FormData.AllKeys) {
                    foreach (var val in provider.FormData.GetValues(key)) {
                        DateTime tmpVal;
                        var vResult = DateTime.TryParse(val, out tmpVal);//convert from string to date
                        if (vResult)//if date send date value
                            {
                            Utils.SetObjectProperty(key, tmpVal, model);
                        } else {
                            Utils.SetObjectProperty(key, val, model);
                        }

                    }
                }
            } catch (Exception ex) {
                ex.ToExceptionless()
               .SetMessage("Error setting object value.")
               .MarkAsCritical()
               .AddTags("Controller Error")
               .Submit();

            }


            Byte[] image = null;

            foreach (MultipartFileData file in provider.FileData) {
                image = File.ReadAllBytes(file.LocalFileName);
            }


            var userService = new ApplicationUserDataService();
            var updatedUser = model;

            if (image != null) {
                updatedUser.PictureID = Guid.NewGuid().ToString();
            }



            ///UpdateUser
            var theQuery = new SQLQuery();

            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@Id",LoggedInUser.Id),
                new SqlParameter("@CompanyID",GetLoggedInUserCompanyId()),
                new SqlParameter("@StreetAddress1",updatedUser.StreetAddress1),
                new SqlParameter("@StreetAddress2",updatedUser.StreetAddress2==null?"":updatedUser.StreetAddress2),
                new SqlParameter("@City",updatedUser.City),
                new SqlParameter("@State",updatedUser.State),
                new SqlParameter("@Zip",updatedUser.Zip),
                new SqlParameter("@PictureID",updatedUser.PictureID+".jpg"),
                new SqlParameter("@Ssn",updatedUser.Ssn),
                new SqlParameter("@DateOfBirth",updatedUser.DateOfBirth)
              };
            var strQuery = "UPDATE [dbo].[UserOnBoardData] SET[Id] = @Id,[CompanyID] =@CompanyID,[StreetAddress1] =@StreetAddress1,[StreetAddress2] =@StreetAddress2,[City] =@City,[State] =@State,[Zip] = @Zip,[PictureID] = @PictureID,[Ssn] = @Ssn,[DateOfBirth] =@DateOfBirth,[DateModified] =GetDate() WHERE [Id]=@Id";
            var userResult = await theQuery.ExecuteNonQueryAsync(CommandType.Text, strQuery, parameters);
            if (!userResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Failed to update on board information.", "Failed to update on board information.", true, false);
                return Ok(processingResult);
            }

            processingResult.Data = updatedUser;

            if (image != null) {
                // Upload the image
                var imageDataService = new ImageDataService();
                var imageUploadResult = await imageDataService.UploadImageAsync(image, updatedUser.PictureID, "Storage", LoggedInUser.CompanyId);
                if (!imageUploadResult.IsSuccessful) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = imageUploadResult.Error;
                    return Ok(processingResult);
                }
            }
            ////update asp.net user to show onboard done not used, left in case we need to force users to enter data.
            //SqlParameter[] updateparameters = new SqlParameter[] {
            //    new SqlParameter("@Id",LoggedInUser.Id),
            //    new SqlParameter("@OnBoard",1)
            //    };
            //strQuery = "UPDATE [dbo].[AspNetUsers] SET [OnBoard] =@OnBoard WHERE [Id]=@Id";

            //var updateResult = await theQuery.ExecuteNonQueryAsync(CommandType.Text,strQuery,updateparameters);
            //if (!userResult.IsSuccessful)
            //    {
            //    processingResult.IsSuccessful = false;
            //    processingResult.Error = new ProcessingError("Failed to update on board user information.","Failed to update on board user information.",true,false);
            //    return Ok(processingResult);
            //    }

            processingResult.Data = updatedUser;

            return Ok(processingResult);





        }

        [HttpGet]
        [Route("checkFields")]
        public async Task<IHttpActionResult> CheckFields(string userId) {
            var processingResult = new ServiceProcessingResult<bool>() { IsSuccessful = false, Data = false };

            if (userId == null) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GET_USER_WITHOUT_ID_ERROR;
                return Ok(processingResult);
            }

            var companyId = GetLoggedInUserCompanyId();
            var theQuery = new SQLQuery();

            SqlParameter[] parameters = new SqlParameter[] {
               new SqlParameter("@Id", userId),

           };
            var strQuery =
                "SELECT Id FROM UserOnBoardData  WHERE(Id = @Id) AND (COALESCE(StreetAddress1,'') != '' And COALESCE(City,'') != '' AND COALESCE(PictureID,'') != '' AND COALESCE(SSN,'') != '' And COALESCE(State,'') != '' And COALESCE(Zip,'') != '' And COALESCE(PictureID,'') != '')";
            var userResult = await theQuery.ExecuteReaderAsync<UserOnBoardReturnView>(CommandType.Text, strQuery, parameters);
            if (!userResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error retrieving onboard data.", "Error retrieving onboard data.", true, false);
                return Ok(processingResult);
            }
            if (userResult.Data == null) {
                processingResult.IsSuccessful = true;
                processingResult.Data = false;
            } else {
                processingResult.IsSuccessful = true;
                processingResult.Data = true;
            }

            return Ok(processingResult);
        }
        [HttpGet]
        [Route("getUserForOnBoarding")]
        public async Task<IHttpActionResult> GetUserForOnBoarding(string userId) {
            var processingResult = new ServiceProcessingResult<UserOnBoardReturnView>();
            List<UserOnBoardReturnView> user;
            if (userId == null) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GET_USER_WITHOUT_ID_ERROR;
                return Ok(processingResult);
            }

            var companyId = GetLoggedInUserCompanyId();
            var theQuery = new SQLQuery();

            SqlParameter[] parameters = new SqlParameter[] {
               new SqlParameter("@Id", userId),

           };
            var strQuery =
                "Select Id,CompanyID,StreetAddress1,StreetAddress2,City,State,Zip,PictureID,Ssn,DateOfBirth From UserOnboardData where Id=@Id";
            var userResult = await theQuery.ExecuteReaderAsync<UserOnBoardReturnView>(CommandType.Text, strQuery, parameters);
            if (!userResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error retrieving onboard data.", "Error retrieving onboard data.", true, false);
                return Ok(processingResult);
            }
            if (userResult.Data == null)//record not found so create one
                {
                SqlParameter[] parameters1 = new SqlParameter[] {
                       new SqlParameter("@Id", userId),
                       new SqlParameter("@CompanyId",companyId)
                       };
                strQuery = "Insert into UserOnboardData (Id,CompanyId,DateCreated,DateModified,IsDeleted) Values(@Id,@CompanyId,GetDate(),GetDate(),0)";
                var result = await theQuery.ExecuteNonQueryAsync(CommandType.Text, strQuery, parameters1);
                if (!result.IsSuccessful) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("Error retrieving onboard data.", "Error retrieving onboard data.", true, false);
                    return Ok(processingResult);
                }
                strQuery = "Select * From UserOnboardData where id=@Id";
                SqlParameter[] parameters2 = new SqlParameter[] {
               new SqlParameter("@Id", userId),

           };
                var finaluserResult = await theQuery.ExecuteReaderAsync<UserOnBoardReturnView>(CommandType.Text, strQuery, parameters2);
                if (!finaluserResult.IsSuccessful) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("Error retrieving onboard data.", "Error retrieving onboard data.", true, false);
                    return Ok(processingResult);
                }
                user = (List<UserOnBoardReturnView>)finaluserResult.Data;


            } else {
                //data present first go around
                user = (List<UserOnBoardReturnView>)userResult.Data;
            }

            if (!String.IsNullOrEmpty(user[0].PictureID)) {
                var credentialService = new ExternalStorageCredentialsDataService();
                var getCredentialsResult = await credentialService.GetProofImageStorageCredentialsFromCompanyId(companyId, "Storage");
                if (getCredentialsResult.IsSuccessful)//if not  don't retieve image url but return rest of data
                    {
                    var externalStorageService = new ExternalStorageService(getCredentialsResult.Data);
                    var url = externalStorageService.GeneratePreSignedUrl(user[0].PictureID);
                    user[0].PictureUrl = url.Data;

                }
            }
            processingResult.IsSuccessful = true;
            processingResult.Data = user[0];
            return Ok(processingResult);

        }
        [HttpGet]
        [Route("getUserForExport")]
        public async Task<IHttpActionResult> GetUserForExport() {
            var processingResult = new ServiceProcessingResult<List<ExportUserViewBindingModel>>();
            var companyId = GetLoggedInUserCompanyId();
            var theQuery = new SQLQuery();
            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@Exported", false),//not using
               new SqlParameter("@CompanyId", companyId),

           };
            var strQuery = "SELECT O.Id, O.CompanyID,O.StreetAddress1, O.StreetAddress2,O.City, O.State, O.Zip,                 O.PictureID, O.Ssn, O.DateOfBirth, O.IsDeleted, O.DateCreated, O.DateModified, O.Exported,                   U.FirstName, U.LastName,U.Email FROM UserOnBoardData O INNER JOIN AspNetUsers U ON O.Id = U.Id where O.CompanyId=@CompanyId";
            var userResult = await theQuery.ExecuteReaderAsync<ExportUserViewBindingModel>(CommandType.Text, strQuery, parameters);
            if (!userResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error retrieving onboard data.", "Error retrieving onboard data.", true, false);
                return Ok(processingResult);
            }
            if (userResult.Data == null) {
                processingResult.IsSuccessful = true;
                processingResult.Data = null;
                return Ok(processingResult);
            }
            List<ExportUserViewBindingModel> ExportList = new List<ExportUserViewBindingModel>();
            ExportList = (List<ExportUserViewBindingModel>)userResult.Data;

            List<ExportUserViewBindingModel> Users = new List<ExportUserViewBindingModel>();

            processingResult.IsSuccessful = true;
            processingResult.Data = ExportList;

            return Ok(processingResult);

        }


        [Route("getUserForEdit")]
        public async Task<IHttpActionResult> GetUserForEdit(string userId) {
            var processingResult = new ServiceProcessingResult<UserViewBindingModel>();

            if (userId == null) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GET_USER_WITHOUT_ID_ERROR;
                return Ok(processingResult);
            }

            var companyId = GetLoggedInUserCompanyId();

            var dataService = new ApplicationUserDataService();
            //var getUserResult = await dataService.GetUserInLoggedInUsersSalesGroupTreeAsync(userId, LoggedInUser, companyId);
            var getUserResult = await dataService.GetAsync(userId);
            if (!getUserResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = getUserResult.Error;
                if (getUserResult.IsFatalFailure()) {
                    var logMessage =
                        String.Format("A fatal error occurred while getting User with Id: {0} from Company with Id: {1}",
                            userId, companyId);
                    //Logger.Fatal(logMessage);
                }
                return Ok(processingResult);
            }

            var validateDeletabilityResult = await dataService.ValidateUserCanBeDeletedAsync(getUserResult.Data);
            if (!validateDeletabilityResult.IsSuccessful) {
                // If something goes wrong with this call, just assume the user can't be deleted
                validateDeletabilityResult.Data = new ValidationResult { IsValid = false };
            }

            var user = getUserResult.Data;

            processingResult.IsSuccessful = true;
            processingResult.Data = user.ToUserViewBindingModel(validateDeletabilityResult.Data.IsValid);

            return Ok(processingResult);
        }

        public async Task<IHttpActionResult> GetAllManagersForCurrentCompany() {
            var processingResult = new ServiceProcessingResult<ManagerViewBindingModel>();
            var companyId = GetLoggedInUserCompanyId();

            var dataService = new ApplicationUserDataService();
            var ManagersResult = await dataService.GetsGroupManagersForLoggeInUserAsync(companyId, LoggedInUser);
            if (!ManagersResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ManagersResult.Error;
                if (ManagersResult.IsFatalFailure()) {
                    //Logger.Fatal("A fatal error occurred while getting Group Managers for Company with Id: " + companyId);
                }
                return Ok(processingResult);
            }

            processingResult.Data = ManagersResult.Data;
            processingResult.IsSuccessful = true;

            return Ok(processingResult);
        }

        [Route("getAllUsersUnderLoggedInUserInTree")]
        public async Task<IHttpActionResult> GetAllUsersUnderLoggedInUserInTree(string UserID, string Rank) {
            var processingResult = new ServiceProcessingResult<List<UserListScreenViewBindingModel>>();
            var companyId = GetLoggedInUserCompanyId();

            var sqlQuery = new SQLQuery();

            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@UserId", UserID),
                new SqlParameter("@FilterUserName", null),
                new SqlParameter("@FilterRank", Rank),
                new SqlParameter("@FilterUserID", null)
            };

            var getUsersResult = await sqlQuery.ExecuteReaderAsync<UserListScreenViewBindingModel>(CommandType.StoredProcedure, "usp_GetUsers", parameters);
            if (!getUsersResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error looking up users for manage user screen.", "Error looking up users for manage user screen.", false, false);

                return Ok(processingResult);
            }

            var users = (List<UserListScreenViewBindingModel>)getUsersResult.Data;
            var data = new List<UserListScreenViewBindingModel>();

            if (users != null) {
                data = users.OrderBy(a => a.FirstName).ToList();
            }

            processingResult.IsSuccessful = true;
            processingResult.Data = data;
            
            return Ok(processingResult);
        }

        [Route("createUser")]
        public async Task<IHttpActionResult> CreateUser(UserCreationBindingModel model) {
            var processingResult = new ServiceProcessingResult<string>() { IsSuccessful = true };
            var dataService = new CompanyDataService();
            //Check If Permission is granted to create
            string companyname = "";
            string GroupId = model.GroupId;
          
            try {
                var getCompanyResult = await dataService.GetCompanyAsync(LoggedInUser.CompanyId);
                companyname = getCompanyResult.Data.Name;
                if (LoggedInUser.Role.Rank > getCompanyResult.Data.MinToChangeTeam) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.PERMISSION_ERROR;
                    return Ok(processingResult);
                }
            } catch (Exception ex) {
                ex.ToExceptionless()
                      .SetMessage(ErrorValues.PERMISSION_ERROR)
                      .MarkAsCritical()
                      .Submit();
                //company was not found or id passed as null
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.PERMISSION_ERROR;
                return Ok(processingResult);
            }

            if (!ModelState.IsValid) {
                var userHelp = GetModelStateErrorsAsString(ModelState);
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError(UserValidationFailedUserMessage, userHelp, false, true);
                return Ok(processingResult);
            }

            var roleBeingAssigned = AuthAndUserManager.GetRoleWithId(model.RoleId);
            //make sure reps have a sales team assigned
            if (roleBeingAssigned.Rank > ValidApplicationRoles.LevelThreeManager.Rank) {
                if (model.SalesTeamId == "" || model.SalesTeamId == null) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("A Sales Team must be assigned to sales reps.", "A Sales Team must be assigned to sales reps.", true, false);
                    return Ok(processingResult);
                }
            }

            var appUserService = new ApplicationUserDataService();

            var userToCreate = model.ToApplicationUser(LoggedInUser.CompanyId, LoggedInUserId);

            var validationProcessingResult = await appUserService.ValidateUserCreationAsync(userToCreate, roleBeingAssigned, LoggedInUser);
            if (!validationProcessingResult.IsSuccessful || !validationProcessingResult.Data.IsValid) {
                if (validationProcessingResult.IsFatalFailure()) {
                    // Logger.Fatal(String.Format("A fatal error occurred while validating user creation for User with Id: {0}", LoggedInUserId));
                }

                processingResult.IsSuccessful = false;
                processingResult.Error = !validationProcessingResult.IsSuccessful ? validationProcessingResult.Error : validationProcessingResult.Data.ToProcessingError(ErrorValues.UserCreationValidationFailedUserMessage);

                return Ok(processingResult);
            }

            var user = model.ToApplicationUser(LoggedInUser.CompanyId, LoggedInUserId);
            user.Language = "en";

            try {
                var result = await AuthAndUserManager.CreateUserAndAssignToRoleAsync(user, roleBeingAssigned.Name, companyname);

                if (!result.IsSuccessful) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = result.Error;
                    return Ok(processingResult);
                }

            } catch (Exception ex) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.USER_CREATION_FAILED_ERROR;
                if (ex.Message == "Failure sending mail.") {
                    processingResult.Error = new ProcessingError("User added but failed to send email.", "User added but failed to send email", true);
                    ex.ToExceptionless()
                   .SetMessage("Failed to send email.")
                   .MarkAsCritical()
                   .AddTags("Email User Password Error")
                   .AddObject(user, "UserToCreate")
                   .Submit();
                    if (System.Configuration.ConfigurationManager.AppSettings["Environment"] == "DEV") {
                        processingResult.IsSuccessful = true;
                        return Ok(processingResult);
                    }
                }
                ex.ToExceptionless()
               .SetMessage("Failed to create user.")
               .MarkAsCritical()
               .AddTags("Create User Error")
               .AddObject(user, "UserToCreate")
               .Submit();
                //Logger.Fatal("Failed to create user. " + ex.Message, ex);
                return Ok(processingResult);
            }

            //Add manager to group if specified find out if level 1 ad 2 or 3
            if (user.Role.Rank == ApplicationRoleRulesHelper.Level1SalesGroupManagerRank) {
                var groupDataService = new Level1SalesGroupDataService();
                if (GroupId != null && GroupId != "0") {
                    var existingSalesGroupResult = await groupDataService.GetExistingLevel1SalesGroupInCompanyAsync(GroupId, LoggedInUser.CompanyId);

                    var existingSalesGroupManagers = existingSalesGroupResult.Data.Managers.ToList(); ;
                    existingSalesGroupManagers.Add(user);
                    existingSalesGroupResult.Data.Managers.Clear();
                    existingSalesGroupResult.Data.Managers = existingSalesGroupManagers;
                    var upDateResult = await groupDataService.UpdateAsync(existingSalesGroupResult.Data, LoggedInUser);
                    if (!upDateResult.IsSuccessful) {
                        processingResult.Error = upDateResult.Error;
                    }
                }
            } else if (user.Role.Rank == ApplicationRoleRulesHelper.Level2SalesGroupManagerRank) {
                var groupDataService = new Level2SalesGroupDataService();
                if (GroupId != null && GroupId != "0") {
                    var existingSalesGroupResult = await groupDataService.GetExistingLevel2SalesGroupInCompany(GroupId, LoggedInUser.CompanyId);

                    var existingSalesGroupManagers = existingSalesGroupResult.Data.Managers.ToList(); ;
                    existingSalesGroupManagers.Add(user);
                    existingSalesGroupResult.Data.Managers.Clear();
                    existingSalesGroupResult.Data.Managers = existingSalesGroupManagers;
                    var upDateResult = await groupDataService.UpdateAsync(existingSalesGroupResult.Data, LoggedInUser);
                    if (!upDateResult.IsSuccessful) {
                        processingResult.Error = upDateResult.Error;
                    }
                };

            } else if (user.Role.Rank == ApplicationRoleRulesHelper.Level3SalesGroupManagerRank) {
                var groupDataService = new Level3SalesGroupDataService();
                if (GroupId != null && GroupId != "0") {
                    var existingSalesGroupResult = await groupDataService.GetExistingLevel3SalesGroupInCompanyAsync(GroupId, LoggedInUser.CompanyId);

                    var existingSalesGroupManagers = existingSalesGroupResult.Data.Managers.ToList(); ;
                    existingSalesGroupManagers.Add(user);
                    existingSalesGroupResult.Data.Managers.Clear();
                    existingSalesGroupResult.Data.Managers = existingSalesGroupManagers;
                    var upDateResult = await groupDataService.UpdateAsync(existingSalesGroupResult.Data, LoggedInUser);
                    if (!upDateResult.IsSuccessful) {
                        processingResult.Error = upDateResult.Error;
                    }
                };
            }

            return Ok(processingResult);
        }

        [Route("editUser")]
        public async Task<IHttpActionResult> EditUser(UserUpdateBindingModel model) {
            var processingResult = new ServiceProcessingResult<ApplicationUser>() { IsSuccessful = true };
            var dataService = new CompanyDataService();
            string GroupId = model.GroupId;
            try {
                var getCompanyResult = await dataService.GetCompanyAsync(LoggedInUser.CompanyId);
                if (LoggedInUser.Role.Rank > getCompanyResult.Data.MinToChangeTeam) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.PERMISSION_ERROR;
                    return Ok(processingResult);
                }
            } catch (Exception ex) {
                ex.ToExceptionless()
                      .SetMessage(ErrorValues.PERMISSION_ERROR)
                      .MarkAsCritical()
                      .Submit();
                //company was not found or id passed as null
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.PERMISSION_ERROR;
                return Ok(processingResult);
            }
            var roleBeingAssigned = AuthAndUserManager.GetRoleWithId(model.RoleId);
            if (roleBeingAssigned == null) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.UPDATE_USER_INVALID_USER_ROLE_ID_PROVIDED_ERROR;
                return Ok(processingResult);
            }

            if (!ModelState.IsValid) {
                var userHelp = GetModelStateErrorsAsString(ModelState);
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError(UserValidationFailedUserMessage, userHelp, false, true);
                return Ok(processingResult);
            }

            //validate put in from create user has dublicate email check
            var userService = new ApplicationUserDataService();
            var updatedUser = model.ToApplicationUser(GetLoggedInUserCompanyId());


            var validationProcessingResult = await userService.ValidateUserCreationAsync(updatedUser, roleBeingAssigned, LoggedInUser);
            if (!validationProcessingResult.IsSuccessful || !validationProcessingResult.Data.IsValid) {
                if (validationProcessingResult.IsFatalFailure()) {
                    ExceptionlessClient.Default.CreateLog(typeof(AppUserController).FullName, String.Format("A fatal error occurred while validating user creation for User with Id: {0}", LoggedInUserId), "Error").AddTags("Controller Error").Submit();
                    //Logger.Fatal(String.Format();
                }

                processingResult.IsSuccessful = false;
                processingResult.Error = !validationProcessingResult.IsSuccessful ? validationProcessingResult.Error : validationProcessingResult.Data.ToProcessingError(ErrorValues.UserCreationValidationFailedUserMessage);

                return Ok(processingResult);
            }
            //validate email just that it is good not if duplicate in db

            if (model.Email != model.OriginalEmail) {
                var emailHelper = new EmailHelper();
                var emailBody = "Your Lifeline Services email address has been changed.If you did not request this address change contact your supervisor immediately.";

                try {
                    var sendEmailResult = new EmailHelper().SendEmail("Email Address Change", model.Email, "", emailBody, null);
                    if (!sendEmailResult.Result.IsSuccessful) {
                        processingResult.IsSuccessful = false;

                        processingResult.Error = new ProcessingError("User update failed because new email address failed validation. Reason:" + sendEmailResult.Result.Error.UserHelp, "User update failed because new email address failed validation. Reason:" + sendEmailResult.Result.Error.UserHelp, true, false);
                        ExceptionlessClient.Default.CreateLog("Error Sending Email")
                            .SetMessage("User update failed because new email address failed validation. Reason:" + sendEmailResult.Result.Error.UserHelp)
                            .Submit();

                        return Ok(processingResult);

                    }

                } catch (Exception ex) {
                    processingResult.IsSuccessful = false;
                    var msg = ex.Message;
                    processingResult.Error = new ProcessingError("User update failed because new email address failed validation. Reason:" + msg, "User update failed because new email address failed validation. Reason:" + msg, true, false);
                    ex.ToExceptionless()
                    .SetMessage("Update failed,invalid Email")
                    .AddTags("User Update Error")
                    .AddObject(model, "User")
                    .MarkAsCritical()
                    .Submit();
                    return Ok(processingResult);
                }

            }

            //var updatedUser = model.ToApplicationUser(GetLoggedInUserCompanyId());
            updatedUser.ModifiedByUserId = LoggedInUser.Id;
            processingResult = await userService.UpdateUserNotOwnedByLoggedInUserAsync(updatedUser, roleBeingAssigned, LoggedInUser);

            if (!processingResult.IsSuccessful) {
                if (processingResult.IsFatalFailure()) {
                    ExceptionlessClient.Default.CreateLog(typeof(AppUserController).FullName, String.Format("A fatal error occurred while editing user with Id:{ 0}. Logged in User:{ 1}", model.UserId, LoggedInUser.UserName), "Error").AddTags("User Update Failure").Submit();

                    //var logMessage =
                    //String.Format("A fatal error occurred while editing user with Id: {0}. Logged in User: {1}",
                    //    model.UserId, LoggedInUserId);
                    //Logger.Fatal(logMessage);
                }
                return Ok(processingResult);
            }

            updatedUser = processingResult.Data;

            if (updatedUser.Role.Id != roleBeingAssigned.Id) {
                var group1DataService = new Level1SalesGroupDataService();
                //Change this query below to an ado.net query. 
                var validateUserNotManagerOfGroup = group1DataService.NotAManagerAsync(updatedUser);
                if (validateUserNotManagerOfGroup.Result.Data.IsValid) {
                    var roleUpdateResult = await AuthAndUserManager.UpdateUserRoleIfNecessary(roleBeingAssigned.Name, updatedUser.Role, updatedUser.Id);

                    processingResult.IsSuccessful = roleUpdateResult.IsSuccessful;

                    processingResult.Error = roleUpdateResult.Error;
                    if (roleUpdateResult.IsFatalFailure()) {
                        ExceptionlessClient.Default.CreateLog(typeof(AppUserController).FullName, String.Format("Failed to update user role during user edit.User Id: { 0}. Current User:{ 1}", model.UserId, LoggedInUser.UserName), "Error").AddTags("User Update Failure").Submit();

                        //Logger.Fatal(String.Format("Failed to update user role during user edit. User Id: {0}. Current User: {1}", model.UserId, LoggedInUserId));
                    }
                } else {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("Unable to save role change.  Please remove the user from any groups they are currently managing.", "Unable to save role change.  Please remove the user from any groups they are currently managing.", true, false);
                    return Ok(processingResult);
                }
            }
            //get updateduser with possible new role

            var roleupdatedUserresult = await userService.GetAsync(updatedUser.Id);
            var roleupdatedUser = roleupdatedUserresult.Data;

            //Add manager to group if specified find out if level 1 ad 2 or 3
            if (roleupdatedUser.Role.Rank == ApplicationRoleRulesHelper.Level1SalesGroupManagerRank) {

                var groupDataService = new Level1SalesGroupDataService();
                if (GroupId != null && GroupId != "0") {
                    var existingSalesGroupResult = await groupDataService.GetExistingLevel1SalesGroupInCompanyAsync(GroupId, LoggedInUser.CompanyId);

                    var existingSalesGroupManagers = existingSalesGroupResult.Data.Managers.ToList(); ;
                    existingSalesGroupManagers.Add(updatedUser);
                    existingSalesGroupResult.Data.Managers.Clear();
                    existingSalesGroupResult.Data.Managers = existingSalesGroupManagers;
                    var upDateResult = await groupDataService.UpdateAsync(existingSalesGroupResult.Data, LoggedInUser);
                    if (!upDateResult.IsSuccessful) {
                        processingResult.Error = upDateResult.Error;
                    }
                }
            } else if (roleupdatedUser.Role.Rank == ApplicationRoleRulesHelper.Level2SalesGroupManagerRank) {
                var groupDataService = new Level2SalesGroupDataService();
                if (GroupId != null && GroupId != "0") {
                    var existingSalesGroupResult = await groupDataService.GetExistingLevel2SalesGroupInCompany(GroupId, LoggedInUser.CompanyId);

                    var existingSalesGroupManagers = existingSalesGroupResult.Data.Managers.ToList(); ;
                    existingSalesGroupManagers.Add(updatedUser);
                    existingSalesGroupResult.Data.Managers.Clear();
                    existingSalesGroupResult.Data.Managers = existingSalesGroupManagers;
                    var upDateResult = await groupDataService.UpdateAsync(existingSalesGroupResult.Data, LoggedInUser);
                    if (!upDateResult.IsSuccessful) {
                        processingResult.Error = upDateResult.Error;
                    }
                };

            } else if (roleupdatedUser.Role.Rank == ApplicationRoleRulesHelper.Level3SalesGroupManagerRank) {
                var groupDataService = new Level3SalesGroupDataService();
                if (GroupId != null && GroupId != "0") {
                    var existingSalesGroupResult = await groupDataService.GetExistingLevel3SalesGroupInCompanyAsync(GroupId, LoggedInUser.CompanyId);

                    var existingSalesGroupManagers = existingSalesGroupResult.Data.Managers.ToList(); ;
                    existingSalesGroupManagers.Add(updatedUser);
                    existingSalesGroupResult.Data.Managers.Clear();
                    existingSalesGroupResult.Data.Managers = existingSalesGroupManagers;
                    var upDateResult = await groupDataService.UpdateAsync(existingSalesGroupResult.Data, LoggedInUser);
                    if (!upDateResult.IsSuccessful) {
                        processingResult.Error = upDateResult.Error;
                    }

                }
            }

            return Ok(processingResult);
        }

        [Route("editLoggedInUser")]
        public async Task<IHttpActionResult> EditLoggedInUser(EditLoggedInUserBindingModel model) {
            var processingResult = new ServiceProcessingResult();
            var dataService = new CompanyDataService();
            try {
                var getCompanyResult = await dataService.GetCompanyAsync(LoggedInUser.CompanyId);
                if (LoggedInUser.Role.Rank > getCompanyResult.Data.MinToChangeTeam) {
                    //company was not found or id passed as null
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.PERMISSION_ERROR;
                    return Ok(processingResult);
                }
            } catch (Exception ex) {
                ex.ToExceptionless()
                      .SetMessage(ErrorValues.PERMISSION_ERROR)
                      .MarkAsCritical()
                      .Submit();
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.PERMISSION_ERROR;
                return Ok(processingResult);
            }
            var validationProcessingResult = await ValidateModelForLoggedInUserEdit(model);
            if (!validationProcessingResult.Data.IsValid) {
                processingResult.IsSuccessful = false;
                processingResult.Error = validationProcessingResult.Data.ToProcessingError(ErrorValues.UserUpdateValidationFailedUserMessage);
                return Ok();
            }

            var userService = new ApplicationUserDataService();

            var rowVersion = Convert.FromBase64String(model.RowVersion);
            var updatedUser = new ApplicationUser {
                Id = LoggedInUserId,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
                RowVersion = rowVersion
            };
            processingResult = await userService.UpdateUserOwnedByLoggedInUserAsync(updatedUser);

            if (processingResult.IsFatalFailure()) {
                ExceptionlessClient.Default.CreateLog(typeof(AppUserController).FullName, "Current User update failed.See stack trace for details.", "Error").AddTags("User Update Error").Submit();


                //Logger.Fatal("Current User update failed. See stack trace for details.");
            }

            if (!processingResult.IsSuccessful || model.UserName == LoggedInUserUserName) {
                return Ok(processingResult);
            }

            updatedUser.CompanyId = GetLoggedInUserCompanyId();
            await AuthAndUserManager.RefreshAuthenticationCookie(updatedUser);

            return Ok(processingResult);
        }

        [Route("getAllRoles")]
        public async Task<IHttpActionResult> GetAllRoles() {
            var roleService = new ApplicationRoleDataService();
            var processingResult = await roleService.GetAllAsync();
            if (!processingResult.IsSuccessful) {
                processingResult.Error = new ProcessingError(
                    "An error occurred while getting all available User Roles.",
                    "Could not retrieve User Roles, please try again. If the problem persists contact support.", true);
                return Ok(processingResult);
            }

            processingResult.Data = processingResult.Data.OrderBy(r => r.Rank).ToList();

            return Ok(processingResult);
        }

        [Route("getLoggedInUserRole")]
        public IHttpActionResult GetLoggedInUserRole() {
            var processingResult = new ServiceProcessingResult<ApplicationRole>();
            var loggedInUserRole = base.GetLoggedInUserRole();
            if (loggedInUserRole == null) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("An error occurred while getting current User's role.",
                    "An error occurred while retrieving your role. If the problem persists, contact support.", true);
                ExceptionlessClient.Default.CreateLog(typeof(AppUserController).FullName, "An error occurred while retrieving current User's role.", "Error").AddTags("Get Role Error").Submit();

                // Logger.Error("An error occurred while retrieving current User's role.");
                return Ok(processingResult);
            }

            processingResult.IsSuccessful = true;
            processingResult.Data = loggedInUserRole;
            return Ok(processingResult);
        }
        //Removes user from user table and role table. Set up to clean out if email fails when user is created. Needs to be completed RW
        //public async Task<IHttpActionResult> ZapUser(string userId)
        //{
        //    var appUserService = new ApplicationUserDataService();
        //    var processingResult =
        //        await appUserService.ZapUserAsync(userId, LoggedInUser.CompanyId, LoggedInUser.Role);

        //    if (processingResult.IsFatalFailure())
        //    {
        //        Logger.Fatal("A fatal error occurred while attempting to delete User with Id: " + userId);
        //    }

        //    return Ok(processingResult);
        //}

        public async Task<IHttpActionResult> Delete(string userId) {
            var processingResult = new ServiceProcessingResult();
            var companyService = new CompanyDataService();
            var companyProcessingResult = await companyService.GetCompanyAsync(LoggedInUser.CompanyId);
            //check permissions

            if (LoggedInUser.Role.Rank > ValidApplicationRoles.Admin.Rank) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.PERMISSION_ERROR;
                return Ok(processingResult);
            }

            var appUserService = new ApplicationUserDataService();

            processingResult = await appUserService.MarkUserAsDeletedAsync(userId, LoggedInUser.CompanyId, this.LoggedInUser);

            if (processingResult.IsFatalFailure()) {
                ExceptionlessClient.Default.CreateLog(typeof(AppUserController).FullName, "A fatal error occurred while attempting to delete User with Id: " + userId, "Error").AddTags("Delete User Error").Submit();
                //Logger.Fatal("A fatal error occurred while attempting to delete User with Id: " + userId);
            }

            return Ok(processingResult);
        }

        [Route("getLoggedInUser")]
        public async Task<IHttpActionResult> GetLoggedInUser() {
            var processingResult = new ServiceProcessingResult<LoggedInUserViewBindingModel> { IsSuccessful = true };

            var loggedInUserId = LoggedInUserId;

            var appUserService = new ApplicationUserDataService();
            var loggedInUser = await appUserService.GetAsync(loggedInUserId);
            if (!loggedInUser.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GET_LOGGED_IN_USER_INFO_ERROR;
                return Ok(processingResult);
            }

            var sigType = "";
            bool? TeamActive = null;
            if (loggedInUser.Data.SalesTeamId != null) {
                var salesTeamDataService = new SalesTeamDataService(loggedInUserId);
                var salesTeamResult = salesTeamDataService.Get(loggedInUser.Data.SalesTeamId);
                if (!salesTeamResult.IsSuccessful) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = salesTeamResult.Error;
                    return Ok(processingResult);
                }
                sigType = salesTeamResult.Data.SigType;
                TeamActive = salesTeamResult.Data.IsActive;
            } else {
                sigType = "Tablet";//default,will get over written in select saleteam directive if different
            }

            processingResult.Data = loggedInUser.Data.ToLoggedInUserViewBindingModel(sigType, TeamActive);

            return Ok(processingResult);
        }

        [Route("setCompanyId")]
        public async Task<IHttpActionResult> SetCompanyId(string userid, string newcompanyId) {
            var processingResult = new ServiceProcessingResult();
            if (LoggedInUser.Role.IsSuperAdmin()) {
                //Set the new companyid in the database.
                var dataService = new ApplicationUserDataService();
                processingResult = await dataService.UpdateCompanyId(newcompanyId, userid);
                if (processingResult.IsFatalFailure()) {
                    ExceptionlessClient.Default.CreateLog(typeof(AppUserController).FullName, "A fatal error occurred while setting CompanyId:" + processingResult.Error.UserHelp, "Error").AddTags("Set Company Id Error").Submit();
                    //Logger.Fatal("A fatal error occurred while setting CompanyId");
                    processingResult.IsSuccessful = false;
                } else {
                    var getLoggedInUserResult = await dataService.GetAsync(LoggedInUserId);
                    if (!getLoggedInUserResult.IsSuccessful) {
                        processingResult.IsSuccessful = false;
                        processingResult.Error = ErrorValues.GENERIC_COULD_NOT_FIND_USER_ERROR;
                        return Ok(processingResult);
                    }
                    var user = getLoggedInUserResult.Data;
                    LoggedInUser.CompanyId = newcompanyId;
                    var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
                    //Sign Off and back in to refresh cookie.
                    try {
                        await AuthAndUserManager.RefreshAuthenticationCookie(user);
                        processingResult.IsSuccessful = true;
                    } catch (Exception ex) {
                        ex.ToExceptionless()
                       .SetMessage("A fatal error refreshing user cookie.")
                       .MarkAsCritical()
                       .AddTags("Set Company Id Error")
                       .AddObject(user, "User")
                       .Submit();
                        //Logger.Fatal("A fatal error occurred while setting CompanyId");
                        processingResult.IsSuccessful = false;
                    }
                }

            }
            return Ok(processingResult);
        }

        [Route("setUserLanguage")]
        public async Task<IHttpActionResult> SetUserLanguage(string Language) {
            var processingResult = new ServiceProcessingResult();

            var userDataService = new ApplicationUserDataService();
            try {
                var updatingUserLanguageResult = userDataService.UpdateLanguage(Language, LoggedInUser.Id);
                if (!updatingUserLanguageResult.IsSuccessful) {
                    processingResult.IsSuccessful = updatingUserLanguageResult.IsSuccessful;
                    processingResult.Error = updatingUserLanguageResult.Error;
                }
            } catch (Exception ex) {

            }


            var getLoggedInUserResult = await userDataService.GetAsync(LoggedInUserId);
            if (!getLoggedInUserResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_COULD_NOT_FIND_USER_ERROR;
                return Ok(processingResult);
            }

            var user = getLoggedInUserResult.Data;
            try {
                LoggedInUser.Language = Language;
            } catch (Exception ex) { }

            var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;

            //Sign Off and back in to refresh cookie.
            try {
                await AuthAndUserManager.RefreshAuthenticationCookie(user);
                processingResult.IsSuccessful = true;
            } catch (Exception ex) {
                ex.ToExceptionless()
               .SetMessage("A fatal error refreshing user cookie.")
               .MarkAsCritical()
               .AddTags("Set User Language Error")
               .AddObject(user, "User")
               .Submit();
                //Logger.Fatal("A fatal error occurred while setting CompanyId");
                processingResult.IsSuccessful = false;
            }

            return Ok(processingResult);
        }
        [Route("validateEmail")]
        public async Task<IHttpActionResult> ValidateEmail(string email, string id) {
            var processingResult = new ServiceProcessingResult<string>() { IsSuccessful = true };
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            try {
                await userManager.SendEmailAsync(id, "Email Address Change", "Your Lifeline Services email address has been changed. If you did not request this address change contact your supervisor immediatly.");
            } catch (Exception ex) {
                ex.ToExceptionless()
                 .SetMessage("Validation email did not send.")
                 .MarkAsCritical()
                 .AddTags("Validate Email")
                 .Submit();
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError(ex.Message, ex.Message, true, false);
                return Ok(processingResult);
            }
            processingResult.IsSuccessful = true;

            return Ok(processingResult);
        }

        [HttpPost]
        [Route("resetUsersPassword")]
        public async Task<IHttpActionResult> ResetUsersPassword(string userId, string email) {
            var processingResult = new ServiceProcessingResult();

            var appUserService = new ApplicationUserDataService();
            var loggedInUserCanResetPasswordValidationResult =
                await appUserService.ValidateLoggedInUserCanResetPasswordForUser(userId, LoggedInUserId);
            if (!loggedInUserCanResetPasswordValidationResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = loggedInUserCanResetPasswordValidationResult.Error;
                return Ok(processingResult);
            }

            var validationResult = loggedInUserCanResetPasswordValidationResult.Data;
            if (!validationResult.IsValid) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.CANNOT_RESET_PASSWORD_FOR_USER_ROLE_ERROR;
                return Ok(processingResult);
            }

            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();

            var baseUrl = Request.RequestUri.Authority;
            var code = await userManager.GeneratePasswordResetTokenAsync(userId);

            var helper = new System.Web.Mvc.UrlHelper(HttpContext.Current.Request.RequestContext);
            var callbackPath = helper.Action("ResetPassword", "Authentication", new { c = userId, code = code });
            var callbackUrl = "https://" + baseUrl + callbackPath;
            var emailHelper = new EmailHelper();
            var emailBody = "A password reset request has been made for your account on arrow.spinlifeserv.com.If you do not recognize this request, you can safely ignore it.<br/><br/> <a href =\"" + callbackUrl + "\">Reset Password Now</a><br><br><span style='font-size:9px'><em>Link not working? Paste the following link into your browser:</em><br>" + callbackUrl + "</span>";

            var sendEmailResult = new EmailHelper().SendEmail("Reset Password", email, "", emailBody, null);
            if (!sendEmailResult.Result.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError(sendEmailResult.Result.Error.UserHelp, sendEmailResult.Result.Error.UserHelp, true, false);
                ExceptionlessClient.Default.CreateLog("Error Sending Email")
                  .SetMessage("Reset user password email did not send:" + sendEmailResult.Result.Error.UserHelp)
                  .AddTags("Reset Password Email Error")
                  .AddObject(userId)
                  .MarkAsCritical()
                  .Submit();
                return Ok(processingResult);
            }
            processingResult.IsSuccessful = true;
            return Ok(processingResult);
        }

        [HttpGet]
        [Route("getManagedGroups")]
        public async Task<IHttpActionResult> GetGroupsManaged(string userId) {
            var processingResult = new ServiceProcessingResult<List<UserManagedGroupsBindingModel>>();

            var appUserService = new ApplicationUserDataService();
            processingResult = await appUserService.GetManagedGroups(userId);

            if (!processingResult.IsSuccessful) {
                return Ok(processingResult);
            }

            return Ok(processingResult);
        }

        [HttpGet]
        [Route("getTeamUsers")]
        public async Task<IHttpActionResult> GetTeamUsers(string teamId) {
            var processingResult = new ServiceProcessingResult<List<TeamUserView>>();

            var appUserService = new ApplicationUserDataService();
            processingResult = await appUserService.GetTeamUsers(teamId);

            if (!processingResult.IsSuccessful) {
                return Ok(processingResult);
            }

            return Ok(processingResult);
        }

        private async Task<ServiceProcessingResult<ValidationResult>> ValidateModelForLoggedInUserEdit(EditLoggedInUserBindingModel model) {
            var processingResult = new ServiceProcessingResult<ValidationResult> { IsSuccessful = true };
            var validationResult = new ValidationResult {
                IsValid = true
            };

            if (!ModelState.IsValid) {
                validationResult.IsValid = false;
                validationResult.Errors =
                    ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage).ToList();
            } else if (model.UserName != LoggedInUserUserName && await AuthAndUserManager.UserNameAlreadyExists(model.UserName)) {
                validationResult.IsValid = false;
                validationResult.Errors.Add(UserNameAlreadyExistsUserHelp);
            }
            processingResult.Data = validationResult;

            return processingResult;
        }
    }
}
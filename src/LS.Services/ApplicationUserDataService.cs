using System;
using System.Configuration;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LinqKit;
using LS.Core;
using LS.Domain;
using LS.Repositories;
using Microsoft.AspNet.Identity;
using System.Data.SqlClient;
using LS.ApiBindingModels;
using LS.Repositories.DBContext;
using MoreLinq;
using System.Data;
using Exceptionless;
using Exceptionless.Models;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using LS.Utilities;
using Exceptionless.Json;
using System.IO;
using System.Net;
using ApiBindingModels;
using Newtonsoft.Json;

namespace LS.Services {
    public class ApplicationUserDataService : BaseDataService<ApplicationUser, string> {
        private ApplicationRoleDataService _roleService;

        public override BaseRepository<ApplicationUser, string> GetDefaultRepository() {
            return new ApplicationUserRepository();
        }

        protected override string[] GetDefaultIncludes() {
            return new[] { "Roles.Role" };
        }


        public async Task<ServiceProcessingResult<ApplicationUser>> GetAsync(string userId) {
            var includes = new[] { "Company", "SalesTeam" };
            return await GetWhereAsync(u => u.Id == userId, includes);
        }

        public async Task<ServiceProcessingResult<ApplicationUser>> GetByUserNameAsync(string userName) {
            var result = await GetWhereAsync(u => u.UserName == userName);
            if (result.Data == null) {
                result.IsSuccessful = false;
            }
            return result;
        }

        public async Task<ServiceProcessingResult<ValidationResult>> ValidateUserCreationAsync(
            ApplicationUser userBeingCreated, ApplicationRole roleBeingAssigned, ApplicationUser createdByUser) {
            var processingResult = new ServiceProcessingResult<ValidationResult> { IsSuccessful = true };
            var validationResult = new ValidationResult {
                IsValid = false
            };
            var createdByUserWithCompany = await GetUserInCompanyAsync(createdByUser.Id, createdByUser.CompanyId);
            if (createdByUser.Role.Rank > createdByUserWithCompany.Data.Company.MinToChangeTeam) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.PERMISSION_ERROR;
                return processingResult;
            }

            if (roleBeingAssigned == null) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.INVALID_ROLE_ASSIGNMENT_ERROR;
                return processingResult;
            }

            if (!createdByUser.Role.CanCreateOrUpdateSalesUser()) {
                validationResult.Errors.Add(ErrorValues.UserCrudPermissionsUserHelp);
                processingResult.Data = validationResult;
                return processingResult;
            }

            if (userBeingCreated.CompanyId != createdByUser.CompanyId) {
                validationResult.Errors.Add(ErrorValues.CreateUserNotInCompanyUserHelp);
                processingResult.Data = validationResult;
                return processingResult;
            }

            //Run checks that hit the DB before other validation checks since they could have fatal errors
            var existingUserWithUserNameResult = await base.GetWhereAsyncWithDeleted(u => (u.UserName == userBeingCreated.UserName || u.Email == userBeingCreated.Email || (!String.IsNullOrEmpty(userBeingCreated.ExternalUserID) && u.ExternalUserID == userBeingCreated.ExternalUserID)) && u.Id != userBeingCreated.Id);
            if (!existingUserWithUserNameResult.IsSuccessful) {
                ExceptionlessClient.Default.CreateLog(typeof(ApplicationUserDataService).FullName, "An error occurred while checking for existing user data during user creation.", "Error").AddTags("ValidateUserCreationAsync").AddObject(existingUserWithUserNameResult.Error.UserMessage).Submit();
                //Logger.Error("An error occurred while checking for existing user data during user creation."+ existingUserWithUserNameResult.Error.UserMessage);
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Fatal back end error.", "Fatal back end error", false, true);
                return processingResult;
            }

            var salesTeamService = new SalesTeamDataService(createdByUser.Id);
            var salesTeamOwnershipValidationResult = await salesTeamService.ValidateSalesTeamOwnershipAsync(userBeingCreated.SalesTeamId, roleBeingAssigned, createdByUser);
            if (!salesTeamOwnershipValidationResult.IsSuccessful) {
                //Logger.Error(String.Format("An error occurred while validating sales team assignment during user creation. Created By User Id: {0}", createdByUser.Id));


                ExceptionlessClient.Default.CreateLog(typeof(ApplicationUserDataService).FullName, String.Format("An error occurred while validating sales team assignment during user creation. Created By User Id: {0}", createdByUser.Id), "Error").AddTags("ValidateUserCreationAsync").Submit();
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.USER_CREATION_FAILED_ERROR;
                return processingResult;
            }

            if (!salesTeamOwnershipValidationResult.Data.IsValid) {
                validationResult.Errors.AddRange(salesTeamOwnershipValidationResult.Data.Errors);
            }

            if (!createdByUser.Role.CanAssign(roleBeingAssigned)) {
                validationResult.Errors.Add(ErrorValues.CannotAssignRoleToUserUserHelp);
            }
            if (existingUserWithUserNameResult.Data != null) {
                validationResult.Errors.Add(ErrorValues.UserNameAlreadyExistsUserHelp);
            }

            validationResult.IsValid = validationResult.Errors.Count == 0;

            processingResult.Data = validationResult;

            return processingResult;
        }

        public async Task<ServiceProcessingResult<ValidationResult>> ValidateUserCanBeginOrder(string userId,
            string companyId) {
            var processingResult = new ServiceProcessingResult<ValidationResult> { IsSuccessful = true };
            var validationResult = new ValidationResult();

            var includes = new[] { "Company", "SalesTeam" };
            var getUserResult = await GetWhereAsync(u => u.Id == userId, includes);
            if (!getUserResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.COULD_NOT_FIND_USER_TO_BEGIN_ORDER_ERROR;
                return processingResult;
            }
            if (!getUserResult.Data.PermissionsAccountOrder) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.PEMISSION_DENIED_ERROR;
                return processingResult;
            }

            var userToValidate = getUserResult.Data;

            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(ConfigurationManager.AppSettings["WebAPIUsername"] + ":" + ConfigurationManager.AppSettings["WebAPIPassword"]));
            //if (ConfigurationSettings.AppSettings["Environment"].ToString() == "DEV" || ConfigurationSettings.AppSettings["Environment"].ToString() == "STAGING") {
            //    validationResult.IsValid = true;
            //    processingResult.Data = validationResult;
            //    return processingResult;


            //    }
			var companySettingsResult = await new CompanyDataService().GetCompanyAsync(companyId);
			if (!companySettingsResult.IsSuccessful)
			{
				processingResult.IsSuccessful = false;
				processingResult.Error = companySettingsResult.Error;
				return processingResult;
			}
			var companyData = (Company)companySettingsResult.Data;
			if (companyData.DoPromoCodeCheck)
			{

				var externalUserIDValidationResult = new ExternalUserIDValidationResponse();

				try {
					using (var client = new HttpClient()) {
						client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ExternalUserIDValidationBaseURL"]);
						client.DefaultRequestHeaders.Accept.Clear();
						client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
						client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

						var logEntry = CreateInitialExternalUserIDValidationApiLogEntry("validateExternalUserID?ExternalUserID=" + userToValidate.ExternalUserID);

						HttpResponseMessage response = await client.GetAsync("validateExternalUserID?ExternalUserID=" + userToValidate.ExternalUserID);
						var httpResponse = await response.Content.ReadAsStringAsync();
						if (!Utils.IsJSON(httpResponse)) {
							processingResult.IsSuccessful = true;
							processingResult.Error = new ProcessingError("An error occurred calling External Portals API (GetLevel1Groups) (Invalid Response). Please try again, if error persists contact support.", "An error occurred calling External Portals API (GetLevel1Groups) (Invalid Response). Please try again, if error persists contact support.", false, false);
						}

						var jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ExternalUserIDValidationResponse>(httpResponse);
						FinishLogEntry(logEntry, httpResponse.ToString());
						externalUserIDValidationResult = jsonResponse;
					}
				} catch (WebException exception) {
					string responseText;
					exception.ToExceptionless()
						  .SetMessage("Error calling External User ID Validation API (Web Exception)")
						  .MarkAsCritical()
						  .Submit();
					using (var reader = new StreamReader(exception.Response.GetResponseStream())) {
						responseText = reader.ReadToEnd();
					}
	;
					processingResult.IsSuccessful = false; ;
					processingResult.Error = new ProcessingError("Error calling External User ID Validation API ", "Error calling External User ID Validation API", true, false);

				} catch (Exception ex) {

					//if (Utils.IsDev()) {
					//externalUserIDValidationResult.IsError = false;
					// externalUserIDValidationResult.IsActive = true;
					//} else {
					ex.ToExceptionless()
					 .SetMessage("Error calling External User ID Validation API")
					 .MarkAsCritical()
					 .Submit();
					processingResult.IsSuccessful = false;
					processingResult.Error = new ProcessingError("Error calling External User ID Validation API", "Error calling External User ID Validation API", true, false);

					return processingResult;
					//}
				}

				if (externalUserIDValidationResult.IsError) {
					processingResult.IsSuccessful = false;
					processingResult.Error = new ProcessingError(externalUserIDValidationResult.ErrorMessage, externalUserIDValidationResult.ErrorDescription, false, false);
					return processingResult;
				}

				if (!externalUserIDValidationResult.IsActive) {
					processingResult.IsSuccessful = false;
					processingResult.Error = new ProcessingError("You are attempting to take an order but your promo code is inactive. If you think this is a mistake, please contact an Administrator.", "You are attempting to take an order but your promo code is inactive. If you think this is a mistake, please contact an Administrator.", false, false);
					return processingResult;
				}
				if (externalUserIDValidationResult.IsError)
				{
					processingResult.IsSuccessful = false;
					processingResult.Error = new ProcessingError(externalUserIDValidationResult.ErrorMessage, externalUserIDValidationResult.ErrorDescription, false, false);
					return processingResult;
				}

				if (!externalUserIDValidationResult.IsActive)
				{
					processingResult.IsSuccessful = false;
					processingResult.Error = new ProcessingError("You are attempting to take an order but your promo code is inactive. If you think this is a mistake, please contact an Administrator.", "You are attempting to take an order but your promo code is inactive. If you think this is a mistake, please contact an Administrator.", false, false);
					return processingResult;
				}
			}//end promocode check

            if (!userToValidate.IsActive) {
                validationResult.Errors.Add(ErrorValues.CannotTakeOrderWithInactiveUserUserMessage);
            }


            

            if (userToValidate.CompanyId != companyId) {
                // TODO: Figure out what this error should look like / if it can happen.
            }

            if (userToValidate.Company.IsDeleted) {
                // TODO: Figure out what this error should look like / does there need to be an active flag on companies?
            }

            // TODO: What if the user is a Admin or SalesGroup Manager?
            if (userToValidate.SalesTeam != null && !userToValidate.SalesTeam.IsActive) {
                validationResult.Errors.Add(ErrorValues.CannotTakeOrderWithInactiveSalesTeamUserMessage);
            }

            validationResult.IsValid = validationResult.Errors.Count == 0;
            processingResult.Data = validationResult;
            return processingResult;
        }

        public async Task<ServiceProcessingResult> UpdateUserOwnedByLoggedInUserAsync(ApplicationUser updatedLoggedInUser) {
            var processingResult = new ServiceProcessingResult();
            var ExistingUserResult = await GetUserInCompanyAsync(updatedLoggedInUser.Id, updatedLoggedInUser.Company.Id);

            if (ExistingUserResult.Data.Company.MinToChangeTeam < updatedLoggedInUser.Role.Rank) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.PERMISSION_ERROR;
                return processingResult;
            }

            using (var contextScope = DbContextScopeFactory.Create()) {
                try {
                    var repo = new ApplicationUserRepository();
                    processingResult =
                        repo.UpdateUserOwnedByLoggedInUser(updatedLoggedInUser)
                            .ToServiceProcessingResult(ErrorValues.GENERIC_UPDATE_USER_FAILED_ERROR);

                    await contextScope.SaveChangesAsync();
                } catch (Exception ex) {
                    ex.ToExceptionless()
                      .SetMessage("Failed to edit the current user.")
                      .AddTags("UpdateUserOwnedByLoggedInUserAsync")
                      .AddObject(updatedLoggedInUser)
                      .MarkAsCritical()
                      .Submit();
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_UPDATE_USER_FAILED_ERROR;
                    //Logger.Error("Failed to edit the current userToUpdate.", ex);
                }
            }

            return processingResult;
        }

        public async Task<ServiceProcessingResult<ApplicationUser>> UpdateUserNotOwnedByLoggedInUserAsync(ApplicationUser updatedUser, ApplicationRole roleBeingAssigned, ApplicationUser updatedByUser) {
            // TODO: Make sure that only Admins can update Permissions... and IsActive fields.
            var processingResult = await ValidateNonLoggedInUserUpdateAndReturnExistingUser(updatedUser, roleBeingAssigned, updatedByUser);

            if (!processingResult.IsSuccessful) {
                if (processingResult.IsFatalFailure()) {
                    ExceptionlessClient.Default.CreateLog(typeof(ApplicationUserDataService).FullName, String.Format(
                            "An error occurred while validating non logged in user update. Updated by User Id: {0}",
                            updatedUser.Id), "Error").AddTags("UpdateUserNotOwnedByLoggedInUserAsync").AddObject(updatedUser).Submit();

                    //var logMessage =
                    //    String.Format(
                    //        "An error occurred while validating non logged in user update. Updated by User Id: {0}",
                    //        updatedUser.Id);
                    //Logger.Error(logMessage);
                }
                return processingResult;
            }

            var existingUser = processingResult.Data;

            using (var contextScope = DbContextScopeFactory.Create()) {
                try {
                    var repo = new ApplicationUserRepository();
                    processingResult =
                        repo.UpdateUserNotOwnedByLoggedInUser(updatedUser)
                            .ToServiceProcessingResult(ErrorValues.GENERIC_UPDATE_USER_FAILED_ERROR);

                    await contextScope.SaveChangesAsync();
                    updatedUser.Role = existingUser.Role;
                } catch (Exception ex) {
                    processingResult.IsSuccessful = false;
                    ex.ToExceptionless()
                      .SetMessage("Failed to edit the current user.")
                      .AddTags("UpdateUserNotOwnedByLoggedInUserAsync")
                      .AddObject(updatedUser)
                      .MarkAsCritical()
                      .Submit();

                    string exMsg = ex.ToString().ToUpper();
                    Boolean isDuplicate = exMsg.Contains("THE DUPLICATE KEY VALUE IS (");
                    if (isDuplicate) {
                        int index1 = exMsg.IndexOf("THE DUPLICATE KEY VALUE IS");
                        int index2 = exMsg.IndexOf(").") + 1;
                        string keymsg = exMsg.Substring(index1, (index2 - index1));
                        processingResult.Error = new ProcessingError("Unable to edit this user.  There is a duplicate key:" + keymsg, "Unable to edit this user.  There is a duplicate key:" + keymsg, true);
                        //Logger.Error("Duplicate key failure on user update: " + keymsg, ex);
                    } else {
                        processingResult.Error = ErrorValues.GENERIC_UPDATE_USER_FAILED_ERROR;
                        ExceptionlessClient.Default.CreateLog(typeof(ApplicationUserDataService).FullName, "Failed to edit the user", "Error").AddTags("UpdateUserNotOwnedByLoggedInUserAsync").Submit();
                        //Logger.Error("Failed to edit the userToUpdate.", ex);
                    }

                }
            }

            return processingResult;
        }

        public async Task<ServiceProcessingResult<ValidationResult>> ValidateLoggedInUserCanResetPasswordForUser(string userId, string loggedInUserId) {
            var processingResult = new ServiceProcessingResult<ValidationResult> { IsSuccessful = true };
            var validationResult = new ValidationResult();

            var getUserForPasswordReset = await GetAsync(userId);
            var getLoggedInUser = await GetAsync(loggedInUserId);
            if (!getUserForPasswordReset.IsSuccessful || !getLoggedInUser.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_COULD_NOT_FIND_USER_ERROR;
                return processingResult;
            }
            var userForPasswordReset = getUserForPasswordReset.Data;
            var loggedInUser = getLoggedInUser.Data;

            if (loggedInUser.Role.Rank >= userForPasswordReset.Role.Rank) {
                validationResult.Errors.Add("Logged in user does not have permission to reset password for this user.");
            }

            if ((loggedInUser.Role.IsSuperAdmin() || loggedInUser.Role.IsAdmin()) && loggedInUser.Role.Rank < userForPasswordReset.Role.Rank && loggedInUser.CompanyId == userForPasswordReset.CompanyId) {
                validationResult.IsValid = true;
                processingResult.Data = validationResult;
                return processingResult;
            }

            var level1SalesGroupService = new Level1SalesGroupDataService();
            var loggedInUserSalesGroupTreeResult =
                await
                    level1SalesGroupService.GetCompanySalesGroupTreeWhereManagerInTree(loggedInUser.CompanyId,
                        loggedInUser.Id);
            if (!loggedInUserSalesGroupTreeResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_GET_SALES_GROUP_WITH_MANAGER_IN_TREE_ERROR;
                return processingResult;
            }
            var loggedInUsersSalesGroupTree = loggedInUserSalesGroupTreeResult.Data;

            var userForPasswordResetIsInSalesGroupTree = UserIsInSalesGroupTree(userForPasswordReset, loggedInUsersSalesGroupTree);

            if (!userForPasswordResetIsInSalesGroupTree) {
                validationResult.Errors.Add(
                    "The users whose password you are trying to reset does not fall within your sales group tree.");
            }

            validationResult.IsValid = validationResult.Errors.Count == 0;

            processingResult.Data = validationResult;
            return processingResult;
        }

        // This method does not return ValidationResult object because the ApplicationUser that's used 
        // to make validation checks is expected by the method that calls this method. This also prevents
        // the extra hit to the db that would be required if we were to return ValidationResult here.
        private async Task<ServiceProcessingResult<ApplicationUser>> ValidateNonLoggedInUserUpdateAndReturnExistingUser(
            ApplicationUser updatedUser, ApplicationRole roleBeingAssigned, ApplicationUser updatedByUser) {
            var processingResult = new ServiceProcessingResult<ApplicationUser> { IsSuccessful = true };
            if (updatedByUser.Role.Rank > ValidApplicationRoles.AdminRank)//not super admin then do regular check, super admin can do anything
            {                             // If role can't CRUD Sales User, then they can't CRUD any user, so just return here.
                if (!updatedByUser.Role.CanCreateOrUpdateSalesUser()) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.USER_UPDATE_PERMISSIONS_ERROR;
                    return processingResult;
                }
            }

            processingResult = await ValidateAndReturnExistingUserForUpdate(updatedUser, updatedByUser);

            if (!processingResult.IsSuccessful) {
                return processingResult;
            }

            var existingUser = processingResult.Data;

            var validationResult = new ValidationResult {
                IsValid = false
            };

            // Only need to validate SalesTeam ownership if the user was assigned to a different SalesTeam
            if (existingUser.SalesTeamId != updatedUser.SalesTeamId) {
                var salesTeamService = new SalesTeamDataService(updatedByUser.Id);
                var reassignedSalesTeamOwnershipValidationResult =
                    await salesTeamService.ValidateSalesTeamOwnershipAsync(updatedUser.SalesTeamId, roleBeingAssigned, updatedByUser);
                if (!reassignedSalesTeamOwnershipValidationResult.IsSuccessful) {
                    if (reassignedSalesTeamOwnershipValidationResult.IsFatalFailure()) {
                        //var logMessage =
                        //        //String.Format(
                        //        //    "An error occurred while validating sales team assignment during user update. Updated By User Id: {0}",
                        //        //    updatedByUser.Id);
                        //Logger.Error(logMessage);
                        ExceptionlessClient.Default.CreateLog(typeof(ApplicationUserDataService).FullName, String.Format("An error occurred while validating sales team assignment during user update. Updated By User Id: {0}", updatedByUser.Id), "Error").AddTags("ValidateNonLoggedInUserUpdateAndReturnExistingUser").Submit();
                    }

                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_UPDATE_USER_FAILED_ERROR;

                    return processingResult;
                }

                if (!reassignedSalesTeamOwnershipValidationResult.Data.IsValid) {
                    validationResult.Errors.AddRange(reassignedSalesTeamOwnershipValidationResult.Data.Errors);
                }

            }

            if (!updatedByUser.Role.CanAssign(roleBeingAssigned)) {
                validationResult.Errors.Add(ErrorValues.CannotAssignRoleToUserUserHelp);
            }

            if (validationResult.Errors.Count > 0) {
                processingResult.IsSuccessful = false;
                processingResult.Error = validationResult.ToProcessingError(ErrorValues.UserUpdateValidationFailedUserMessage);
                processingResult.Data = null;
            }

            return processingResult;
        }

        // This method does not return ValidationResult object because the ApplicationUser that's used 
        // to make validation checks is expected by the method that calls this method. This also prevents
        // the extra hit to the db that would be required if we were to return ValidationResult here.
        private async Task<ServiceProcessingResult<ApplicationUser>> ValidateAndReturnExistingUserForUpdate(ApplicationUser updatedUser, ApplicationUser updatedByUser) {
            var processingResult = new ServiceProcessingResult<ApplicationUser>();

            var existingUserResult = await GetUserInCompanyAsync(updatedUser.Id, updatedUser.CompanyId);


            if (!existingUserResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.COULD_NOT_FIND_USER_TO_UPDATE_ERROR;

                if (existingUserResult.IsFatalFailure()) {
                    ExceptionlessClient.Default.CreateLog(typeof(ApplicationUserDataService).FullName, String.Format(
                            "An error occurred while retrieving user in company during non-logged in user update. Updated by User with Id: {0}",
                            updatedByUser.Id), "Error").AddTags("ValidateAndReturnExistingUserForUpdate").Submit();
                    //var logMessage =
                    //    String.Format(
                    //        "An error occurred while retrieving user in company during non-logged in user update. Updated by User with Id: {0}",
                    //        updatedByUser.Id);
                    //Logger.Error(logMessage);

                    processingResult.Error = ErrorValues.GENERIC_UPDATE_USER_FAILED_ERROR;
                }

                return processingResult;
            }

            //compares minimum rank to update company(company value is current company)
            if (updatedByUser.Role.Rank > existingUserResult.Data.Company.MinToChangeTeam) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.PERMISSION_ERROR;
                return processingResult;
            }

            var existingUser = existingUserResult.Data;

            // Admin users can't be edited by anybody besides the actual User, and all Level Managers can only be edited by Admins
            if ((existingUser.Role.IsSuperAdmin() || (existingUser.Role.IsAdmin() && !updatedByUser.Role.IsSuperAdmin())) || (updatedByUser.Role.Rank > ValidApplicationRoles.LevelThreeManagerRank) || (updatedByUser.Role.Rank >= existingUser.Role.Rank)) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.USER_UPDATE_PERMISSIONS_ERROR;
                return processingResult;
            }

            var salesTeamService = new SalesTeamDataService(updatedByUser.Id);

            // Level Managers can only edit sales users that are assigned to a sales team that fall under their ownership,
            // so validate that here
            if (!existingUser.Role.IsNonSalesRole() && updatedByUser.Role.IsLevelManager()) {
                var salesTeamOwnershipValidationResult = await salesTeamService.ValidateSalesTeamOwnershipAsync(
                        existingUser.SalesTeamId, existingUser.Role, updatedByUser
                    );
                if (!salesTeamOwnershipValidationResult.IsSuccessful || !salesTeamOwnershipValidationResult.Data.IsValid) {
                    if (salesTeamOwnershipValidationResult.IsFatalFailure()) {
                        //var logMessage =
                        //    String.Format(
                        //        "An error occurred while validating sales team ownership of the existing user during non-logged in user update. Updated by User with Id: {0}",
                        //        updatedByUser.Id);
                        //Logger.Error(logMessage);
                        ExceptionlessClient.Default.CreateLog(typeof(ApplicationUserDataService).FullName, String.Format(
                                "An error occurred while validating sales team ownership of the existing user during non-logged in user update. Updated by User with Id: {0}",
                                updatedByUser.Id), "Error").AddTags("ValidateAndReturnExistingUserForUpdate").Submit();
                    }

                    processingResult.IsSuccessful = false;
                    processingResult.Error = !salesTeamOwnershipValidationResult.IsSuccessful
                        ? ErrorValues.GENERIC_UPDATE_USER_FAILED_ERROR
                        : salesTeamOwnershipValidationResult.Data.ToProcessingError(ErrorValues.UserUpdateValidationFailedUserMessage);

                    return processingResult;
                }
            }
            processingResult.Data = existingUser;
            processingResult.IsSuccessful = true;

            return processingResult;
        }

        private static bool UserIsInSalesGroupTree(ApplicationUser user, IEnumerable<GroupTreeViewBindingModel> salesGroupTree) {
            foreach (var level1SalesGroup in salesGroupTree) {
                if (level1SalesGroup.Managers.Any(m => m.Id == user.Id)) {
                    return true;
                }

                if (level1SalesGroup.ChildSalesGroups.Any(csg => csg.Managers.Any(m => m.Id == user.Id))) {
                    return true;
                }

                if (level1SalesGroup.ChildSalesGroups.Any(csg => csg.ChildSalesGroups.Any(ccsg => ccsg.Managers.Any(m => m.Id == user.Id)))) {
                    return true;
                }

                if (user.SalesTeamId != null &&
                    level1SalesGroup.ChildSalesGroups.Any(
                        csg => csg.ChildSalesGroups.Any(ccsg => ccsg.SalesTeams.Any(st => st.Id == user.SalesTeamId)))) {
                    return true;
                }
            }

            return false;
        }
        public async Task<ServiceProcessingResult<List<UserManagedGroupsBindingModel>>> GetManagedGroups(string userId) {
            var processingResult = new ServiceProcessingResult<List<UserManagedGroupsBindingModel>> { };

            var groupsManagedResult = new List<UserManagedGroupsBindingModel>();



            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            var sqlString = "SELECT * FROM v_GroupsManaged WHERE UserID=@UserID ORDER BY GroupName";


            SqlCommand cmd = new SqlCommand(sqlString, connection);
            SqlDataReader rdr = null;
            cmd.Parameters.Clear();
            cmd.Parameters.Add(new SqlParameter("@UserID", userId));


            try {
                try {
                    connection.Open();
                    rdr = cmd.ExecuteReader();
                } catch (Exception ex) {
                    processingResult.IsSuccessful = false;
                    ex.ToExceptionless()
                    .SetMessage("Failed to get groups managed by user.")
                    .AddTags("GetManagedGroups")
                    .AddObject(userId)
                    .MarkAsCritical()
                    .Submit();

                    processingResult.Error = new ProcessingError("Failed to get groups managed by user.", "Failed to get groups managed by user.", true, false);
                    //Logger.Error(String.Format("Groups managed by user database call failed (Lookup User: {0})",userId),ex);
                    return processingResult;
                }
                while (rdr.Read()) {

                    groupsManagedResult.Add(new UserManagedGroupsBindingModel {
                        UseID = rdr["UserID"].ToString(),
                        GroupName = rdr["GroupName"].ToString()
                    });
                }
            } catch (Exception ex1) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Failed to get groups managed by user.", "Failed to get groups managed by user.", true, false);
                ex1.ToExceptionless()
                   .SetMessage(String.Format("Groups managed by user database call failed (Lookup User: {0})", userId))
                   .AddTags("GetManagedGroups")
                   .AddObject(userId)
                   .MarkAsCritical()
                   .Submit();

                //Logger.Error(String.Format("Groups managed by user database call failed (Lookup User: {0})",userId,ex1);
                return processingResult;

            } finally { connection.Close(); }

            processingResult.IsSuccessful = true;
            processingResult.Data = groupsManagedResult;
            return processingResult;

        }
        public async Task<ServiceProcessingResult<ApplicationUser>> GetUserInLoggedInUsersSalesGroupTreeAsync(string userId, ApplicationUser loggedInUser, string companyId) {
            var processingResult = new ServiceProcessingResult<ApplicationUser>();

            var getUserResult = await GetUserInCompanyAsync(userId, companyId);
            if (!getUserResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_COULD_NOT_FIND_USER_ERROR;
                return processingResult;
            }

            var user = getUserResult.Data;

            if ((loggedInUser.Role.IsAdmin() || loggedInUser.Role.IsSuperAdmin()) && loggedInUser.Role.Rank < user.Role.Rank && loggedInUser.CompanyId == user.CompanyId) {
                processingResult.IsSuccessful = true;
                processingResult.Data = user;
                return processingResult;
            }

            var validateUserIsInLoggedInUsersSalesTreeResult = await ValidateUserIsInLoggedInUsersSalesTreeAsync(loggedInUser.Id, user.Id, user.SalesTeamId, companyId);
            if (!validateUserIsInLoggedInUsersSalesTreeResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_VALIDATE_USER_PERMISSIONS_ERROR;
                return processingResult;
            }
            var validationResult = validateUserIsInLoggedInUsersSalesTreeResult.Data;
            if (!validationResult.IsValid) {
                processingResult.IsSuccessful = false;
                processingResult.Error = validationResult.ToProcessingError("Validation failed for editing this user.");
                return processingResult;
            }

            if (loggedInUser.Role.Rank >= user.Role.Rank) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.USER_CRUD_PERMISSIONS_ERROR;
                return processingResult;
            }

            processingResult.Data = user;
            processingResult.IsSuccessful = true;

            return processingResult;
        }

        public async Task<ServiceProcessingResult<ApplicationUser>> GetUserInCompanyAsync(string userId, string companyId, bool includeSalesTeamAndParents = false) {
            var processingResult = new ServiceProcessingResult<ApplicationUser>();

            using (var contextScope = DbContextScopeFactory.CreateReadOnly()) {
                try {
                    var includes = includeSalesTeamAndParents
                        ? new[]
                        {
                            "SalesTeam",
                            "SalesTeam.Level3SalesGroup.Managers.Roles.Role",
                            "SalesTeam.Level3SalesGroup.ParentSalesGroup.Managers.Roles.Role",
                            "SalesTeam.Level3SalesGroup.ParentSalesGroup.ParentSalesGroup.Managers.Roles.Role"
                        }
                        : new string[] { "Company" };
                    processingResult = await GetWhereAsync(u => u.Id == userId && u.CompanyId == companyId, includes);

                    if (!processingResult.IsSuccessful) {
                        //Logger.Error("Failed to get user in company.");

                        ExceptionlessClient.Default.CreateLog(typeof(ApplicationUserDataService).FullName, "Failed to get user in company.", "Error").AddTags("GetUserInCompanyAsync").Submit();
                        processingResult.Error = ErrorValues.GENERIC_COULD_NOT_FIND_USER_ERROR;
                    }
                    if (processingResult.Data == null) {
                        processingResult.IsSuccessful = false;
                        processingResult.Error = ErrorValues.GENERIC_COULD_NOT_FIND_USER_ERROR;
                    }

                } catch (Exception ex) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_COULD_NOT_FIND_USER_ERROR;
                    ex.ToExceptionless()
                     .SetMessage(String.Format("An error occured while trying to get User of Id: {0} in Company with Id: {1}", userId, companyId))
                     .AddTags("GetUserInCompanyAsync")
                     .AddObject(userId)
                     .MarkAsCritical()
                     .Submit();

                    //var logMessage =
                    //    String.Format("An error occured while trying to get User of Id: {0} in Company with Id: {1}",
                    //        userId, companyId);
                    //Logger.Error(logMessage);
                }
            }

            return processingResult;
        }

        public ServiceProcessingResult<List<ApplicationUser>> GetUsersInCompanyFromIDList(List<string> userIds, string companyId) {
            var processingResult = GetAllWhere(u => userIds.Contains(u.Id) && u.CompanyId == companyId);
            if (!processingResult.IsSuccessful) {
                processingResult.Error = ErrorValues.GENERIC_COULD_NOT_FIND_USERS_ERROR;
                //var logMessage = String.Format("Failed to Get Users in ID List in company with id: {0}.", companyId);
                //Logger.Error(logMessage);

                ExceptionlessClient.Default.CreateLog(typeof(ApplicationUserDataService).FullName, String.Format("Failed to Get Users in ID List in company with id: {0}.", companyId), "Error").AddTags(" GetUsersInCompanyFromIDList").Submit();
            }

            return processingResult;
        }

        public async Task<ServiceProcessingResult> MarkUserAsDeletedAsync(string userIdToDelete, string loggedInUserCompanyId,
           ApplicationUser loggedInUser) {
            var processingResult = new ServiceProcessingResult();

            if (!loggedInUser.Role.IsAdmin() && !loggedInUser.Role.IsSuperAdmin()) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.USER_DELETE_PERMISSIONS_ERROR;
                return processingResult;
            }

            var userToDeleteResult = await GetUserInCompanyAsync(userIdToDelete, loggedInUserCompanyId);

            if (!userToDeleteResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = userToDeleteResult.Error.CanBeFixedByUser
                    ? ErrorValues.COULD_NOT_FIND_USER_TO_DELETE_ERROR
                    : ErrorValues.GENERIC_DELETE_USER_FAILED_ERROR;
                return processingResult;
            }

            _roleService = new ApplicationRoleDataService();

            var userToDelete = userToDeleteResult.Data;
            userToDelete.ModifiedByUserId = loggedInUser.Id;

            var userRole = userToDelete.Roles.ToList()[0];
            var userToDeleteRole = _roleService.Get(userRole.RoleId).Data;

            using (var contextScope = DbContextScopeFactory.Create()) {
                try {
                    var repo = new ApplicationUserRepository();

                    processingResult =
                        repo.MarkUserAsDeleted(userToDelete)
                            .ToServiceProcessingResult(ErrorValues.GENERIC_DELETE_USER_FAILED_ERROR);
                    if (!processingResult.IsSuccessful) {
                        //Logger.Error("Failed to mark user as deleted in the repository.");

                        ExceptionlessClient.Default.CreateLog(typeof(ApplicationUserDataService).FullName, "Failed to mark user as deleted.", "Error").AddTags("MarkUserAsDeletedAsync").Submit();
                        return processingResult;
                    }

                    await contextScope.SaveChangesAsync();
                } catch (Exception ex) {
                    processingResult.IsSuccessful = false;
                    ex.ToExceptionless()
                     .SetMessage("Failed to save changes to mark user as deleted.")
                     .AddTags("MarkUserAsDeletedAsync")
                     .AddObject(userIdToDelete)
                     .MarkAsCritical()
                     .Submit();
                    processingResult.Error = ErrorValues.GENERIC_DELETE_USER_FAILED_ERROR;
                    //Logger.Error("Failed to save changes to mark user as deleted.", ex);
                }
            }

            return processingResult;
        }


        public override ServiceProcessingResult Delete(string id) {
            throw new NotImplementedException("Delete is not implemented for ApplicationUser. Please use the MarkAsDeleted method instead.");
        }

        public override ServiceProcessingResult<ApplicationUser> Update(ApplicationUser entity) {
            throw new NotImplementedException("Update cannot be called for an ApplicationUser. Please use one of the other provided update methods.");
        }

        public async Task<ServiceProcessingResult<ManagerViewBindingModel>> GetsGroupManagersForLoggeInUserAsync(string companyId, ApplicationUser loggedInUser) {
            var processingResult = new ServiceProcessingResult<ManagerViewBindingModel>();
            //Get Users in Tree

            var prelimUsers = new List<UserNameRoleViewBindingModel>();
            var users = new List<UserNameViewBindingModel>();

            if (loggedInUser.Role.IsAdmin() || loggedInUser.Role.IsSuperAdmin()) {
                var getAllUsersInCompanyResult = await GetAllWhereAsync(u => u.CompanyId == companyId);
                if (!getAllUsersInCompanyResult.IsSuccessful) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_COULD_NOT_FIND_USERS_ERROR;
                    return processingResult;
                }

                foreach (var user in getAllUsersInCompanyResult.Data) {
                    prelimUsers.Add(user.ToUserNameRoleViewBindingModel());
                }
            } else if (loggedInUser.Role.IsLevelManager()) {
                var level1SalesGroupService = new Level1SalesGroupDataService();
                var salesGroupTreeWhereUserInTree = await level1SalesGroupService.GetCompanySalesGroupTreeWhereManagerInTree(companyId, loggedInUser.Id);
                if (!salesGroupTreeWhereUserInTree.IsSuccessful) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_GET_SALES_GROUP_WITH_MANAGER_IN_TREE_ERROR;
                    return processingResult;
                }
                var salesGroupTree = salesGroupTreeWhereUserInTree.Data;

                if (loggedInUser.Role.Rank < ApplicationRoleRulesHelper.Level3SalesGroupManagerRank) {
                    prelimUsers.AddRange(PluckManagersFromSalesGroupTree(salesGroupTree));
                }

                var salesTeamIds = PluckSalesTeamIdsFromSalesGroupTree(salesGroupTree);

                var getUsersAssignedToSalesTeamsResult = await GetAllWhereAsync(u => salesTeamIds.Contains(u.SalesTeamId));
                if (!getUsersAssignedToSalesTeamsResult.IsSuccessful) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_COULD_NOT_FIND_USER_ERROR;
                    return processingResult;
                }

                foreach (var user in getUsersAssignedToSalesTeamsResult.Data) {
                    prelimUsers.Add(user.ToUserNameRoleViewBindingModel());
                }
            }

            //Take out users not under logged in users controll
            prelimUsers = prelimUsers.Where(u => u.Role != null && u.Role.Rank > loggedInUser.Role.Rank).ToList();
            prelimUsers = prelimUsers.DistinctBy(a => a.Id).ToList();

            _roleService = new ApplicationRoleDataService();
            var level1SalesGroupManagerRoleId =
                _roleService.GetApplicationRoleByRank(ApplicationRoleRulesHelper.Level1SalesGroupManagerRank).Data.Id;
            var level2SalesGroupManagerRoleId =
              _roleService.GetApplicationRoleByRank(ApplicationRoleRulesHelper.Level2SalesGroupManagerRank).Data.Id;
            var level3SalesGroupManagerRoleId =
              _roleService.GetApplicationRoleByRank(ApplicationRoleRulesHelper.Level3SalesGroupManagerRank).Data.Id;
            var TeamManagerRoleId =
              _roleService.GetApplicationRoleByRank(ApplicationRoleRulesHelper.SalesTeamManagerRank).Data.Id;

            var ManagerGroups = new ManagerViewBindingModel();
            ManagerGroups.Level1Managers = new List<UserNameViewBindingModel>();
            ManagerGroups.Level2Managers = new List<UserNameViewBindingModel>();
            ManagerGroups.Level3Managers = new List<UserNameViewBindingModel>();
            ManagerGroups.TeamManagers = new List<UserNameViewBindingModel>();

            //pull out managers
            foreach (var user in prelimUsers) {
                if (user.Role.Id == level1SalesGroupManagerRoleId) {
                    ManagerGroups.Level1Managers.Add(user.ToUserNameViewBindingModel());
                } else if (user.Role.Id == level2SalesGroupManagerRoleId) {
                    ManagerGroups.Level2Managers.Add(user.ToUserNameViewBindingModel());
                } else if (user.Role.Id == level3SalesGroupManagerRoleId) {
                    ManagerGroups.Level3Managers.Add(user.ToUserNameViewBindingModel());
                } else if (user.Role.Id == TeamManagerRoleId) {
                    ManagerGroups.TeamManagers.Add(user.ToUserNameViewBindingModel());
                }
            }

            processingResult.Data = ManagerGroups;
            processingResult.IsSuccessful = true;

            return processingResult;
        }

        public async Task<ServiceProcessingResult<List<ApplicationUser>>> GetLevel2SalesGroupManagersForCompanyAsync(
            string companyId) {
            _roleService = new ApplicationRoleDataService();
            var level2SalesGroupManagerRoleId =
                _roleService.GetApplicationRoleByRank(ApplicationRoleRulesHelper.Level2SalesGroupManagerRank).Data.Id;

            var processingResult = await GetAllWhereAsync(
                    u => u.Roles.FirstOrDefault().RoleId == level2SalesGroupManagerRoleId && u.CompanyId == companyId
                );
            if (!processingResult.IsSuccessful) {
                processingResult.Error = ErrorValues.GENERIC_COULD_NOT_FIND_MANAGERS_ERROR;
                //var logMessage = String.Format("Failed to get Level 2 Sales Group Managers for Company with Id: {0}",
                //    companyId);
                //Logger.Error(logMessage);

                ExceptionlessClient.Default.CreateLog(typeof(ApplicationUserDataService).FullName, String.Format("Failed to get Level 2 Sales Group Managers for Company with Id: {0}",
                    companyId), "Error").AddTags("GetLevel2SalesGroupManagersForCompanyAsync").Submit();
            }

            return processingResult;
        }

        public async Task<ServiceProcessingResult<List<ApplicationUser>>> GetLevel3SalesGroupManagersForCompanyAsync(
            string companyId) {
            _roleService = new ApplicationRoleDataService();
            var level3SalesGroupManagerRoleId =
                _roleService.GetApplicationRoleByRank(ApplicationRoleRulesHelper.Level3SalesGroupManagerRank).Data.Id;

            var processingResult = await GetAllWhereAsync(
                    u => u.Roles.FirstOrDefault().RoleId == level3SalesGroupManagerRoleId && u.CompanyId == companyId
                );
            if (!processingResult.IsSuccessful) {
                processingResult.Error = ErrorValues.GENERIC_COULD_NOT_FIND_MANAGERS_ERROR;
                //var logMessage = String.Format("Failed to get Level 3 Sales Group Managers for Company with Id: {0}",
                //    companyId);
                //Logger.Error(logMessage);

                ExceptionlessClient.Default.CreateLog(typeof(ApplicationUserDataService).FullName, String.Format("Failed to get Level 3 Sales Group Managers for Company with Id: {0}",
                    companyId), "Error").AddTags("GetLevel3SalesGroupManagersForCompanyAsync").Submit();
            }

            return processingResult;
        }

        public async Task<ServiceProcessingResult<List<ApplicationUser>>> GetSalesTeamManagersForCompanyAsync(
            string companyId) {
            _roleService = new ApplicationRoleDataService();
            var salesTeamManagerRoleId =
                _roleService.GetApplicationRoleByRank(ApplicationRoleRulesHelper.SalesTeamManagerRank).Data.Id;

            var processingResult =
                await
                    GetAllWhereAsync(
                        u => u.Roles.FirstOrDefault().RoleId == salesTeamManagerRoleId && u.CompanyId == companyId);
            if (!processingResult.IsSuccessful) {
                processingResult.Error = ErrorValues.GENERIC_COULD_NOT_FIND_MANAGERS_ERROR;
                //var logMessage = String.Format("Failed to get Sales Team Managers for Company with Id: {0}", companyId);
                //Logger.Error(logMessage);

                ExceptionlessClient.Default.CreateLog(typeof(ApplicationUserDataService).FullName, String.Format("Failed to get Sales Team Managers for Company with Id: {0}", companyId), "Error").AddTags("GetSalesTeamManagersForCompanyAsync").Submit();
            }

            return processingResult;
        }

        public async Task<ServiceProcessingResult<List<ApplicationUser>>> GetSalesTeamManagersAsync(string salesTeamId,
            string companyId) {
            _roleService = new ApplicationRoleDataService();
            var salesTeamManagerRoleId =
                _roleService.GetApplicationRoleByRank(ApplicationRoleRulesHelper.SalesTeamManagerRank).Data.Id;

            var processingResult =
                await
                    GetAllWhereAsync(
                        u =>
                            u.Roles.FirstOrDefault().RoleId == salesTeamManagerRoleId && u.CompanyId == companyId &&
                            u.SalesTeamId == salesTeamId);

            if (!processingResult.IsSuccessful) {
                processingResult.Error = ErrorValues.GENERIC_COULD_NOT_FIND_MANAGERS_ERROR;


                ExceptionlessClient.Default.CreateLog(typeof(ApplicationUserDataService).FullName, String.Format("Failed to get Managers for Sales Team with Id: {0}", salesTeamId), "Error").AddTags("GetSalesTeamManagersAsync").Submit();
                //var logMessage = String.Format("Failed to get Managers for Sales Team with Id: {0}", salesTeamId);
                //Logger.Error(logMessage);
            }

            return processingResult;
        }

        public async Task<ServiceProcessingResult<ValidationResult>> ValidateUserCanBeDeletedAsync(ApplicationUser user) {
            var processingResult = new ServiceProcessingResult<ValidationResult> { IsSuccessful = true };
            var validationResult = new ValidationResult();

            if (user.Role.Rank == ValidApplicationRoles.LevelOneManagerRank) {
                return await ValidateLevel1ManagerCanBeDeletedAsync(user.Id, user.CompanyId);
            }

            if (user.Role.Rank == ValidApplicationRoles.LevelTwoManagerRank) {
                return await ValidateLevel2ManagerCanBeDeletedAsync(user.Id, user.CompanyId);
            }

            if (user.Role.Rank == ValidApplicationRoles.LevelThreeManagerRank) {
                return await ValidateLevel3ManagerCanBeDeletedAsync(user.Id, user.CompanyId);
            }

            if (!user.Role.IsNonSalesRole()) {
                validationResult.IsValid = true;
            } else if (user.Role.IsAdmin() || user.Role.IsSuperAdmin()) {
                validationResult.IsValid = false;
                validationResult.Errors.Add(ErrorValues.AdminsCannotBeDeletedUserHelp);
            }
            processingResult.Data = validationResult;

            return processingResult;
        }

        private async Task<ServiceProcessingResult<ValidationResult>> ValidateUserIsInLoggedInUsersSalesTreeAsync(string loggedInUserId, string userId, string userSalesTeamId, string companyId) {
            var processingResult = new ServiceProcessingResult<ValidationResult> { IsSuccessful = true };
            var validationResult = new ValidationResult { IsValid = false };
            var level1SalesGroupService = new Level1SalesGroupDataService();
            var salesGroupTree = new ServiceProcessingResult<List<GroupAdminTreeViewBindingModel>>();

            var userToEditResult = await GetAsync(userId);
            if (!userToEditResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = userToEditResult.Error;
                //Logger.Error("Error getting (userToEditResult) in ValidateUserIsInLoggedInUsersSalesTreeAsync UserID: " + userId + " LoggedInUserID: " + userSalesTeamId);

                ExceptionlessClient.Default.CreateLog(typeof(ApplicationUserDataService).FullName, "Error getting (userToEditResult) in ValidateUserIsInLoggedInUsersSalesTreeAsync UserID: " + userId + " LoggedInUserID: " + userSalesTeamId, "Error").AddTags("ValidateUserIsInLoggedInUsersSalesTreeAsync").Submit();
                return processingResult;
            }

            var userDoingEditting = await GetAsync(loggedInUserId);
            if (!userDoingEditting.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = userDoingEditting.Error;

                ExceptionlessClient.Default.CreateLog(typeof(ApplicationUserDataService).FullName, "Error getting (userDoingEditting) in ValidateUserIsInLoggedInUsersSalesTreeAsync. UserID: " + userId + " LoggedInUserID: " + userSalesTeamId, "Error").AddTags("ValidateUserIsInLoggedInUsersSalesTreeAsync").Submit();

                //Logger.Error("Error getting (userDoingEditting) in ValidateUserIsInLoggedInUsersSalesTreeAsync. UserID: " + userId + " LoggedInUserID: " + userSalesTeamId);
                return processingResult;
            }

            if (userDoingEditting.Data.Role.IsSuperAdmin() || userDoingEditting.Data.Role.IsAdmin()) { //This will also cover when the userToEdit is a SuperAdmin (should never happen) or an Admin (in which case the person doing the editting has to be a SuperAdmin)
                validationResult.IsValid = userDoingEditting.Data.CompanyId == userToEditResult.Data.CompanyId;
            } else if (userToEditResult.Data.Role.IsLevelManager()) {
                var groupManagers = new ServiceProcessingResult<List<UserNameViewBindingModel>>();
                if (userToEditResult.Data.Role.Name == "Level 1 Manager") {
                    groupManagers = await level1SalesGroupService.getManagersInTree(userDoingEditting.Data.CompanyId, userDoingEditting.Data.Id);
                } else if (userToEditResult.Data.Role.Name == "Level 2 Manager") {
                    var level2SalesGroupService = new Level2SalesGroupDataService();
                    groupManagers = await level2SalesGroupService.getManagersInTree(userDoingEditting.Data.CompanyId, userDoingEditting.Data.Id);
                } else {
                    var level3SalesGroupService = new Level3SalesGroupDataService();
                    groupManagers = await level3SalesGroupService.getManagersInTree(userDoingEditting.Data.CompanyId, userDoingEditting.Data.Id);
                }
                foreach (var manager in groupManagers.Data) {
                    if (manager.Id == userId) {
                        validationResult.IsValid = true;
                        break;
                    }
                }
            } else {
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                var sqlString = "SELECT Id FROM v_UserActiveTeams WHERE UserID=@UserID";
                SqlCommand cmd = new SqlCommand(sqlString, connection);
                SqlDataReader rdr = null;
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqlParameter("@UserID", loggedInUserId));

                try {
                    connection.Open();
                    rdr = cmd.ExecuteReader();

                    while (rdr.Read()) {
                        var test = rdr["Id"].ToString();
                        if (rdr["Id"].ToString() == userToEditResult.Data.SalesTeamId) {
                            validationResult.IsValid = true;
                            break;
                        }
                    }
                } catch (Exception ex) {
                    ex.ToExceptionless()
                     .SetMessage(String.Format("An error occurred attempting to retrieve/process teams for User with Id: {0} from Company with Id: {1}", loggedInUserId, companyId))
                     .AddTags("ValidateUserIsInLoggedInUsersSalesTreeAsync")
                     .MarkAsCritical()
                     .Submit();

                    //var logMessage = String.Format("An error occurred attempting to retrieve/process teams for User with Id: {0} from Company with Id: {1}", loggedInUserId, companyId);
                    //Logger.Error(logMessage);

                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("Error attempting to validate user not assigned as level 1 sales group manager.", "Error attempting to validate user not assigned as level 1 sales group manager.", true, false);
                    return processingResult;
                }

            }

            if (!validationResult.IsValid) {
                validationResult.Errors.Add("The user was not found in the Sales Group Tree.");
            }

            processingResult.Data = validationResult;
            return processingResult;

            //var level1SalesGroupService = new Level1SalesGroupDataService();
            //var salesGroupTreeResult = await level1SalesGroupService.GetCompanySalesGroupTreeWhereManagerInTree(companyId,
            //if (!salesGroupTreeResult.IsSuccessful) {
            //    processingResult.IsSuccessful = false;
            //    processingResult.Error = ErrorValues.GENERIC_GET_SALES_GROUP_WITH_MANAGER_IN_TREE_ERROR;
            //    return processingResult;
            //}
            //var salesGroupTree = salesGroupTreeResult.Data;

            //var validationResult = new ValidationResult {
            //    IsValid = false
            //};

            //foreach (var level1SalesGroup in salesGroupTree) {
            //    if (level1SalesGroup.Managers.Any(m => m.Id == userId)) {
            //        validationResult.IsValid = true;
            //        break;
            //    }

            //    foreach (var level2SalesGroup in level1SalesGroup.ChildSalesGroups) {
            //        if (level2SalesGroup.Managers.Any(m => m.Id == userId)) {
            //            validationResult.IsValid = true;
            //            break;
            //        }

            //        foreach (var level3SalesGroup in level2SalesGroup.ChildSalesGroups) {
            //            if (level3SalesGroup.Managers.Any(m => m.Id == userId)) {
            //                validationResult.IsValid = true;
            //                break;
            //            }

            //            if (level3SalesGroup.SalesTeams.Any(st => st.Id == userSalesTeamId)) {
            //                validationResult.IsValid = true;
            //                break;
            //            }
            //        }
            //    }
            //}
        }

        private static IEnumerable<UserNameRoleViewBindingModel> PluckManagersFromSalesGroupTree(IEnumerable<GroupTreeViewBindingModel> salesGroupTree) {
            var usersToReturn = new List<UserNameRoleViewBindingModel>();

            foreach (var level1SalesGroup in salesGroupTree) {
                usersToReturn.AddRange(level1SalesGroup.Managers);

                foreach (var level2SalesGroup in level1SalesGroup.ChildSalesGroups) {
                    foreach (var manager in level2SalesGroup.Managers) {
                        if (!usersToReturn.Any(item => item.Id == manager.Id)) {
                            usersToReturn.AddRange(level2SalesGroup.Managers);
                        }
                    }

                    foreach (var level3SalesGroup in level2SalesGroup.ChildSalesGroups) {
                        foreach (var manager in level3SalesGroup.Managers) {
                            if (!usersToReturn.Any(item => item.Id == manager.Id)) {
                                usersToReturn.AddRange(level3SalesGroup.Managers);
                            }
                        }
                    }
                }
            }

            return usersToReturn;
        }

        private static IEnumerable<UserListViewBindingModel> PluckManagersFromUserTree(IEnumerable<UserTreeViewBindingModel> salesGroupTree) {
            var usersToReturn = new List<UserListViewBindingModel>();

            foreach (var level1SalesGroup in salesGroupTree) {
                usersToReturn.AddRange(level1SalesGroup.Managers);

                foreach (var level2SalesGroup in level1SalesGroup.ChildSalesGroups) {
                    foreach (var manager in level2SalesGroup.Managers) {
                        if (!usersToReturn.Any(item => item.Id == manager.Id)) {
                            usersToReturn.Add(manager);
                        }
                    }

                    foreach (var level3SalesGroup in level2SalesGroup.ChildSalesGroups) {
                        foreach (var manager in level3SalesGroup.Managers) {
                            if (!usersToReturn.Any(item => item.Id == manager.Id)) {
                                usersToReturn.Add(manager);
                            }
                        }
                    }
                }
            }

            return usersToReturn;
        }

        private static IEnumerable<string> PluckSalesTeamIdsFromSalesGroupTree(IEnumerable<GroupTreeViewBindingModel> salesGroupTree) {
            var salesTeamIds = new List<string>();

            foreach (var level3SalesGroup in from level1SalesGroup in salesGroupTree from level2SalesGroup in level1SalesGroup.ChildSalesGroups from level3SalesGroup in level2SalesGroup.ChildSalesGroups select level3SalesGroup) {
                salesTeamIds.AddRange(level3SalesGroup.SalesTeams.Select(st => st.Id).ToList());
            }

            return salesTeamIds;
        }

        private static IEnumerable<string> PluckSalesTeamIdsFromUserTree(IEnumerable<UserTreeViewBindingModel> salesGroupTree) {
            var salesTeamIds = new List<string>();

            foreach (var level3SalesGroup in from level1SalesGroup in salesGroupTree from level2SalesGroup in level1SalesGroup.ChildSalesGroups from level3SalesGroup in level2SalesGroup.ChildSalesGroups select level3SalesGroup) {
                salesTeamIds.AddRange(level3SalesGroup.SalesTeams.Select(st => st.Id).ToList());
            }

            return salesTeamIds;
        }

        private static async Task<ServiceProcessingResult<ValidationResult>> ValidateLevel1ManagerCanBeDeletedAsync(string userId, string companyId) {
            var level1SalesGroupDataService = new Level1SalesGroupDataService();
            return await level1SalesGroupDataService.ValidateUserIsNotAssignedAsLevel1SalesGroupManagerInCompany(userId, companyId);
        }

        private static async Task<ServiceProcessingResult<ValidationResult>> ValidateLevel2ManagerCanBeDeletedAsync(string userId, string companyId) {
            var level2SalesGroupDataService = new Level2SalesGroupDataService();
            return await level2SalesGroupDataService.ValidateUserIsNotAssignedAsLevel2SalesGroupManagerInCompany(userId, companyId);
        }

        private static async Task<ServiceProcessingResult<ValidationResult>> ValidateLevel3ManagerCanBeDeletedAsync(string userId, string companyId) {
            var level3SalesGroupDataService = new Level3SalesGroupDataService();
            return await level3SalesGroupDataService.ValidateUserIsNotAssignedAsLevel3SalesGroupManagerInCompany(userId, companyId);
        }
        public async Task<ServiceProcessingResult<List<UserSearchViewBindingModel>>> SearchUsersInCompany(string searchString, string companyId, ApplicationUser loggedInUser) {
            var processingResult = new ServiceProcessingResult<List<UserSearchViewBindingModel>> { IsSuccessful = true };
            using (var contextScope = DbContextScopeFactory.Create()) {
                try {
                    List<UserSearchViewBindingModel> user = new List<UserSearchViewBindingModel>();
                    ApplicationRole role;
                    ApplicationUserRole userRole;
                    string connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                    SqlConnection connection = new SqlConnection(connectionstring);
                    SqlDataReader rdr = null;
                    SqlCommand cmd = new SqlCommand("usp_GetUsers", connection);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter("@FilterUserName", searchString));
                    cmd.Parameters.Add(new SqlParameter("@UserId", loggedInUser.Id));
                    try {
                        connection.Open();
                        rdr = cmd.ExecuteReader();
                        while (rdr.Read()) {
                            role = new ApplicationRole() { Name = rdr["Name"].ToString(), Rank = rdr.GetInt32(rdr.GetOrdinal("Rank")) };
                            userRole = new ApplicationUserRole() { UserId = rdr["Id"].ToString(), RoleId = "" };
                            user.Add(new UserSearchViewBindingModel() {
                                FirstName = rdr["FirstName"].ToString(),
                                LastName = rdr["LastName"].ToString(),
                                UserName = rdr["UserName"].ToString(),
                                Team=rdr["Team"].ToString(),
                                Id = rdr["Id"].ToString(),
                                ExternalUserID = rdr["ExternalUserID"].ToString(),
                                ExternalDisplayName= rdr["ExternalDisplayName"].ToString(),
                                IsExternalUserIDActive = rdr.GetBoolean(rdr.GetOrdinal("IsExternalUserIDActive")),
                                IsActive = rdr.GetBoolean(rdr.GetOrdinal("IsActive")),
                                Role = role.Name.ToString(),
                            });
                        }

                        processingResult.Data = user;
                    } catch (Exception ex) {
                        processingResult.IsSuccessful = false;
                        processingResult.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                        ex.ToExceptionless()
                      .SetMessage("An error occurred with datareader.")
                      .AddTags("ValidateLevel3ManagerCanBeDeletedAsync")
                      .MarkAsCritical()
                      .Submit();
                        //Logger.Error("An error occurred with datareader.",ex);
                    } finally { connection.Close(); }

                    if (!processingResult.IsSuccessful) {

                    }
                    await contextScope.SaveChangesAsync();
                } catch (Exception ex) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_UPDATE_SALES_TEAM_COMMISSIONS_ERROR;
                    ex.ToExceptionless()
                      .SetMessage("An error occurred while retrieving users.")
                      .AddTags("SearchUsersInCompany")
                      .MarkAsCritical()
                      .Submit();
                    //Logger.Error("An error occurred while retrieving users.",ex);
                    return processingResult;
                }
            }
            return processingResult;
        }

        public async Task<ServiceProcessingResult<ValidationResult>> UpdateCompanyId(string companyId, string userid) {
            var processingResult = new ServiceProcessingResult<ValidationResult> { IsSuccessful = true };

            var sqlQuery = new SQLQuery();
            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@CompanyID", companyId), new SqlParameter("@id", userid) };
            var updateCompanyResult = await sqlQuery.ExecuteNonQueryAsync(CommandType.Text, "UPDATE AspNetUsers SET CompanyId=@CompanyID WHERE Id=@ID;", parameters);
            if (updateCompanyResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error changing companies (update query)", "Error changing companies", false, false);
            }

            return processingResult;
        }

        public ServiceProcessingResult UpdateLanguage(string language, string loggedInUserId) {
            var processingResult = new ServiceProcessingResult { IsSuccessful = true };
            try {
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

                var sqlString = "UPDATE AspNetUsers SET Language=@language WHERE Id=@userid";
                SqlCommand cmd = new SqlCommand(sqlString, connection);
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqlParameter("@language", language));
                cmd.Parameters.Add(new SqlParameter("@userid", loggedInUserId));

                try {
                    connection.Open();
                    var numRows = cmd.ExecuteNonQuery();
                    processingResult.IsSuccessful = true;
                } catch (Exception ex) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                    ex.ToExceptionless()
                      .SetMessage("An error occurred changing language.")
                      .AddTags("UpdateLanguage")
                      .MarkAsCritical()
                      .Submit();
                    //Logger.Error("An error occurred changing language.", ex);
                } finally { connection.Close(); }

                if (!processingResult.IsSuccessful) {

                }
            } catch (Exception ex) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_UPDATE_SALES_TEAM_COMMISSIONS_ERROR;
                ex.ToExceptionless()
                      .SetMessage("An error occurred while changing the company.")
                      .AddTags("UpdateLanguage")
                      .MarkAsCritical()
                      .Submit();

                //Logger.Error("An error occurred while changing the company.", ex);
                return processingResult;
            }
            return processingResult;
        }

        public async Task<ServiceProcessingResult<List<TeamUserView>>> GetTeamUsers(string teamId) {
            var processingResult = new ServiceProcessingResult<List<TeamUserView>> { IsSuccessful = true };
            using (var contextScope = DbContextScopeFactory.Create()) {
                try {
                    List<TeamUserView> user = new List<TeamUserView>();
                    string connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                    SqlConnection connection = new SqlConnection(connectionstring);
                    SqlDataReader rdr = null;
                    SqlCommand cmd = new SqlCommand("SELECT AspNetUsers.Id, dbo.AspNetUsers.FirstName, AspNetUsers.LastName,AspNetUsers.IsActive, AspNetUsers.UserName, AspNetRoles.Name AS Role FROM AspNetUsers INNER JOIN AspNetUserRoles ON AspNetUsers.Id =AspNetUserRoles.UserId INNER JOIN AspNetRoles ON AspNetUserRoles.RoleId = AspNetRoles.Id WHERE(AspNetUsers.SalesTeamId = @TeamId)", connection);
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter("@TeamId", teamId));


                    try {
                        connection.Open();
                        rdr = cmd.ExecuteReader();
                        while (rdr.Read()) {

                            user.Add(new TeamUserView() {
                                FirstName = rdr["FirstName"].ToString(),
                                LastName = rdr["LastName"].ToString(),
                                UserName = rdr["UserName"].ToString(),
                                Role = rdr["Role"].ToString(),
                                Id = rdr["Id"].ToString(),
                                IsActive = rdr.GetBoolean(rdr.GetOrdinal("IsActive")),

                            });
                        }

                        processingResult.Data = user;
                    } catch (Exception ex) {
                        processingResult.IsSuccessful = false;
                        processingResult.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                        ex.ToExceptionless()
                      .SetMessage("An error occurred with datareader.")
                      .AddTags("GetTeamUsers")
                      .MarkAsCritical()
                      .Submit();
                        //Logger.Error("An error occurred with datareader.", ex);
                    } finally { connection.Close(); }

                    if (!processingResult.IsSuccessful) {

                    }
                    await contextScope.SaveChangesAsync();
                } catch (Exception ex) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("An error occured retrieving users in the team.", "An error occured retrieving users in the team.", true, false);
                    ex.ToExceptionless()
                     .SetMessage("An error occurred while retrieving users.")
                     .AddTags("GetTeamUsers")
                     .MarkAsCritical()
                     .Submit();
                    //Logger.Error("An error occurred while retrieving users.", ex);
                    return processingResult;
                }
            }
            return processingResult;

        }

        public async Task<ServiceProcessingResult<UserListViewNoTeamBindingModel>> GetUserByUsernameWithRole(string Username) {
            var processingResult = new ServiceProcessingResult<UserListViewNoTeamBindingModel> { IsSuccessful = true };
            using (var contextScope = DbContextScopeFactory.Create()) {
                try {

                    string connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                    SqlConnection connection = new SqlConnection(connectionstring);
                    SqlDataReader rdr = null;
                    SqlCommand cmd = new SqlCommand("SELECT U.Id, U.FirstName, U.LastName, U.UserName, U.isActive, R.Name, R.Rank FROM AspNetUsers (NOLOCK) U INNER JOIN AspNetUserRoles (NOLOCK) UR ON U.Id=UR.UserId INNER JOIN AspNetRoles (NOLOCK) R ON UR.RoleId=R.Id WHERE U.Username=@Username AND IsDeleted=0", connection);
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter("@Username", Username));
                    try {
                        connection.Open();
                        rdr = cmd.ExecuteReader();
                        while (rdr.Read()) {
                            var role = new RoleSimpleViewBindingModel() {
                                Id = rdr["Id"].ToString(),
                                Rank = (int)rdr["Rank"],
                                Name = rdr["Name"].ToString()
                            };

                            var user = new UserListViewNoTeamBindingModel() {
                                Id = rdr["Id"].ToString(),
                                FirstName = rdr["FirstName"].ToString(),
                                LastName = rdr["LastName"].ToString(),
                                UserName = rdr["UserName"].ToString(),
                                IsActive = rdr.GetBoolean(rdr.GetOrdinal("IsActive")),
                                Role = role
                            };
                            processingResult.Data = user;
                        }


                    } catch (Exception ex) {
                        processingResult.IsSuccessful = false;
                        processingResult.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                        ex.ToExceptionless()
                      .SetMessage("An error occurred with datareader.")
                      .AddTags("GetByUserNameWithrole")
                      .MarkAsCritical()
                      .Submit();

                        //Logger.Error("An error occurred with datareader.",ex);
                    } finally { connection.Close(); }


                    await contextScope.SaveChangesAsync();
                } catch (Exception ex) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("An error occured retrieving user.", "An error occured retrieving user.", true, false);
                    ex.ToExceptionless()
                    .SetMessage("An error occurred while retrieving users.")
                    .AddTags("GetByUserNameWithrole")
                    .MarkAsCritical()
                    .Submit();
                    //Logger.Error("An error occurred while retrieving users.",ex);
                    return processingResult;
                }
            }
            return processingResult;

        }
        public async Task<ServiceProcessingResult<List<string>>> GetTeamAssociatedWithUser(string userId) {
            var processingResult = new ServiceProcessingResult<List<string>> { IsSuccessful = true };
            List<string> Teams = new List<string>();
            using (var contextScope = DbContextScopeFactory.Create()) {
                try {

                    string connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                    SqlConnection connection = new SqlConnection(connectionstring);
                    SqlDataReader rdr = null;
                    SqlCommand cmd = new SqlCommand("SELECT ExternalPrimaryId from v_UserActiveTeams WHERE userId=@Id", connection);
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter("@Id", userId));
                    try {
                        connection.Open();
                        rdr = cmd.ExecuteReader();
                        while (rdr.Read()) {
                            Teams.Add(rdr["ExternalPrimaryId"].ToString());
                        }
                        processingResult.Data = Teams;

                    } catch (Exception ex) {
                        processingResult.IsSuccessful = false;
                        processingResult.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                        ex.ToExceptionless()
                   .SetMessage("An error occurred with datareader.")
                   .AddTags("GetTeamAssociateWithUser")
                   .MarkAsCritical()
                   .Submit();
                        //Logger.Error("An error occurred with datareader.",ex);
                    } finally { connection.Close(); }


                    await contextScope.SaveChangesAsync();
                } catch (Exception ex) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("An error occured retrieving teams the user has access to.", "An error occured retrieving teams the user has access to.", true, false);
                    ex.ToExceptionless()
                   .SetMessage("An error occured retrieving teams the user has access to.")
                   .AddTags("GetTeamAssociateWithUser")
                   .MarkAsCritical()
                   .Submit();

                    //Logger.Error("An error occured retrieving teams the user has access to.",ex);
                    return processingResult;
                }
            }
            return processingResult;

        }

        private static void FinishLogEntry(ApiLogEntry logEntry, string content) {
            logEntry.Response = content;
            logEntry.DateEnded = DateTime.UtcNow;
            var logEntryRepository = new ApiLogEntryRepository();
            logEntryRepository.Add(logEntry);
        }

        private static ApiLogEntry CreateInitialExternalUserIDValidationApiLogEntry(string input) {
            return new ApiLogEntry {
                Api = "Lifeline Services Users",
                DateStarted = DateTime.UtcNow,
                Function = "ValidateExternalUserID",
                Input = input
            };
        }
    }
}

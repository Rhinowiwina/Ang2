using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using LS.Core;
using LS.Domain;
using LS.Repositories;
using LS.Utilities;
using System.Configuration;
using Exceptionless;
using Exceptionless.Models;
namespace LS.Services {
    public class SalesTeamDataService : BaseDataService<SalesTeam, string> {
        public override BaseRepository<SalesTeam, string> GetDefaultRepository() {
            return new SalesTeamRepository();
        }

        public override ServiceProcessingResult<SalesTeam> Add(SalesTeam entity) {
            throw new NotImplementedException("Basic Add is not allowed for Sales Teams. Please use the provided AddSalesTeam method.");
        }

        private readonly string _loggedInUserId;

        public SalesTeamDataService(string loggedInUserId) {
            _loggedInUserId = loggedInUserId;
        }

        public async Task<ServiceProcessingResult<SalesTeam>> AddSalesTeamAsync(SalesTeam salesTeam, ApplicationUser loggedInUser) {
            var processingResult = new ServiceProcessingResult<SalesTeam> { IsSuccessful = true };
            var validationResult = ValidateStateAndRoleForAddOrUpdate(salesTeam, loggedInUser.Role).Data;

            if (!validationResult.IsValid) {
                processingResult.IsSuccessful = false;
                processingResult.Error = validationResult.ToProcessingError(ErrorValues.SalesTeamCreateValidationFailedUserMessage);
                return processingResult;
            }

            //check is admin or super admin because LS.ServiceSpecs.SpecsForApplicationUser Line 193 function can only give roles not rank
            if (!loggedInUser.Role.IsAdmin() && !loggedInUser.Role.IsSuperAdmin()) {
                var validationProcessingResult = ValidateLevel3SalesGroupOwnership(salesTeam, salesTeam.CreatedByUserId);

                if (!validationProcessingResult.IsSuccessful || !validationProcessingResult.Data.IsValid) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = !validationProcessingResult.IsSuccessful
                        ? validationProcessingResult.Error
                        : validationProcessingResult.Data.ToProcessingError(ErrorValues.SalesTeamCreateValidationFailedUserMessage);
                    return processingResult;
                }
            } else {
                var validationProcessingResult = await ValidateSalesGroupBelongsToCompanyAsync(salesTeam.Level3SalesGroupId, salesTeam.CompanyId);
                if (!validationProcessingResult.Data.IsValid) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = validationProcessingResult.Data.ToProcessingError(ErrorValues.SalesTeamCreateValidationFailedUserMessage);
                    return processingResult;
                }
            }

            processingResult = await base.AddAsync(salesTeam);

            if (!processingResult.IsSuccessful) {
                processingResult.Error = ErrorValues.GENERIC_ADD_SALES_TEAM_ERROR;
            }

            return processingResult;
        }

        public async Task<ServiceProcessingResult<List<CompleteSalesTeam>>> GetSalesTeamsForUserAsync(string UserID) {
            var processingResult = new ServiceProcessingResult<List<CompleteSalesTeam>> { IsSuccessful = true };

            var connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionstring);
            var sqlString = "SELECT * FROM v_UserActiveTeams WHERE UserID=@UserID";
            SqlDataReader rdr = null;
            SqlCommand cmd = new SqlCommand(sqlString, connection);
            cmd.Parameters.Clear();
            cmd.Parameters.Add(new SqlParameter("@UserID", UserID));

            connection.Open();
            rdr = cmd.ExecuteReader();

            var salesTeamList = new List<CompleteSalesTeam>();

            while (rdr.Read()) {
                var salesTeam = new CompleteSalesTeam() {
                    Id = rdr["Id"].ToString(),
                    Name = rdr["Name"].ToString(),
                    SigType = rdr["SigType"].ToString(),
                    CompanyId = rdr["CompanyId"].ToString(),
                    ExternalPrimaryId = rdr["ExternalPrimaryId"].ToString(),
                    ExternalDisplayName = rdr["ExternalDisplayName"].ToString(),
                    Address1 = rdr["Address1"].ToString(),
                    Address2 = rdr["Address2"].ToString(),
                    City = rdr["City"].ToString(),
                    State = rdr["State"].ToString(),
                    Zip = rdr["Zip"].ToString(),
                    Phone = rdr["Phone"].ToString(),
                    TaxId = rdr["TaxId"].ToString(),
                    Level1Id = rdr["Level1Id"].ToString(),
                    Level1Name = rdr["Level1Name"].ToString(),
                    Level2Id = rdr["Level2Id"].ToString(),
                    Level2Name = rdr["Level2Name"].ToString(),
                    Level3Id = rdr["Level3Id"].ToString(),
                    IsActive = rdr["IsActive"] as bool? ?? false,
                    Level3Name = rdr["Level3Name"].ToString(),
                    

            };
                salesTeamList.Add(salesTeam);
            }
            connection.Close();
            processingResult.Data = salesTeamList;
            return processingResult;
        }

        public async Task<ServiceProcessingResult<SalesTeam>> GetSalesTeamsForUserWhereAsync(string SalesTeamID, string UserID) {
            var processingResult = new ServiceProcessingResult<SalesTeam> { IsSuccessful = true };

            var allSalesTeams = await GetSalesTeamsForUserAsync(UserID);

            foreach (var team in allSalesTeams.Data) {
                var finalSalesTeam = new SalesTeam {
                    Id = team.Id,
                    Name = team.Name,
                    CompanyId = team.CompanyId,
                    ExternalPrimaryId = team.ExternalPrimaryId,
                    ExternalDisplayName = team.ExternalDisplayName,
                    Address1 = team.Address1,
                    Address2 = team.Address2,
                    City = team.City,
                    State = team.State,
                    Zip = team.Zip,
                    Phone = team.Phone,
                    TaxId = team.TaxId,
                    PayPalEmail = team.PayPalEmail,
                    CycleCountTypeDevice = team.CycleCountTypeDevice,
                    CycleCountTypeSim = team.CycleCountTypeSim,
                    CreatedByUserId = team.CreatedByUserId,
                    IsActive = team.IsActive,
                    IsDeleted = team.IsDeleted,
                    DateCreated = team.DateCreated,
                    DateModified = team.DateModified

                };
                if (finalSalesTeam.Id == SalesTeamID) {
                    processingResult.Data = finalSalesTeam;
                    break;
                }
            }
            return processingResult;
        }

        public async Task<ServiceProcessingResult<List<SalesTeam>>> getSalesTeamsByExternalDisplayName(string SalesTeamName) {
            var processingResult = new ServiceProcessingResult<List<SalesTeam>> { IsSuccessful = true };

            var salesTeamResult = await GetAllWhereAsync(st => st.ExternalDisplayName == SalesTeamName && st.IsDeleted == false);
            if (!salesTeamResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error retrieving sales team for user creation.", "Error retrieving sales team for user creation.", false, false);
                return processingResult;
            }

            processingResult.Data = salesTeamResult.Data;
            return processingResult;
        }

        private ServiceProcessingResult<ValidationResult> ValidateStateAndRoleForAddOrUpdate(SalesTeam salesTeam, ApplicationRole createdByUserRole) {
            var processingResult = new ServiceProcessingResult<ValidationResult> { IsSuccessful = true };
            var validationResult = new ValidationResult {
                IsValid = true
            };

            if (!salesTeam.State.IsValidStateCode()) {
                validationResult.IsValid = false;
                validationResult.Errors.Add(ErrorValues.InvalidStateUserHelp);
            }

            var roleValidationResult = ValidateRoleForAddOrUpdate(salesTeam, createdByUserRole).Data;
            if (roleValidationResult.Errors.Count > 0) {
                validationResult.IsValid = false;
                validationResult.Errors.AddRange(roleValidationResult.Errors);
            }
            processingResult.Data = validationResult;
            return processingResult;
        }

        private ServiceProcessingResult<ValidationResult> ValidateRoleForAddOrUpdate(SalesTeam salesTeam,
            ApplicationRole createdByUserRole) {
            var processingResult = new ServiceProcessingResult<ValidationResult> { IsSuccessful = true };
            var validationResult = new ValidationResult {
                IsValid = true
            };

            if (!createdByUserRole.CanCrudSalesTeam()) {
                validationResult.IsValid = false;
                validationResult.Errors.Add(ErrorValues.SalesTeamCrudPermissionsUserHelp);
            }
            processingResult.Data = validationResult;

            return processingResult;
        }

        public async Task<ServiceProcessingResult<SalesTeam>> UpdateSalesTeamAsync(SalesTeam updatedSalesTeam,
            ApplicationUser loggedInUser) {
            var processingResult = new ServiceProcessingResult<SalesTeam> { IsSuccessful = true };
            if (updatedSalesTeam.IsDeleted == true)
            {
                if (!loggedInUser.Role.IsAdmin() && !loggedInUser.Role.IsSuperAdmin())
                {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.SALES_TEAM_CRUD_PERMISSIONS_ERROR;
                    return processingResult;
                }

                var validationProcessingResult = await ValidateSalesTeamForDeleteAsync(updatedSalesTeam.Id, loggedInUser.CompanyId, loggedInUser.Role);
                if (!validationProcessingResult.IsSuccessful || !validationProcessingResult.Data.IsValid)
                {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = validationProcessingResult.IsSuccessful
                        ? validationProcessingResult.Data.ToProcessingError(ErrorValues.SalesTeamDeleteValidationFailedUserMessage)
                        : validationProcessingResult.Error;
                    if (validationProcessingResult.IsSuccessful)
                    {
                        return processingResult;
                    }
                    //var logMessage = String.Format(
                    //    "An error occurred while validating deletion of SalesTeam with Id: {0} in Company with Id: {1}",
                    //    updatedSalesTeam.Id, loggedInUser.Id);
                     //Logger.Error(logMessage);
                    ExceptionlessClient.Default.CreateLog(typeof(SalesTeamDataService).FullName,String.Format(
                        "An error occurred while validating deletion of SalesTeam with Id: {0} in Company with Id: {1}",
                        updatedSalesTeam.Id,loggedInUser.Id),"Error").AddTags("Data Service Error").Submit();

                   
                    return processingResult;
                }


            }
            if (updatedSalesTeam.IsDeleted != true)
            {
                var validationResult = ValidateStateAndRoleForAddOrUpdate(updatedSalesTeam, loggedInUser.Role).Data;

                if (!validationResult.IsValid)
                {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = validationResult.ToProcessingError(ErrorValues.SalesTeamUpdateValidationFailedUserMessage);
                    return processingResult;
                }
            }
            using (var contextScope = DbContextScopeFactory.Create()) {
                var validationProcessingResult = await ValidateSalesTeamOwnershipForUpdateOrDeleteAsync(updatedSalesTeam, loggedInUser.Role);
                if (!validationProcessingResult.IsSuccessful || !validationProcessingResult.Data.IsValid) {
                    //var logMessage =
                    //    String.Format(
                    //        "An error occurred while validating ownership for SalesTeam with Id: {0} and User with Id: {1}.",
                    //        updatedSalesTeam.Id, _loggedInUserId);
                    //Logger.Error(logMessage);
                    ExceptionlessClient.Default.CreateLog(typeof(SalesTeamDataService).FullName,String.Format(
                            "An error occurred while validating ownership for SalesTeam with Id: {0} and User with Id: {1}.",
                            updatedSalesTeam.Id,_loggedInUserId),"Error").AddTags("Data Service Error").Submit();


                    processingResult.IsSuccessful = false;
                    processingResult.Error = validationProcessingResult.IsSuccessful
                        ? validationProcessingResult.Data.ToProcessingError(ErrorValues.SalesTeamUpdateValidationFailedUserMessage)
                        : validationProcessingResult.Error;
                    return processingResult;
                }

                try {
                    processingResult = GetRepository().Update(updatedSalesTeam)
                            .ToServiceProcessingResult(ErrorValues.GENERIC_UPDATE_SALES_TEAM_ERROR);
                    if (!processingResult.IsSuccessful) {
                        //var logMessage = String.Format("An error occurred while updating a SalesTeam with Id: {0}",
                        //    updatedSalesTeam.Id);
                        //Logger.Error(logMessage);
                        ExceptionlessClient.Default.CreateLog(typeof(SalesTeamDataService).FullName,String.Format("An error occurred while updating a SalesTeam with Id: {0}",
                            updatedSalesTeam.Id),"Error").AddTags("Data Service Error").Submit();

                        return processingResult;
                    }

                    await contextScope.SaveChangesAsync();
                } catch (Exception ex) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_UPDATE_SALES_TEAM_ERROR;
                    Logger.Error("An error occurred while updating a SalesTeam", ex);
                }

            }

            return processingResult;
        }

        public async Task<ServiceProcessingResult> DeleteSalesTeamAsync(string salesTeamId, string companyId,
            ApplicationRole deletedByUserRole) {
            var processingResult = new ServiceProcessingResult();

            if (String.IsNullOrWhiteSpace(salesTeamId)) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.DELETE_SALES_TEAM_WITHOUT_ID_ERROR;
                return processingResult;
            }

            if (!deletedByUserRole.IsAdmin() && !deletedByUserRole.IsSuperAdmin()) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.SALES_TEAM_CRUD_PERMISSIONS_ERROR;
                return processingResult;
            }

            var validationProcessingResult = await ValidateSalesTeamForDeleteAsync(salesTeamId, companyId, deletedByUserRole);
            if (!validationProcessingResult.IsSuccessful || !validationProcessingResult.Data.IsValid) {
                processingResult.IsSuccessful = false;
                processingResult.Error = validationProcessingResult.IsSuccessful
                    ? validationProcessingResult.Data.ToProcessingError(ErrorValues.SalesTeamDeleteValidationFailedUserMessage)
                    : validationProcessingResult.Error;
                if (validationProcessingResult.IsSuccessful) {
                    return processingResult;
                }
                //var logMessage = String.Format(
                //    "An error occurred while validating deletion of SalesTeam with Id: {0} in Company with Id: {1}",
                //    salesTeamId, companyId);
                //Logger.Error(logMessage);
                ExceptionlessClient.Default.CreateLog(typeof(SalesTeamDataService).FullName,String.Format(
                    "An error occurred while validating deletion of SalesTeam with Id: {0} in Company with Id: {1}",
                    salesTeamId,companyId),"Error").AddTags("Data Service Error").Submit();

                return processingResult;
            }

            using (var contextScope = DbContextScopeFactory.Create()) {

                try {
                    processingResult = GetRepository().Delete(salesTeamId)
                        .ToServiceProcessingResult(ErrorValues.GENERIC_DELETE_SALES_TEAM_ERROR);
                    if (!processingResult.IsSuccessful) {
                        //var logMessage = String.Format("An error occurred while deleting SalesTeam with Id: {0}",
                        //    salesTeamId);
                        //Logger.Error(logMessage);
                        ExceptionlessClient.Default.CreateLog(typeof(SalesTeamDataService).FullName,String.Format("An error occurred while deleting SalesTeam with Id: {0}",
                            salesTeamId),"Error").AddTags("Data Service Error").Submit();

                        }

                    await contextScope.SaveChangesAsync();
                } catch (Exception ex) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_DELETE_SALES_TEAM_ERROR;
                    //var logMessage = String.Format("An error occurred while deleting SalesTeam with Id: {0}",
                    //    salesTeamId);
                    //Logger.Error(logMessage, ex);
                    ExceptionlessClient.Default.CreateLog(typeof(SalesTeamDataService).FullName,String.Format("An error occurred while deleting SalesTeam with Id: {0}",
                        salesTeamId),"Error").AddTags("Data Service Error").Submit();

                    }
                }

            return processingResult;
        }

        public async Task<ServiceProcessingResult<SalesTeam>> GetExistingSalesTeamInCompanyAsync(string salesTeamId, string companyId) {
            var processingResult = new ServiceProcessingResult<SalesTeam>();

            if (salesTeamId == null) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GET_SALES_GROUP_WITHOUT_ID_ERROR;

                return processingResult;
            }

            using (var contextScope = DbContextScopeFactory.CreateReadOnly()) {
                try {
                    var includes = new[] { "Level3SalesGroup", "Users.Roles.Role" };
                    var dataAccessResult =
                        await GetRepository().GetWhereAsync(sg => sg.Id == salesTeamId && sg.CompanyId == companyId, includes);
                    if (!dataAccessResult.IsSuccessful) {
                        processingResult.IsSuccessful = false;
                        processingResult.Error = ErrorValues.GENERIC_GET_SALES_TEAM_IN_COMPANY_ERROR;

                        //var logMessage = String.Format(
                        //        "An error occurred while retrieving SalesTeam with Id: {0} in Company: {1}",
                        //        salesTeamId, companyId
                        //    );
                        //Logger.Error(logMessage);
                        ExceptionlessClient.Default.CreateLog(typeof(SalesTeamDataService).FullName,String.Format(
                                "An error occurred while retrieving SalesTeam with Id: {0} in Company: {1}",
                                salesTeamId,companyId
                            ),"Error").AddTags("Data Service Error").Submit();

                        return processingResult;
                    }

                    processingResult.Data = dataAccessResult.Data;
                    processingResult.IsSuccessful = true;

                    if (dataAccessResult.Data == null) {
                        processingResult.IsSuccessful = false;
                        processingResult.Error = ErrorValues.COULD_NOT_FIND_SALES_TEAM_IN_COMPANY_ERROR;
                    }
                } catch (Exception ex) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_GET_SALES_TEAM_IN_COMPANY_ERROR;
                    var logMessage = String.Format(
                            "An error occurred while retrieving SalesTeam with Id: {0} in Company: {1}. {2}",
                            salesTeamId, companyId, ex
                        );
                    Logger.Error(logMessage, ex);
                    ExceptionlessClient.Default.CreateLog(typeof(SalesTeamDataService).FullName,String.Format(
                            "An error occurred while retrieving SalesTeam with Id: {0} in Company: {1}. {2}",
                            salesTeamId,companyId),"Error").AddTags("Data Service Error").Submit();

                    }

                }

            return processingResult;
        }

        public async Task<ServiceProcessingResult<List<SalesTeam>>> GetAllSalesTeamsInCompanyAsync(string companyId) {
            var processingResult = new ServiceProcessingResult<List<SalesTeam>>();

            using (var contextScope = DbContextScopeFactory.CreateReadOnly()) {
                try {
                    processingResult = await GetAllWhereAsync(st => st.CompanyId == companyId);
                } catch (Exception ex) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_GET_SALES_TEAM_IN_COMPANY_ERROR;
                    //var logMessage =
                    //    String.Format("An error occured while getting all Sales Teams in Company with Id: {0}. {1}",
                    //        companyId, ex);
                    //Logger.Error(logMessage);
                    ExceptionlessClient.Default.CreateLog(typeof(SalesTeamDataService).FullName,String.Format("An error occured while getting all Sales Teams in Company with Id: {0}. {1}",
                            companyId),"Error").AddTags("Data Service Error").Submit();

                    }
                }

            return processingResult;
        }

        public async Task<ServiceProcessingResult<SalesTeam>> GetSalesTeamForCommissionUpdate(string salesTeamId, string companyId) {
            var processingResult = new ServiceProcessingResult<SalesTeam>();

            if (salesTeamId == null) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GET_SALES_GROUP_WITHOUT_ID_ERROR;

                return processingResult;
            }

            using (var contextScope = DbContextScopeFactory.CreateReadOnly()) {
                try {
                    var includes = new[]
                    {
                        "Level3SalesGroup.Managers.Roles.Role",
                        "Level3SalesGroup.ParentSalesGroup.Managers.Roles.Role",
                        "Level3SalesGroup.ParentSalesGroup.ParentSalesGroup.Managers.Roles.Role"
                    };
                    var dataAccessResult =
                        await GetRepository().GetWhereAsync(sg => sg.Id == salesTeamId && sg.CompanyId == companyId, includes);
                    if (!dataAccessResult.IsSuccessful) {
                        processingResult.IsSuccessful = false;
                        processingResult.Error = ErrorValues.GENERIC_GET_SALES_TEAM_IN_COMPANY_ERROR;

                        //var logMessage = String.Format(
                        //        "An error occurred while retrieving SalesTeam with Id: {0} in Company: {1}",
                        //        salesTeamId, companyId
                        //    );
                        //Logger.Error(logMessage);
                        ExceptionlessClient.Default.CreateLog(typeof(SalesTeamDataService).FullName,String.Format(
                                "An error occurred while retrieving SalesTeam with Id: {0} in Company: {1}",
                                salesTeamId,companyId
                            ),"Error").AddTags("Data Service Error").Submit();

                        return processingResult;
                    }

                    processingResult.Data = dataAccessResult.Data;
                    processingResult.IsSuccessful = true;

                    if (dataAccessResult.Data == null) {
                        processingResult.IsSuccessful = false;
                        processingResult.Error = ErrorValues.COULD_NOT_FIND_SALES_TEAM_IN_COMPANY_ERROR;
                    }
                } catch (Exception ex) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_GET_SALES_TEAM_IN_COMPANY_ERROR;
                    //var logMessage = String.Format(
                    //        "An error occurred while retrieving SalesTeam with Id: {0} in Company: {1}. {2}",
                    //        salesTeamId, companyId, ex
                    //    );
                    //Logger.Error(logMessage, ex);
                    ExceptionlessClient.Default.CreateLog(typeof(SalesTeamDataService).FullName,String.Format(
                            "An error occurred while retrieving SalesTeam with Id: {0} in Company: {1}. {2}",
                            salesTeamId,companyId),"Error").AddTags("Data Service Error").Submit();

                    }
                }

            return processingResult;
        }


        public async Task<ServiceProcessingResult<ValidationResult>> ValidateSalesTeamOwnershipAsync(string assignedSalesTeamId,
            ApplicationRole assignedUserRole, ApplicationUser assignedByUser) {
            var processingResult = new ServiceProcessingResult<ValidationResult> { IsSuccessful = true };
            var validationResult = new ValidationResult {
                IsValid = false
            };

            if (assignedSalesTeamId == null && !assignedUserRole.CanBeAddedToSalesTeam()) {
                validationResult.IsValid = true;
                processingResult.Data = validationResult;
                return processingResult;
            }

            if (assignedSalesTeamId == null && assignedUserRole.CanBeAddedToSalesTeam()) {
                validationResult.Errors.Add(ErrorValues.UserMustBeAssignedToSalesTeamUserHelp);
                processingResult.Data = validationResult;
                return processingResult;
            }

            if (assignedSalesTeamId != null && !assignedUserRole.CanBeAddedToSalesTeam()) {
                validationResult.Errors.Add(ErrorValues.InvalidRoleForSalesTeamAssignmentUserHelp);
                processingResult.Data = validationResult;
                return processingResult;
            }

            if (assignedByUser.Role.IsAdmin() || assignedByUser.Role.IsSuperAdmin()) {
                var existingSalesTeamResult = await GetExistingSalesTeamInCompanyAsync(assignedSalesTeamId, assignedByUser.CompanyId);

                if (existingSalesTeamResult.IsSuccessful) {
                    validationResult.IsValid = true;
                } else if (existingSalesTeamResult.IsFatalFailure()) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = existingSalesTeamResult.Error;
                    processingResult.Error.UserMessage = ErrorValues.SalesTeamAssignmentInvalidUserMessage;
                    //Logger.Error(String.Format("A fatal error occurred while retrieving Sales Team in Company with Id: {0}.",assignedByUser.CompanyId));
                    ExceptionlessClient.Default.CreateLog(typeof(SalesTeamDataService).FullName,String.Format("A fatal error occurred while retrieving Sales Team in Company with Id: {0}.",assignedByUser.CompanyId),"Error").AddTags("Data Service Error").Submit();

                    } else if (!existingSalesTeamResult.IsSuccessful) {
                    validationResult.Errors.Add(existingSalesTeamResult.Error.UserHelp);
                }
                processingResult.Data = validationResult;

                return processingResult;
            }

            using (var contextScope = DbContextScopeFactory.CreateReadOnly()) {
                try {
                    var repo = new SalesTeamRepository();
                    var salesTeamUnderManagerResult = await repo.GetSalesTeamUnderManagerAsync(
                        assignedSalesTeamId, _loggedInUserId, assignedByUser.CompanyId
                        );
                    if (!salesTeamUnderManagerResult.IsSuccessful) {
                        //var logMessage =
                        //    String.Format("An error occurred while validating assignment of SalesTeam with Id: {0}",
                        //        assignedSalesTeamId);
                        //Logger.Error(logMessage);
                        ExceptionlessClient.Default.CreateLog(typeof(SalesTeamDataService).FullName,String.Format("An error occurred while validating assignment of SalesTeam with Id: {0}",
                                assignedSalesTeamId),"Error").AddTags("Data Service Error").Submit();

                        processingResult.IsSuccessful = false;
                        processingResult.Error = ErrorValues.SALES_TEAM_ASSIGNMENT_VALIDATION_FAILED_ERROR;
                    } else if (salesTeamUnderManagerResult.Data == null) {
                        validationResult.Errors.Add(ErrorValues.ManagerDoesntOwnSalesTeamUserHelp);
                    } else {
                        validationResult.IsValid = true;
                    }
                } catch (Exception ex) {
                    //var logMessage =
                    //    String.Format("An error occurred while validating assignment of SalesTeam with Id: {0}",
                    //        assignedSalesTeamId);
                    //Logger.Error(logMessage, ex);
                    ExceptionlessClient.Default.CreateLog(typeof(SalesTeamDataService).FullName,String.Format("An error occurred while validating assignment of SalesTeam with Id: {0}",
                            assignedSalesTeamId),"Error").AddTags("Data Service Error").Submit();

                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.SALES_TEAM_ASSIGNMENT_VALIDATION_FAILED_ERROR;
                }
            }
            processingResult.Data = validationResult;

            return processingResult;
        }

        public async Task<ServiceProcessingResult<List<SalesTeam>>> SearchSalesTeamsInCompany(string searchString,string companyId) {
            var processingResult = await GetAllWhereAsync(t => t.Name.Contains(searchString) || t.ExternalDisplayName.Contains(searchString));
            if (!processingResult.IsSuccessful) {
                processingResult.Error = ErrorValues.GENERIC_SEARCH_ERROR;
                //var logMessage = String.Format("Failed to search SalesTeams in Company with Id: {0}", companyId);
                //Logger.Error(logMessage);
                ExceptionlessClient.Default.CreateLog(typeof(SalesTeamDataService).FullName,String.Format("Failed to search SalesTeams in Company with Id: {0}",companyId),"Error").AddTags("Data Service Error").Submit();

                }

            return processingResult;
        }

        private ServiceProcessingResult<ValidationResult> ValidateLevel3SalesGroupOwnership(SalesTeam salesTeam, string userId) {
            var processingResult = new ServiceProcessingResult<ValidationResult> { IsSuccessful = true };
            var validationResult = new ValidationResult {
                IsValid = true
            };

            var salesGroupService = new Level3SalesGroupDataService();
            var level3SalesGroupResult =
                salesGroupService.GetLevel3SalesGroupWhereManagerInTree(salesTeam.Level3SalesGroupId, userId,
                    salesTeam.CompanyId);
            if (!level3SalesGroupResult.IsSuccessful) {
               // Logger.Error("An error occurred while while validating Level3SalesGroup ownership.");
                ExceptionlessClient.Default.CreateLog(typeof(SalesTeamDataService).FullName,"An error occurred while while validating Level3SalesGroup ownership.","Error").AddTags("Data Service Error").Submit();

                processingResult.IsSuccessful = false;
                processingResult.Error = level3SalesGroupResult.Error;
            } else if (level3SalesGroupResult.Data == null) {
                validationResult.IsValid = false;
                validationResult.Errors.Add(ErrorValues.ManageSalesTeamUserPermissionUserHelp);
            }
            processingResult.Data = validationResult;

            return processingResult;
        }

        private async Task<ServiceProcessingResult<ValidationResult>> ValidateSalesTeamOwnershipForUpdateOrDeleteAsync(SalesTeam salesTeamToValidate,
            ApplicationRole ownerUserRole, bool isForDelete = false) {
            var processingResult = new ServiceProcessingResult<ValidationResult> { IsSuccessful = true };
            var validationResult = new ValidationResult {
                IsValid = true
            };

            if (!ownerUserRole.IsAdmin() && !ownerUserRole.IsSuperAdmin()) {
                var validationProcessingResult = ValidateLevel3SalesGroupOwnership(salesTeamToValidate, _loggedInUserId);

                if (!validationProcessingResult.IsSuccessful) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = validationProcessingResult.Error;
                    var userMessage = isForDelete
                        ? ErrorValues.SalesTeamDeleteFailedUserMessage
                        : ErrorValues.SalesTeamUpdateFailedUserMessage;
                    processingResult.Error.UserMessage = userMessage;
                    return processingResult;
                }
                if (!validationProcessingResult.Data.IsValid) {
                    validationResult = validationProcessingResult.Data;
                    processingResult.Data = validationResult;
                    return processingResult;
                }
            } else {
                var existingSalesTeamResult = await GetExistingSalesTeamInCompanyAsync(salesTeamToValidate.Id, salesTeamToValidate.CompanyId);
                if (!existingSalesTeamResult.IsSuccessful) {
                    processingResult.IsSuccessful = false;
                    var error = isForDelete
                        ? ErrorValues.DELETE_SALES_TEAM_NOT_IN_COMPANY_ERROR
                        : ErrorValues.UPDATE_SALES_TEAM_NOT_IN_COMPANY_ERROR;

                    if (existingSalesTeamResult.IsFatalFailure()) {
                        //Logger.Error("An error occurred while retrieving existing SalesTeam in Company during SalesTeam update.");
                        ExceptionlessClient.Default.CreateLog(typeof(SalesTeamDataService).FullName,"An error occurred while retrieving existing SalesTeam in Company during SalesTeam update.","Error").AddTags("Data Service Error").Submit();

                        error = isForDelete
                        ? ErrorValues.GENERIC_DELETE_SALES_TEAM_ERROR
                        : ErrorValues.GENERIC_UPDATE_SALES_TEAM_ERROR;
                    }

                    processingResult.Error = error;
                    return processingResult;
                }

                var shouldValidateSalesGroupInCompany = !isForDelete &&
                                                        existingSalesTeamResult.Data.Level3SalesGroupId !=
                                                        salesTeamToValidate.Level3SalesGroupId;
                if (shouldValidateSalesGroupInCompany) {
                    return await ValidateSalesGroupBelongsToCompanyAsync(salesTeamToValidate.Level3SalesGroupId,
                        salesTeamToValidate.CompanyId, true);
                }
            }
            processingResult.Data = validationResult;

            return processingResult;
        }

        private async Task<ServiceProcessingResult<ValidationResult>> ValidateSalesTeamForDeleteAsync(string salesTeamId, string companyId,
            ApplicationRole deletedByUserRole) {
            var processingResult = new ServiceProcessingResult<ValidationResult> { IsSuccessful = true };
            var validationResult = new ValidationResult {
                IsValid = true
            };

            var salesTeamToDelete = Get(salesTeamId).Data;
            if (salesTeamToDelete == null) {
                validationResult.IsValid = false;
                validationResult.Errors.Add(ErrorValues.DeleteSalesTeamNotInCompanyUserHelp);
                processingResult.Data = validationResult;
                return processingResult;
            }

            // Returned SalesTeam could still belong to different company, so set CompanyId here to ensure accurate
            // validation of ownership
            salesTeamToDelete.CompanyId = companyId;

            var validationProcessingResult = await ValidateSalesTeamOwnershipForUpdateOrDeleteAsync(salesTeamToDelete, deletedByUserRole,
                true);
            if (!validationProcessingResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = validationProcessingResult.Error;
                return processingResult;
            }
            if (!validationProcessingResult.Data.IsValid) {
                validationResult.IsValid = false;
                validationResult.Errors.AddRange(validationProcessingResult.Data.Errors);
            }
            processingResult.Data = validationResult;

            return processingResult;
        }

        private async Task<ServiceProcessingResult<ValidationResult>> ValidateSalesGroupBelongsToCompanyAsync(string salesGroupId, string companyId, bool isForUpdate = false) {
            var processingResult = new ServiceProcessingResult<ValidationResult> { IsSuccessful = true };
            var validationResult = new ValidationResult {
                IsValid = true
            };

            var salesGroupService = new Level3SalesGroupDataService();
            var existingSalesGroupResult = await salesGroupService.GetExistingLevel3SalesGroupInCompanyAsync(salesGroupId, companyId);
            if (!existingSalesGroupResult.IsSuccessful) {
                validationResult.IsValid = false;
                validationResult.Errors.Add(ErrorValues.ModifySalesTeamSalesGroupNotInCompanyUserHelp);
            }
            processingResult.Data = validationResult;

            return processingResult;
        }
    }
}

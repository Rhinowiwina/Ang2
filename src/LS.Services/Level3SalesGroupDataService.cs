using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LS.Core;
using LS.Domain;
using LS.Repositories;
using LS.ApiBindingModels;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using MoreLinq;
using Exceptionless;
using Exceptionless.Models;

namespace LS.Services {
    public class Level3SalesGroupDataService : BaseSalesGroupDataService<Level3SalesGroup, string> {
        public Level3SalesGroupDataService() : base(ApplicationRoleRulesHelper.Level3SalesGroupManagerRank) {
        }

        public override BaseRepository<Level3SalesGroup, string> GetDefaultRepository() {
            return new Level3SalesGroupRepository();
        }

        public async Task<ServiceProcessingResult<Level3SalesGroup>> AddAsync(Level3SalesGroup salesGroup, ApplicationUser loggedInUser) {
            var result = new ServiceProcessingResult<Level3SalesGroup>();

            if (loggedInUser.Role.Rank > ValidApplicationRoles.LevelOneManagerRank) {
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_SALES_GROUP_PERMISSIONS_ERROR;
                return result;
            }
           
            var getParentSalesGroupResult = await GetExistingLevel2SalesGroupInCompany(salesGroup.ParentSalesGroupId, salesGroup.CompanyId);
            if (!getParentSalesGroupResult.IsSuccessful) {
                result.IsSuccessful = false;
                result.Error = ErrorValues.ADD_SALES_GROUP_PARENT_SALES_GROUP_NOT_IN_COMPANY_ERROR;
                //var logMessage = String.Format(
                //    "Unable to find Level 2 Sales Group with Id: {0} in Company with Id: {1} for Parent Sales Group to Add a Level 3 Sales Group.");
                //Logger.Error(logMessage);
                ExceptionlessClient.Default.CreateLog(typeof(Level3SalesGroupDataService).FullName,String.Format(
                    "Unable to find Level 2 Sales Group with Id: {0} in Company with Id: {1} for Parent Sales Group to Add a Level 3 Sales Group.",salesGroup.ParentSalesGroupId,salesGroup.CompanyId),"Error").AddTags("Data Service Error").Submit();
                return result;
            }
            salesGroup.ParentSalesGroupId = getParentSalesGroupResult.Data.Id;

            var managerIds = salesGroup.Managers.Select(m => m.Id).ToList();
            var getVerifiedManagersResult = GetValidManagersInCompanyFromListOfIds(managerIds, salesGroup.CompanyId);
            if (!getVerifiedManagersResult.IsSuccessful) {
                //Logger.Error("Failed to get valid managers for Level 3 Sales Group.");
                ExceptionlessClient.Default.CreateLog(typeof(Level3SalesGroupDataService).FullName,"Failed to get valid managers for Level 3 Sales Group.","Error").AddTags("Data Service Error").Submit();
                }

            salesGroup.Managers = getVerifiedManagersResult.Data;

            result = await base.AddAsync(salesGroup);

            if (!result.IsSuccessful) {
                result.Error = ErrorValues.GENERIC_ADD_SALES_GROUP_ERROR;
            }

            return result;
        }

        public async Task<ServiceProcessingResult<Level3SalesGroup>> UpdateAsync(Level3SalesGroup salesGroup, ApplicationUser loggedInUser) {
            var result = new ServiceProcessingResult<Level3SalesGroup>();

            if (loggedInUser.Role.Rank > ValidApplicationRoles.LevelOneManagerRank) {
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_SALES_GROUP_PERMISSIONS_ERROR;
                return result;
            }
            if (salesGroup.IsDeleted == true)
            {
                var validateDeletabilityResult = await ValidateLevel3SalesGroupCanBeDeletedAsync(salesGroup.Id, loggedInUser.CompanyId);
                if (!validateDeletabilityResult.IsSuccessful)
                {
                    result.IsSuccessful = false;
                    result.Error = ErrorValues.DELETE_SALES_GROUP_NOT_IN_COMPANY_ERROR;
                    //var logMessage =
                    //    String.Format(
                    //        "An error occurred while validating deletion for Sales Group with Id: {0} in Company with Id: {1}",
                    //        loggedInUser.Id, loggedInUser.CompanyId);
                    //Logger.Error(logMessage);
                    ExceptionlessClient.Default.CreateLog(typeof(Level3SalesGroupDataService).FullName,String.Format(
                            "An error occurred while validating deletion for Sales Group with Id: {0} in Company with Id: {1}",
                            loggedInUser.Id,loggedInUser.CompanyId),"Error").AddTags("Data Service Error").Submit();
                    return result;
                }

                if (!validateDeletabilityResult.Data.IsValid)
                {
                    result.IsSuccessful = false;
                    result.Error =
                        validateDeletabilityResult.Data.ToProcessingError(
                            ErrorValues.SalesGroupDeleteValidationFailedUserMessage);
                    return result;
                }

            }
            var getSalesGroupResult = await GetExistingLevel3SalesGroupInCompanyAsync(salesGroup.Id, salesGroup.CompanyId);
            if (!getSalesGroupResult.IsSuccessful) {
                result.IsSuccessful = false;
                result.Error = ErrorValues.UPDATE_SALES_GROUP_NOT_IN_COMPANY_ERROR;
                //var logMessage =
                //    String.Format("Failed to get Level 3 Sales Group with Id: {0} in Company with Id: {1} for Update.",
                //        salesGroup.Id, salesGroup.CompanyId);
                //Logger.Error(logMessage);
                ExceptionlessClient.Default.CreateLog(typeof(Level3SalesGroupDataService).FullName,String.Format("Failed to get Level 3 Sales Group with Id: {0} in Company with Id: {1} for Update.",
                        salesGroup.Id,salesGroup.CompanyId),"Error").AddTags("Data Service Error").Submit();
                return result;
            }
            var salesGroupToUpdate = getSalesGroupResult.Data;

            var getParentSalesGroupResult = await GetExistingLevel2SalesGroupInCompany(salesGroup.ParentSalesGroupId, salesGroup.CompanyId);
            if (!getParentSalesGroupResult.IsSuccessful) {
                result.IsSuccessful = false;
                result.Error = ErrorValues.UPDATE_SALES_GROUP_PARENT_SALES_GROUP_NOT_IN_COMPANY_ERROR;
                //var logMessage = String.Format(
                //    "Failed to get Level 2 Sales Group with Id: {0} in Company with Id: {1} for Parent Sales Group to Update Level 3 Sales Group.",
                //    salesGroup.Id, salesGroup.CompanyId);
                //Logger.Error(logMessage);
                ExceptionlessClient.Default.CreateLog(typeof(Level3SalesGroupDataService).FullName,String.Format(
                    "Failed to get Level 2 Sales Group with Id: {0} in Company with Id: {1} for Parent Sales Group to Update Level 3 Sales Group.",
                    salesGroup.Id,salesGroup.CompanyId),"Error").AddTags("Data Service Error").Submit();
                return result;
            }
            salesGroupToUpdate.ParentSalesGroupId = getParentSalesGroupResult.Data.Id;

            var managerIds = salesGroup.Managers.Select(m => m.Id).ToList();
            var getVerifiedManagersResult = GetValidManagersInCompanyFromListOfIds(managerIds, salesGroup.CompanyId);

            if (!getVerifiedManagersResult.IsSuccessful) {
                //Logger.Error("Failed to get valid managers for Level 1 Sales Group");
                ExceptionlessClient.Default.CreateLog(typeof(Level3SalesGroupDataService).FullName,"Failed to get valid managers for Level 1 Sales Group","Error").AddTags("Data Service Error").Submit();
                }
      
            salesGroupToUpdate.Name = salesGroup.Name;
            salesGroupToUpdate.Managers = getVerifiedManagersResult.Data;
            salesGroupToUpdate.ModifiedByUserId = salesGroup.ModifiedByUserId;
            salesGroupToUpdate.IsDeleted = salesGroup.IsDeleted;
            result = await base.UpdateAsync(salesGroupToUpdate);

            if (!result.IsSuccessful) {
                result.Error = ErrorValues.GENERIC_UPDATE_SALES_GROUP_ERROR;
            }

            return result;
        }

        public async Task<ServiceProcessingResult> DeleteAsync(string id, string companyId, ApplicationRole loggedInUserRole) {
            var processingResult = new ServiceProcessingResult();

            if (id == null) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.DELETE_SALES_GROUP_WITHOUT_ID_ERROR;
                return processingResult;
            }

            if (!loggedInUserRole.IsAdmin() && !loggedInUserRole.IsSuperAdmin()) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_SALES_GROUP_PERMISSIONS_ERROR;
                return processingResult;
            }

            var validateDeletabilityResult = await ValidateLevel3SalesGroupCanBeDeletedAsync(id, companyId);
            if (!validateDeletabilityResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.DELETE_SALES_GROUP_NOT_IN_COMPANY_ERROR;
                //var logMessage =
                //    String.Format(
                //        "An error occurred while validating deletion for Sales Group with Id: {0} in Company with Id: {1}",
                //        id, companyId);
                //Logger.Error(logMessage);
                ExceptionlessClient.Default.CreateLog(typeof(Level3SalesGroupDataService).FullName,String.Format(
                        "An error occurred while validating deletion for Sales Group with Id: {0} in Company with Id: {1}",id,companyId),"Error").AddTags("Data Service Error").Submit();
                return processingResult;
            }

            if (!validateDeletabilityResult.Data.IsValid) {
                processingResult.IsSuccessful = false;
                processingResult.Error =
                    validateDeletabilityResult.Data.ToProcessingError(
                        ErrorValues.SalesGroupDeleteValidationFailedUserMessage);
                return processingResult;
            }

            processingResult = await base.DeleteAsync(id);

            if (!processingResult.IsSuccessful) {
                processingResult.Error = ErrorValues.GENERIC_DELETE_SALES_GROUP_ERROR;
            }

            return processingResult;
        }

        public async Task<ServiceProcessingResult<List<Level3SalesGroup>>> GetLevel3SalesGroupsInCompany(string companyId) {
            var processingResult = new ServiceProcessingResult<List<Level3SalesGroup>>();

            using (var contextScope = DbContextScopeFactory.CreateReadOnly()) {
                try {
                    processingResult = await GetAllWhereAsync(sg => sg.CompanyId == companyId);

                    if (!processingResult.IsSuccessful) {
                        processingResult.Error = ErrorValues.GENERIC_GET_ALL_SALES_GROUPS_IN_COMPANY_ERROR;
                        //var logMessage = String.Format("Failed to get all Level3SalesGroups in company with Id: {0}",
                        //                   companyId);
                        //Logger.Error(logMessage);
                        ExceptionlessClient.Default.CreateLog(typeof(Level3SalesGroupDataService).FullName,String.Format("Failed to get all Level3SalesGroups in company with Id: {0}",companyId),"Error").AddTags("Data Service Error").Submit();
                        return processingResult;
                    }
                } catch (Exception ex) {
                    ex.ToExceptionless()
                     .SetMessage(String.Format("Failed to get all Level3SalesGroups in company with Id: {0}",
                        companyId))
                     .MarkAsCritical()
                     .Submit();
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_GET_ALL_SALES_GROUPS_IN_COMPANY_ERROR;
                    //var logMessage = String.Format("Failed to get all Level3SalesGroups in company with Id: {0}",
                    //    companyId);
                    //Logger.Error(logMessage);
                }
            }

            return processingResult;
        }

        public async Task<ServiceProcessingResult<List<SalesTeamNameActiveViewBindingModel>>> GetLevel3GroupTeams(string companyId, string Id) {
            var processingResult = new ServiceProcessingResult<List<SalesTeamNameActiveViewBindingModel>>();
            processingResult.Data = new List<SalesTeamNameActiveViewBindingModel>();

            using (var contextScope = DbContextScopeFactory.CreateReadOnly()) {
                try {
                    var includes = new[] { "SalesTeams" };

                    var groupResult = await GetAllWhereAsync(sg => sg.CompanyId == companyId && sg.Id == Id, includes);

                    if (!groupResult.IsSuccessful) {
                        processingResult.Error = ErrorValues.COULD_NOT_FIND_SALES_GROUP_IN_COMPANY_ERROR;
                        //var logMessage = String.Format("Unable to find Level 3 Sales Group (Id: {0})", Id);
                        //Logger.Error(logMessage);
                        ExceptionlessClient.Default.CreateLog(typeof(Level3SalesGroupDataService).FullName,String.Format("Unable to find Level 3 Sales Group (Id: {0})",Id),"Error").AddTags("Data Service Error").Submit();
                        return processingResult;
                    }

                    processingResult.IsSuccessful = true;
                    foreach (var group in groupResult.Data) {
                        if (group.SalesTeams != null) {
                            foreach (var team in group.SalesTeams) {
                                processingResult.Data.Add(team.ToSalesTeamNameActiveViewBindingModel());
                            }
                        }
                    }
                } catch (Exception ex) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.COULD_NOT_FIND_SALES_GROUP_IN_COMPANY_ERROR;
                    //var logMessage = String.Format("Failed to get Level 3 Sales Group (Id: {0})", Id);
                    //Logger.Error(logMessage);
                    ExceptionlessClient.Default.CreateLog(typeof(Level3SalesGroupDataService).FullName,String.Format("Failed to get Level 3 Sales Group (Id: {0})",Id),"Error").AddTags("Data Service Error").Submit();
                    }
            }

            return processingResult;
        }

        public async Task<ServiceProcessingResult<List<UserNameViewBindingModel>>> GetLevel3GroupManagers(string companyId, string Id) {
            var processingResult = new ServiceProcessingResult<List<UserNameViewBindingModel>>();
            processingResult.Data = new List<UserNameViewBindingModel>();

            using (var contextScope = DbContextScopeFactory.CreateReadOnly()) {
                try {
                    var includes = new[] { "Managers" };

                    var groupResult = await GetAllWhereAsync(sg => sg.CompanyId == companyId && sg.Id == Id, includes);

                    if (!groupResult.IsSuccessful) {
                        processingResult.Error = ErrorValues.COULD_NOT_FIND_SALES_GROUP_IN_COMPANY_ERROR;
                        //var logMessage = String.Format("Unable to find Level 3 Sales Group (Id: {0})", Id);
                        //Logger.Error(logMessage);
                        ExceptionlessClient.Default.CreateLog(typeof(Level3SalesGroupDataService).FullName,String.Format("Unable to find Level 3 Sales Group (Id: {0})",Id),"Error").AddTags("Data Service Error").Submit();
                        return processingResult;
                    }

                    processingResult.IsSuccessful = true;
                    foreach (var group in groupResult.Data) {
                        if (group.Managers != null) {
                            foreach (var user in group.Managers) {
                                processingResult.Data.Add(user.ToUserNameViewBindingModel());
                            }
                        }
                    }
                } catch (Exception ex) {
                    ex.ToExceptionless()
                      .SetMessage(String.Format("Failed to get Level 3 Sales Group (Id: {0})",Id))
                      .MarkAsCritical()
                      .Submit();
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.COULD_NOT_FIND_SALES_GROUP_IN_COMPANY_ERROR;
                    //var logMessage = String.Format("Failed to get Level 3 Sales Group (Id: {0})", Id);
                    //Logger.Error(logMessage);
                }
            }

            return processingResult;
        }

        public async Task<ServiceProcessingResult<List<UserNameViewBindingModel>>> getManagersInTree(string companyId, string userId) {
            var processingResult = new ServiceProcessingResult<List<UserNameViewBindingModel>>();
            processingResult.Data = new List<UserNameViewBindingModel>();
            var level1SalesGroupDataService = new Level1SalesGroupDataService();
            var salesGroupTree = await level1SalesGroupDataService.getCompanySalesGroupAdminTreeWhereManagerInTree(companyId, userId);
            if (!salesGroupTree.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = salesGroupTree.Error;
                return processingResult;
            }

            if (salesGroupTree.Data.Any()) {
                var groupIDList = new List<string>();
                foreach (var level1SalesGroup in salesGroupTree.Data) {
                    //groupIDList.Add(level1SalesGroup.Id);

                    foreach (var level2SalesGroup in level1SalesGroup.ChildSalesGroups) {
                        //groupIDList.Add(level2SalesGroup.Id);

                        foreach (var level3SalesGroup in level2SalesGroup.ChildSalesGroups) {
                            groupIDList.Add(level3SalesGroup.Id);
                        }
                    }
                }

                using (var contextScope = DbContextScopeFactory.CreateReadOnly()) {
                    try {
                        var includes = new[] { "Managers" };

                        var groupResult = await GetAllWhereAsync(sg => sg.CompanyId == companyId && groupIDList.Contains(sg.Id), includes);

                        if (!groupResult.IsSuccessful) {
                            processingResult.Error = new ProcessingError("Unable to find any Level 3 Sales Groups", "Unable to find any Level 3 Sales Groups", false, false);
                            //Logger.Error(String.Format("Unable to find any Level 3 Sales Groups"));
                            ExceptionlessClient.Default.CreateLog(typeof(Level3SalesGroupDataService).FullName,"Unable to find any Level 3 Sales Groups","Error").AddTags("Data Service Error").Submit();
                            return processingResult;
                        }

                        processingResult.IsSuccessful = true;
                        foreach (var group in groupResult.Data) {
                            if (group.Managers != null) {
                                foreach (var user in group.Managers) {
                                    processingResult.Data.Add(user.ToUserNameViewBindingModel());
                                }
                            }
                        }
                    } catch (Exception ex) {
                        processingResult.IsSuccessful = false;
                        processingResult.Error = ErrorValues.COULD_NOT_FIND_SALES_GROUP_IN_COMPANY_ERROR;
                        ex.ToExceptionless()
                      .SetMessage(String.Format("Failed to get Level 3 Sales Group (UserID: {0})",userId))
                      .MarkAsCritical()
                      .Submit();
                        //Logger.Error(String.Format("Failed to get Level 3 Sales Group (UserID: {0})", userId));
                    }
                }
            }

            return processingResult;
        }

        public async Task<ServiceProcessingResult<List<UserListViewBindingModel>>> getManagersFromGroupList(string companyId, List<string> groupIDList) {
            var processingResult = new ServiceProcessingResult<List<UserListViewBindingModel>>();
            processingResult.Data = new List<UserListViewBindingModel>();

            using (var contextScope = DbContextScopeFactory.CreateReadOnly()) {
                try {
                    var includes = new[] { "Managers" };

                    var groupResult = await GetAllWhereAsync(sg => sg.CompanyId == companyId && groupIDList.Contains(sg.Id), includes);
                    //var data = groupResult.Data.Any(sg=> sg.Managers.Select(a=>a.Id).Distinct().ToList());
                  
                    
                    if (!groupResult.IsSuccessful) {
                        processingResult.Error = new ProcessingError("Unable to find any Level 3 Sales Groups", "Unable to find any Level 3 Sales Groups", false, false);
                        ExceptionlessClient.Default.CreateLog(typeof(Level3SalesGroupDataService).FullName,"Unable to find any Level 3 Sales Groups","Error").AddTags("Data Service Error").Submit();
                        //Logger.Error(String.Format("Unable to find any Level 3 Sales Groups"));
                        return processingResult;
                    }

                    processingResult.IsSuccessful = true;
                    var L3Managers = new List<UserListViewBindingModel>();
                    foreach (var group in groupResult.Data) {
                        if (group.Managers != null) {
                            foreach (var user in group.Managers) {
                                L3Managers.Add(user.ToUserListViewBindingModel());
                               
                            }
                        }
                    }
                   L3Managers= L3Managers.DistinctBy(a=> a.Id).ToList();
                    processingResult.Data = L3Managers;

                } catch (Exception ex) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.COULD_NOT_FIND_SALES_GROUP_IN_COMPANY_ERROR;
                    //Logger.Error(String.Format("Failed to get Level 3 Sales Group (groupIDList: {0})", groupIDList));
                    ExceptionlessClient.Default.CreateLog(typeof(Level3SalesGroupDataService).FullName,String.Format("Failed to get Level 3 Sales Group (groupIDList: {0})",groupIDList),"Error").AddTags("Data Service Error").Submit();
                    }
            }

            return processingResult;
        }

        public ServiceProcessingResult<Level3SalesGroup> GetLevel3SalesGroupWhereManagerInTree(string level3SalesGroupId, string userId, string companyId) {
            var processingResult = new ServiceProcessingResult<Level3SalesGroup>();

            using (var contextScope = DbContextScopeFactory.CreateReadOnly()) {
                try {
                    var repo = new Level3SalesGroupRepository();
                    processingResult = repo.GetLevel3SalesGroupWhereManagerInTree(level3SalesGroupId, userId, companyId)
                        .ToServiceProcessingResult(ErrorValues.GENERIC_GET_SALES_GROUP_WITH_MANAGER_IN_TREE_ERROR);
                } catch (Exception ex) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_GET_SALES_GROUP_WITH_MANAGER_IN_TREE_ERROR;
                    //Logger.Error("Failed to get Level3SalesGroup where Manager in tree.", ex);
                    ExceptionlessClient.Default.CreateLog(typeof(Level3SalesGroupDataService).FullName,"Failed to get Level3SalesGroup where Manager in tree.","Error").AddTags("Data Service Error").Submit();
                    }
            }

            return processingResult;
        }

        public async Task<ServiceProcessingResult<Level3SalesGroup>> GetExistingLevel3SalesGroupInCompanyAsync(string id, string companyId) {
            var processingResult = new ServiceProcessingResult<Level3SalesGroup>();

            if (id == null) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GET_SALES_GROUP_WITHOUT_ID_ERROR;
                return processingResult;
            }

            var includes = new[] { "SalesTeams" };
            processingResult = await GetWhereAsync(sg => sg.CompanyId == companyId && sg.Id == id, includes);

            if (!processingResult.IsSuccessful) {
                return processingResult;
            }

            if (processingResult.Data == null) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.COULD_NOT_FIND_SALES_GROUP_IN_COMPANY_ERROR;
                return processingResult;
            }

            processingResult.Data.SalesTeams = RemoveDeletedSalesTeams(processingResult.Data.SalesTeams);

            return processingResult;
        }

        public async Task<ServiceProcessingResult<ValidationResult>> ValidateUserIsNotAssignedAsLevel3SalesGroupManagerInCompany(string userId, string companyId) {
            var processingResult = new ServiceProcessingResult<ValidationResult> { IsSuccessful = true };
            var validationResult = new ValidationResult { IsValid = false };

            if (userId == null) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GET_USER_WITHOUT_ID_ERROR;
                return processingResult;
            }

            var NumGroups = 0;
           
            var sqlQuery = new SQLQuery();

            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@UserID", userId) };
            var sqlString = "SELECT COUNT(*) AS NumGroups FROM Level3SalesGroupApplicationUser U LEFT JOIN Level3SalesGroup L ON U.Level3SalesGroup_Id = L.Id WHERE[ApplicationUser_Id] = @UserID AND L.IsDeleted = 0 ";
            var groupResult = await sqlQuery.ExecuteScalarAsync(CommandType.Text,sqlString,parameters);
            if (!groupResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error attempting to validate user not assigned as level 3 sales group manager.","Error attempting to validate user not assigned as level 3 sales group manager.",true,false);
                return processingResult;
                };
            

            if (NumGroups > 0) {
                validationResult.Errors.Add(ErrorValues.UserDeleteFailedDueToManagerAssignmentUserHelp);
            } else {
                validationResult.IsValid = true;
            }

            processingResult.Data = validationResult;

            return processingResult;
        }

        public async Task<ServiceProcessingResult<List<SalesTeam>>> GetSalesTeamsUnderManagerAsync(string userId, string companyId) {
            var processingResult = new ServiceProcessingResult<List<SalesTeam>> { IsSuccessful = true };
            var includes = new[] { "Managers", "SalesTeams" };
            var salesGroupOwnedByUserResult =
                await GetAllWhereAsync(sg => sg.Managers.Any(m => m.Id == userId) && sg.CompanyId == companyId, includes);
            if (!salesGroupOwnedByUserResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_GET_SALES_GROUP_WITH_MANAGER_IN_TREE_ERROR;
                return processingResult;
            }

            processingResult.Data = PluckSalesTeamsFromLevel3SalesGroups(salesGroupOwnedByUserResult.Data);

            return processingResult;
        }

        private List<SalesTeam> PluckSalesTeamsFromLevel3SalesGroups(IEnumerable<Level3SalesGroup> level3SalesGroups) {
            return (from level3SalesGroup in level3SalesGroups from salesTeam in level3SalesGroup.SalesTeams where salesTeam.IsActive select salesTeam).ToList();
        }

        private async Task<ServiceProcessingResult<ValidationResult>> ValidateLevel3SalesGroupCanBeDeletedAsync(
            string salesGroupId, string companyId) {
            var processingResult = new ServiceProcessingResult<ValidationResult> { IsSuccessful = true };
            var validationResult = new ValidationResult();

            var getSalesGroupResult = await GetExistingLevel3SalesGroupInCompanyAsync(salesGroupId, companyId);
            if (!getSalesGroupResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.DELETE_SALES_GROUP_NOT_IN_COMPANY_ERROR;
                //var logMessage =
                //    String.Format(
                //        "Unable to find Level 3 Sales Group with Id: {0} in Company with Id: {1} for deletion.",
                //        salesGroupId, companyId);
                //Logger.Error(logMessage);
                ExceptionlessClient.Default.CreateLog(typeof(Level3SalesGroupDataService).FullName,String.Format(
                        "Unable to find Level 3 Sales Group with Id: {0} in Company with Id: {1} for deletion.",
                        salesGroupId,companyId),"Error").AddTags("Data Service Error").Submit();
                return processingResult;
            }
            var salesGroup = getSalesGroupResult.Data;

            if (salesGroup.SalesTeams.Count != 0) {
                validationResult.Errors.Add(ErrorValues.CannotDeleteSalesGroupWithChildSalesGroupsOrTeamsUserHelp);
            }

            if (salesGroup.Managers.Count != 0) {
                validationResult.Errors.Add(ErrorValues.CannotDeleteSalesGroupWithManagersAssignedUserHelp);
            }

            validationResult.IsValid = validationResult.Errors.Count == 0;
            processingResult.Data = validationResult;

            return processingResult;
        }

        private async Task<ServiceProcessingResult<Level2SalesGroup>> GetExistingLevel2SalesGroupInCompany(string id, string companyId) {
            var level2SalesGroupDataService = new Level2SalesGroupDataService();
            var result = await level2SalesGroupDataService.GetExistingLevel2SalesGroupInCompany(id, companyId);

            return result;
        }

        private static List<SalesTeam> RemoveDeletedSalesTeams(IEnumerable<SalesTeam> salesTeams) {
            return salesTeams.Where(salesTeam => !salesTeam.IsDeleted).ToList();
        }
    }
}

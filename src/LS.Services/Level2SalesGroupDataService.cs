using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LS.Core;
using LS.Domain;
using LS.Repositories;
using LS.ApiBindingModels;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using Exceptionless;
using Exceptionless.Models;
namespace LS.Services
{
    public class Level2SalesGroupDataService : BaseSalesGroupDataService<Level2SalesGroup, string>
    {

        public Level2SalesGroupDataService() : base(ApplicationRoleRulesHelper.Level2SalesGroupManagerRank)
        {
        }

        public override BaseRepository<Level2SalesGroup, string> GetDefaultRepository()
        {
            return new Level2SalesGroupRepository();
        }

        public async Task<ServiceProcessingResult<Level2SalesGroup>> AddAsync(Level2SalesGroup salesGroup, ApplicationUser loggedInUser)
        {
            var result = new ServiceProcessingResult<Level2SalesGroup>();

            if (loggedInUser.Role.Rank> ValidApplicationRoles.LevelOneManagerRank)
            {
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_SALES_GROUP_PERMISSIONS_ERROR;
                return result;
            }

            var getParentSalesGroupResult = await GetExistingLevel1SalesGroupInCompany(salesGroup.ParentSalesGroupId, salesGroup.CompanyId);
            if (!getParentSalesGroupResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Error = ErrorValues.ADD_SALES_GROUP_PARENT_SALES_GROUP_NOT_IN_COMPANY_ERROR;
                //var logMessage =
                //    String.Format(
                //        "Unable to find Level 1 Sales Group with Id: {0} in Company with Id: {1} for Parent Sales Group to Add a Level 2 Sales Group.");
                //Logger.Error(logMessage);
                ExceptionlessClient.Default.CreateLog(typeof(Level2SalesGroupDataService).FullName,String.Format(
                        "Unable to find Level 1 Sales Group with Id: {0} in Company with Id: {1} for Parent Sales Group to Add a Level 2 Sales Group.",salesGroup.ParentSalesGroupId,salesGroup.CompanyId),"Error").AddTags("Data Service Error").Submit();
                return result;
            }
            salesGroup.ParentSalesGroupId = getParentSalesGroupResult.Data.Id;

            var managerIds = salesGroup.Managers.Select(m => m.Id).ToList();
            var getVerifiedManagersResult = GetValidManagersInCompanyFromListOfIds(managerIds, salesGroup.CompanyId);
            if (!getVerifiedManagersResult.IsSuccessful)
            {
                var logMessage =
                    String.Format("Failed to get valid managers for new Level 2 Sales Group from User Ids: {0}.",
                        String.Join(", ", managerIds));
                Logger.Error(logMessage);
                ExceptionlessClient.Default.CreateLog(typeof(Level2SalesGroupDataService).FullName,String.Format("Failed to get valid managers for new Level 2 Sales Group from User Ids: {0}.",
                        String.Join(", ",managerIds)),"Error").AddTags("Data Service Error").Submit();
                }

            salesGroup.Managers = getVerifiedManagersResult.Data;

            result = await base.AddAsync(salesGroup);

            if (!result.IsSuccessful)
            {
                result.Error = ErrorValues.GENERIC_ADD_SALES_GROUP_ERROR;
            }

            return result;
        }

        public async Task<ServiceProcessingResult<Level2SalesGroup>> UpdateAsync(Level2SalesGroup salesGroup,ApplicationUser loggedInUser)
        {
            var result = new ServiceProcessingResult<Level2SalesGroup>();

            if ( loggedInUser.Role.Rank>2)
            {
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_SALES_GROUP_PERMISSIONS_ERROR;
                return result;
            }
            if (salesGroup.IsDeleted == true)
            {
                var validateDeletabilityResult = await ValidateLevel2SalesGroupCanBeDeletedAsync(salesGroup.Id, loggedInUser.CompanyId);
                if (!validateDeletabilityResult.IsSuccessful)
                {
                    result.IsSuccessful = false;
                    result.Error = ErrorValues.DELETE_SALES_GROUP_NOT_IN_COMPANY_ERROR;
                    //var logMessage =
                    //    String.Format(
                    //        "An error occurred while validating deletion for Sales Group with Id: {0} in Company with Id: {1}",
                    //        loggedInUser.Id, loggedInUser.CompanyId);
                    //Logger.Error(logMessage);

                    ExceptionlessClient.Default.CreateLog(typeof(Level2SalesGroupDataService).FullName,String.Format(
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

            var getSalesGroupResult = await GetExistingLevel2SalesGroupInCompany(salesGroup.Id, salesGroup.CompanyId);
            if (!getSalesGroupResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Error = ErrorValues.UPDATE_SALES_GROUP_NOT_IN_COMPANY_ERROR;
                //var logMessage =
                    //String.Format("Failed to get Level 2 Sales Group with Id: {0} in Company with Id: {1} for Update.",
                    //    salesGroup.Id, salesGroup.CompanyId);
                //Logger.Error(logMessage);
                ExceptionlessClient.Default.CreateLog(typeof(Level2SalesGroupDataService).FullName,String.Format("Failed to get Level 2 Sales Group with Id: {0} in Company with Id: {1} for Update.",salesGroup.Id,salesGroup.CompanyId),"Error").AddTags("Data Service Error").Submit();
                return result;
            }
            var salesGroupToUpdate = getSalesGroupResult.Data;
            
            var getParentSalesGroupResult = await GetExistingLevel1SalesGroupInCompany(salesGroup.ParentSalesGroupId, salesGroup.CompanyId);
            if (!getParentSalesGroupResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Error = ErrorValues.UPDATE_SALES_GROUP_PARENT_SALES_GROUP_NOT_IN_COMPANY_ERROR;
                //var logMessage =
                //    String.Format(
                //        "Failed to get Level 1 Sales Group with Id: {0} in Company with Id: {1} for Parent Sales Group to Update Level 2 Sales Group.");
                //Logger.Error(logMessage);
                ExceptionlessClient.Default.CreateLog(typeof(Level2SalesGroupDataService).FullName,String.Format(
                        "Failed to get Level 1 Sales Group with Id: {0} in Company with Id: {1} for Parent Sales Group to Update Level 2 Sales Group.",salesGroup.ParentSalesGroupId,salesGroup.CompanyId),"Error").AddTags("Data Service Error").Submit();
                return result;
            }
            salesGroupToUpdate.ParentSalesGroupId = getParentSalesGroupResult.Data.Id;

            var managerIds = salesGroup.Managers.Select(m => m.Id).ToList();
            var getVerifiedManagersResult = GetValidManagersInCompanyFromListOfIds(managerIds, salesGroup.CompanyId);
            if (!getVerifiedManagersResult.IsSuccessful)
            {
                //var logMessage =
                //    String.Format(
                //        "Failed to get valid managers for Level 2 Sales Group with Id: {0} from User Ids: {1}.",
                //        salesGroup.Id, String.Join(", ", managerIds));
                //Logger.Error(logMessage);
                ExceptionlessClient.Default.CreateLog(typeof(Level2SalesGroupDataService).FullName,String.Format(
                        "Failed to get valid managers for Level 2 Sales Group with Id: {0} from User Ids: {1}.",
                        salesGroup.Id,String.Join(", ",managerIds)),"Error").AddTags("Data Service Error").Submit();
                }

            salesGroupToUpdate.Name = salesGroup.Name;
            salesGroupToUpdate.Managers = getVerifiedManagersResult.Data;
            salesGroupToUpdate.ModifiedByUserId = salesGroup.ModifiedByUserId;
            salesGroupToUpdate.IsDeleted = salesGroup.IsDeleted;
            result = await base.UpdateAsync(salesGroupToUpdate);

            if (!result.IsSuccessful)
            {
                result.Error = ErrorValues.GENERIC_UPDATE_SALES_GROUP_ERROR;
            }

            return result;
        }

        public async Task<ServiceProcessingResult> DeleteAsync(string id, string companyId, ApplicationRole loggedInUserRole)
        {
            var processingResult = new ServiceProcessingResult();

            if (id == null)
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.DELETE_SALES_GROUP_WITHOUT_ID_ERROR;

                return processingResult;
            }

            if (!loggedInUserRole.IsAdmin() && !loggedInUserRole.IsSuperAdmin())
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_SALES_GROUP_PERMISSIONS_ERROR;
                return processingResult;
            }

            var validateDeletabilityResult = await ValidateLevel2SalesGroupCanBeDeletedAsync(id, companyId);
            if (!validateDeletabilityResult.IsSuccessful)
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.DELETE_SALES_GROUP_NOT_IN_COMPANY_ERROR;
                //var logMessage =
                //    String.Format(
                //        "An error occurred while validating deletion for Sales Group with Id: {0} in Company with Id: {1}",
                //        id, companyId);
                //Logger.Error(logMessage);
                ExceptionlessClient.Default.CreateLog(typeof(Level2SalesGroupDataService).FullName,String.Format(
                        "An error occurred while validating deletion for Sales Group with Id: {0} in Company with Id: {1}",
                        id,companyId),"Error").AddTags("Data Service Error").Submit();

                return processingResult;
            }

            if (!validateDeletabilityResult.Data.IsValid)
            {
                processingResult.IsSuccessful = false;
                processingResult.Error =
                    validateDeletabilityResult.Data.ToProcessingError(
                        ErrorValues.SalesGroupDeleteValidationFailedUserMessage);
                return processingResult;
            }

            processingResult = await DeleteAsync(id);

            if (!processingResult.IsSuccessful)
            {
                processingResult.Error = ErrorValues.GENERIC_DELETE_SALES_GROUP_ERROR;
            }

            return processingResult;
        } 

        public async Task<ServiceProcessingResult<Level2SalesGroup>> GetExistingLevel2SalesGroupInCompany(string id, string companyId)
        {
            var processingResult = new ServiceProcessingResult<Level2SalesGroup>();

            if (id == null)
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GET_SALES_GROUP_WITHOUT_ID_ERROR;
                return processingResult;
            }

            var includes = new[] { "ChildSalesGroups" };
            processingResult = await GetWhereAsync(sg => sg.CompanyId == companyId && sg.Id == id, includes);

            if (!processingResult.IsSuccessful)
            {
                return processingResult;
            }

            if (processingResult.Data == null)
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_GET_SALES_GROUP_IN_COMPANY_ERROR;
                return processingResult;
            }

            processingResult.Data.ChildSalesGroups =
                RemoveDeletedLevel3SalesGroups(processingResult.Data.ChildSalesGroups);

            return processingResult;
        }

        public async Task<ServiceProcessingResult<ValidationResult>> ValidateUserIsNotAssignedAsLevel2SalesGroupManagerInCompany(string userId, string companyId) {
            var processingResult = new ServiceProcessingResult<ValidationResult> { IsSuccessful = true };
            var validationResult = new ValidationResult { IsValid = false };

            if (userId == null) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GET_USER_WITHOUT_ID_ERROR;
                return processingResult;
            }

            int NumGroups = 0;
            

                var sqlQuery = new SQLQuery();

                SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@UserID", userId) };
                var sqlString = "SELECT COUNT(*) AS NumGroups FROM Level2SalesGroupApplicationUser U LEFT JOIN Level2SalesGroup L ON U.Level2SalesGroup_Id = L.Id WHERE[ApplicationUser_Id] = @UserID AND L.IsDeleted = 0";
                var groupResult = await sqlQuery.ExecuteScalarAsync(CommandType.Text,sqlString, parameters);
            if (!groupResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error attempting to validate user not assigned as level 2 sales group manager.","Error attempting to validate user not assigned as level 2 sales group manager.",true,false);
                return processingResult;
                };
            NumGroups =(int) groupResult.Data; //expecting an integer or a count.
            
            if (NumGroups > 0) {
                validationResult.Errors.Add(ErrorValues.UserDeleteFailedDueToManagerAssignmentUserHelp);
            } else {
                validationResult.IsValid = true;
            }

            processingResult.Data = validationResult;

            return processingResult;
        }

        public async Task<ServiceProcessingResult<List<SalesTeam>>> GetSalesTeamsUnderManager(string userId, string companyId)
        {
            var processingResult = new ServiceProcessingResult<List<SalesTeam>> { IsSuccessful = true };
            var includes = new[] {"Managers", "ChildSalesGroups.SalesTeams"};
            var salesGroupOwnedByManagerResult =
                await GetAllWhereAsync(sg => sg.Managers.Any(m => m.Id == userId) && sg.CompanyId == companyId, includes);
            if (!salesGroupOwnedByManagerResult.IsSuccessful || salesGroupOwnedByManagerResult.Data == null)
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_GET_SALES_GROUP_WITH_MANAGER_IN_TREE_ERROR;
                return processingResult;
            }

            processingResult.Data = PluckSalesTeamsFromLevel2SalesGroups(salesGroupOwnedByManagerResult.Data);

            return processingResult;
        }

        private List<SalesTeam> PluckSalesTeamsFromLevel2SalesGroups(IEnumerable<Level2SalesGroup> level2SalesGroups)
        {
            return (from level2SalesGroup in level2SalesGroups
                from level3SalesGroup in level2SalesGroup.ChildSalesGroups
                from salesTeam in level3SalesGroup.SalesTeams
                where salesTeam.IsActive
                select salesTeam).ToList();
        }

        private async Task<ServiceProcessingResult<ValidationResult>> ValidateLevel2SalesGroupCanBeDeletedAsync(string salesGroupId, string companyId)
        {
            var processingResult = new ServiceProcessingResult<ValidationResult> { IsSuccessful = true };
            var validationResult = new ValidationResult();

            var getSalesGroupResult = await GetExistingLevel2SalesGroupInCompany(salesGroupId, companyId);
            if (!getSalesGroupResult.IsSuccessful)
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.DELETE_SALES_GROUP_NOT_IN_COMPANY_ERROR;
                var logMessage =
                    String.Format(
                        "Unable to find Level 2 Sales Group with Id: {0} in Company with Id: {1} for deletion.",
                        salesGroupId, companyId);
                Logger.Error(logMessage);
                ExceptionlessClient.Default.CreateLog(typeof(Level2SalesGroupDataService).FullName,String.Format(
                        "Unable to find Level 2 Sales Group with Id: {0} in Company with Id: {1} for deletion.",
                        salesGroupId,companyId),"Error").AddTags("Data Service Error").Submit();

                return processingResult;
            }
            var salesGroup = getSalesGroupResult.Data;

            if (salesGroup.ChildSalesGroups.Count != 0)
            {
                validationResult.Errors.Add(ErrorValues.CannotDeleteSalesGroupWithChildSalesGroupsOrTeamsUserHelp);
            }

            if (salesGroup.Managers.Count != 0)
            {
                validationResult.Errors.Add(ErrorValues.CannotDeleteSalesGroupWithManagersAssignedUserHelp);
            }

            validationResult.IsValid = validationResult.Errors.Count == 0;
            processingResult.Data = validationResult;

            return processingResult;
        }

        private async Task<ServiceProcessingResult<Level1SalesGroup>> GetExistingLevel1SalesGroupInCompany(string id, string companyId)
        {
            var level1SalesGroupDataService = new Level1SalesGroupDataService();
            var result = await level1SalesGroupDataService.GetExistingLevel1SalesGroupInCompanyAsync(id, companyId);

            return result;
        }

        public async Task<ServiceProcessingResult<List<GroupTreeViewBindingModel>>> GetLevel2GroupGroups(string companyId, string Id, string userId) {
            var processingResult = new ServiceProcessingResult<List<GroupTreeViewBindingModel>>() { IsSuccessful = true };
            processingResult.Data = new List<GroupTreeViewBindingModel>();

            using (var contextScope = DbContextScopeFactory.CreateReadOnly()) {
                try {
                    SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                    var sqlString = "SELECT * FROM v_UserActiveTeams WHERE CompanyId = @CompanyID AND Level2ID = @Level2ID AND UserID = @UserID  ORDER BY Level2Id, Level3Id, Id";
                    SqlCommand cmd = new SqlCommand(sqlString, connection);
                    SqlDataReader rdr = null;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter("@CompanyId", companyId));
                    cmd.Parameters.Add(new SqlParameter("@Level2ID", Id));
                    cmd.Parameters.Add(new SqlParameter("@UserID", userId));
                    try {
                        connection.Open();
                        rdr = cmd.ExecuteReader();
                        var userGroupsResult = new List<GroupTreeViewBindingModel>();
                        var currLevel2ID = "";
                        var currLevel3ID = "";
                        var currentLevel2Row = -1;
                        var currentLevel3Row = -1;

                        while (rdr.Read())
                            {
                            if (rdr["Level2Id"].ToString() != currLevel2ID)
                                {
                                userGroupsResult.Add(new GroupTreeViewBindingModel
                                    {
                                    Id = rdr["Level2Id"].ToString(),
                                    Name = rdr["Level2Name"].ToString(),
                                    ChildSalesGroups = new List<GroupTreeViewBindingModel>()
                                    });
                                currentLevel2Row = userGroupsResult.Count - 1;
                                currLevel2ID = rdr["Level2Id"].ToString();
                                currLevel3ID = "";
                                currentLevel3Row = -1;
                                }

                            if (rdr["Level3Id"].ToString() != currLevel3ID)
                                {
                                userGroupsResult[currentLevel2Row].ChildSalesGroups.Add(new GroupTreeViewBindingModel
                                    {
                                    Id = rdr["Level3Id"].ToString(),
                                    Name = rdr["Level3Name"].ToString(),
                                    ChildSalesGroups = new List<GroupTreeViewBindingModel>(),
                                    SalesTeams = new List<SalesTeamNameUsersNoTeamViewBindingModel>()
                                    });
                                currentLevel3Row = userGroupsResult[currentLevel2Row].ChildSalesGroups.Count - 1;
                                currLevel3ID = rdr["Level3Id"].ToString();
                                }

                            if (rdr["Id"].ToString() != "")
                                {
                                userGroupsResult[currentLevel2Row].ChildSalesGroups[currentLevel3Row].SalesTeams.Add(new SalesTeamNameUsersNoTeamViewBindingModel
                                    {
                                    Id = rdr["Id"].ToString()
                                    ,
                                    Name = rdr["Name"].ToString()
                                    ,
                                    IsActive = (bool)rdr["IsActive"]
                                    ,
                                    ExternalDisplayName = rdr["ExternalDisplayName"].ToString()
                                    });
                                }
                            }
                        processingResult.Data = userGroupsResult;

                        }
                    catch (Exception ex) {
                        ex.ToExceptionless()
                      .SetMessage(String.Format("GetLevel2GroupGroups database call failed (Lookup User: {0}, Lookup Level 2: {1})",userId,Id))
                      .MarkAsCritical()
                      .Submit();
                        processingResult.IsSuccessful = false;
                        processingResult.Error = ErrorValues.GENERIC_GET_SALES_GROUP_IN_COMPANY_ERROR;
                        //Logger.Error(String.Format("GetLevel2GroupGroups database call failed (Lookup User: {0}, Lookup Level 2: {1})", userId, Id), ex);

                        return processingResult;
                        }
                
                   

                  

                    //var appUserDataService = new ApplicationUserDataService();
                    //var getUserResult = await appUserDataService.GetAsync(userId);
                    //if (!getUserResult.IsSuccessful) {
                    //    processingResult.IsSuccessful = false;
                    //    processingResult.Error = ErrorValues.GENERIC_COULD_NOT_FIND_USER_ERROR;
                    //    return processingResult;
                    //}
                    //var user = getUserResult.Data;

                    //var includes = new[] {
                    //   "Managers.Roles.Role", "ChildSalesGroups.Managers.Roles.Role",
                    //   "ChildSalesGroups.SalesTeams","ChildSalesGroups.SalesTeams.users","ChildSalesGroups.SalesTeams.users.roles",
                    //   "ChildSalesGroups.SalesTeams.users.roles.role"
                    //};

                    //var getResult = new ServiceProcessingResult<List<Level2SalesGroup>>();
                    //if (user.Role.IsAdmin() || user.Role.IsSuperAdmin()) {
                    //    getResult = await GetAllWhereAsync(sg => sg.CompanyId == companyId && sg.Id == Id, includes);
                    //} else if (user.Role.Rank == ValidApplicationRoles.LevelOneManagerRank) {
                    //    //TODO: Make sure the Level 2 GroupID that is passed in is within this Level 1 Manager's tree
                    //    getResult = await GetAllWhereAsync(sg => sg.CompanyId == companyId && sg.Id == Id, includes);
                    //} else if (user.Role.Rank == ValidApplicationRoles.LevelTwoManagerRank) {
                    //    getResult = await GetAllWhereAsync(sg => sg.CompanyId == companyId && sg.Id == Id && sg.Managers.Any(m => m.Id == userId), includes);
                    //} else {
                    //    getResult = await GetAllWhereAsync(sg => sg.CompanyId == companyId && sg.Id == Id && sg.ChildSalesGroups.Any(csg => csg.Managers.Any(m => m.Id == userId)), includes);
                    //}

                    //if (!getResult.IsSuccessful) {
                    //    processingResult.Error = ErrorValues.COULD_NOT_FIND_SALES_GROUP_IN_COMPANY_ERROR;
                    //    var logMessage = String.Format("Unable to find Level 2 Sales Group (Id: {0})", Id);
                    //    Logger.Error(logMessage);
                    //    return processingResult;
                    //}

                    //processingResult.IsSuccessful = true;
                    //getResult.Data = RemoveDeletedLevel2And3SalesGroupsAndSalesTeamsFromSalesGroupTree(getResult.Data);
                    //foreach (var group in getResult.Data) {
                    //    processingResult.Data.Add(group.ToLevel2GroupTreeViewBindingModel());
                    //}
                } catch (Exception ex) {
                    ex.ToExceptionless()
                     .SetMessage(String.Format("Failed to get Level 2 Sales Group (Id: {0})",Id))
                     .MarkAsCritical()
                     .Submit();
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.COULD_NOT_FIND_SALES_GROUP_IN_COMPANY_ERROR;
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
                        groupIDList.Add(level2SalesGroup.Id);

                        //foreach (var level3SalesGroup in level2SalesGroup.ChildSalesGroups) {
                        //    groupIDList.Add(level3SalesGroup.Id);
                        //}
                    }
                }

                using (var contextScope = DbContextScopeFactory.CreateReadOnly()) {
                    try {
                        var includes = new[] { "Managers" };

                        var groupResult = await GetAllWhereAsync(sg => sg.CompanyId == companyId && groupIDList.Contains(sg.Id), includes);

                        if (!groupResult.IsSuccessful) {
                            processingResult.Error = new ProcessingError("Unable to find any Level 2 Sales Groups", "Unable to find any Level 2 Sales Groups", false, false);
                            //Logger.Error(String.Format("Unable to find any Level 2 Sales Groups"));
                            ExceptionlessClient.Default.CreateLog(typeof(Level2SalesGroupDataService).FullName,"Unable to find any Level 2 Sales Groups","Error").AddTags("Data Service Error").Submit();
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
                        ExceptionlessClient.Default.CreateLog(typeof(Level2SalesGroupDataService).FullName,String.Format("Failed to get Level 2 Sales Group (UserID: {0})",userId),"Error").AddTags("Data Service Error").Submit();
                        processingResult.IsSuccessful = false;
                        processingResult.Error = ErrorValues.COULD_NOT_FIND_SALES_GROUP_IN_COMPANY_ERROR;
                        //Logger.Error(String.Format("Failed to get Level 2 Sales Group (UserID: {0})", userId));
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

                    if (!groupResult.IsSuccessful) {
                        processingResult.Error = new ProcessingError("Unable to find any Level 2 Sales Groups", "Unable to find any Level 2 Sales Groups", false, false);
                        ExceptionlessClient.Default.CreateLog(typeof(Level2SalesGroupDataService).FullName,"Unable to find any Level 2 Sales Groups","Error").AddTags("Data Service Error").Submit();
                        //Logger.Error(String.Format("Unable to find any Level 2 Sales Groups"));
                        return processingResult;
                    }

                    processingResult.IsSuccessful = true;
                    foreach (var group in groupResult.Data) {
                        if (group.Managers != null) {
                            foreach (var user in group.Managers) {
                                processingResult.Data.Add(user.ToUserListViewBindingModel());
                            }
                        }
                    }
                } catch (Exception ex) {
                    ex.ToExceptionless()
                      .SetMessage(String.Format("Failed to get Level 2 Sales Group (groupIDList: {0})",groupIDList))
                      .MarkAsCritical()
                      .Submit();
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.COULD_NOT_FIND_SALES_GROUP_IN_COMPANY_ERROR;
                    Logger.Error(String.Format("Failed to get Level 2 Sales Group (groupIDList: {0})", groupIDList));
                }
            }

            return processingResult;
        }

        public async Task<ServiceProcessingResult<List<UserNameViewBindingModel>>> GetLevel2GroupManagers(string companyId, string Id) {
            var processingResult = new ServiceProcessingResult<List<UserNameViewBindingModel>>();
            processingResult.Data = new List<UserNameViewBindingModel>();

            using (var contextScope = DbContextScopeFactory.CreateReadOnly()) {
                try {
                    var includes = new[] { "Managers" };

                    var groupResult = await GetAllWhereAsync(sg => sg.CompanyId == companyId && sg.Id == Id, includes);

                    if (!groupResult.IsSuccessful) {
                        processingResult.Error = ErrorValues.COULD_NOT_FIND_SALES_GROUP_IN_COMPANY_ERROR;
                        //var logMessage = String.Format("Unable to find Level 2 Sales Group (Id: {0})", Id);
                        //Logger.Error(logMessage);
                        ExceptionlessClient.Default.CreateLog(typeof(Level2SalesGroupDataService).FullName,String.Format("Unable to find Level 2 Sales Group (Id: {0})",Id),"Error").AddTags("Data Service Error").Submit();

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
                      .SetMessage(String.Format("Failed to get Level 2 Sales Group (Id: {0})",Id))
                      .MarkAsCritical()
                      .Submit();
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.COULD_NOT_FIND_SALES_GROUP_IN_COMPANY_ERROR;
                    //var logMessage = String.Format("Failed to get Level 2 Sales Group (Id: {0})", Id);
                    //Logger.Error(logMessage);
                }
            }

            return processingResult;
        }

        private static List<Level3SalesGroup> RemoveDeletedLevel3SalesGroups(IEnumerable<Level3SalesGroup> level3SalesGroups)
        {
            return level3SalesGroups.Where(level3SalesGroup => !level3SalesGroup.IsDeleted).ToList();
        }

        private static List<Level2SalesGroup> RemoveDeletedLevel2And3SalesGroupsAndSalesTeamsFromSalesGroupTree(IEnumerable<Level2SalesGroup> level2SalesGroups) {
            var level2SalesGroupsToReturn = new List<Level2SalesGroup>();
            foreach (var level2SalesGroup in level2SalesGroups) {
                if (!level2SalesGroup.IsDeleted) {
                    var newLevel2SalesGroup = level2SalesGroup;
                    newLevel2SalesGroup.ChildSalesGroups =
                        RemoveDeletedLevel3SalesGroupsAndSalesTeamsFromSalesGroupTree(level2SalesGroup.ChildSalesGroups);
                    level2SalesGroupsToReturn.Add(newLevel2SalesGroup);
                }
            }

            return level2SalesGroupsToReturn;
        }

        private static List<Level3SalesGroup> RemoveDeletedLevel3SalesGroupsAndSalesTeamsFromSalesGroupTree(IEnumerable<Level3SalesGroup> level3SalesGroups) {
            var level3SalesGroupsToReturn = new List<Level3SalesGroup>();
            foreach (var level3SalesGroup in level3SalesGroups) {
                if (!level3SalesGroup.IsDeleted) {
                    var newLevel3SalesGroup = level3SalesGroup;
                    newLevel3SalesGroup.SalesTeams = RemoveDeletedSalesTeams(level3SalesGroup.SalesTeams);
                    level3SalesGroupsToReturn.Add(newLevel3SalesGroup);
                }
            }

            return level3SalesGroupsToReturn;
        }

        private static List<SalesTeam> RemoveDeletedSalesTeams(IEnumerable<SalesTeam> salesTeams) {
            var teams = salesTeams.Where(salesTeam => !salesTeam.IsDeleted).ToList();
            //remove deleted user
            foreach (var team in teams) {
                foreach (var user in team.Users.ToList()) {
                    if (user.IsDeleted == true) {
                        team.Users.Remove(user);
                    }
                }
            }
            return teams;
        }
    }
}

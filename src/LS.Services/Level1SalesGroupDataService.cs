using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LS.Core;
using LS.Domain;
using LS.Repositories;
using LS.ApiBindingModels;
using System.Data.SqlClient;
using System.Configuration;
using Exceptionless;
using Exceptionless.Models;
namespace LS.Services {
    public class Level1SalesGroupDataService : BaseSalesGroupDataService<Level1SalesGroup,string> {
        public Level1SalesGroupDataService() : base(ApplicationRoleRulesHelper.Level1SalesGroupManagerRank) {

            }

        public override BaseRepository<Level1SalesGroup,string> GetDefaultRepository() {
            return new Level1SalesGroupRepository();
            }

        public async Task<ServiceProcessingResult<Level1SalesGroup>> AddAsync(Level1SalesGroup salesGroup,ApplicationRole loggedInUserRole) {
            var processingResult = new ServiceProcessingResult<Level1SalesGroup>();

            if (!loggedInUserRole.IsAdmin() && !loggedInUserRole.IsSuperAdmin())
                {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_SALES_GROUP_PERMISSIONS_ERROR;
                return processingResult;
                }

            var managerIds = salesGroup.Managers.Select(m => m.Id).ToList();
            var getVerifiedManagersResult = GetValidManagersInCompanyFromListOfIds(managerIds,salesGroup.CompanyId);
            if (!getVerifiedManagersResult.IsSuccessful)
                {
                var logMessage =
                    String.Format("Failed to get valid managers for new Level 1 Sales Group from User Ids for Company With Id: {0}.",
                        salesGroup.CompanyId);
                Logger.Error(logMessage);
                }

            salesGroup.Managers = getVerifiedManagersResult.Data;

            processingResult = await base.AddAsync(salesGroup);

            if (!processingResult.IsSuccessful)
                {
                processingResult.Error = ErrorValues.GENERIC_ADD_SALES_GROUP_ERROR;
                }

            return processingResult;
            }

        public async Task<ServiceProcessingResult<Level1SalesGroup>> UpdateAsync(Level1SalesGroup salesGroup,ApplicationUser loggedInUser) {
            var result = new ServiceProcessingResult<Level1SalesGroup>();

            if (!loggedInUser.Role.IsAdmin() && !loggedInUser.Role.IsSuperAdmin())
                {
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_SALES_GROUP_PERMISSIONS_ERROR;
                return result;
                }
            if (salesGroup.IsDeleted == true)
                {
                var validateDeletabilityResult = await ValidateLevel1SalesGroupCanBeDeletedAsync(salesGroup.Id,loggedInUser.CompanyId);
                if (!validateDeletabilityResult.IsSuccessful)
                    {
                    result.IsSuccessful = false;
                    result.Error = ErrorValues.DELETE_SALES_GROUP_NOT_IN_COMPANY_ERROR;
                    var logMessage =
                        String.Format(
                            "An error occurred while validating deletion for Sales Group with Id: {0} in Company with Id: {1}",
                            loggedInUser.Id,loggedInUser.CompanyId);
                    Logger.Error(logMessage);
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





            var getSalesGroupResult = await GetExistingLevel1SalesGroupInCompanyAsync(salesGroup.Id,salesGroup.CompanyId);
            if (!getSalesGroupResult.IsSuccessful)
                {
                result.IsSuccessful = false;
                result.Error = ErrorValues.UPDATE_SALES_GROUP_NOT_IN_COMPANY_ERROR;
                var logMessage =
                    String.Format("Failed to get Level 1 Sales Group with Id: {0} in Company with Id: {1} for Update.",
                        salesGroup.Id,salesGroup.CompanyId);
                Logger.Error(logMessage);
                return result;
                }





            var salesGroupToUpdate = getSalesGroupResult.Data;

            var managerIds = salesGroup.Managers.Select(m => m.Id).ToList();
            var getVerifiedManagersResult = GetValidManagersInCompanyFromListOfIds(managerIds,salesGroup.CompanyId);
            if (!getVerifiedManagersResult.IsSuccessful)
                {
                var logMessage = String.Format("Failed to get valid managers for Level 1 Sales Group of Id: {0} from User Ids: {1}.",salesGroupToUpdate.Id,String.Join(", ",managerIds));
                Logger.Error(logMessage);
                }

            salesGroupToUpdate.Managers = getVerifiedManagersResult.Data;
            salesGroupToUpdate.Name = salesGroup.Name;
            salesGroupToUpdate.ModifiedByUserId = salesGroup.ModifiedByUserId;
            salesGroupToUpdate.IsDeleted = salesGroup.IsDeleted;
            result = await base.UpdateAsync(salesGroupToUpdate);

            if (!result.IsSuccessful)
                {
                result.Error = ErrorValues.GENERIC_UPDATE_SALES_GROUP_ERROR;
                }

            return result;
            }

        //public async Task<ServiceProcessingResult<List<UserTreeViewBindingModel>>> GetCompanyUserTreeWhereManagerInTree(string companyId, string userId) {
        //    var processingResult = new ServiceProcessingResult<List<UserTreeViewBindingModel>> { };

        //    var appUserDataService = new ApplicationUserDataService();
        //    var getUserResult = await appUserDataService.GetAsync(userId);
        //    if (!getUserResult.IsSuccessful) {
        //        processingResult.IsSuccessful = false;
        //        processingResult.Error = ErrorValues.GENERIC_COULD_NOT_FIND_USER_ERROR;
        //        return processingResult;
        //    }
        //    var user = getUserResult.Data;

        //    var includes = new[] {
        //       "Managers.Roles.Role", "ChildSalesGroups.Managers.Roles.Role", "ChildSalesGroups.ChildSalesGroups.Managers.Roles.Role",
        //       "ChildSalesGroups.ChildSalesGroups.SalesTeams","ChildSalesGroups.ChildSalesGroups.SalesTeams.users","ChildSalesGroups.ChildSalesGroups.SalesTeams.users.roles",
        //       "ChildSalesGroups.ChildSalesGroups.SalesTeams.users.roles.role"
        //    };

        //    var getResult = new ServiceProcessingResult<List<Level1SalesGroup>>();
        //    if (user.Role.IsAdmin() || user.Role.IsSuperAdmin()) {
        //        getResult = await GetAllWhereAsync(sg => sg.CompanyId == companyId, includes);
        //    } else {
        //        getResult = await GetAllWhereAsync(sg => sg.CompanyId == companyId &&
        //            (
        //                sg.Managers.Any(m => m.Id == userId)
        //                || sg.ChildSalesGroups.Any(csg => csg.Managers.Any(m => m.Id == userId))
        //                || sg.ChildSalesGroups.Any(csg => csg.ChildSalesGroups.Any(ccsg => ccsg.Managers.Any(m => m.Id == userId)))
        //                || sg.ChildSalesGroups.Any(csg => csg.ChildSalesGroups.Any(ccsg => ccsg.SalesTeams.Any(st => st.Id == user.SalesTeamId)))
        //            ), includes);
        //    }

        //    if (!getResult.IsSuccessful) {
        //        processingResult.Error = ErrorValues.GENERIC_GET_SALES_GROUP_IN_COMPANY_ERROR;
        //        Logger.Error(String.Format("GetCompanyUserTreeWhereManagerInTree call failed (Lookup User: {0}) || Error: {1}", userId, getResult.Error.UserMessage));
        //        return processingResult;
        //    }

        //    processingResult.IsSuccessful = true;
        //    processingResult.Data = DomainModelsConverter.ToLevel1UserTreeViewBindingModelList(RemoveDeletedSalesGroupsAndSalesTeamsFromSalesGroupTree(getResult.Data));

        //    return processingResult;
        //}

        public async Task<ServiceProcessingResult<List<GroupAdminTreeViewBindingModel>>> getCompanySalesGroupAdminTreeWhereManagerInTree(string companyId,string userId) {
            var processingResult = new ServiceProcessingResult<List<GroupAdminTreeViewBindingModel>> { };

            var appUserDataService = new ApplicationUserDataService();
            var getUserResult = await appUserDataService.GetAsync(userId);
            if (!getUserResult.IsSuccessful)
                {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_COULD_NOT_FIND_USER_ERROR;
                return processingResult;
                }
            var user = getUserResult.Data;
            var userGroupsResult = new List<GroupAdminTreeViewBindingModel>();

            var getResult = new ServiceProcessingResult<List<Level1SalesGroup>>();

            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            var sqlString = "SELECT * FROM v_UserGroups WHERE UserID=@UserID ORDER BY Level1Id, Level2Id, Level3Id";


            SqlCommand cmd = new SqlCommand(sqlString,connection);
            SqlDataReader rdr = null;
            cmd.Parameters.Clear();
            cmd.Parameters.Add(new SqlParameter("@UserID",userId));


            try
                {
                connection.Open();
                rdr = cmd.ExecuteReader();
                var currLevel1ID = "";
                var currLevel2ID = "";
                var currentLevel1Row = -1;
                var currentLevel2Row = -1;
                try
                    {
                    while (rdr.Read())
                        {
                        if (rdr["Level1Id"].ToString() != currLevel1ID)
                            {
                            userGroupsResult.Add(new GroupAdminTreeViewBindingModel
                                {
                                Id = rdr["Level1Id"].ToString(),
                                Name = rdr["Level1Name"].ToString(),
                                ChildSalesGroups = new List<GroupAdminTreeViewBindingModel>()
                                });
                            currentLevel1Row = userGroupsResult.Count - 1;
                            currLevel1ID = rdr["Level1Id"].ToString();
                            currLevel2ID = "";
                            currentLevel2Row = -1;
                            }

                        if (rdr["Level2Id"].ToString() != currLevel2ID)
                            {
                            userGroupsResult[currentLevel1Row].ChildSalesGroups.Add(new GroupAdminTreeViewBindingModel
                                {
                                Id = rdr["Level2Id"].ToString(),
                                Name = rdr["Level2Name"].ToString(),
                                ChildSalesGroups = new List<GroupAdminTreeViewBindingModel>()
                                });
                            currentLevel2Row = userGroupsResult[currentLevel1Row].ChildSalesGroups.Count - 1;
                            currLevel2ID = rdr["Level2Id"].ToString();
                            }

                        if (rdr["Level3Id"].ToString() != "")
                            {
                            userGroupsResult[currentLevel1Row].ChildSalesGroups[currentLevel2Row].ChildSalesGroups.Add(new GroupAdminTreeViewBindingModel
                                {
                                Id = rdr["Level3Id"].ToString(),
                                Name = rdr["Level3Name"].ToString()
                                });
                            }
                        }
                    }
                catch (Exception ex)
                    {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                    //Logger.Error(String.Format("An error occurred generating GroupAdminTreeViewBindingModel object from v_UserGroups result set. UserID: {0}",userId),ex);
                    ex.ToExceptionless()
                      .SetMessage(String.Format("An error occurred generating GroupAdminTreeViewBindingModel object from v_UserGroups result set. UserID: {0}",userId))
                      .MarkAsCritical()
                      .Submit();
                    }
                finally { connection.Close(); }

                }
            catch (Exception ex)
                {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_GET_SALES_GROUP_IN_COMPANY_ERROR;
                Logger.Error(String.Format("getCompanySalesGroupAdminTreeWhereManagerInTree database call failed (Lookup User: {0})",userId),ex);
                ex.ToExceptionless()
                      .SetMessage(String.Format("getCompanySalesGroupAdminTreeWhereManagerInTree database call failed (Lookup User: {0})",userId))
                      .MarkAsCritical()
                      .Submit();
                return processingResult;
                }

            processingResult.IsSuccessful = true;
            processingResult.Data = userGroupsResult;

            return processingResult;

            }

        public async Task<ServiceProcessingResult<List<GroupTreeViewBindingModel>>> GetCompanySalesGroupTreeWhereManagerInTree(string companyId,string userId) {
            var processingResult = new ServiceProcessingResult<List<GroupTreeViewBindingModel>> { };

            var appUserDataService = new ApplicationUserDataService();
            var getUserResult = await appUserDataService.GetAsync(userId);
            if (!getUserResult.IsSuccessful)
                {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_COULD_NOT_FIND_USER_ERROR;
                return processingResult;
                }
            var user = getUserResult.Data;

            var includes = new[] {
               "Managers.Roles.Role", "ChildSalesGroups.Managers.Roles.Role", "ChildSalesGroups.ChildSalesGroups.Managers.Roles.Role",
               "ChildSalesGroups.ChildSalesGroups.SalesTeams","ChildSalesGroups.ChildSalesGroups.SalesTeams.users","ChildSalesGroups.ChildSalesGroups.SalesTeams.users.roles",
               "ChildSalesGroups.ChildSalesGroups.SalesTeams.users.roles.role"
            };

            var getResult = new ServiceProcessingResult<List<Level1SalesGroup>>();
            if (user.Role.IsAdmin() || user.Role.IsSuperAdmin())
                {
                getResult = await GetAllWhereAsync(sg => sg.CompanyId == companyId,includes);
                }
            else
                {
                getResult = await GetAllWhereAsync(sg => sg.CompanyId == companyId &&
                    (sg.Managers.Any(m => m.Id == userId) ||
                    sg.ChildSalesGroups.Any(csg => csg.Managers.Any(m => m.Id == userId)) ||
                    sg.ChildSalesGroups.Any(csg => csg.ChildSalesGroups.Any(ccsg => ccsg.Managers.Any(m => m.Id == userId))) ||
                    sg.ChildSalesGroups.Any(csg => csg.ChildSalesGroups.Any(ccsg => ccsg.SalesTeams.Any(st => st.Id == user.SalesTeamId)))),includes);
                }

            if (!getResult.IsSuccessful)
                {
                processingResult.Error = ErrorValues.GENERIC_GET_SALES_GROUP_IN_COMPANY_ERROR;
                //Logger.Error(String.Format("GetCompanySalesGroupTreeWhereManagerInTree call failed (Lookup User: {0}) || Error: {1}",userId,getResult.Error.UserMessage));

                ExceptionlessClient.Default.CreateLog(typeof(ApplicationUserDataService).FullName,String.Format("GetCompanySalesGroupTreeWhereManagerInTree call failed (Lookup User: {0}) || Error: {1}",userId,getResult.Error.UserMessage),"Error").Submit();
                return processingResult;
                }

            processingResult.IsSuccessful = true;
            processingResult.Data = DomainModelsConverter.ToLevel1GroupTreeViewBindingModelList(RemoveDeletedSalesGroupsAndSalesTeamsFromSalesGroupTree(getResult.Data));

            return processingResult;
            }

        public async Task<ServiceProcessingResult> DeleteAsync(string id,string companyId,ApplicationRole loggedInUserRole) {
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

            var validateDeletabilityResult = await ValidateLevel1SalesGroupCanBeDeletedAsync(id,companyId);
            if (!validateDeletabilityResult.IsSuccessful)
                {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.DELETE_SALES_GROUP_NOT_IN_COMPANY_ERROR;
                //var logMessage =
                //    String.Format(
                //        "An error occurred while validating deletion for Sales Group with Id: {0} in Company with Id: {1}",
                //        id,companyId);
                //Logger.Error(logMessage);
                ExceptionlessClient.Default.CreateLog(typeof(ApplicationUserDataService).FullName,String.Format(
                        "An error occurred while validating deletion for Sales Group with Id: {0} in Company with Id: {1}",
                        id,companyId),"Error").Submit();
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

        public async Task<ServiceProcessingResult<Level1SalesGroup>> GetExistingLevel1SalesGroupInCompanyAsync(string id,string companyId) {
            var processingResult = new ServiceProcessingResult<Level1SalesGroup>();
            if (id == null)
                {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GET_SALES_GROUP_WITHOUT_ID_ERROR;
                return processingResult;
                }

            var includes = new[] { "ChildSalesGroups" };
            processingResult = await GetWhereAsync(sg => sg.CompanyId == companyId && sg.Id == id,includes);

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
                RemoveDeletedLevel2SalesGroups(processingResult.Data.ChildSalesGroups);

            return processingResult;
            }

        public async Task<ServiceProcessingResult<ValidationResult>> ValidateUserIsNotAssignedAsLevel1SalesGroupManagerInCompany(string userId,string companyId) {
            var processingResult = new ServiceProcessingResult<ValidationResult> { IsSuccessful = true };
            var validationResult = new ValidationResult { IsValid = false };

            if (userId == null)
                {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GET_USER_WITHOUT_ID_ERROR;
                return processingResult;
                }

            var NumGroups = 0;
            try
                {
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                SqlCommand cmd = new SqlCommand(
                    @"
                        SELECT COUNT(*) AS NumGroups
                        FROM Level1SalesGroupApplicationUser U
                        LEFT JOIN Level1SalesGroup L ON U.Level1SalesGroup_Id = L.Id
                        WHERE[ApplicationUser_Id] = @UserID AND L.IsDeleted = 0
                    ",connection);
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqlParameter("@UserID",userId));
                connection.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                    {
                    NumGroups = rdr.GetInt32(rdr.GetOrdinal("NumGroups"));
                    }
               
                }
            catch (Exception ex)
                {

                //var logMessage = String.Format("An error occurred attempting to retrieve User with Id: {0} from Company with Id: {1}",userId,companyId);
                //Logger.Error(logMessage);
                ex.ToExceptionless()
                      .SetMessage(String.Format("An error occurred attempting to retrieve User with Id: {0} from Company with Id: {1}",userId,companyId))
                      .MarkAsCritical()
                      .Submit();
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error attempting to validate user not assigned as level 1 sales group manager.","Error attempting to validate user not assigned as level 1 sales group manager.",true,false);
                return processingResult;
                }


            if (NumGroups > 0)
                {
                validationResult.Errors.Add(ErrorValues.UserDeleteFailedDueToManagerAssignmentUserHelp);
                }
            else
                {
                validationResult.IsValid = true;
                }

            processingResult.Data = validationResult;

            return processingResult;
            }

        public async Task<ServiceProcessingResult<List<SalesTeam>>> GetSalesTeamsUnderManager(string userId,string companyId) {
            var processingResult = new ServiceProcessingResult<List<SalesTeam>> { IsSuccessful = true };
            var includes = new[] { "Managers","ChildSalesGroups.ChildSalesGroups.SalesTeams" };
            var salesGroupsOwnedByManagerResult =
                await GetAllWhereAsync(sg => sg.Managers.Any(m => m.Id == userId) && sg.CompanyId == companyId,includes);
            if (!salesGroupsOwnedByManagerResult.IsSuccessful || salesGroupsOwnedByManagerResult.Data == null)
                {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_GET_SALES_GROUP_WITH_MANAGER_IN_TREE_ERROR;
                return processingResult;
                }

            processingResult.Data = PluckSalesTeamsFromLevel1SalesGroups(salesGroupsOwnedByManagerResult.Data);

            return processingResult;
            }

        public async Task<ServiceProcessingResult<List<UserNameViewBindingModel>>> getManagersInTree(string companyId,string userId) {
            var processingResult = new ServiceProcessingResult<List<UserNameViewBindingModel>>();
            processingResult.Data = new List<UserNameViewBindingModel>();

            var salesGroupTree = await getCompanySalesGroupAdminTreeWhereManagerInTree(companyId,userId);
            if (!salesGroupTree.IsSuccessful)
                {
                processingResult.IsSuccessful = false;
                processingResult.Error = salesGroupTree.Error;
                return processingResult;
                }

            if (salesGroupTree.Data.Any())
                {
                var groupIDList = new List<string>();
                foreach (var level1SalesGroup in salesGroupTree.Data)
                    {
                    groupIDList.Add(level1SalesGroup.Id);

                    //foreach (var level2SalesGroup in level1SalesGroup.ChildSalesGroups) {
                    //    groupIDList.Add(level2SalesGroup.Id);

                    //    foreach (var level3SalesGroup in level2SalesGroup.ChildSalesGroups) {
                    //        groupIDList.Add(level3SalesGroup.Id);
                    //    }
                    //}
                    }

                using (var contextScope = DbContextScopeFactory.CreateReadOnly())
                    {
                    try
                        {
                        var includes = new[] { "Managers" };

                        var groupResult = await GetAllWhereAsync(sg => sg.CompanyId == companyId && groupIDList.Contains(sg.Id),includes);

                        if (!groupResult.IsSuccessful)
                            {
                            processingResult.Error = new ProcessingError("Unable to find any Level 1 Sales Groups","Unable to find any Level 1 Sales Groups",false,false);
                            Logger.Error(String.Format("Unable to find any Level 1 Sales Groups"));
                            return processingResult;
                            }

                        processingResult.IsSuccessful = true;
                        foreach (var group in groupResult.Data)
                            {
                            if (group.Managers != null)
                                {
                                foreach (var user in group.Managers)
                                    {
                                    processingResult.Data.Add(user.ToUserNameViewBindingModel());
                                    }
                                }
                            }
                        }
                    catch (Exception ex)
                        {
                        processingResult.IsSuccessful = false;
                        ex.ToExceptionless()
                      .SetMessage(String.Format("Failed to get Level 1 Sales Group (UserID: {0})",userId))
                       .MarkAsCritical()
                      .Submit();
                        processingResult.Error = ErrorValues.COULD_NOT_FIND_SALES_GROUP_IN_COMPANY_ERROR;
                        //Logger.Error(String.Format("Failed to get Level 1 Sales Group (UserID: {0})",userId));
                        }
                    }
                }

            return processingResult;
            }

        public async Task<ServiceProcessingResult<List<UserNameViewBindingModel>>> GetLevel1GroupManagers(string companyId,string Id) {
            var processingResult = new ServiceProcessingResult<List<UserNameViewBindingModel>>();
            processingResult.Data = new List<UserNameViewBindingModel>();

            using (var contextScope = DbContextScopeFactory.CreateReadOnly())
                {
                try
                    {
                    var includes = new[] { "Managers" };

                    var groupResult = await GetAllWhereAsync(sg => sg.CompanyId == companyId && sg.Id == Id,includes);

                    if (!groupResult.IsSuccessful)
                        {
                        processingResult.Error = ErrorValues.COULD_NOT_FIND_SALES_GROUP_IN_COMPANY_ERROR;
                        var logMessage = String.Format("Unable to find Level 1 Sales Group (Id: {0})",Id);
                        Logger.Error(logMessage);
                        return processingResult;
                        }

                    processingResult.IsSuccessful = true;
                    foreach (var group in groupResult.Data)
                        {
                        if (group.Managers != null)
                            {
                            foreach (var user in group.Managers)
                                {
                                processingResult.Data.Add(user.ToUserNameViewBindingModel());
                                }
                            }
                        }
                    }
                catch (Exception ex)
                    {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.COULD_NOT_FIND_SALES_GROUP_IN_COMPANY_ERROR;
                    ex.ToExceptionless()
                      .SetMessage(String.Format("Failed to get Level 1 Sales Group (Id: {0})",Id))
                      .MarkAsCritical()
                      .Submit();
                    //var logMessage = String.Format("Failed to get Level 1 Sales Group (Id: {0})",Id);
                    //Logger.Error(logMessage);
                    }
                }

            return processingResult;
            }

        public async Task<ServiceProcessingResult<ValidationResult>> NotAManagerAsync(ApplicationUser user) {
            var processingResult = new ServiceProcessingResult<ValidationResult> { IsSuccessful = true };
            var validationResult = new ValidationResult();

            validationResult.IsValid = false;

            try
                {
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                SqlCommand cmd = new SqlCommand(
                @"
                    SELECT 'Level1' AS GroupType, Level1SalesGroup_Id AS GroupID FROM Level1SalesGroupApplicationUser
                    WHERE ApplicationUser_Id=@UserID
                    UNION
                    SELECT 'Level2' AS GroupType, Level2SalesGroup_Id AS GroupID FROM Level2SalesGroupApplicationUser
                    WHERE ApplicationUser_Id=@UserID
                    UNION
                    SELECT 'Level3' AS GroupType, Level3SalesGroup_Id AS GroupID FROM Level3SalesGroupApplicationUser
                    WHERE ApplicationUser_Id=@UserID
                ",connection);
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqlParameter("@UserID",user.Id));
                connection.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                var IsManager = false;
                while (rdr.Read())
                    { IsManager = rdr.HasRows; }

                if (IsManager)
                    {
                    validationResult.IsValid = false;
                    }
                else
                    {
                    validationResult.IsValid = true;
                    }
                processingResult.Data = validationResult;
                
                }
            catch (Exception ex)
                {
                ex.ToExceptionless()
                     .SetMessage("Error in sql data reader for NotAManagerAsync")
                     .MarkAsCritical()
                     .Submit();
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Failed to retrieve list of Managers.","Failed to retrieve list of Managers.",true,false);
                //var logMessage = String.Format("Error in sql data reader for NotAManagerAsync",user.CompanyId,user.Id);
                //Logger.Error(logMessage);
                }

            //var getSalesGroupResult = await GetCompanySalesGroupTreeWhereManagerInTree(user.CompanyId, user.Id);

            //if (!getSalesGroupResult.IsSuccessful) {
            //    processingResult.IsSuccessful = false;
            //    processingResult.Error = new ProcessingError("Failed to retrieve list of Managers.", "Failed to retrieve list of Managers.", true, false);
            //    var logMessage = String.Format("Error in GroupLevel1 service GetCompanySalesGroupTreeWhereManagerInTree", user.CompanyId, user.Id);
            //    Logger.Error(logMessage);
            //    return processingResult;
            //}
            //var Lev1Groups = getSalesGroupResult.Data;
            //Boolean vbreak = false;
            //foreach (var lev1 in Lev1Groups) {
            //    if (vbreak == true) {
            //        break;
            //    }
            //    foreach (var manager in lev1.Managers) {
            //        if (vbreak == true) {
            //            break;
            //        }
            //        if (manager.Id == user.Id) {
            //            validationResult.IsValid = false;
            //            vbreak = true;
            //            //We are done return User role can not be changed.
            //            break;
            //        }
            //    }
            //    var lev1childs = lev1.ChildSalesGroups;
            //    foreach (var lev2 in lev1childs) {
            //        if (vbreak == true) {
            //            break;
            //        }
            //        foreach (var manager in lev2.Managers) {
            //            if (vbreak == true) {
            //                break;
            //            }
            //            if (manager.Id == user.Id) {
            //                validationResult.IsValid = false;
            //                vbreak = true;
            //                //We are done return User role can not be changed.
            //                break;
            //            }
            //        }
            //        var lev2childs = lev2.ChildSalesGroups;
            //        foreach (var lev3 in lev2childs) {
            //            if (vbreak == true) {
            //                break;
            //            }
            //            foreach (var manager in lev3.Managers) {
            //                if (vbreak == true) {
            //                    break;
            //                }
            //                if (manager.Id == user.Id) {
            //                    validationResult.IsValid = false;
            //                    vbreak = true;
            //                    //We are done return User role can not be changed.
            //                    break;
            //                }
            //            }
            //        }
            //    }
            //}

            //validationResult.IsValid = validationResult.Errors.Count == 0;
            processingResult.Data = validationResult;

            return processingResult;
            }
        private async Task<ServiceProcessingResult<ValidationResult>> ValidateLevel1SalesGroupCanBeDeletedAsync(string salesGroupId,string companyId) {
            var processingResult = new ServiceProcessingResult<ValidationResult> { IsSuccessful = true };
            var validationResult = new ValidationResult();

            var getSalesGroupResult = await GetExistingLevel1SalesGroupInCompanyAsync(salesGroupId,companyId);
            if (!getSalesGroupResult.IsSuccessful)
                {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.DELETE_SALES_GROUP_NOT_IN_COMPANY_ERROR;
                //var logMessage =
                //    String.Format(
                //        "Unable to find Level 1 Sales Group with Id: {0} in Company with Id: {1} for deletion.",salesGroupId,
                //        companyId);
                //Logger.Error(logMessage);
                ExceptionlessClient.Default.CreateLog(typeof(ApplicationUserDataService).FullName,String.Format(
                        "Unable to find Level 1 Sales Group with Id: {0} in Company with Id: {1} for deletion.",salesGroupId,
                        companyId),"Error").Submit();
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

        private static List<SalesTeam> PluckSalesTeamsFromLevel1SalesGroups(
            IEnumerable<Level1SalesGroup> level1SalesGroups) {
            return (from level1SalesGroup in level1SalesGroups
                    from level2SalesGroup in level1SalesGroup.ChildSalesGroups
                    from level3SalesGroup in level2SalesGroup.ChildSalesGroups
                    from salesTeam in level3SalesGroup.SalesTeams
                    where salesTeam.IsActive
                    select salesTeam).ToList();
            }

        private static List<Level1SalesGroup> RemoveDeletedSalesGroupsAndSalesTeamsFromSalesGroupTree(IEnumerable<Level1SalesGroup> salesGroupTree) {
            var level1SalesGroupsToReturn = new List<Level1SalesGroup>();
            foreach (var level1SalesGroup in salesGroupTree)
                {
                level1SalesGroup.ChildSalesGroups = RemoveDeletedLevel2And3SalesGroupsAndSalesTeamsFromSalesGroupTree(level1SalesGroup.ChildSalesGroups);
                level1SalesGroupsToReturn.Add(level1SalesGroup);
                }

            return level1SalesGroupsToReturn;
            }

        private static List<Level2SalesGroup> RemoveDeletedLevel2And3SalesGroupsAndSalesTeamsFromSalesGroupTree(IEnumerable<Level2SalesGroup> level2SalesGroups) {
            var level2SalesGroupsToReturn = new List<Level2SalesGroup>();
            foreach (var level2SalesGroup in level2SalesGroups)
                {
                if (!level2SalesGroup.IsDeleted)
                    {
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
            foreach (var level3SalesGroup in level3SalesGroups)
                {
                if (!level3SalesGroup.IsDeleted)
                    {
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
            foreach (var team in teams)
                {
                foreach (var user in team.Users.ToList())
                    {
                    if (user.IsDeleted == true)
                        {
                        team.Users.Remove(user);
                        }
                    }
                }
            return teams;
            }

        private static List<Level2SalesGroup> RemoveDeletedLevel2SalesGroups(IEnumerable<Level2SalesGroup> level2SalesGroups) {
            return level2SalesGroups.Where(level2SalesGroup => !level2SalesGroup.IsDeleted).ToList();
            }
        }
    }

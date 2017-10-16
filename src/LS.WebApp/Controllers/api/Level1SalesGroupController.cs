using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using LS.Core;
using LS.Domain;
using LS.Services;
using LS.ApiBindingModels;
using LS.WebApp.CustomAttributes;
using System.Linq;
using Exceptionless;
using Exceptionless.Models;
using System.Data.SqlClient;
using System.Data;

namespace LS.WebApp.Controllers.api {
    //        [Authorize]
   // [SingleSessionAuthorize]
    [RoutePrefix("api/level1SalesGroup")]
    public class Level1SalesGroupController : BaseAPIController {
        private static readonly string Level1SalesGroupValidationFailedUserMessage =
            "Validation failed for level 1 sales group.";

        public async Task<IHttpActionResult> Post(SalesGroupAddBindingModel model) {
            var processingResult = new ServiceProcessingResult<Level1SalesGroup>();
            var companyService = new CompanyDataService();
            var companyProcessingResult = await companyService.GetCompanyAsync(LoggedInUser.CompanyId);
            //check permissions (use company.mintochang for dynamic permision)
            try {
                if (LoggedInUser.Role.Rank > ValidApplicationRoles.Admin.Rank) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.PERMISSION_ERROR;
                    return Ok(processingResult);
                }
            } catch {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.PERMISSION_ERROR;
                return Ok(processingResult);
            }

            if (!ModelState.IsValid) {
                var errors = GetModelStateErrorsAsString(ModelState);
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError(Level1SalesGroupValidationFailedUserMessage, errors, false, true);
                return Ok(ModelState);
            }

            var dataService = new Level1SalesGroupDataService();
            var salesGroupToAdd = model.ToLevel1SalesGroup(LoggedInUserId, GetLoggedInUserCompanyId());

            processingResult = await dataService.AddAsync(salesGroupToAdd, GetLoggedInUserRole());

            if (processingResult.IsFatalFailure()) {
                //var logMessage =
                //    String.Format("A fatal error occurred while creating Level1SalesGroup for Company with Id: {0}.",
                //        GetLoggedInUserCompanyId());
                //Logger.Fatal(logMessage);
                ExceptionlessClient.Default.CreateLog(typeof(Level1SalesGroupController).FullName,String.Format("A fatal error occurred while creating Level1SalesGroup for Company with Id: {0}.",
                        GetLoggedInUserCompanyId()),"Error").AddTags("Controller Error").Submit();
                }

            return Ok(processingResult);
        }

        public async Task<IHttpActionResult> Put(SalesGroupEditBindingModel model)
        {
            var processingResult = new ServiceProcessingResult<Level1SalesGroup>();
            try
            {
                if (LoggedInUser.Role.Rank > ValidApplicationRoles.AdminRank)
                {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.PERMISSION_ERROR;
                    return Ok(processingResult);
                }
            }
            catch
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.PERMISSION_ERROR;
                return Ok(processingResult);
            }
            if (!ModelState.IsValid)
            {
                var errors = GetModelStateErrorsAsString(ModelState);
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError(Level1SalesGroupValidationFailedUserMessage, errors, false, true);
                return Ok(ModelState);
            }

            var dataService = new Level1SalesGroupDataService();
            var salesGroupToUpdate = model.ToLevel1SalesGroup(GetLoggedInUserCompanyId());
            salesGroupToUpdate.IsDeleted = model.IsDeleted;
            salesGroupToUpdate.ModifiedByUserId = LoggedInUser.Id;
            processingResult = await dataService.UpdateAsync(salesGroupToUpdate, LoggedInUser);

            if (processingResult.IsFatalFailure())
            {
                //var logMessage = String.Format("A fatal error occurred while updating Level1SalesGroup with Id: {0}", model.Id);
                //Logger.Fatal(logMessage);

                ExceptionlessClient.Default.CreateLog(typeof(Level1SalesGroupController).FullName,String.Format("A fatal error occurred while updating Level1SalesGroup with Id: {0}",model.Id),"Error").AddTags("Controller Error").Submit();
                }

            return Ok(processingResult);
        }

        public async Task<IHttpActionResult> Get(string id) {
            var dataService = new Level1SalesGroupDataService();

            var processingResult = await dataService.GetExistingLevel1SalesGroupInCompanyAsync(id, GetLoggedInUserCompanyId());

            if (processingResult.IsFatalFailure()) {
                //var logMessage = String.Format("A fatal error occurred while retrieving Level1SalesGroup with Id: {0}", id);
                //Logger.Fatal(logMessage);

                ExceptionlessClient.Default.CreateLog(typeof(Level1SalesGroupController).FullName,String.Format("A fatal error occurred while retrieving Level1SalesGroup with Id: {0}",id),"Error").AddTags("Controller Error").Submit();
                }

            return Ok(processingResult);
        }

        public async Task<IHttpActionResult> Delete(string id) {
            var processingResult = new ServiceProcessingResult();
            var companyService = new CompanyDataService();
            var companyProcessingResult = await companyService.GetCompanyAsync(LoggedInUser.CompanyId);
            //check permissions (use company.mintochang for dynamic permision)
            try {
                if (LoggedInUser.Role.Rank > ValidApplicationRoles.Admin.Rank) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.PERMISSION_ERROR;
                    return Ok(processingResult);
                }
            } catch {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.PERMISSION_ERROR;
                return Ok(processingResult);
            }

            var dataService = new Level1SalesGroupDataService();

            processingResult = await dataService.DeleteAsync(id, GetLoggedInUserCompanyId(), GetLoggedInUserRole());

            if (processingResult.IsFatalFailure()) {
                //var logMessage = String.Format("A fatal error occurred while deleting Level1SalesGroup with Id: {0}", id);
                //Logger.Fatal(logMessage);

                ExceptionlessClient.Default.CreateLog(typeof(Level1SalesGroupController).FullName,String.Format("A fatal error occurred while deleting Level1SalesGroup with Id: {0}",id),"Error").AddTags("Controller Error").Submit();
                }

            return Ok(processingResult);
        }

        [HttpGet]
        [Route("getCompanySalesGroupTreeWhereManagerInTree")]
        public async Task<IHttpActionResult> GetCompanySalesGroupTreeWhereManagerInTree() {
            var processingResult = new ServiceProcessingResult<List<GroupTreeViewBindingModel>>();

            var companyId = GetLoggedInUserCompanyId();
            var dataService = new Level1SalesGroupDataService();
            processingResult = await dataService.GetCompanySalesGroupTreeWhereManagerInTree(companyId, LoggedInUserId);
            var Level1Groups = processingResult.Data.OrderBy(x => x.Name).ToList();

            if (!processingResult.IsSuccessful) {
                //var logMessage = String.Format("A fatal error occurred while getting Sales Group Tree for Company with Id: {0}", companyId);
                //Logger.Error(logMessage);

                ExceptionlessClient.Default.CreateLog(typeof(Level1SalesGroupController).FullName,String.Format("A fatal error occurred while getting Sales Group Tree for Company with Id: {0}",companyId),"Error").AddTags("Controller Error").Submit();
                }

            foreach (var vLev1 in Level1Groups) {
                //Order Lev2 group
                var vLev2Groups = vLev1.ChildSalesGroups.OrderBy(x => x.Name).ToList();
                vLev1.ChildSalesGroups.Clear();
                vLev1.ChildSalesGroups = vLev2Groups;

                foreach (var vLev2 in vLev1.ChildSalesGroups) {
                    //Order Lev3 group
                    var vLev3Groups = vLev2.ChildSalesGroups.OrderBy(x => x.Name).ToList();
                    vLev2.ChildSalesGroups.Clear();
                    vLev2.ChildSalesGroups = vLev3Groups;

                    foreach (var vLev3 in vLev2.ChildSalesGroups) {
                        //Order Teams
                        var vTeams = vLev3.SalesTeams.OrderBy(x => x.ExternalDisplayName).ToList();
                        vLev3.SalesTeams.Clear();
                        vLev3.SalesTeams = vTeams;

                        foreach (var team in vLev3.SalesTeams) {
                            //Orderusers
                            var vUsers = team.Users.OrderBy(x => x.LastName).ThenBy(a => a.FirstName).ToList();
                            team.Users.Clear();
                            team.Users = vUsers;
                        }//team
                    }//L3
                }//L2
            }//L1

            if (processingResult.IsFatalFailure()) {
                //var logMessage = String.Format("A fatal error occurred while getting Sales Group Tree for Company with Id: {0}",companyId);
                //Logger.Error(logMessage);

                ExceptionlessClient.Default.CreateLog(typeof(Level1SalesGroupController).FullName,String.Format("A fatal error occurred while getting Sales Group Tree for Company with Id: {0}",companyId),"Error").AddTags("Controller Error").Submit();
                }

            return Ok(processingResult);
        }

        [HttpGet]
        [Route("getCompanySalesGroupAdminTreeWhereManagerInTree")]
        public async Task<IHttpActionResult> GetCompanySalesGroupAdminTreeWhereManagerInTree() {
            var processingResult = new ServiceProcessingResult<List<GroupAdminTreeViewBindingModel>>();

            var companyId = GetLoggedInUserCompanyId();
            var dataService = new Level1SalesGroupDataService();
            processingResult = await dataService.getCompanySalesGroupAdminTreeWhereManagerInTree(companyId, LoggedInUserId);
            var Level1Groups = processingResult.Data.OrderBy(x => x.Name).ToList();

            if (!processingResult.IsSuccessful) {
                //var logMessage = String.Format("A fatal error occurred while getting Sales Group Tree for Company with Id: {0}", companyId);

                ExceptionlessClient.Default.CreateLog(typeof(Level1SalesGroupController).FullName,String.Format("A fatal error occurred while getting Sales Group Tree for Company with Id: {0}",companyId),"Error").AddTags("Controller Error").Submit();
                //Logger.Error(logMessage);
            }

            foreach (var vLev1 in Level1Groups) {
                //Order Lev2 group
                var vLev2Groups = vLev1.ChildSalesGroups.OrderBy(x => x.Name).ToList();
                vLev1.ChildSalesGroups.Clear();
                vLev1.ChildSalesGroups = vLev2Groups;

                foreach (var vLev2 in vLev1.ChildSalesGroups) {
                    //Order Lev3 group
                    var vLev3Groups = vLev2.ChildSalesGroups.OrderBy(x => x.Name).ToList();
                    vLev2.ChildSalesGroups.Clear();
                    vLev2.ChildSalesGroups = vLev3Groups;
                }//L2
            }//L1

            if (processingResult.IsFatalFailure()) {
                //var logMessage = String.Format("A fatal error occurred while getting Sales Group Tree for Company with Id: {0}", companyId);
                //Logger.Error(logMessage);

                ExceptionlessClient.Default.CreateLog(typeof(Level1SalesGroupController).FullName,String.Format("A fatal error occurred while getting Sales Group Tree for Company with Id: {0}",companyId),"Error").AddTags("Controller Error").Submit();
                }

            return Ok(processingResult);
        }

        [HttpGet]
        [Route("getLevel1SalesGroupsInCompany")]
        public async Task<IHttpActionResult> GetLevel1SalesGroupsInCompany()
        {
            var processingResult = new ServiceProcessingResult<List<GroupSimpleViewBindingModel>>();
            var companyId = GetLoggedInUserCompanyId();

            var sqlText = "SELECT Level1Id AS Id, Level1Name AS Name FROM v_UserGroups WHERE UserID=@UserID AND Level2Id IS NULL AND Level3Id IS NULL ORDER BY Level1Name";

            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@UserID",LoggedInUserId)
            };

            var sqlClient = new SQLQuery();
            var result = await sqlClient.ExecuteReaderAsync<GroupSimpleViewBindingModel>(CommandType.Text,sqlText, parameters);
            if (!result.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error getting level 1 sales group for reporting", "Error getting level 1 sales group for reporting", false, false);
                return Ok(processingResult);
            }

            processingResult.Data = (List<GroupSimpleViewBindingModel>)result.Data;
            processingResult.IsSuccessful = true;

            return Ok(processingResult);
        }

        [Route("getGroupManagers")]
        public async Task<IHttpActionResult> GetGroupManagers(string id) {
            var processingResult = new ServiceProcessingResult<List<UserNameViewBindingModel>>();

            var companyId = GetLoggedInUserCompanyId();

            var dataService = new Level1SalesGroupDataService();
            processingResult = await dataService.GetLevel1GroupManagers(GetLoggedInUserCompanyId(), id);

            if (processingResult.IsFatalFailure()) {
                //var logMessage = String.Format("A fatal error occurred while retrieving all managers for Level 1 Sales Group (Id: {0}).", id);
                //Logger.Fatal(logMessage);

                ExceptionlessClient.Default.CreateLog(typeof(Level1SalesGroupController).FullName,String.Format("A fatal error occurred while retrieving all managers for Level 1 Sales Group (Id: {0}).",id),"Error").AddTags("Controller Error").Submit();
                }

            return Ok(processingResult);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using LS.ApiBindingModels;
using LS.Core;
using LS.Domain;
using LS.Services;
using LS.WebApp.CustomAttributes;
using Exceptionless;
using Exceptionless.Models;
namespace LS.WebApp.Controllers.api
{
    [Authorize]
    //[SingleSessionAuthorize]
    [RoutePrefix("api/level3SalesGroup")]
    public class Level3SalesGroupController : BaseAPIController
    {
        private static readonly string Level3SalesGroupValidationFailedUserMessage =
            "Validationfailed for level 3 sales group.";

        public async Task<IHttpActionResult> Post(SalesGroupAddBindingModel model)
        {
            var processingResult = new ServiceProcessingResult<Level3SalesGroup>();
            var companyService = new CompanyDataService();
            var companyProcessingResult = await companyService.GetCompanyAsync(LoggedInUser.CompanyId);
            //check permissions (use company.mintochang for dynamic permision)
            try
            {
                if (LoggedInUser.Role.Rank > ValidApplicationRoles.LevelOneManagerRank)
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
                processingResult.Error = new ProcessingError(Level3SalesGroupValidationFailedUserMessage, errors, false, true);
                return Ok(processingResult);
            }
            
            var dataService = new Level3SalesGroupDataService();
            var salesGroupToAdd = model.ToLevel3SalesGroup(LoggedInUserId, GetLoggedInUserCompanyId());

            processingResult = await dataService.AddAsync(salesGroupToAdd, LoggedInUser);

            if (processingResult.IsFatalFailure())
            {
                //var logMessage =
                //    String.Format("A fatal error occurred while creating Level3SalesGroup for Company with Id: {0}.",
                //        GetLoggedInUserCompanyId());
                //Logger.Fatal(logMessage);
                ExceptionlessClient.Default.CreateLog(typeof(Level3SalesGroupController).FullName,String.Format("A fatal error occurred while creating Level3SalesGroup for Company with Id: {0}.",
                        GetLoggedInUserCompanyId()),"Error").AddTags("Controller Error").Submit();
                }

            return Ok(processingResult);
        }

        public async Task<IHttpActionResult> Put(SalesGroupEditBindingModel model)
        {
            var processingResult = new ServiceProcessingResult<Level3SalesGroup>();
            var companyService = new CompanyDataService();
            var companyProcessingResult = await companyService.GetCompanyAsync(LoggedInUser.CompanyId);
            //check permissions (use company.mintochang for dynamic permision)
            try
            {
                if (LoggedInUser.Role.Rank > ValidApplicationRoles.LevelOneManagerRank)
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
                processingResult.Error = new ProcessingError(Level3SalesGroupValidationFailedUserMessage, errors, false, true);
                return Ok(processingResult);
            }

            var dataService = new Level3SalesGroupDataService();
            var salesGroupToUpdate = model.ToLevel3SalesGroup(GetLoggedInUserCompanyId());
            salesGroupToUpdate.ModifiedByUserId = LoggedInUser.Id;
            salesGroupToUpdate.IsDeleted = model.IsDeleted;
            processingResult = await dataService.UpdateAsync(salesGroupToUpdate, LoggedInUser);

            if (processingResult.IsFatalFailure())
            {
                //var logMessage = String.Format("A fatal error occurred while updating Level3SalesGroup with Id: {0}", model.Id);
                //Logger.Fatal(logMessage);
                ExceptionlessClient.Default.CreateLog(typeof(Level3SalesGroupController).FullName,String.Format("A fatal error occurred while updating Level3SalesGroup with Id: {0}",model.Id),"Error").AddTags("Controller Error").Submit();
                }

            return Ok(processingResult);
        }

        public async Task<IHttpActionResult> Get(string id)
        {
            var dataService = new Level3SalesGroupDataService();
            var processingResult = await dataService.GetExistingLevel3SalesGroupInCompanyAsync(id, GetLoggedInUserCompanyId());

            if (processingResult.IsFatalFailure())
            {
                //var logMessage = String.Format("A fatal error occurred while retrieving Level3SalesGroup with Id: {0}", id);
                //Logger.Fatal(logMessage);
                ExceptionlessClient.Default.CreateLog(typeof(Level3SalesGroupController).FullName,String.Format("A fatal error occurred while retrieving Level3SalesGroup with Id: {0}",id),"Error").AddTags("Controller Error").Submit();
                }

            return Ok(processingResult);
        }

        [Route("getLevel3SalesGroupsInCompany")]
        public async Task<IHttpActionResult> GetLevel3SalesGroupsInCompany()
        {
            var processingResult = new ServiceProcessingResult<List<Level3SalesGroup>>();

            var companyId = GetLoggedInUserCompanyId();

            var dataService = new Level3SalesGroupDataService();
            processingResult = await dataService.GetLevel3SalesGroupsInCompany(companyId);

            if (processingResult.IsFatalFailure())
            {
                //var logMessage =
                //    String.Format(
                //        "A fatal error occurred while retrieving all Level3SalesGroups in Company with Id: {0}.",
                //        companyId);
                //Logger.Fatal(logMessage);
                ExceptionlessClient.Default.CreateLog(typeof(Level3SalesGroupController).FullName,String.Format(
                        "A fatal error occurred while retrieving all Level3SalesGroups in Company with Id: {0}.",
                        companyId),"Error").AddTags("Controller Error").Submit();
                }

            return Ok(processingResult);
        }

        [Route("getLevel3GroupTeams")]
        public async Task<IHttpActionResult> GetLevel3GroupTeams(string id) {
            var processingResult = new ServiceProcessingResult<List<SalesTeamNameActiveViewBindingModel>>();

            var companyId = GetLoggedInUserCompanyId();

            var dataService = new Level3SalesGroupDataService();
            processingResult = await dataService.GetLevel3GroupTeams(GetLoggedInUserCompanyId(), id);

            if (processingResult.IsFatalFailure()) {
                //var logMessage = String.Format("A fatal error occurred while retrieving all teams for Level 3 Sales Group (Id: {0}).", id);
                //Logger.Fatal(logMessage);
                ExceptionlessClient.Default.CreateLog(typeof(Level3SalesGroupController).FullName,String.Format("A fatal error occurred while retrieving all teams for Level 3 Sales Group (Id: {0}).",id),"Error").AddTags("Controller Error").Submit();
                }

            return Ok(processingResult);
        }

        [Route("getGroupManagers")]
        public async Task<IHttpActionResult> GetGroupManagers(string id) {
            var processingResult = new ServiceProcessingResult<List<UserNameViewBindingModel>>();

            var companyId = GetLoggedInUserCompanyId();

            var dataService = new Level3SalesGroupDataService();
            processingResult = await dataService.GetLevel3GroupManagers(GetLoggedInUserCompanyId(), id);

            if (processingResult.IsFatalFailure()) {
                //var logMessage = String.Format("A fatal error occurred while retrieving all managers for Level 3 Sales Group (Id: {0}).", id);
                //Logger.Fatal(logMessage);
                ExceptionlessClient.Default.CreateLog(typeof(Level3SalesGroupController).FullName,String.Format("A fatal error occurred while retrieving all managers for Level 3 Sales Group (Id: {0}).",id),"Error").AddTags("Controller Error").Submit();
                }

            return Ok(processingResult);
        }

        public async Task<IHttpActionResult> Delete(string id)
        {
            var processingResult = new ServiceProcessingResult();
            var dataService = new Level3SalesGroupDataService();
             processingResult = await dataService.DeleteAsync(id, GetLoggedInUserCompanyId(), GetLoggedInUserRole());
            var companyService = new CompanyDataService();
            var companyProcessingResult = await companyService.GetCompanyAsync(LoggedInUser.CompanyId);
            //check permissions (use company.mintochang for dynamic permision)
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
            if (processingResult.IsFatalFailure())
            {
                //var logMessage = String.Format("A fatal error occurred while deleting Level3SalesGroup with Id: {0}", id);
                //Logger.Fatal(logMessage);
                ExceptionlessClient.Default.CreateLog(typeof(Level3SalesGroupController).FullName,String.Format("A fatal error occurred while deleting Level3SalesGroup with Id: {0}",id),"Error").AddTags("Controller Error").Submit();
                }

            return Ok(processingResult);
        }
    }
}
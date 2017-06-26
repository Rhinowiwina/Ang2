using System;
using System.Threading.Tasks;
using System.Web.Http;
using LS.ApiBindingModels;
using LS.Core;
using LS.Domain;
using LS.Services;
using LS.WebApp.CustomAttributes;
using System.Collections.Generic;
using Exceptionless;
using Exceptionless.Models;
namespace LS.WebApp.Controllers.api
{
//        [Authorize]
    [SingleSessionAuthorize]
    [RoutePrefix("api/level2SalesGroup")]
    public class Level2SalesGroupController : BaseAPIController
    {
        private static readonly string Level2SalesGroupValidationFailedUserMessage =
            "Validation failed for level 2 sales group.";

        public async Task<IHttpActionResult> Post(SalesGroupAddBindingModel model)
        {
            var processingResult = new ServiceProcessingResult<Level2SalesGroup>();
            var companyService = new CompanyDataService();
            var companyProcessingResult = await companyService.GetCompanyAsync(LoggedInUser.CompanyId);
            //check permissions (use company.mintochang for dynamic permision)
            try
            {
                if (LoggedInUser.Role.Rank >ValidApplicationRoles.LevelOneManager.Rank)
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
                processingResult.Error = new ProcessingError(Level2SalesGroupValidationFailedUserMessage, errors, false, true);
                return Ok(ModelState);
            }

            var dataService = new Level2SalesGroupDataService();
            var salesGroupToAdd = model.ToLevel2SalesGroup(LoggedInUserId, GetLoggedInUserCompanyId());

            processingResult = await dataService.AddAsync(salesGroupToAdd,LoggedInUser);

            if (processingResult.IsFatalFailure())
            {
                //var logMessage =
                //    String.Format("A fatal error occurred while creating Level2SalesGroup for Company with Id: {0}.",
                //        GetLoggedInUserCompanyId());
                //Logger.Fatal(logMessage);
                ExceptionlessClient.Default.CreateLog(typeof(Level2SalesGroupController).FullName,String.Format("A fatal error occurred while creating Level2SalesGroup for Company with Id: {0}.",
                        GetLoggedInUserCompanyId()),"Error").AddTags("Controller Error").Submit();
                }

            return Ok(processingResult);
        }

        public async Task<IHttpActionResult> Put(SalesGroupEditBindingModel model)
        {
            var processingResult = new ServiceProcessingResult<Level2SalesGroup>();
            var companyService = new CompanyDataService();
            var companyProcessingResult = await companyService.GetCompanyAsync(LoggedInUser.CompanyId);
            //check permissions (use company.mintochang for dynamic permision)
            try
            {
                if (LoggedInUser.Role.Rank > ValidApplicationRoles.LevelOneManager.Rank)
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
                processingResult.Error = new ProcessingError(Level2SalesGroupValidationFailedUserMessage, errors, false, true);
                return Ok(ModelState);
            }

            var dataService = new Level2SalesGroupDataService();
            var salesGroupToUpdate = model.ToLevel2SalesGroup(GetLoggedInUserCompanyId());
            salesGroupToUpdate.ModifiedByUserId = LoggedInUser.Id;
            salesGroupToUpdate.IsDeleted = model.IsDeleted;
            processingResult = await dataService.UpdateAsync(salesGroupToUpdate,LoggedInUser);

            if (processingResult.IsFatalFailure())
            {
                //var logMessage = String.Format("A fatal error occurred while updating Level2SalesGroup with Id: {0}", model.Id);
                //Logger.Fatal(logMessage);
                ExceptionlessClient.Default.CreateLog(typeof(Level2SalesGroupController).FullName,String.Format("A fatal error occurred while updating Level2SalesGroup with Id: {0}",model.Id),"Error").AddTags("Controller Error").Submit();
                }

            return Ok(processingResult);
        }

        public async Task<IHttpActionResult> Get(string id)
        {
            var dataService = new Level2SalesGroupDataService();
            var processingResult = await dataService.GetExistingLevel2SalesGroupInCompany(id, GetLoggedInUserCompanyId());

            if (processingResult.IsFatalFailure())
            {
                //var logMessage = String.Format("A fatal error occurred while retrieving Level2SalesGroup with Id: {0}", id);
                //Logger.Fatal(logMessage);
                ExceptionlessClient.Default.CreateLog(typeof(Level2SalesGroupController).FullName,String.Format("A fatal error occurred while retrieving Level2SalesGroup with Id: {0}",id),"Error").AddTags("Controller Error").Submit();
                }

            return Ok(processingResult);
        }

        public async Task<IHttpActionResult> Delete(string id)
        {
            var processingResult = new ServiceProcessingResult();
            var companyService = new CompanyDataService();
            var companyProcessingResult = await companyService.GetCompanyAsync(LoggedInUser.CompanyId);
            //check permissions (use company.mintochang for dynamic permision)
            try
            {
                if (LoggedInUser.Role.Rank > ValidApplicationRoles.Admin.Rank)
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
            var dataService = new Level2SalesGroupDataService();
          processingResult = await dataService.DeleteAsync(id, GetLoggedInUserCompanyId(), GetLoggedInUserRole());

            if (processingResult.IsFatalFailure())
            {
                //var logMessage = String.Format("A fatal error occurred while deleting Level2SalesGroup with Id: {0}", id);
                //Logger.Fatal(logMessage);
                ExceptionlessClient.Default.CreateLog(typeof(Level2SalesGroupController).FullName,String.Format("A fatal error occurred while deleting Level2SalesGroup with Id: {0}",id),"Error").AddTags("Controller Error").Submit();
                }

            return Ok(processingResult);
        }

        [Route("getGroupManagers")]
        public async Task<IHttpActionResult> GetGroupManagers(string id) {
            var processingResult = new ServiceProcessingResult<List<UserNameViewBindingModel>>();

            var companyId = GetLoggedInUserCompanyId();

            var dataService = new Level2SalesGroupDataService();
            processingResult = await dataService.GetLevel2GroupManagers(GetLoggedInUserCompanyId(), id);

            if (processingResult.IsFatalFailure()) {
                //var logMessage = String.Format("A fatal error occurred while retrieving all managers for Level 2 Sales Group (Id: {0}).", id);
                //Logger.Fatal(logMessage);
                ExceptionlessClient.Default.CreateLog(typeof(Level2SalesGroupController).FullName,String.Format("A fatal error occurred while retrieving all managers for Level 2 Sales Group (Id: {0}).",id),"Error").AddTags("Controller Error").Submit();
                }

            return Ok(processingResult);
        }

        [Route("getLevel2GroupGroups")]
        public async Task<IHttpActionResult> GetLevel2GroupGroups(string id) {
            var processingResult = new ServiceProcessingResult<List<GroupTreeViewBindingModel>>();

            var companyId = GetLoggedInUserCompanyId();

            var dataService = new Level2SalesGroupDataService();
            processingResult = await dataService.GetLevel2GroupGroups(GetLoggedInUserCompanyId(), id, LoggedInUserId);

            if (processingResult.IsFatalFailure()) {
                //var logMessage = String.Format("A fatal error occurred while retrieving all groups for Level 2 Sales Group (Id: {0}).", id);
                //Logger.Fatal(logMessage);
                ExceptionlessClient.Default.CreateLog(typeof(Level2SalesGroupController).FullName,String.Format("A fatal error occurred while retrieving all groups for Level 2 Sales Group (Id: {0}).",id),"Error").AddTags("Controller Error").Submit();
                }

            return Ok(processingResult);
        }
    }
}
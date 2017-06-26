using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using LS.ApiBindingModels;
using LS.Core;
using LS.Domain;
using LS.Repositories;
using LS.Services;
using LS.WebApp.CustomAttributes;
using Exceptionless;
using Exceptionless.Models;
namespace LS.WebApp.Controllers.api {
    [RoutePrefix("api/salesTeam")]
    //    [Authorize(Roles = "Administrator, Level 1 Manager, Level 2 Manager, Level 3 Manager")] // Does this accurately represent what can access this controller?
    //        [Authorize]
    [SingleSessionAuthorize]
    public class SalesTeamController : BaseAPIController {
        private static readonly string SalesTeamValidationFailedUserMessage = "Validation failed for sales team.";

      

        public async Task<IHttpActionResult> Post(SalesTeamCreationBindingModel model) {
            var processingResult = new ServiceProcessingResult<SalesTeam>();
            var companyService = new CompanyDataService();
            var companyProcessingResult = await companyService.GetCompanyAsync(LoggedInUser.CompanyId);
            //check permissions (use company.mintochang for dynamic permision)
            try {
                if (LoggedInUser.Role.Rank > ValidApplicationRoles.AdminRank) {
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
                processingResult.Error = new ProcessingError(SalesTeamValidationFailedUserMessage, errors, false, true);
                return Ok(processingResult);
            }

            var salesTeam = model.ToSalesTeam(GetLoggedInUserCompanyId(), LoggedInUserId);

            var salesTeamService = new SalesTeamDataService(LoggedInUserId);
            processingResult = await salesTeamService.AddSalesTeamAsync(salesTeam, LoggedInUser);

            if (processingResult.IsFatalFailure()) {
                //var logMessage =
                //    String.Format("A fatal error occurred while creating SalesTeam for Company with Id: {0}.",
                //        GetLoggedInUserCompanyId());
                //Logger.Fatal(logMessage);

                ExceptionlessClient.Default.CreateLog(typeof(SalesTeamController).FullName,String.Format("A fatal error occurred while creating SalesTeam for Company with Id: {0}.",
                        GetLoggedInUserCompanyId()),"Error").AddTags("Controller Error").Submit();
                }

            return Ok(processingResult);
        }

        public async Task<IHttpActionResult> Put(SalesTeamEditBindingModel model) {
            var processingResult = new ServiceProcessingResult<SalesTeam>();
            var companyService = new CompanyDataService();
            var companyProcessingResult = await companyService.GetCompanyAsync(LoggedInUser.CompanyId);
            //check permissions (use company.mintochang for dynamic permision)
            try {
                if (LoggedInUser.Role.Rank > ValidApplicationRoles.LevelOneManagerRank) {
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
                processingResult.Error = new ProcessingError(SalesTeamValidationFailedUserMessage, errors, false, true);
                return Ok(processingResult);
            }

          

            var salesTeam = model.ToSalesTeam(GetLoggedInUserCompanyId());
            salesTeam.ModifiedByUserId = LoggedInUser.Id;
            var salesTeamService = new SalesTeamDataService(LoggedInUserId);
            processingResult = await salesTeamService.UpdateSalesTeamAsync(salesTeam, LoggedInUser);

            if (processingResult.IsFatalFailure()) {
                //var logMessage = String.Format("A fatal error occurred while updating SalesTeam with Id: {0}", model.Id);
                //Logger.Fatal(logMessage);
                ExceptionlessClient.Default.CreateLog(typeof(SalesTeamController).FullName,String.Format("A fatal error occurred while updating SalesTeam with Id: {0}",model.Id),"Error").AddTags("Controller Error").Submit();
                }

            return Ok(processingResult);
        }

        public async Task<IHttpActionResult> Get(string salesTeamId) {
            var dataService = new SalesTeamDataService(LoggedInUserId);
            var processingResult = await dataService.GetExistingSalesTeamInCompanyAsync(salesTeamId, GetLoggedInUserCompanyId());

            if (processingResult.IsFatalFailure()) {
                //var logMessage = String.Format("A fatal error occurred while retrieving SalesTeam with Id: {0}", salesTeamId);
                //Logger.Fatal(logMessage);
                ExceptionlessClient.Default.CreateLog(typeof(SalesTeamController).FullName,String.Format("A fatal error occurred while retrieving SalesTeam with Id: {0}",salesTeamId),"Error").AddTags("Controller Error").Submit();
                }

            return Ok(processingResult);
        }

        [Route("getSalesTeamsForSelection")]
        public async Task<IHttpActionResult> GetSalesTeamsForSelection() {
           var  processingResult = new ServiceProcessingResult<List<CompleteSalesTeam>> { IsSuccessful = true,Error=null,Data=null};
            var salesTeamService = new SalesTeamDataService(LoggedInUserId);
            var salesTeamResult = await salesTeamService.GetSalesTeamsForUserAsync(LoggedInUserId);
            var salesTeams = salesTeamResult.Data;
            if (!salesTeamResult.IsSuccessful)
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = salesTeamResult.Error;
                return Ok(processingResult);
            }
            //remove inactive teams

          salesTeams.RemoveAll(a => a.IsActive == false);
          processingResult.Data= salesTeams.OrderBy(a=>a.Level1Name).ThenBy(b=>b.Level2Name).ThenBy(c=>c.Name).ToList();
         
            return Ok(processingResult);
        }

        public async Task<IHttpActionResult> GetAllSalesTeamsInCompany() {
            var processingResult = new ServiceProcessingResult<List<SalesTeam>>();
            processingResult.IsSuccessful = true;

            var companyId = GetLoggedInUserCompanyId();

            var dataService = new SalesTeamDataService(LoggedInUserId);
            var salesTeams = await dataService.GetAllSalesTeamsInCompanyAsync(companyId);

            processingResult.Data = salesTeams.Data.OrderBy(a => a.ExternalDisplayName).ToList();

            if (salesTeams.IsFatalFailure()) {
                //var logMessage = String.Format("A fatal error occured while trying to get all active Sales Teams in Company with Id: {0}", companyId);
                //Logger.Fatal(logMessage);
                ExceptionlessClient.Default.CreateLog(typeof(SalesTeamController).FullName,String.Format("A fatal error occured while trying to get all active Sales Teams in Company with Id: {0}",companyId),"Error").AddTags("Controller Error").Submit();
                processingResult.IsSuccessful = false;
                return Ok(processingResult);
            }

            return Ok(processingResult);
        }

        [Route("getSalesTeamForCommissionUpdate")]
        public async Task<IHttpActionResult> GetSalesTeamForCommissionUpdate(string salesTeamId) {
            var processingResult = new ServiceProcessingResult<SalesTeamAndManagersViewModel>();

            var companyId = GetLoggedInUserCompanyId();
            var dataService = new SalesTeamDataService(LoggedInUserId);
            var getSalesTeamResult =
                await dataService.GetSalesTeamForCommissionUpdate(salesTeamId, companyId);
            if (!getSalesTeamResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_GET_SALES_TEAM_IN_COMPANY_ERROR;
                if (getSalesTeamResult.IsFatalFailure()) {
                    //var logMessage = String.Format("A fatal error occurred while retrieving SalesTeam with Id: {0}",
                    //    salesTeamId);
                    //Logger.Fatal(logMessage);

                    ExceptionlessClient.Default.CreateLog(typeof(SalesTeamController).FullName,String.Format("A fatal error occurred while retrieving SalesTeam with Id: {0}",
                        salesTeamId),"Error").AddTags("Controller Error").Submit();
                    }
                return Ok(processingResult);
            }

            var appUserDataService = new ApplicationUserDataService();
            var getSalesTeamManagersResult = await appUserDataService.GetSalesTeamManagersAsync(salesTeamId, companyId);
            // Not 100% sure if this should cause failure
            if (!getSalesTeamManagersResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_COULD_NOT_FIND_MANAGERS_ERROR;
                if (getSalesTeamManagersResult.IsFatalFailure()) {
                    //var logMessage =
                    //    String.Format("A fatal error occurred while retrieving Managers for Sales Team with Id: {0}",
                    //        salesTeamId);
                    //Logger.Fatal(logMessage);
                    ExceptionlessClient.Default.CreateLog(typeof(SalesTeamController).FullName,String.Format("A fatal error occurred while retrieving Managers for Sales Team with Id: {0}",
                            salesTeamId),"Error").AddTags("Controller Error").Submit();
                    }
                return Ok(processingResult);
            }

            processingResult.Data = new SalesTeamAndManagersViewModel();
            processingResult.Data.SalesTeam = getSalesTeamResult.Data;
            processingResult.Data.SalesTeamManagers = getSalesTeamManagersResult.Data;
            processingResult.IsSuccessful = true;

            return Ok(processingResult);
        }



        [HttpGet]
        public async Task<IHttpActionResult> SearchSalesTeams(string searchString) {
            var processingResult = new ServiceProcessingResult<List<CompleteSalesTeam>>();
            if (searchString == null) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_SEARCH_WITHOUT_CRITERIA_ERROR;
                return Ok(processingResult);
            }

            var salesTeamService = new SalesTeamDataService(LoggedInUserId);
            processingResult = await salesTeamService.GetSalesTeamsForUserAsync(LoggedInUserId);
            if (processingResult.IsFatalFailure()) {
                //Logger.Fatal("A fatal occurred while searching SalesTeams");
                ExceptionlessClient.Default.CreateLog(typeof(SalesTeamController).FullName,"A fatal occurred while searching SalesTeams","Error").AddTags("Controller Error").Submit();
                }

            var teamsToSearch = processingResult.Data;

            var teamsFound = teamsToSearch.Where(t => t.Name.ToLower().Contains(searchString.ToLower()) || t.ExternalDisplayName.ToLower().Contains(searchString.ToLower())).ToList();
            processingResult.Data = teamsFound;

            return Ok(processingResult);
        }

        // TODO: Validate that sales team meets conditions for deletability

        public async Task<IHttpActionResult> Delete(string salesTeamId) {
            var processingResult = new ServiceProcessingResult();
            var companyService = new CompanyDataService();
            var companyProcessingResult = await companyService.GetCompanyAsync(LoggedInUser.CompanyId);
            //check permissions
            try {
                if (LoggedInUser.Role.Rank > companyProcessingResult.Data.MinToChangeTeam) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.PERMISSION_ERROR;
                    return Ok(processingResult);
                }
            } catch {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.PERMISSION_ERROR;
                return Ok(processingResult);
            }
            var salesTeamService = new SalesTeamDataService(LoggedInUserId);
            processingResult = await salesTeamService.DeleteSalesTeamAsync(salesTeamId, GetLoggedInUserCompanyId(),
                  GetLoggedInUserRole());

            if (processingResult.IsFatalFailure()) {
                //var logMessage = String.Format("A fatal error occurred while deleting SalesTeam with Id: {0}",
                //    salesTeamId);
                //Logger.Fatal(logMessage);
                ExceptionlessClient.Default.CreateLog(typeof(SalesTeamController).FullName,String.Format("A fatal error occurred while deleting SalesTeam with Id: {0}",
                    salesTeamId),"Error").AddTags("Controller Error").Submit();

                }

            return Ok(processingResult);
        }
    }
}

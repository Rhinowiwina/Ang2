using System.Threading.Tasks;
using System.Web.Http;
using LS.Core;
using LS.Domain;
using LS.Services;
using System;
using Exceptionless;
using LS.WebApp.CustomAttributes;
namespace LS.WebApp.Controllers.api
{

    [SingleSessionAuthorize]
    [RoutePrefix("api/tenant")]
    public class TenantController : BaseApiController
    {
        protected async Task<DataAccessResult<SalesTeam>> GetSalesTeam(string TeamID)
        {
            var processingResult = new DataAccessResult<SalesTeam> { IsSuccessful = true };

            var salesTeamDataService = new SalesTeamDataService(LoggedInUserId);
            var salesTeamResult = await salesTeamDataService.GetSalesTeamsForUserWhereAsync(TeamID, LoggedInUserId);
            if (!salesTeamResult.IsSuccessful)
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = salesTeamResult.Error;
                return processingResult;
            }
            if (salesTeamResult.Data == null)
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error retrieving team information from database", "Error retrieving team information", true, false);
                //Logger.Error(String.Format("Error finding sales team for completeFulfillment. SalesTeamID:{0} LoggedInUserID: {1}", TeamID, LoggedInUserId));
                ExceptionlessClient.Default.CreateLog(typeof(TenantController).FullName,String.Format("Error finding sales team for completeFulfillment. SalesTeamID:{0} LoggedInUserID: {1}",TeamID,LoggedInUserId),"Error").AddTags("Controller Error").Submit();
                return processingResult;
            }
            var salesTeam = salesTeamResult.Data;

            processingResult.Data = salesTeam;

            return processingResult;
        }
    }
}

using System.Threading.Tasks;
using System.Web.Http;
using LS.Core;
using LS.Services;
using LS.WebApp.Utilities;

namespace LS.WebApp.Controllers.api
{
    [RoutePrefix("api/devData")]
    public class DevDataController : ApiController
    {
        [HttpGet]
        [Route("getGoodOrder")]
        [ExceptionHandling("Could not retrieve dev data, please try again. If the problem persists contact support.")]
        public async Task<IHttpActionResult> GetGoodOrder(string state)
        {
            var devDataService = new DevDataService();
            var processingResult = await devDataService.GetGoodOrder(state);

            if (!processingResult.IsSuccessful)
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError(
                    "An error occurred while getting dev data.",
                    "Could not retrieve dev data, please try again. If the problem persists contact support.", true);
                return Ok(processingResult);
            }
            return Ok(processingResult);
        }

        [HttpGet]
        [Route("getEnvironment")]
        public string GetEnvironment()
        {
            var devDataService = new DevDataService();
            var processingResult = devDataService.GetEnvironment();
            return processingResult;
        }

    }
}

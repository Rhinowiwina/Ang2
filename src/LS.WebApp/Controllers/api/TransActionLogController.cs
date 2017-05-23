using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Configuration;
using System.Web.Http;
using Microsoft.AspNet.Identity.Owin;
using LS.ApiBindingModels;
using LS.Core;
using LS.Domain;
using LS.Services;
using LS.WebApp.CustomAttributes;
using LS.WebApp.Models;
using NLog.Internal;
using Exceptionless;
using Exceptionless.Models;
namespace LS.WebApp.Controllers.api
{
//        [Authorize]
    [SingleSessionAuthorize]
    [RoutePrefix("api/transactionLog")]
    public class TransactionLogController : BaseApiController
    {

        [HttpPost]
        [Route("getTransactionID")]
        public async Task<IHttpActionResult> GetTransactionID(TransactionLog model) {
            var processingResult = new ServiceProcessingResult<string>() { IsSuccessful = true };

            var transactionDataService = new TransactionLogDataService();
            model.Id = Guid.NewGuid().ToString();
            processingResult.Data = model.Id;
            var logResult = await transactionDataService.AddAsync(model);
            
            if (!logResult.IsSuccessful)
            {
                //Logger.Fatal("A fatal error occurred while creating a log for order. Message:" + logResult.Error.UserMessage);
                ExceptionlessClient.Default.CreateLog(typeof(TransactionLogController).FullName,"A fatal error occurred while creating a log for order. Message:" + logResult.Error.UserMessage,"Error").AddTags("Controller Error").Submit();

                processingResult.IsSuccessful = false;
                processingResult.Error =new ProcessingError("Failed to create transactionid. Reason:"+ logResult.Error.UserHelp, "Failed to create transactionid. Reason:" + logResult.Error.UserHelp,true,false);

                return Ok(processingResult);
            }
            
            processingResult.IsSuccessful = true;

            return Ok(processingResult);
        }
    }
}

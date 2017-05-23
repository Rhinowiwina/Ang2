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

namespace LS.WebApp.Controllers.api {

    //        [Authorize]
    [SingleSessionAuthorize]
    [RoutePrefix("api/payment")]
    public class PaymentsController : BaseApiController {

        private static string UserName = "ACCOUNTING";
        private static string passWord = "c0mmi5s1ons";
        private static List<ApiLoggedInUser> LoggedOnUsers = new List<ApiLoggedInUser>();
        private static DateTime TimeStamp = DateTime.Now;
        private bool UserLoggedIn() {
            var user = LoggedOnUsers.FirstOrDefault(a => a.UserId == LoggedInUser.Id);
            if (user != null) {
                //check timestamp if longer than 10minutes go to login.
                TimeSpan timeSpan = DateTime.Now - user.TimeStamp;
                int diffenceMinutes = timeSpan.Minutes;

                if (diffenceMinutes > 10) {
                    //Need to relogin
                    LoggedOnUsers.Remove(user);
                    return false;
                } else {
                    //refresh timestamp
                    user.TimeStamp = DateTime.Now;
                }
            } else//user not logged in, not in list
                  {
                return false;
            }
            //all is good
            return true;
        }
        [HttpPost]
        [Route("commissionPaymentLogin")]
        public async Task<IHttpActionResult> CommmissionPaymentLogin(Credentials model) {
            var processingResult = new DataAccessResult();
            //if user hit login after already logged in.
            var vuser = LoggedOnUsers.FirstOrDefault(a => a.UserId == LoggedInUser.Id);
            if (vuser != null) {
                LoggedOnUsers.Remove(vuser);
            }
            //-----------------------------------
            if (model.UserName.ToUpper() != UserName.ToUpper() || model.Password != passWord) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Invalid login credentials.", "Invalid login credentials.", true, false);
                return Ok(processingResult);
            }
            ApiLoggedInUser user = new ApiLoggedInUser() { UserName = model.UserName, UserId = LoggedInUser.Id, TimeStamp = DateTime.Now };
            LoggedOnUsers.Add(user);
            processingResult.IsSuccessful = true;
            return Ok(processingResult);
        }
        [HttpGet]
        [Route("getDetailTransaction")]
        public async Task<IHttpActionResult> GetDetailTransaction(string transactionID, string type) {
            //type is either process or batch
            var processingResult = new ServiceProcessingResult<List<PaymentDetailViewBindingModel>>();

            if (!UserLoggedIn()) {
                //return: check for message in js controler and send to login page.
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Not Logged On", "Not Logged On", true, false);
                return Ok(processingResult);
            }
            if (!CommissionCheck()) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error in commissions table. Contact support.", "Error in commissions table. Contact support.", true, false);
                return Ok(processingResult);
            }

            var dataService = new PaymentsDataService();
            var detailBatchResult = dataService.GetDetailTransaction(transactionID, type);

            if (!detailBatchResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = detailBatchResult.Error;
                if (processingResult.IsFatalFailure()) {
                    //Logger.Fatal("A fatal error occurred while getting pending detail PayPal payments.");
                    ExceptionlessClient.Default.CreateLog(typeof(PaymentsController).FullName, "A fatal error occurred while getting pending detail PayPal payments.", "Error").AddTags("Controller Error").Submit();
                }
                return Ok(processingResult);
            }

            processingResult.Data = detailBatchResult.Data;
            processingResult.IsSuccessful = true;
            return Ok(processingResult);
        }
        [HttpGet]
        [Route("paymentUpdate")]
        public async Task<IHttpActionResult> MarkBatchPaid(string processID) {
            //type is either process or batch
            var processingResult = new ServiceProcessingResult<string>();

            if (!UserLoggedIn()) {
                //return: check for message in js controler and send to login page.
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Not Logged On", "Not Logged On", true, false);
                return Ok(processingResult);
            }
            if (!CommissionCheck()) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error in commissions table. Contact support.", "Error in commissions table. Contact support.", true, false);
                return Ok(processingResult);
            }

            var dataService = new PaymentsDataService();
            var detailBatchResult = dataService.MarkBatchPaid(processID);

            if (!detailBatchResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = detailBatchResult.Error;
                if (processingResult.IsFatalFailure()) {
                    //Logger.Fatal("A fatal error occurred while getting pending detail PayPal payments.");
                    ExceptionlessClient.Default.CreateLog(typeof(PaymentsController).FullName, "A fatal error occurred while getting pending detail PayPal payments.", "Error").AddTags("Controller Error").Submit();
                }
                return Ok(processingResult);
            }

            processingResult.Data = detailBatchResult.Data;
            processingResult.IsSuccessful = true;
            return Ok(processingResult);
        }
        [HttpGet]
        [Route("getPaymentBatches")]
        public async Task<IHttpActionResult> GetPaymentBatches(bool paidOnly) {
            var processingResult = new ServiceProcessingResult<List<PaymentViewBindingModel>>();
            if (!UserLoggedIn()) {
                //return: check for message in js controler and send to login page.
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Not Logged On", "Not Logged On", true, false);
                return Ok(processingResult);
            }
            if (!CommissionCheck()) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error in commissions table. Contact support.", "Error in commissions table. Contact support.", true, false);
                return Ok(processingResult);
            }
            var dataService = new PaymentsDataService();
            var paidBatchResult = dataService.PaymentBatches(paidOnly);

            if (!paidBatchResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = paidBatchResult.Error;
                if (processingResult.IsFatalFailure()) {
                    //Logger.Fatal("A fatal error occurred while getting PayPal payments.");
                    ExceptionlessClient.Default.CreateLog(typeof(PaymentsController).FullName, "A fatal error occurred while getting PayPal payments.", "Error").AddTags("Controller Error").Submit();
                }
                return Ok(processingResult);
            }

            processingResult.Data = paidBatchResult.Data;
            processingResult.IsSuccessful = true;
            return Ok(processingResult);
        }
        [HttpPost]
        [Route("createPaymentBatches")]
        public async Task<IHttpActionResult> CreatePaymentBatch() {
            var processingResult = new ServiceProcessingResult<string>();
            if (!UserLoggedIn()) {
                //return: check for message in js controler and send to login page.
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Not Logged On", "Not Logged On", true, false);
                return Ok(processingResult);
            }
            if (!CommissionCheck()) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error in commissions table. Contact support.", "Error in commissions table. Contact support.", true, false);
                return Ok(processingResult);
            }
            var dataService = new PaymentsDataService();
            var createBatchResult = dataService.CreatePaymentBatch();

            if (!createBatchResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = createBatchResult.Error;
                if (processingResult.IsFatalFailure()) {
                    //Logger.Fatal("A fatal error occurred while creating payment batches.");
                    ExceptionlessClient.Default.CreateLog(typeof(PaymentsController).FullName, "A fatal error occurred while creating payment batches.", "Error").AddTags("Controller Error").Submit();
                }
                return Ok(processingResult);
            }

            processingResult.Data = createBatchResult.Data;
            processingResult.IsSuccessful = true;
            return Ok(processingResult);
        }




        public bool CommissionCheck() {

            var dataService = new PaymentsDataService();
            var result = dataService.CommissionCheck();

            return result.Data;

        }

        public class Credentials {
            public string UserName { get; set; }
            public string Password { get; set; }
        }
        public class ApiLoggedInUser {
            public string UserName { get; set; }
            public string UserId { get; set; }
            public DateTime TimeStamp { get; set; }
        }
    }

}

using System;
using System.Configuration;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Text;
using LS.Services;
using LS.Utilities;
using LS.Domain;
using LS.Domain.ScheduledTasks;
using LS.Core;
using LS.ApiBindingModels;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;
namespace LS.WebApp.Controllers.MVC {
    public class SchedTasksController : Controller {
        // GET: SchedTasks
        static string _companyId = "65eab0c7 - c7b8 - 496b - 9325 - dd8c9ba8ce1c";
        static string _portalId = "de77ba48-47b0-402f-a50a-cbdde615c898";
        public ActionResult Index() {
            return View("CommissionPayment");
        }

        private bool RequestIsAuthorized() {
            var webresult = new System.IO.StreamReader(System.Web.HttpContext.Current.Request.InputStream).ReadToEnd();

            Match username = Regex.Match(webresult, @"<username>\s*(.+?)\s*</username>");
            var usernameStripped = Regex.Replace(username.Value.ToString(), @"<[^>]*>", String.Empty);
            Match password = Regex.Match(webresult, @"<password>\s*(.+?)\s*</password>");
            var passwordStripped = Regex.Replace(password.Value.ToString(), @"<[^>]*>", String.Empty);

            if (usernameStripped == ConfigurationManager.AppSettings["SchedTasksUsername"] && passwordStripped == ConfigurationManager.AppSettings["SchedTasksPassword"]) {
                return true;
            } else {
                return false;
            }
        }

        public async Task<ViewResult> ApiLogArchive() {
            if (!RequestIsAuthorized()) {
                ViewBag.Content = "<strong>Not Authorized</strong>";
                return View();
            }
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var body = "";
            var subject = "";

            var ApiLogDataService = new ApiLogEntryDataService();
            var apiLogResult = await ApiLogDataService.ArchiveEntries();
            watch.Stop();
            var elapsed = watch.Elapsed;
            if (!apiLogResult.IsSuccessful) {
                ViewBag.Content = apiLogResult.Error.UserHelp;
                subject = "API LOG Archive Failed";
                body = "API LOG Archive Failed, check exceptionless logs.";
                } else {

                ViewBag.Content = "API Log Archive Succeeded.";
                subject = "API Log Archive Success";
                body = "API Log Archive successfully archived " + apiLogResult.Data[0].ToString() + " records and removed " + apiLogResult.Data[1].ToString() + " records older than 180 days from archive in " + elapsed.TotalMinutes.ToString() + " minutes.";

                }

            var emailHelper = new EmailHelper();

            var sendEmail = emailHelper.SendEmail(subject, "randy@305spin.com,jarrett@305spin.com", "", body);


            return View();
        }

        public async Task<ViewResult> SendFailureEmail() {
            var webresult = new System.IO.StreamReader(System.Web.HttpContext.Current.Request.InputStream).ReadToEnd();
            Match subject = Regex.Match(webresult, @"<subject>\s*(.+?)\s*</subject>");
            var subjectStripped = Regex.Replace(subject.Value.ToString(), @"<[^>]*>", String.Empty);

            Match content = Regex.Match(webresult, @"<content>\s*(.+?)\s*</content>");
            var contentStripped = content.ToString().Replace("<content>", String.Empty).Replace("</content>", String.Empty);

            var emailHelper = new EmailHelper();

            var sendEmail = emailHelper.SendEmail(subjectStripped, "kevin@305spin.com,jarrett@305spin.com", "", contentStripped);
            if (!sendEmail.Result.IsSuccessful) {
                ViewBag.Content = "<span>Email failed to send.</span>";
                return View();
            }
            ViewBag.Content = contentStripped;
            return View();
        }

        public async Task<ViewResult> ClearOrders() {
            if (!RequestIsAuthorized()) {
                ViewBag.Content = "<strong>Not Authorized</strong>";
                return View();
            }
            var emailSubject = "Sched Task: Clear Orders";
            var schedTasksDataService = new SchedTasksDataService();
            var schedTaskServiceResult = await schedTasksDataService.ClearOrders();
            var emailContent = "";
            if (!schedTaskServiceResult.IsSuccessful) {
                if (schedTaskServiceResult.Data.Content != null) { emailContent = schedTaskServiceResult.Data.Content; }
                if (schedTaskServiceResult.Error.UserHelp != null) { emailContent += "<br><br>=====================<br>ERROR<br>" + schedTaskServiceResult.Error.UserHelp; }
                emailSubject = "ERROR || " + emailSubject;
                schedTaskServiceResult.Data.SendEmail = true;
            } else {
                if (schedTaskServiceResult.Data.Content != null) { emailContent = schedTaskServiceResult.Data.Content; }
            }
            ViewBag.Content = emailContent;

            if (schedTaskServiceResult.Data.SendEmail) {
                var emailHelper = new EmailHelper();
                var sendEmail = emailHelper.SendEmail(emailSubject, "jarrett@305spin.com,kevin@305spin.com", "", emailContent);
            }

            return View();
        }

        public async Task<ViewResult> MoveDataFiles() {
            if (!RequestIsAuthorized()) {
                ViewBag.Content = "<strong>Not Authorized</strong>";
                return View();
            }
            var emailSubject = "Sched Task: Lifeline Services Move";
            var schedTasksDataService = new SchedTasksDataService();
            var schedTaskServiceResult = await schedTasksDataService.MoveDataFiles();
            var emailContent = "";
            if (!schedTaskServiceResult.IsSuccessful) {
                if (schedTaskServiceResult.Data.Content != null) { emailContent = schedTaskServiceResult.Data.Content; }
                if (schedTaskServiceResult.Error.UserHelp != null) { emailContent += "<br><br>=====================<br>ERROR<br>" + schedTaskServiceResult.Error.UserHelp; }
                emailSubject = "ERROR || " + emailSubject;
                schedTaskServiceResult.Data.SendEmail = true;
            } else {
                if (schedTaskServiceResult.Data.Content != null) { emailContent = schedTaskServiceResult.Data.Content; }
            }
            ViewBag.Content = emailContent;

            if (schedTaskServiceResult.Data.SendEmail) {
                var emailHelper = new EmailHelper();
                var sendEmail = emailHelper.SendEmail(emailSubject, "jarrett@305spin.com,kevin@305spin.com", "", emailContent);
            }

            return View();
        }

        public async Task<ViewResult> ImportDataFilesCheck() {
            if (!RequestIsAuthorized()) {
                ViewBag.Content = "<strong>Not Authorized</strong>";
                return View();
            }
            var emailSubject = "Sched Task: Lifeline Services Arrow Import CHECK";
            var schedTasksDataService = new SchedTasksDataService();
            var schedTaskServiceResult = await schedTasksDataService.ImportDataFilesCheck();
            var emailContent = "";
            if (!schedTaskServiceResult.IsSuccessful) {
                if (schedTaskServiceResult.Data.Content != null) { emailContent = schedTaskServiceResult.Data.Content; }
                if (schedTaskServiceResult.Error.UserHelp != null) { emailContent += "<br><br>=====================<br>ERROR<br>" + schedTaskServiceResult.Error.UserHelp; }
                emailSubject = "ERROR || " + emailSubject;
                schedTaskServiceResult.Data.SendEmail = true;
            } else {
                if (schedTaskServiceResult.Data.Content != null) { emailContent = schedTaskServiceResult.Data.Content; }
            }
            ViewBag.Content = emailContent;

            if (schedTaskServiceResult.Data.SendEmail) {
                var emailHelper = new EmailHelper();
                var sendEmail = emailHelper.SendEmail(emailSubject, "jarrett@305spin.com,kevin@305spin.com", "", emailContent);
            }

            return View();
        }

        public async Task<ViewResult> DetailReport() {
            if (!RequestIsAuthorized()) {
                ViewBag.Content = "<strong>Not Authorized</strong>";
                return View();
            }

            var schedTasksDataService = new SchedTasksDataService();
            var schedTaskServiceResult = await schedTasksDataService.DetailReport();
            //var emailContent = "";
            if (!schedTaskServiceResult.IsSuccessful) {

                string emailContent = "";
                string emailSubject = "Detail Report Export";
                if (schedTaskServiceResult.Error.UserHelp != null) { emailContent += "<br><br>=====================<br>ERROR<br>" + schedTaskServiceResult.Error.UserHelp; }
                emailSubject = "ERROR || " + emailSubject;

                ViewBag.Content = schedTaskServiceResult.Error.UserHelp;
                var emailHelper = new EmailHelper();
                var sendEmail = emailHelper.SendEmail(emailSubject, "jarrett@305spin.com,kevin@305spin.com", "", emailContent);


            } else {
                ViewBag.Content = "Detail Report was successfull.";
            }

            return View();
        }

        public async Task<ViewResult> ImportDataFiles() {
            if (!RequestIsAuthorized()) {
                ViewBag.Content = "<strong>Not Authorized</strong>";
                return View();
            }
            var emailSubject = "Sched Task: Lifeline Services Import";
            var schedTasksDataService = new SchedTasksDataService();
            var schedTaskServiceResult = await schedTasksDataService.ImportDataFiles();
            var emailContent = "";
            if (!schedTaskServiceResult.IsSuccessful) {
                if (schedTaskServiceResult.Data.Content != null) { emailContent = schedTaskServiceResult.Data.Content; }
                if (schedTaskServiceResult.Error.UserHelp != null) { emailContent += "<br><br>=====================<br>ERROR<br>" + schedTaskServiceResult.Error.UserHelp; }
                emailSubject = "ERROR || " + emailSubject;
                schedTaskServiceResult.Data.SendEmail = true;
            } else {
                if (schedTaskServiceResult.Data.Content != null) { emailContent = schedTaskServiceResult.Data.Content; }
            }
            ViewBag.Content = emailContent;

            if (schedTaskServiceResult.Data.SendEmail) {
                var emailHelper = new EmailHelper();
                var sendEmail = emailHelper.SendEmail(emailSubject, "jarrett@305spin.com,kevin@305spin.com", "", emailContent);
            }

            return View();
        }

        public async Task<ViewResult> ProcessDataCheck() {
            if (!RequestIsAuthorized()) {
                ViewBag.Content = "<strong>Not Authorized</strong>";
                return View();
            }

            HttpContext.Server.ScriptTimeout = 300;

            var emailSubject = "Sched Task: Lifeline Services Process CHECK";
            var schedTasksDataService = new SchedTasksDataService();
            var schedTaskServiceResult = await schedTasksDataService.ProcessDataCheck();
            var emailContent = "";
            if (!schedTaskServiceResult.IsSuccessful) {
                if (schedTaskServiceResult.Data.Content != null) { emailContent = schedTaskServiceResult.Data.Content; }
                if (schedTaskServiceResult.Error.UserHelp != null) { emailContent += "<br><br>=====================<br>ERROR<br>" + schedTaskServiceResult.Error.UserHelp; }
                emailSubject = "ERROR || " + emailSubject;
                schedTaskServiceResult.Data.SendEmail = true;
            } else {
                if (schedTaskServiceResult.Data.Content != null) { emailContent = schedTaskServiceResult.Data.Content; }
            }
            ViewBag.Content = emailContent;

            if (schedTaskServiceResult.Data.SendEmail) {
                var emailHelper = new EmailHelper();
                var sendEmail = emailHelper.SendEmail(emailSubject, "jarrett@305spin.com,kevin@305spin.com", "", emailContent);
            }

            return View();
        }

        public async Task<ViewResult> ProcessData() {
            if (!RequestIsAuthorized()) {
                ViewBag.Content = "<strong>Not Authorized</strong>";
                return View();
            }

            HttpContext.Server.ScriptTimeout = 300;

            var emailSubject = "Sched Task: Lifeline Services Process";
            var schedTasksDataService = new SchedTasksDataService();
            var schedTaskServiceResult = await schedTasksDataService.ProcessData();
            var emailContent = "";
            if (!schedTaskServiceResult.IsSuccessful) {
                if (schedTaskServiceResult.Data.Content != null) { emailContent = schedTaskServiceResult.Data.Content; }
                if (schedTaskServiceResult.Error.UserHelp != null) { emailContent += "<br><br>=====================<br>ERROR<br>" + schedTaskServiceResult.Error.UserHelp; }
                emailSubject = "ERROR || " + emailSubject;
                schedTaskServiceResult.Data.SendEmail = true;
            } else {
                if (schedTaskServiceResult.Data.Content != null) { emailContent = schedTaskServiceResult.Data.Content; }
            }
            ViewBag.Content = emailContent;

            if (schedTaskServiceResult.Data.SendEmail) {
                var emailHelper = new EmailHelper();
                var sendEmail = emailHelper.SendEmail(emailSubject, "jarrett@305spin.com,kevin@305spin.com", "", emailContent);
            }

            return View();
        }

        public async Task<ViewResult> ApiLogEhdbCheckToEhdbResponseLog() {
            if (!RequestIsAuthorized()) {
                ViewBag.Content = "<strong>Not Authorized</strong>";
                return View();
            }

            var ApiLogDataService = new ApiLogEntryDataService();
            var EhdbResponseLogDataService = new ResponseLogsCGMEHDBDataService();
            var EhdbResponseLogDetailsDataService = new EhdbResponseLogDetailDataService();

            var apiLogResult = await ApiLogDataService.GetUnImportedLogEntries("CGM-EHDB", "Check");
            if (!apiLogResult.IsSuccessful) {
                ViewBag.Content = "Error retrieving json content.";
                return View();
            }

            //list of CGM-EHDB Check entries. Need to parse into fields an update table
            var apiEntries = apiLogResult.Data;
            string returnresult;
            var recount = apiEntries.Count;
            int processRecs = 0;
            foreach (var entry in apiEntries) {
                var _apiResponse = entry.Response;
                var logDate = entry.DateCreated;

                var json_serializer = new JavaScriptSerializer();
                var apiObject = json_serializer.Deserialize<ApiResponse>(_apiResponse);

                string EhdbResponseLogId = Guid.NewGuid().ToString();
                var EhdbResponseLog = new ResponseLogsCGMEHDB() {
                    TransactionId = apiObject.TransactionId,
                    Blacklist = apiObject.Blacklist,
                    Status = apiObject.Status,
                    LogDate = logDate,
                    Message = apiObject.Message,
                    APILogEntriesID = entry.Id,
                    EhdbDetails = null,
                    Id = EhdbResponseLogId
                };

                List<ResponseLogsCGMEHDBDetails> ListOfEhdbDetails = new List<ResponseLogsCGMEHDBDetails>();
                //gather and add child records
                if (apiObject.SubscriberCheck != null) {
                    foreach (var row in apiObject.SubscriberCheck) {
                        var EhdbDetails = new ResponseLogsCGMEHDBDetails() {
                            Type = "SubscriberCheck",
                            ResponseLogsCGMEHDBId = EhdbResponseLogId,
                            PeriodDays = row.PeriodDays,
                            Matches = row.Matches,
                            IsMatched = null
                        };
                        ListOfEhdbDetails.Add(EhdbDetails);
                    }
                }
                if (apiObject.AgentCheck != null) {
                    foreach (var row in apiObject.AgentCheck) {
                        var EhdbDetails = new ResponseLogsCGMEHDBDetails() {
                            Type = "AgentCheck",
                            ResponseLogsCGMEHDBId = EhdbResponseLogId,
                            PeriodDays = row.PeriodDays,
                            Matches = null,
                            IsMatched = row.IsMatched
                        };
                        ListOfEhdbDetails.Add(EhdbDetails);
                    }
                }
                if (apiObject.DeviceCheck != null) {
                    foreach (var row in apiObject.DeviceCheck) {
                        var EhdbDetails = new ResponseLogsCGMEHDBDetails() {
                            Type = "DeviceCheck",
                            ResponseLogsCGMEHDBId = EhdbResponseLogId,
                            PeriodDays = row.PeriodDays,
                            Matches = null,
                            IsMatched = row.IsMatched
                        };
                        ListOfEhdbDetails.Add(EhdbDetails);
                    }
                }
                if (apiObject.AgentSubscriberCrossCheck != null) {
                    foreach (var row in apiObject.AgentSubscriberCrossCheck) {
                        var EhdbDetails = new ResponseLogsCGMEHDBDetails() {
                            Type = "AgentSubscriberCrossCheck",
                            ResponseLogsCGMEHDBId = EhdbResponseLogId,
                            PeriodDays = row.PeriodDays,
                            Matches = row.Matches,
                            IsMatched = null
                        };
                        ListOfEhdbDetails.Add(EhdbDetails);
                    }
                }

                //insert parent 
                var addResoult = await EhdbResponseLogDataService.AddAsync(EhdbResponseLog);
                if (!addResoult.IsSuccessful) {
                    continue; //skip record if bad
                }
                processRecs += 1;//number of updated records
                //update parent as imported
                entry.JsonImported = true;
                var apiEntryResult = await ApiLogDataService.UpdateAsync(entry);

                //insert children
                foreach (var record in ListOfEhdbDetails) {
                    var addDetailResult = await EhdbResponseLogDetailsDataService.AddAsync(record);
                }
                ListOfEhdbDetails.Clear();
            }
            if (recount > 0) {
                returnresult = processRecs.ToString() + " of " + recount.ToString() + " records imported and updated.";
            } else {
                returnresult = "No records available to import.";
            }

            ViewBag.Content = returnresult;
            return View();
        }

        public async Task<ViewResult> SetUserInactiveDrugExpiration() {

            if (!RequestIsAuthorized()) {
                ViewBag.Content = "<strong>Not Authorized</strong>";
                return View();
            }
            //User portal Creds-same as CaPortal creds
            var username = ConfigurationManager.AppSettings["WebAPIUsername"];
            var password = ConfigurationManager.AppSettings["WebAPIPassword"];
            var domain = ConfigurationManager.AppSettings["UserPortalDomain"];
            HttpClient client;

            //The URL of the WEB API Service
            string url = domain + "/webapi/user/expiredDrugCert?companyId=" + _companyId + "&portalId=" + _portalId;


            //Set the base address and the Header Formatter
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));
            client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            HttpResponseMessage responseMessage = await client.GetAsync(url);
            ServiceProcessingResult<List<UserDrugExpirations>> expiredUsersResult = new ServiceProcessingResult<List<UserDrugExpirations>>();
            if (responseMessage.IsSuccessStatusCode) {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                expiredUsersResult = JsonConvert.DeserializeObject<ServiceProcessingResult<List<UserDrugExpirations>>>(responseData);

            }
            if (!expiredUsersResult.IsSuccessful) {
                //log and send email
                var emailContent = expiredUsersResult.Error.UserHelp;

                var emailHelper = new EmailHelper();
                var sendEmail = emailHelper.SendEmail("Error retrieving expired drug certification users", "jarrett@305spin.com,kevin@305spin.com,randy@305spin.com", "", "Error returned from User Portal:" + emailContent);
                ViewBag.Content = "Failed to retrieve expired users.";
                return View();
            }

            var resultmsg = "";
            if (expiredUsersResult.Data != null) {
                List<UserDrugExpirations> expireredUsers = new List<UserDrugExpirations>();
                expireredUsers = expiredUsersResult.Data;
                foreach (var user in expireredUsers) {
                    var sqlQuery = new SQLQuery();

                    SqlParameter[] parameters = new SqlParameter[] {
                 new SqlParameter("@UserId",user.PortalUserId),
                      };
                    var strQuery = "UPDATE [dbo].[AspNetUsers] SET [IsActive]=0 WHERE [Id]=@UserId";
                    //if user not found to be updated does not exist in this portal
                    var result = await sqlQuery.ExecuteNonQueryAsync(CommandType.Text, strQuery, parameters);
                    if (!result.IsSuccessful) {
                        //log and email
                        var emailHelper = new EmailHelper();
                        var sendEmail = emailHelper.SendEmail("Scheduled Task SetUserInactiveDrugExpiration:Fatal Error in SqlQuery", "jarrett@305spin.com,kevin@305spin.com,randy@305spin.com", "", result.Error.UserHelp);
                    }
                }
            } else {
                ViewBag.Content = "No expired users found." + resultmsg;
                return View();
            }

            ViewBag.Content = "Scheduled Task SetUserInactiveDrugExpiration finished.<br>" + resultmsg;
            return View();
        }


        public async Task<ViewResult> TempFileCleanup() {
            if (!RequestIsAuthorized()) {
                ViewBag.Content = "<strong>Not Authorized</strong>";
                return View();
            }
            var emailSubject = "Sched Task: TempFile Cleanup";
            var schedTasksDataService = new SchedTasksDataService();
            var schedTaskServiceResult = await schedTasksDataService.TempFileCleanup();
            var emailContent = "";
            if (!schedTaskServiceResult.IsSuccessful) {
                if (schedTaskServiceResult.Data.Content != null) { emailContent = schedTaskServiceResult.Data.Content; }
                if (schedTaskServiceResult.Error.UserHelp != null) { emailContent += "<br><br>=====================<br>ERROR<br>" + schedTaskServiceResult.Error.UserHelp; }
                emailSubject = "ERROR || " + emailSubject;
                schedTaskServiceResult.Data.SendEmail = true;
            } else {
                if (schedTaskServiceResult.Data.Content != null) { emailContent = schedTaskServiceResult.Data.Content; }
            }
            ViewBag.Content = emailContent;

            if (schedTaskServiceResult.Data.SendEmail) {
                var emailHelper = new EmailHelper();
                var sendEmail = emailHelper.SendEmail(emailSubject, "jarrett@305spin.com", "", emailContent);
            }

            return View();
        }
    }
}
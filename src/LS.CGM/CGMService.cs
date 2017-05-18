using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common.Logging;
using LS.Core;
using LS.Domain;
using LS.Domain.ExternalApiIntegration.CGM;
using LS.Domain.ExternalApiIntegration;
using LS.Utilities;
using Newtonsoft.Json;
using LS.Repositories;
using System.Configuration;

namespace LS.CGM {
    public class CGMService {
        private ILog Logger { get; set; }
        private static readonly string BaseURL = ConfigurationManager.AppSettings["CGMBaseUrl"];
        private static readonly string CheckEndpoint = "Check";
        private static readonly string EnrollEndpoint = "Enroll";
        private static readonly string AuthUrl = "authenticate";
        private static readonly string Username = ConfigurationManager.AppSettings["CGMUsername"];
        private static readonly string Password = ConfigurationManager.AppSettings["CGMPassword"];
        private static readonly string ApiName = "CGM-EHDB";

        public CGMService() {
            Logger = LoggerFactory.GetLogger(GetType());
        }

        public async Task<ServiceProcessingResult<CGMCheckResponse>> Check(CGMCheckRequest subscriber) {
            var result = new ServiceProcessingResult<CGMCheckResponse>();
            string jsonCred = "{\"Username\":\"" + Username + "\"," + "\"Password\":\"" + Password + "\"}";

            string retresult = "";
            //Get security token from cgm
            try {
                using (var client = new WebClient()) {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    retresult = client.UploadString(BaseURL + AuthUrl, "POST", jsonCred);
                    var jsonContent = JsonConvert.DeserializeObject<AuthResponse>(retresult);
                    string token = jsonContent.Token;
                    subscriber.Token = token;

                }

                if (subscriber.SubscriberLast4ssn.Length > 4) {
                    subscriber.SubscriberLast4ssn = subscriber.SubscriberLast4ssn.Substring(0, 4);
                }

                var jsonRequestData = JsonConvert.SerializeObject(subscriber);
                //send data to be checked,get return info 
                var logEntry = CreateInitialCheckApiLogEntry(jsonRequestData);

                using (var client1 = new WebClient()) {
                    client1.Headers[HttpRequestHeader.ContentType] = "application/json";
                    retresult = client1.UploadString(BaseURL + CheckEndpoint, "POST", jsonRequestData);
                    var response = JsonConvert.DeserializeObject<CGMCheckResponse>(retresult);
                    //send back to controller to decide what to do with information
                    FinishLogEntry(logEntry, retresult);
                    result.Data = response;
                    result.Data.Token = subscriber.Token;
                    result.IsSuccessful = true;
                }
            } catch (Exception ex1) {
                Logger.Error("Failed to read CGM REST API Check response", ex1);
                result.IsSuccessful = false;
                result.Error = new ProcessingError("CGM API Check Failed - Reason:" + ex1.Message, "CGM API Check Failed.Reason:" + ex1.Message, true, false);
                return result;
            }

            return result;
        }

        public async Task<ServiceProcessingResult<CGMEnrollResponse>> Enroll(CGMEnrollRequest subscriber) {
            var result = new ServiceProcessingResult<CGMEnrollResponse>();

            try {
                if (String.IsNullOrEmpty(subscriber.Token)) {
                    string jsonCred = "{\"Username\":\"" + Username + "\"," + "\"Password\":\"" + Password + "\"}";
                    string retresult = "";


                    using (var client = new WebClient()) {

                        client.Headers[HttpRequestHeader.ContentType] = "application/json";
                        retresult = client.UploadString(BaseURL + AuthUrl, "POST", jsonCred);
                        var jsonContent = JsonConvert.DeserializeObject<AuthResponse>(retresult);
                        string token = jsonContent.Token;
                        subscriber.Token = token;
                    }
                }

                if (subscriber.SubscriberLast4ssn.Length > 4) {
                    subscriber.SubscriberLast4ssn = subscriber.SubscriberLast4ssn.Substring(0, 4);
                }

                var jsonRequestData = JsonConvert.SerializeObject(subscriber);
                //send data to be checked,get return info 
                var logEntry = CreateInitialEnrollApiLogEntry(jsonRequestData);
                using (var client = new WebClient()) {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    string jsonresult = client.UploadString(BaseURL + EnrollEndpoint, "POST", jsonRequestData);

                    var response = JsonConvert.DeserializeObject<CGMEnrollResponse>(jsonresult);

                    FinishLogEntry(logEntry, jsonresult);
                    result.Data = response;

                    result.IsSuccessful = true;
                }
            } catch (Exception ex1) {
                Logger.Error("Failed to read CGM REST API Check response", ex1);
                result.IsSuccessful = false;
                result.Error = new ProcessingError("CGM API Enroll Failed - Reason:" + ex1.Message, "CGM API Enroll Failed - Reason:" + ex1.Message, true, false);
                return result;
            }

            return result;
        }

        private static void FinishLogEntry(ApiLogEntry logEntry, string content) {
            logEntry.Response = content;
            logEntry.DateEnded = DateTime.UtcNow;

            var logEntryRepository = new ApiLogEntryRepository();
            logEntryRepository.Add(logEntry);
        }

        private static ApiLogEntry CreateInitialCheckApiLogEntry(string input) {
            return new ApiLogEntry {
                Api = ApiName,
                DateStarted = DateTime.UtcNow,
                Function = ApiFunctions.Check,
                Input = input
            };
        }

        private static ApiLogEntry CreateInitialEnrollApiLogEntry(string input) {
            return new ApiLogEntry {
                Api = ApiName,
                DateStarted = DateTime.UtcNow,
                Function = ApiFunctions.Enroll,
                Input = input
            };
        }
    }
}


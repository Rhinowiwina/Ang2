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
using LS.Domain.ExternalApiIntegration;
using LS.Utilities;
using Newtonsoft.Json;
using LS.Repositories;
using System.Configuration;

namespace LS.Services.ExternalApiIntegration 
{
    public class NevadaSolixLifelineApplicationService
    {
        private static readonly string Username = ConfigurationManager.AppSettings["NevadaSolixUsername"];
        private static readonly string Password = ConfigurationManager.AppSettings["NevadaSolixPassword"];
        private static readonly string BaseUrl = ConfigurationManager.AppSettings["NevadaBaseUrl"];
        private static readonly Cookie NevadaSolixAuthorizationCookie = new Cookie("OBBasicAuth", "fromDialog");
        private static readonly string ApiName = "NevadaSolix";
        private const string AuthorizationType = "Basic";

        private static readonly string functionDHHSEligible = "DHHSEligible?val=";
        private ILog Logger { get; set; }
        private static readonly string BaseURL = ConfigurationManager.AppSettings["NevadaBaseUrl"];
        public NevadaSolixLifelineApplicationService()
        {
            Logger = LoggerFactory.GetLogger(GetType());
        }

        public async Task<ServiceProcessingResult<LS.Domain.ExternalApiIntegration.NevadaSolixEligibilityResponseData>> NevadaEligibilityAsync(LS.Domain.ExternalApiIntegration.NevadaSolixEligibilityRequestData requestData)
        {
            var result = new ServiceProcessingResult<LS.Domain.ExternalApiIntegration.NevadaSolixEligibilityResponseData>();
            string jsonCred = "{\"Username\":\"" + Username + "\"," + "\"Password\":\"" + Password + "\"}";
            var convertedDate = Convert.ToDateTime(requestData.DateOfBirth);
            string strToConvert = "lastName=" + requestData.LastName + "&dob=" + convertedDate.ToString("MM/dd/yyyy") + "&last4ssn=" + requestData.Last4Ssn + "&tribalId=";
            string Url = BaseURL + functionDHHSEligible  + Base64Encode(strToConvert);
            try {
                var cookieContainer = new CookieContainer();
                using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
                using (var httpClient = new HttpClient(handler) { BaseAddress = new Uri(BaseUrl) })
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationType, Base64Encode(Username + ":" + Password));
                    cookieContainer.Add(new Uri(BaseURL), NevadaSolixAuthorizationCookie);
                    var logEntry = CreateInitialDHHSEligibleApiLogEntry(Url);
                    var response = await httpClient.GetAsync(Url);

                    if (response.Content == null) {
                        result.IsSuccessful = false;
                        result.Error = new ProcessingError("An error occurred calling NevadaSolix API (No response content)", "An error occurred while checking eligibility with the Nevada system. Please try again, if error persists contact support", true);
                        return result;
                    }
                    try {
                        var content = await response.Content.ReadAsStringAsync();
                        content = content.Trim();
                        FinishLogEntry(logEntry, content);
                        result.Data = new LS.Domain.ExternalApiIntegration.NevadaSolixEligibilityResponseData();
                        if (response.StatusCode == HttpStatusCode.Unauthorized) {
                            result.IsSuccessful = false;
                            result.Error = new ProcessingError("An error occurred calling NevadaSolix API (Unauthorized)", "An error occurred while checking eligibility with the Nevada system. Please try again, if error persists contact support", false, true);
                            return result;
                        }

                        if (!response.IsSuccessStatusCode) {
                            result.IsSuccessful = false;
                            result.Error = new ProcessingError("An error occurred calling NevadaSolix API (Status Code: "+ response.StatusCode + ")", "An error occurred while checking eligibility with the Nevada system. Please try again, if error persists contact support", true);
                            return result;
                        }

                        var solixResponse= JsonConvert.DeserializeObject< LS.Domain.ExternalApiIntegration.NevadaSolixEligibilityResponseData> (content);
                        result.IsSuccessful = true;
                        result.Data = solixResponse;
                        return result;
                    }
                    catch(Exception ex2) {
                        Logger.Fatal("Error making call to Nevada Solix", ex2);
                    }
                   
                }
            }
            catch (Exception ex1)
            {
                Logger.Error("Failed to read NevadaSolix REST API Check response", ex1);
                result.IsSuccessful = false;
                result.Error = new ProcessingError("NevadaSolix API Check Failed - Reason:" + ex1.Message, "NevadaSolix API Check Failed.Reason:" + ex1.Message, true, false);
                return result;
            }

            return result;
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
        private static void FinishLogEntry(ApiLogEntry logEntry, string content)
        {
            logEntry.Response = content;
            logEntry.DateEnded = DateTime.UtcNow;

            var logEntryRepository = new ApiLogEntryRepository();
            logEntryRepository.Add(logEntry);
        }
        private static ApiLogEntry CreateInitialDHHSEligibleApiLogEntry(string input)
        {
            return new ApiLogEntry
            {
                Api = ApiName,
                DateStarted = DateTime.UtcNow,
                Function = "DHHSEligible",
                Input = input
            };
        }
    }
}
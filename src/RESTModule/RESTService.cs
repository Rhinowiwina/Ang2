using Exceptionless;
using LS.Core;
using LS.Domain;
using LS.Repositories;
using LS.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RESTModule
{
    public class RESTAPIInit
    {
        public string BaseURL { get; set; }
        public string AuthUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ApiName { get; set; }
    }
    public class RESTAPIResult {
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
        public string APIResult { get; set; }
    }
    public class RESTService
    {
        private string BaseURL { get; set; }
        private string AuthUrl { get; set; } = "authenticate";
        private string Username { get; set; }
        private string Password { get; set; }
        private string ApiName { get; set; }

        public RESTService(RESTAPIInit model)
        {
            BaseURL = model.BaseURL;
            AuthUrl = model.AuthUrl;
            Username = model.Username;
            Password = model.Password;
            ApiName = model.ApiName;
        }

        private ApiLogEntry InitiateApiLogEntry(string functionName, string input)
        {
            return new ApiLogEntry {
                Api = ApiName,
                DateStarted = DateTime.UtcNow,
                Function = functionName,
                Input = input
            };
        }

        private string InsertLogEntry(ApiLogEntry logEntry, string content)
        {
            var logEntryRepository = new ApiLogEntryRepository();
            var addResult = logEntryRepository.Add(logEntry);
            return addResult.Data.Id;
        }
        private void UpdateLogEntry(ApiLogEntry logEntry)
        {
            logEntry.DateEnded = DateTime.UtcNow;
            var logEntryRepository = new ApiLogEntryRepository();
            logEntryRepository.Update(logEntry);
        }

        public async Task<ServiceProcessingResult<RESTAPIResult>> MakeRESTCall(string functionName, string endpoint, string actionType, string jsonRequestData)
        {
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(Username + ":" + Password));
            var result = new ServiceProcessingResult<RESTAPIResult> { IsSuccessful = true, Data = new RESTAPIResult() };
            var logEntry = InitiateApiLogEntry(functionName, jsonRequestData);
            var apiFunctionString = ApiName + " " + functionName;
            try {
                var cookieContainer = new CookieContainer();
                using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
                using (var httpClient = new HttpClient(handler) { BaseAddress = new Uri(BaseURL) }) {
                    httpClient.DefaultRequestHeaders.Add("Authorization", string.Format("Basic {0}", credentials));
                    logEntry.Id = InsertLogEntry(logEntry, "");
                    var apiResponse = new HttpResponseMessage();
                    if (actionType == "POST") {
                        apiResponse = await httpClient.PostAsync(endpoint, new StringContent(jsonRequestData, Encoding.UTF8, "application/json"));
                    } else {
                        apiResponse = await httpClient.GetAsync(endpoint);
                    }

                    if (apiResponse.StatusCode == HttpStatusCode.Unauthorized) {
                        result.IsSuccessful = false;
                        result.Error = new ProcessingError("An error occurred calling " + apiFunctionString + " API (Unauthorized). Please try again, if error persists contact support", "An error occurred calling " + apiFunctionString + " API (Unauthorized). Please try again, if error persists contact support", true, false);
                    } else if (apiResponse.StatusCode == HttpStatusCode.BadRequest) {
                        result.IsSuccessful = false;
                        result.Error = new ProcessingError("An error occurred calling " + apiFunctionString + " API (Bad request). Please try again, if error persists contact support", "An error occurred calling " + apiFunctionString + " API (Bad request). Please try again, if error persists contact support", true, false);
                    } else if (apiResponse.StatusCode == HttpStatusCode.InternalServerError) {
                        result.IsSuccessful = false;
                        result.Error = new ProcessingError("An error occurred calling " + apiFunctionString + " API (External API error). Please try again, if error persists contact support", "An error occurred calling " + apiFunctionString + " API (External API error). Please try again, if error persists contact support", true, false);
                    } else if (!apiResponse.IsSuccessStatusCode) {
                        result.IsSuccessful = false;
                        result.Error = new ProcessingError("An error occurred calling " + apiFunctionString + " API (Unsuccessful response). Please try again, if error persists contact support", "An error occurred calling " + apiFunctionString + " API (Unsuccessful Response). Please try again, if error persists contact support", true, false);
                    } else if (apiResponse.Content == null) {
                        result.Data.IsError = true;
                        result.Data.ErrorMessage = "An error occurred calling " + apiFunctionString + " API (No data). Please try again, if error persists contact support";
                    } else {
                        result.Data.APIResult = await apiResponse.Content.ReadAsStringAsync();
                        logEntry.Response = result.Data.APIResult = result.Data.APIResult.Trim();
                        if (result.Data.APIResult.Length == 0 || !Utils.IsJSON(result.Data.APIResult)) {
                            result.Data.IsError = true;
                            result.Data.ErrorMessage = "An error occurred calling " + apiFunctionString + " API (Invalid Response). Please try again, if error persists contact support";
                        }
                    }

                    UpdateLogEntry(logEntry);
                }
            } catch (WebException exception) {
                string responseText;

                using (var reader = new StreamReader(exception.Response.GetResponseStream())) {
                    responseText = reader.ReadToEnd();
                }
                exception.ToExceptionless()
                    .SetMessage("Error calling " + ApiName + " " + functionName + " - " + responseText)
                    .MarkAsCritical()
                    .Submit();
                result.IsSuccessful = false;
                result.Data.IsError = false;
                result.Error = new ProcessingError("Error calling " + ApiName + " " + functionName + " API - " + responseText, "Error calling " + ApiName + " " + functionName + " API - " + responseText, true, false);
                UpdateLogEntry(logEntry);
            } catch (Exception ex) {
                ex.ToExceptionless()
                   .SetMessage("Error calling " + ApiName + " " + functionName + " API")
                   .MarkAsCritical()
                   .Submit();
                result.IsSuccessful = false;
                result.Data.IsError = false;
                result.Error = new ProcessingError("Error calling " + ApiName + " " + functionName + " API", "Error calling " + ApiName + " " + functionName + " API", true, false);
            }

            return result;
        }
    }
}

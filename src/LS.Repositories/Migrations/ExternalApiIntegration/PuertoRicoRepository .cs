using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Common.Logging;
using LS.Core;
using LS.Domain;
using LS.Domain.ExternalApiIntegration;
using LS.Domain.ExternalApiIntegration.Interfaces;
using LS.Domain.ExternalApiIntegration.PuertoRico;
using LS.Utilities;
using Newtonsoft.Json;

namespace LS.Repositories.ExternalApiIntegration
{
    public class PuertoRicoRepository : ILifelineApplicationRepository
    {
        private ILog Logger { get; set; }
        private static readonly string VerifyEndpoint = "verify?";
        private static readonly string EnrollEndpoint = "subscriber";
        private static readonly string ApiName = "PuertoRico";
        private static readonly string TransferEndpoint = "transfer";
        private static readonly string SubmitResolutionRequestEndpoint = "resolution";

        private static readonly string UserName = ConfigurationSettings.AppSettings["PrUsername"].ToString();
        private static readonly string Password = ConfigurationSettings.AppSettings["PrPassword"].ToString();

        private static readonly Cookie PuertoRicoAuthorizationCookie = new Cookie("OBBasicAuth", "fromDialog");
        private const string AuthorizationType = "Basic";

        public PuertoRicoRepository()
        {
            Logger = LoggerFactory.GetLogger(GetType());
        }

        public async Task<DataAccessResult<ICheckStatusResponse>> CheckCustomerStatusAsync(ICheckStatusRequestData request)
        {
            var result = new DataAccessResult<ICheckStatusResponse>();

            result.Data.IsSuccessful = true;
            result.IsSuccessful = true;
            return result;
        }

        public async Task<DataAccessResult<ISubmitApplicationResponse>> SubmitApplicationAsync(ISubmitApplicationRequestData request) {
            var result = new DataAccessResult<ISubmitApplicationResponse>();

            result.Data.IsSuccessful = true;
            result.IsSuccessful = true;
            return result;
            }

        public async Task<DataAccessResult<ISubmitApplicationResponse>> SubmitTransferAsync(ISubmitApplicationRequestData request) {
            var result = new DataAccessResult<ISubmitApplicationResponse>();

            result.Data.IsSuccessful = true;
            result.IsSuccessful = true;
            return result;
            }

        public async Task<DataAccessResult<ISubmitApplicationResponse>> SubmitResolutionRequest(ResolutionRequestData requestData) {
            var result = new DataAccessResult<ISubmitApplicationResponse>();

            result.Data.IsSuccessful = true;
            result.IsSuccessful = true;
            return result;
            }

        private List<string> CreateErrorList(PuertoRicoApiErrorResponse puertoricoResponse) {
            var translatedErrorList = new List<string>();

            foreach (var errorList in puertoricoResponse.Body)
                {
                var foundTranslatedErrorCode = false;
                foreach (var singleErrorLine in errorList)
                    {
                    if (PuertoRicoErrorCodes.ExpectedErrorCodes.ContainsKey(singleErrorLine))
                        {
                        translatedErrorList.Add(PuertoRicoErrorCodes.ExpectedErrorCodes[singleErrorLine]);
                        foundTranslatedErrorCode = true;
                        }
                    }
                if (!foundTranslatedErrorCode)
                    {
                    var combinedError = errorList.Aggregate("",(current,errorLine) => current + (errorLine + " "));
                    Logger.Info("Unexpected error code returned from PuertoRico. " + combinedError);
                    translatedErrorList.Add(combinedError);
                    }
                }
            return translatedErrorList;
            }

        private static bool DoesErrorListIndicateTransferType(List<string> errorList)
        {
            return errorList.Any(e => e == PuertoRicoErrorCodes.ExpectedErrorCodes[PuertoRicoErrorCodes.DuplicateSubscriber]) &&
                   errorList.Any(e => e != PuertoRicoErrorCodes.ExpectedErrorCodes[PuertoRicoErrorCodes.InvalidTransfer]);
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
    }


}

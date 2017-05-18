using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using LS.Domain;
using LS.Repositories;
using LS.Domain.ExternalApiIntegration;

namespace LS.Services.Logging
{
    public class ValidateAddressWithSuiteLinkMessageInspector : IClientMessageInspector
    {
        private static string ApiName = "Service Objects";
        private ApiLogEntry _logEntry;

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var requestInput = request.ToString();
            _logEntry = CreateInitialLogEntry(requestInput);
            return null;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            var requestResponse = reply.ToString();
            FinishLogEntry(_logEntry, requestResponse);
        }

        private static ApiLogEntry CreateInitialLogEntry(string input)
        {
            return new ApiLogEntry
            {
                Api = ApiName,
                DateStarted = DateTime.UtcNow,
                Function = "ValidateAddressWithSuiteLink",
                Input = input
            };
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

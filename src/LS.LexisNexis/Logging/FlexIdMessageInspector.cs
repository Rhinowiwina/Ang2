using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using LS.Domain;
using LS.Domain.ExternalApiIntegration;

namespace LS.Repositories.ExternalApiIntegration.Logging
{
    public class FlexIdMessageInspector : IClientMessageInspector
    {
        private static readonly string ApiName = "LexisNexis";
        private ApiLogEntry _logEntry;

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var requestInput = request.ToString();
            _logEntry = CreateInitialSubmitLogEntry(requestInput);
            return null;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            var requestResponse = reply.ToString();
            FinishLogEntry(_logEntry, requestResponse);
        }

        private static ApiLogEntry CreateInitialSubmitLogEntry(string input)
        {
            return new ApiLogEntry
            {
                Api = ApiName,
                DateStarted = DateTime.UtcNow,
                Function = "FlexId",
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

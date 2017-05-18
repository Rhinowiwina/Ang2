using System;

namespace LS.Core
{
    public class APIError
    {
        public string UserMessage { get; set; }
        public string DeveloperMessage { get; set; }

        private APIError() {

        }

        public APIError(string developerMessage, string userMessage)
        {
            DeveloperMessage = developerMessage;
            UserMessage = userMessage;
        }

        public static implicit operator string (APIError v)
        {
            throw new NotImplementedException();
        }
    }
}

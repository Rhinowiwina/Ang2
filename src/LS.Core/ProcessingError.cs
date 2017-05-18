using System;

namespace LS.Core
{
    public class ProcessingError
    {
        public string UserMessage { get; set; }
        public string UserHelp { get; set; }
        public bool IsFatal { get; set; }
        public bool CanBeFixedByUser { get; set; }

        private ProcessingError() {

        }

        public ProcessingError(string userMessage, string userHelp, bool isFatal, bool canBeFixedByUser = false)
        {
            UserMessage = userMessage;
            UserHelp = userHelp;
            IsFatal = isFatal;
            CanBeFixedByUser = canBeFixedByUser;
        }

        public static implicit operator string (ProcessingError v)
        {
            throw new NotImplementedException();
        }
    }
}

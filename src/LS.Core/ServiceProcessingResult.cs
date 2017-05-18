using System.Collections.Generic;

namespace LS.Core
{
    public class ServiceProcessingResult
    {
        public bool IsSuccessful { get; set; }
        public ProcessingError Error { get; set; }
        public string WarningMessage { get; set; }
        public string SuccessMessage { get; set; }

        public bool IsFatalFailure()
        {
            return !IsSuccessful && Error.IsFatal;
        }
    }

    public class ServiceProcessingResult<TReturnedData> : ServiceProcessingResult
    {
        public TReturnedData Data { get; set; }
    }
}

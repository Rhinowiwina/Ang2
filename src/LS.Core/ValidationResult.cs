using System.Collections.Generic;
using System.Text;

namespace LS.Core
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }

        public List<string> Errors { get; set; }

        public ValidationResult()
        {
            Errors = new List<string>();
        }

        public virtual ProcessingError ToProcessingError(string userMessage)
        {
            var errorsBuilder = new StringBuilder();
            Errors.ForEach(error => errorsBuilder.AppendLine(error));

            return new ProcessingError(userMessage, errorsBuilder.ToString(), false, true);
        }
    }

    public class ValidationResult<TValidationData> : ValidationResult
    {
        public TValidationData Data { get; set; }
    }
}

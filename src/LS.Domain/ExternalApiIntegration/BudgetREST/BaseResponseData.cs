using Newtonsoft.Json;

namespace LS.Domain.ExternalApiIntegration.BudgetREST
{
    public class BaseResponseData
    {
        [JsonProperty(PropertyName = "IsError")]
        public bool IsError { get; set; }

        [JsonProperty(PropertyName = "ErrorMessage")]
        public string ErrorMessage { get; set; }

        [JsonProperty(PropertyName = "ErrorCode")]
        public string ErrorCode { get; set; }
    }
}

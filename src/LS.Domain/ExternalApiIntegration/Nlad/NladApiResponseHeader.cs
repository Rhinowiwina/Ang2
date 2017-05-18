using Newtonsoft.Json;

namespace LS.Domain.ExternalApiIntegration.Nlad
{
    public class NladApiResponseHeader
    {
        [JsonProperty(PropertyName = "failureType")]
        public string FailureType { get; set; }

        [JsonProperty(PropertyName = "resolutionId")]
        public string ResolutionId { get; set; }
    }
}

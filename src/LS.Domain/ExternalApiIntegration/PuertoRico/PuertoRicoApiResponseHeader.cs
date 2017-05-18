using Newtonsoft.Json;

namespace LS.Domain.ExternalApiIntegration.PuertoRico
{
    public class PuertoRicoApiResponseHeader
    {
        [JsonProperty(PropertyName = "failureType")]
        public string FailureType { get; set; }

        [JsonProperty(PropertyName = "resolutionId")]
        public string ResolutionId { get; set; }
    }
}

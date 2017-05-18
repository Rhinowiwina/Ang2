using Newtonsoft.Json;

namespace LS.Domain.ExternalApiIntegration.Nlad
{
    public class ResolutionRequestData
    {
        [JsonProperty(PropertyName = "resolutionId")]
        public string ResolutionId { get; set; }

        [JsonProperty(PropertyName = "desc")]
        public string Description { get; set; }

        public string AssignedPhoneNumber { get; set; }
    }
}

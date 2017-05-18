using System.Collections.Generic;
using Newtonsoft.Json;

namespace LS.Domain.ExternalApiIntegration.PuertoRico
{
    public class PuertoRicoApiErrorResponse
    {
        public PuertoRicoApiResponseHeader  Header { get; set; }

        [JsonProperty(PropertyName = "body")]
        public List<List<string>> Body { get; set; }
    }
}

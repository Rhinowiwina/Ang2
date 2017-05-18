using System.Collections.Generic;
using Newtonsoft.Json;

namespace LS.Domain.ExternalApiIntegration.Nlad
{
    public class NladApiErrorResponse
    {
        public NladApiResponseHeader Header { get; set; }

        [JsonProperty(PropertyName = "body")]
        public List<List<string>> Body { get; set; }
    }
}

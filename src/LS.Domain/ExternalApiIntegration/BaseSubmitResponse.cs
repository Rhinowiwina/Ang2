using System.Collections.Generic;
using LS.Domain.ExternalApiIntegration.Interfaces;

namespace LS.Domain.ExternalApiIntegration
{
    public class BaseSubmitResponse : ISubmitApplicationResponse
    {
        public bool IsSuccessful { get; set; }
        public List<string> Errors { get; set; }
        public string AssignedPhoneNumber { get; set; }
        public bool TpivBypassAvailable { get; set; }
        new public string DocumentID { get; set; }
        public string TpivCode { get; set; }
        public string TpivBypassMessage { get; set; }
        public bool ServiceAddressIsRural { get; set; }
    }
}

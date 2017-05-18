using System.Collections.Generic;
using LS.Domain.ExternalApiIntegration.Interfaces;
using LS.Domain.ExternalApiIntegration.Nlad;

namespace LS.Domain.ExternalApiIntegration
{
    public class BaseCheckStatusResponse : ICheckStatusResponse
    {
        public bool IsSuccessful { get; set; }
        public EnrollmentType EnrollmentType { get; set; }
        public bool AddressBypassAvailable { get; set; }
        public List<string> Errors { get; set; }
        public bool TpivBypassAvailable { get; set; }
        public string TpivBypassFailureType { get; set; }
        public string AssignedPhoneNumber { get; set; }
        public bool ServiceAddressIsRural { get; set; }
        public bool Hoh { get; set; }
        public NLADRejections NLADRejections { get; set; }
    }
}

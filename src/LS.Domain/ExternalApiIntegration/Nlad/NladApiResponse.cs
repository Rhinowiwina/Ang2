using System.Collections.Generic;
using LS.Domain.ExternalApiIntegration.Interfaces;

namespace LS.Domain.ExternalApiIntegration.Nlad
{
    public class NladApiResponse : ICheckStatusResponse, ISubmitApplicationResponse
    {
        public bool IsSuccessful { get; set; }
        public EnrollmentType EnrollmentType { get; set; }
        public List<string> Errors { get; set; }
        public bool TpivBypassAvailable { get; set; }
        public bool AddressBypassAvailable { get; set; }
        public string TpivBypassFailureType { get; set; }
        public string DocumentID { get; set; }
        public string TpivCode { get; set; }
        public string TpivBypassMessage { get; set; }
        public string ValidatedAddressId { get; set; }
        public bool ServiceAddressIsRural { get; set; }
        public string AssignedPhoneNumber { get; set; }
        public string ResolutionId { get; set; }
        public bool Hoh { get; set; }
        public NLADRejections NLADRejections { get; set; }
    }
}

using System.Collections.Generic;
using LS.Domain.ExternalApiIntegration.Nlad;

namespace LS.Domain.ExternalApiIntegration.Interfaces
{
    public interface ICheckStatusResponse
    {
        bool IsSuccessful { get; set; }
        EnrollmentType EnrollmentType { get; set; }
        List<string> Errors { get; set; }
        bool TpivBypassAvailable { get; set; }
        bool AddressBypassAvailable { get; set; }
        string TpivBypassFailureType { get; set; }
        bool ServiceAddressIsRural { get; set; }
        bool Hoh { get; set; }    
        NLADRejections NLADRejections { get; set;}
    }
}

using System.Collections.Generic;

namespace LS.Domain.ExternalApiIntegration.Interfaces
{
    public interface ISubmitApplicationResponse
    {
        bool IsSuccessful { get; set; }

        List<string> Errors { get; set; }

        string DocumentID { get; set; }

        string AssignedPhoneNumber { get; set; }

        bool TpivBypassAvailable { get; set; }

        string TpivCode { get; set; }

        string TpivBypassMessage { get; set; }

        bool ServiceAddressIsRural { get; set; }
    }
}

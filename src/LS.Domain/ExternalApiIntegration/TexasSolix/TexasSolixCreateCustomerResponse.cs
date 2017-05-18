using System;
using LS.Domain.ExternalApiIntegration.Interfaces;

namespace LS.Domain.ExternalApiIntegration.TexasSolix
{
    public class TexasSolixCreateCustomerResponse
    {
        public string ConfirmationNumber { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string ExtensionData { get; set; }
        public bool SuccessStatus { get; set; }

    }
}

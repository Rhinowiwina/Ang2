using System;
using LS.Domain.ExternalApiIntegration.Interfaces;

namespace LS.Domain.ExternalApiIntegration
{
    public class NevadaSolixEligibilityResponseData 
    {
        public string Eligible { get; set; }//true/false
        public string StatusCode { get; set; } //1-Eligible 2-NotEligible 3-Validation Error 4-Exception
        public string Message { get; set; }

    }
}

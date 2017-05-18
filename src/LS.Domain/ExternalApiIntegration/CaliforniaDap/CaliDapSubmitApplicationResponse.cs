using System.Collections.Generic;

namespace LS.Domain.ExternalApiIntegration.CaliforniaDap
{
    public class CaliDapSubmitApplicationResponse : BaseSubmitResponse
    {
        public CaliDapSubmitApplicationResponse()
        {
            Errors = new List<string>();
        }
        public string LifelineEnrollmentID { get; set; }
        public string TenantAddressId { get; set; }
        public string StatusCode { get; set; }
        public string StatusDescription { get; set; }
    }
}

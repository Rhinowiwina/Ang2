namespace LS.Domain.ExternalApiIntegration.CaliforniaDap
{
    public class CaliDapCheckStatusResponse : BaseCheckStatusResponse
    {
        public string ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string StatusCode { get; set; }
        public string DocumentID { get; set; }
        public string StatusDescription { get; set; }
    }
}

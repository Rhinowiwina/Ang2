
namespace LS.Core.Interfaces.PayPal
{
    public class MassPaymentResult : IMassPaymentResult
    {
        public string CorrelationID { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDescription { get; set; }
        public bool IsPaymentSuccessful { get; set; }
    }
}

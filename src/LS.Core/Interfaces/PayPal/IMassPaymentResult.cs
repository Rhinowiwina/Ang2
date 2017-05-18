namespace LS.Core.Interfaces.PayPal
{
    public interface IMassPaymentResult
    {
        string CorrelationID { get; set; }
        string ErrorCode { get; set; }
        string ErrorMessage { get; set; }
        string ErrorDescription { get; set; }
        bool IsPaymentSuccessful { get; set; }
    }
}

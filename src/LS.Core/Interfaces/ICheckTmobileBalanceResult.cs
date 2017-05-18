namespace LS.Core.Interfaces
{
    public interface ICheckTmobileBalanceResult {
        int ErrorCode { get; set; }
        string ErrorMessage { get; set; }
        string ErrorDescription { get; set; }
        bool IsError { get; set; }
        string WirelessNumber { get; set; }
        int planBalance_Voice { get; set; }
        int planBalance_Text { get; set; }
        int topUpBalance_Voice { get; set; }
        int topUpBalance_Text { get; set; }
        string ServiceEndDate { get; set; }
        bool TopUpExpirationSet { get; set; }
        string TopUpExpiration { get; set; }
        string topUpBalance_Data { get; set; }
    }
}

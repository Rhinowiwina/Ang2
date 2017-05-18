using LS.Core.Interfaces;

namespace LS.Domain
{
    public class CheckTmobileBalanceResult : ICheckTmobileBalanceResult {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDescription { get; set; }
        public bool IsError { get; set; }
        public string WirelessNumber { get; set; }
        public int planBalance_Voice { get; set; }
        public int planBalance_Text { get; set; }
        public int topUpBalance_Voice { get; set; }
        public int topUpBalance_Text { get; set; }
        public string ServiceEndDate { get; set; }
        public bool TopUpExpirationSet { get; set; }
        public string TopUpExpiration { get; set; }
        public string topUpBalance_Data { get; set; }
    }
}

using LS.Core.Interfaces;

namespace LS.Domain
{
    public class CheckVerizonBalanceResult : ICheckVerizonBalanceResult {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDescription { get; set; }
        public bool IsError { get; set; }
        public string WirelessNumber { get; set; }

        public int PlanBalance_Voice { get; set; }
        public int PlanBalance_Text { get; set; }
        public string PlanBalance_Data { get; set; }
        public int PlanBalance_Combo_VS { get; set; }
        public int PlanBalance_MMS { get; set; }

        public int TopUpBalance_Voice { get; set; }
        public int TopUpBalance_Text { get; set; }
        public string TopUpBalance_Data { get; set; }
        public int TopUpBalance_Combo_VS { get; set; }
        public int TopUpBalance_MMS { get; set; }
        

        public string ServiceEndDate { get; set; }
        public bool TopUpExpirationSet { get; set; }
        public string TopUpExpiration { get; set; }

    }
}

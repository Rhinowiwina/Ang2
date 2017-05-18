namespace LS.Core.Interfaces
{
    public interface ICheckVerizonBalanceResult {
        int ErrorCode { get; set; }
        string ErrorMessage { get; set; }
        string ErrorDescription { get; set; }
        bool IsError { get; set; }
        string WirelessNumber { get; set; }

        int PlanBalance_Voice { get; set; }
        int PlanBalance_Text { get; set; }
        string PlanBalance_Data { get; set; }
        int PlanBalance_Combo_VS { get; set; }
        int PlanBalance_MMS { get; set; }

        int TopUpBalance_Voice { get; set; }
        int TopUpBalance_Text { get; set; }
        string TopUpBalance_Data { get; set; }
        int TopUpBalance_Combo_VS { get; set; }
        int TopUpBalance_MMS { get; set; }


        string ServiceEndDate { get; set; }
        bool TopUpExpirationSet { get; set; }
        string TopUpExpiration { get; set; }
    }
}

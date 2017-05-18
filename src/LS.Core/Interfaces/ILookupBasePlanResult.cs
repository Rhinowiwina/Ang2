namespace LS.Core.Interfaces
{
    public interface ILookupBasePlanResult {
        int ErrorCode { get; set; }
        string ErrorMessage { get; set; }
        string ErrorDescription { get; set; }
        bool IsError { get; set; }
        bool BasePlanDetailsAvailable { get; set; }
        string PlanDescription { get; set; }
        string Voice { get; set; }
        string Text { get; set; }
        string Data { get; set; }
    }
}

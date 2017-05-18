namespace LS.Core.Interfaces
{
    public interface IRecertify_BudgetMobileResult {
        int ErrorCode { get; set; }
        string ErrorMessage { get; set; }
        string ErrorDescription { get; set; }
        bool IsError { get; set; }
        string NewLifeLineExpiration { get; set; }
        string RecertificationCredits { get; set; }
        bool CommissionableRecertification { get; set; }
    }
}

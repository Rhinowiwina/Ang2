
namespace LS.Domain.ExternalApiIntegration.Interfaces.BudgetREST
{
    public class IValidateSIMResponse
    {
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorCode { get; set; }
    }
}

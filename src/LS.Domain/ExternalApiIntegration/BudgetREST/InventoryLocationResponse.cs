using LS.Domain.ExternalApiIntegration.Interfaces.BudgetREST;

namespace LS.Domain.ExternalApiIntegration.BudgetREST
{
    public class InventoryLocationResponse { 
        public string Location { get; set; }
        public bool IsError { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMesage { get; set; }
    }
}

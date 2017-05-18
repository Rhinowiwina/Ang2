using LS.Domain.ExternalApiIntegration.Interfaces.BudgetREST;

namespace LS.Domain.ExternalApiIntegration.BudgetREST
{
    public class OrderSaveResponse : IOrderSaveResponse
    {
        public string OrderID { get; set; }
    }
}

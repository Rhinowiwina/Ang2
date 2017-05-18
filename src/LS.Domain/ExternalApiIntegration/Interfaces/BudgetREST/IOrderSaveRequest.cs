namespace LS.Domain.ExternalApiIntegration.Interfaces.BudgetREST
{
    public class IOrderSaveRequest
    {
        public string OrderID { get; set; }
        public string LSDB { get; set; }
    }
}

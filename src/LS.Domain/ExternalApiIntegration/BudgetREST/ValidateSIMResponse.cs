using LS.Domain.ExternalApiIntegration.Interfaces.BudgetREST;

namespace LS.Domain.ExternalApiIntegration.BudgetREST
{
    public class ValidateSIMResponse : IValidateSIMResponse
    {
        public string IMSI { get; set; }
        public string ICCID { get; set; }
        public string OrderID { get; set; }

        public string SimID { get; set; }
        public string PUK { get; set; }
        public string PUK2 { get; set; }
        public string ADM2 { get; set; }
    }
}

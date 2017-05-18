using System.Collections.Generic;
namespace LS.Domain
{
    public class PlaceOrderDetails
    {
        public string BudgetMobileID { get; set; }
        public string TopUpProductID { get; set; }
        public string TopUpDesc { get; set; }
        public double Price { get; set; }
        public double OrderTotal { get; set; }
        public bool CCPayment { get; set; }
        public string TeamID { get; set; }
        public bool TaxesApply { get; set; }
        public bool PurchaseWithAccountCredits { get; set; }
        public List<TaxItemRow> TaxItems { get; set; }
    }
}

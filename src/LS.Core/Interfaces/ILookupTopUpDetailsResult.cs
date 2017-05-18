using System.Collections.Generic;
namespace LS.Core.Interfaces
{
    public interface ILookupTopUpDetailsResult {
        int ErrorCode { get; set; }
        string ErrorMessage { get; set; }
        string ErrorDescription { get; set; }
        bool IsError { get; set; }

        double OrderTotal { get; set; }
        bool TaxesApply { get; set; }
        string TopUpDescription { get; set; }
        double TopUpCost { get; set; }
        double CouponAmount { get; set; }

        List<ITaxItemRow> TaxItems { get; set; }
    }
}

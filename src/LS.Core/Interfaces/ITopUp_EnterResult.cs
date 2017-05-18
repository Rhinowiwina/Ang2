using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Core.Interfaces
{
    public interface ITopUp_EnterResult {
        int ErrorCode { get; set; }
        string ErrorMessage { get; set; }
        string ErrorDescription { get; set; }
        bool IsError { get; set; }
        int ReferenceID { get; set; }
        double OrderTotal { get; set; }
        string TopUpDescription { get; set; }
        double TopUpCost { get; set; }
        bool TaxesApply { get; set; }
        double CouponAmount { get; set; }
        List<ITaxItemRow> TaxItem { get; set; }
    }
}

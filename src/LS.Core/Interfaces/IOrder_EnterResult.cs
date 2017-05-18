using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Core.Interfaces
{
    public interface IOrder_EnterResult {
        bool IsError { get; set; }
        int ErrorCode { get; set; }
        string ErrorMessage { get; set; }
        string ErrorDescription { get; set; }
        int ReferenceID { get; set; }
        int AccountCreditsAwarded { get; set; }
        double OrderTotal { get; set; }
        string ServicePlanDescription { get; set; }
        double ServicePlanCost { get; set; }
        bool AdditionalChargesApply { get; set; }
        bool TaxesApply { get; set; }
        List<ITaxItemRow> TaxItems { get; set; }
        List<IAdditionalChargeRow> AdditionalCharges { get; set; }
    }
}

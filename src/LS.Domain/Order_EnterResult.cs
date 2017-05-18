using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LS.Core.Interfaces;

namespace LS.Domain
{
    public class Order_EnterResult : IOrder_EnterResult {
        public bool IsError { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDescription { get; set; }
        public int ReferenceID { get; set; }
        public int AccountCreditsAwarded { get; set; }
        public double OrderTotal { get; set; }
        public string ServicePlanDescription { get; set; }
        public double ServicePlanCost { get; set; }
        public bool AdditionalChargesApply { get; set; }
        public bool TaxesApply { get; set; }
    
        public List<ITaxItemRow> TaxItems { get; set; }
        public List<IAdditionalChargeRow> AdditionalCharges { get; set; }
    }
}

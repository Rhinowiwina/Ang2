using LS.Core.Interfaces;
using System.Collections.Generic;
using System;

namespace LS.Domain
{
    public class LookupTopUpDetailsResult : ILookupTopUpDetailsResult {
        public double OrderTotal { get; set; }
        public string TopUpDescription { get; set; }
        public double TopUpCost { get; set; }
        public bool TaxesApply { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDescription { get; set; }
        public bool IsError { get; set; }
        public double CouponAmount { get; set; }
        

        public LookupTopUpDetailsResult()
        {
            TaxItems = new List<ITaxItemRow>();
        }

        public List<ITaxItemRow> TaxItems { get; set; }    
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LS.Core.Interfaces;

namespace LS.Domain
{
    public class Order_CommitResult : IOrder_CommitResult {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDescription { get; set; }
        public bool IsError { get; set; }
        public int BudgetMobileID { get; set; }
        public string MDN { get; set; }
        public string ServiceStartDate { get; set; }
        public string ServiceEndDate { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Core.Interfaces
{
    public interface IOrder_CommitResult {
        bool IsError { get; set; }
        int ErrorCode { get; set; }
        string ErrorMessage { get; set; }
        string ErrorDescription { get; set; }
        int BudgetMobileID { get; set; }
        string MDN { get; set; }
        string ServiceStartDate { get; set; }
        string ServiceEndDate { get; set; }
    }
}

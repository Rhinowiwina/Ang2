using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Core.Interfaces
{
    public interface ITopUp_CommitResult {
        int ErrorCode { get; set; }
        string ErrorMessage { get; set; }
        string ErrorDescription { get; set; }
        bool IsError { get; set; }
        int TopUpID { get; set; }
    }
}

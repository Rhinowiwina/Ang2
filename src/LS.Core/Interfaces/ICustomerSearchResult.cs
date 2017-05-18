using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Core.Interfaces
{
    public interface ICustomerSearchResult
    {
        bool IsError { get; set; }
       int ErrorCode { get; set; }
        string ErrorMessage { get; set; }
        string ErrorDescription { get; set; }
        bool CustomerDetailsAvailable { get; set; }
        ICollection<ICustomerSearchDetailResult> CustomerDetails { get; set; }
    }
}

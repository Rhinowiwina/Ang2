using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Core.Interfaces
{
    public interface ILookupAvailableTopUp_ExistingResult {
        int ErrorCode { get; set; }
        string ErrorMessage { get; set; }
        string ErrorDescription { get; set; }
        bool IsError { get; set; }
        List<ITopUpProduct> TopUpProducts { get; set; }
        bool TopUpProductsAvailable { get; set; }
    }
}

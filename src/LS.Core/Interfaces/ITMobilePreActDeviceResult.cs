using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LS.Core.Interfaces
{
    public interface ITMobilePreActDeviceResult
    {
        string MDN { get; set; }
        short ProviderID { get; set; }
        bool Active { get; set; }
        bool IsError { get; set; }
        int ErrorCode { get; set; }
        string ErrorMessage { get; set; }
        string ErrorDescription { get; set; }
    }
}

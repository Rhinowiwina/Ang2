using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Core.Interfaces
{
    public interface IActivateSprintDeviceResult
    {
        int ErrorCode { get; set; }
        string ErrorMessage { get; set; }
        string ErrorDescription { get; set; }
        bool IsError { get; set; }
        string MDN { get; set; }
        string MSID { get; set; }
        string CSA { get; set; }
        string EffectiveDate { get; set; }
        string SerialNumber { get; set; }
        string MasterSubsidyLock { get; set; }
    }
}

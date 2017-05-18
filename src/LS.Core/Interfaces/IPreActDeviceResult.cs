using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LS.Core.Interfaces
{
    public interface IPreActDeviceResult
    {
        string MDN { get; set; }
        bool UpgradeHandset { get; set; }
        string VerizonActivationDate { get; set; }
        int ProviderID { get; set; }
        string HandsetPrice { get; set; }
        bool ComboPlanProvisioned { get; set; }

        bool IsError { get; set; }
        int ErrorCode { get; set; }
        string ErrorMessage { get; set; }
        string ErrorDescription { get; set; }
    }
}

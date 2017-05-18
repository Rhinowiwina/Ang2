using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Core.Interfaces
{
    public interface IActivateTMobileDeviceDetails
    {
        string OrderId { get; set; }
        string IMSI { get; set; }
        string ZIP { get; set; }
        int SpecialProvisioningCode { get; set; }
    }
}

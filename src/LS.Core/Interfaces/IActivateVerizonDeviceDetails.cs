using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Core.Interfaces
{
    public interface IActivateVerizonDeviceDetails
    {
        string OrderId { get; set; }
        string DeviceID {get; set;}
        string ZIP { get; set; }
        bool PreOrderActivation { get; set; }
        int SpecialProvisioningCode { get; set; }
    }
}

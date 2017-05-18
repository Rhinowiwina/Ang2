using LS.Core.Interfaces;

namespace LS.Domain
{
    public class ActivateVerizonDeviceDetails : IActivateVerizonDeviceDetails
    {
        public string OrderId { get; set; }
        public string DeviceID { get; set; }
        public string ZIP { get; set; }
        public bool PreOrderActivation { get; set; }
        public int SpecialProvisioningCode { get; set; }
    }
}

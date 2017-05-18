using LS.Core.Interfaces;

namespace LS.Domain
{
    public class ActivateSprintDeviceDetails : IActivateSprintDeviceDetails
    {
        public string OrderId { get; set; }
        public string DeviceId { get; set; }
        public string Zip { get; set; }
        public int SpecialProvisioningCode { get; set; }
    }
}

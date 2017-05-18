using LS.Core.Interfaces;

namespace LS.Domain
{
    public class ActivateTMobileDeviceDetails : IActivateTMobileDeviceDetails
    {
        public string OrderId { get; set; }
        public string IMSI { get; set; }
        public string ZIP { get; set; }
        public int SpecialProvisioningCode { get; set; }
    }
}

using LS.Core.Interfaces;

namespace LS.Domain {
    public class DeviceDetails : IDeviceDetails {
        public string Type { get; set; }
        public int DeviceId { get; set; }
        public int Category { get; set; }
        public double SuggestedRetailPrice { get; set; }
    }
}

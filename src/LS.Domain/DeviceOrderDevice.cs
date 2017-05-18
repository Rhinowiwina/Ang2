using System;
using LS.Core.Interfaces;

namespace LS.Domain {
    public class DeviceOrderDevice {
        public DeviceOrderDevice() {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string OrderID { get; set; }
        public string IMEI { get; set; }
        public string PartNumber { get; set; }
        public decimal ASGPrice { get; set; }
        public decimal AgentPrice { get; set; }
        public bool IsReturned { get; set; } = false;
    }
}

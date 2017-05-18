using System;
using LS.Core.Interfaces;

namespace LS.Domain {
    public class DeviceOrder {
        public DeviceOrder() {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string PONumber { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ShipDate { get; set; }
        public DateTime AgentDueDate { get; set; }
        public DateTime ASGDueDate { get; set; }
        public string Level1SalesGroupID { get; set; }
        public bool IsReturned { get; set; } = false;
    }
}

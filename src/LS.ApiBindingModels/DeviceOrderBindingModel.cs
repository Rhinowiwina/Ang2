using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LS.Domain;
using LS.Domain.ExternalApiIntegration.CGM;
using System;

namespace LS.ApiBindingModels
{

    public class DeviceOrderResponse
    {
        public string Id { get; set; }
        public string PONumber { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ShipDate { get; set; }
        public DateTime AgentDueDate { get; set; }
        public DateTime ASGDueDate { get; set; }
        public string Level1SalesGroupID { get; set; }
        public string Level1SalesGroupName { get; set; }
        public bool IsReturned { get; set; } = false;

    }
    public class DeviceOrderCreationBindingModel
    {
        public string PONumber { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ShipDate { get; set; }
        public DateTime AgentDueDate { get; set; }
        public DateTime ASGDueDate { get; set; }
        public string Level1SalesGroupID { get; set; }
        public string OrderArea { get; set; }
        public bool OrderReturnedIndicator { get; set; }
    }

    public class DeviceOrderUpdateBindingModel
    {
        public string Id { get; set; }
        public string OrderID { get; set; }
        public string PONumber { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ShipDate { get; set; }
        public DateTime AgentDueDate { get; set; }
        public DateTime ASGDueDate { get; set; }
        public string Level1SalesGroupID { get; set; }
        public string OrderArea { get; set; }
        public bool OrderReturnedIndicator { get; set; } = false;
    }

    public class UploadDevicesBindingModel
    {
        public string ClearOrders { get; set; }
        public string OrderID { get; set; }
    }

    public class TotalHandsets {
        public string Total { get; set; }
    }
}
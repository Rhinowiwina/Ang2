using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LS.Domain.ExternalApiIntegration.Interfaces.BudgetREST;

namespace LS.Domain
{
    public class ChangeOrderFulfillment : IChangeOrderFulfillment
    {
        public string FulfillmentType { get; set; }
        public string DeviceID { get; set; }
        public string DeviceModel { get; set; }
    }
}

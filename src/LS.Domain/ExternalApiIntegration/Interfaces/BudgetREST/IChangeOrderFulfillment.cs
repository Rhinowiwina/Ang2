using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LS.Domain.ExternalApiIntegration.Interfaces.BudgetREST
{
    public class IChangeOrderFulfillment
    {
        public string FulfillmentType { get; set; }
        public string DeviceID { get; set; }
        public string DeviceModel { get; set; }
    }
}

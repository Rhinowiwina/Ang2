using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Domain.ExternalApiIntegration.Interfaces.BudgetREST
{
    public class IInventoryCheckRequest
    {
        public string deviceid { get; set; }
        public string agentid { get; set; }
    }
}

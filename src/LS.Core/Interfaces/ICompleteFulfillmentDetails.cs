using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Core.Interfaces
{
    public class ICompleteFulfillmentDetails
    {
        public int BudgetMobileID { get; set; }
        public string DeviceID { get; set; }
        public string MSID { get; set; }
        public string OriginalCSA { get; set; }
        public int ProviderID { get; set; }
        public string IMSI { get; set; }
        public string IntraUser { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Domain.ExternalApiIntegration
{
    public class CaPreCheckMsg
    {
        public string dapAccountType { get; set; }
        public string cgmVelocityCheck { get; set; }
        public bool delayedFulfillmentQueue { get; set; }
        public string stopForVelocityFailureMsg { get; set; }
        public bool stopForVelocityFailure { get; set; }
    }
}

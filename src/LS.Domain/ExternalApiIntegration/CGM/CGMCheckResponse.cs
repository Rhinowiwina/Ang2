using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Domain.ExternalApiIntegration.CGM {
    public class CGMCheckResponse:BaseCGMResponse {
        public virtual ICollection<CGMSubscriberCheck> SubscriberCheck { get; set; }

        public string ExternalVelocityCheck { get; set; }
    }
}

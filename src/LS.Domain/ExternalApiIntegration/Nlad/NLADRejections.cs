using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Domain.ExternalApiIntegration.Nlad {
    public class NLADRejections {
        public bool InvalidAddress { get; set; }
        public bool FailedIdentity { get; set; }
        public bool FailedIdentityNameSSN { get; set; }
        public bool FailedIdentityDOB { get; set; }
        public bool DuplicateAddress { get; set; }
        public bool UnknownRejection { get; set; }
        public bool DuplicateSubscriber { get; set; }
        public bool CantTransfer { get; set; }
    }
}

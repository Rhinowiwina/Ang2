using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Domain.ExternalApiIntegration.CGM
{
    public class CGMEnrollResponse
    {
        public string TransactionId { get; set; }
        public string Status { get; set; }//Success Failure
        public string Message { get; set; }
    }
}

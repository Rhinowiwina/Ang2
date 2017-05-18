using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Domain.ExternalApiIntegration.CGM
{
   public class BaseCGMResponse
    {
        public Boolean Blacklist { get; set; }
        public string Transactionid { get; set; }
        public string Token { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string ValidationErrors { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Domain.ExternalApiIntegration.CGM
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public string ApiUrl { get; set; }
        public string Status { get; set; }
        public string ApiVersion { get; set; }
        public string Message { get; set; }
    }
}

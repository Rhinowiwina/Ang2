using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Domain.ExternalApiIntegration.BudgetREST
{
    public class OrderDeviceResponse
    {
        public string ErrorCode { get; set; }
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
    }
}
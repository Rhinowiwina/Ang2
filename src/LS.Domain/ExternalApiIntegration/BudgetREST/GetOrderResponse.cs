using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Domain.ExternalApiIntegration.BudgetREST
{
    public class GetOrderResponse
    {
        public string OrderID { get; set; }
        public string ExternalAccountID { get; set; }
        public bool IsError { get; set; }
        public string ErrorCode { get; set; }
        public int ErrorMessage { get; set; }
        public string DeviceIdentifier { get; set; }
        public string OrderStatus { get; set; }
        public DateTime? DateFulfilled { get; set; }
        public string RTR_Notes { get; set; }
        public bool CanFulfill { get; set; }
    }
}
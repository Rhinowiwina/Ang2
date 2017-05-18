using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LS.Core.Interfaces.PayPal;

namespace LS.Domain.PayPal
{
    public class MassPaymentRequestItem : IMassPaymentRequestItem
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public decimal Amount { get; set; }
    }
}

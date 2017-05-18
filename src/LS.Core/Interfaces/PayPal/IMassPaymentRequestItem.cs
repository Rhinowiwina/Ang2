using System.Collections.Generic;
using LS.Core.Interfaces.PayPal;

namespace LS.Core.Interfaces.PayPal
{
    public interface IMassPaymentRequestItem
    {
        string Id { get; set; }
        string Email { get; set; }
        decimal Amount { get; set; }
    }
}

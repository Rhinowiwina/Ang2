using System.Collections.Generic;
using LS.Core.Interfaces.PayPal;

namespace LS.Core.Interfaces.PayPal
{
    public interface IMassPaymentRequest
    {
        string ReceiverType { get; set; }
        string EmailSubject { get; set; }
        List<IMassPaymentRequestItem> Payments { get; set; }
    }
}

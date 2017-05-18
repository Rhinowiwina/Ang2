using LS.Core.Interfaces.PayPal;
using System.Collections.Generic;

namespace LS.Domain.PayPal
{
    public class MassPaymentRequest : IMassPaymentRequest
    {
        public string ReceiverType { get; set; }
        public string EmailSubject { get; set; }
        public List<IMassPaymentRequestItem> Payments { get; set; }
    }
}

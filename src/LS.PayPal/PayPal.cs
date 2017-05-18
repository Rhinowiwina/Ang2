using System;
using System.Configuration;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LS.Core;
using LS.Domain;
using LS.Core.Interfaces.PayPal;
using LS.PayPal.PayPalSandBox;
using System.Collections.Generic;
using LS.PayPal.Logging;
using Exceptionless;
using Exceptionless.Models;
namespace LS.PayPal
{
    public class PayPalService
    {
        public async Task<ServiceProcessingResult<IMassPaymentResult>> MassPayment(IMassPaymentRequest massPayRequest)
        {
            var result = new ServiceProcessingResult<IMassPaymentResult> { };
            try
            {
                var service = new PayPalAPIInterfaceClient();

                var customSecurityHeaderType = new CustomSecurityHeaderType();
                var myCreds = new UserIdPasswordType()
                {
                    Username = ConfigurationManager.AppSettings["PayPalUsername"],
                    Password = ConfigurationManager.AppSettings["PayPalPassword"],
                    Signature = ConfigurationManager.AppSettings["PayPalSigniture"]
                };

                customSecurityHeaderType.Credentials = myCreds;

                var massPayReq = new MassPayReq();

                var myMassPayRequest = new MassPayRequestType()
                {
                    EmailSubject = massPayRequest.EmailSubject,
                    ReceiverType = ReceiverInfoCodeType.EmailAddress,
                    Version = "65.1"
                };

                massPayReq.MassPayRequest = myMassPayRequest;

                var paymentItemList = new List<MassPayRequestItemType>();

                foreach (var payment in massPayRequest.Payments)
                {

                    var currencyCodeType = CurrencyCodeType.USD;
                    var basicAmountType = new BasicAmountType()
                    {
                        currencyID = currencyCodeType,
                        Value = payment.Amount.ToString() //May need to NumberFormat this to "0.00"
                    };

                    var massPayReqType = new MassPayRequestItemType()
                    {
                        Amount = basicAmountType,
                        ReceiverEmail = payment.Email,
                        UniqueId = payment.Id.Substring(0, 30)
                    };

                    paymentItemList.Add(massPayReqType);

                }


                massPayReq.MassPayRequest.MassPayItem = paymentItemList.ToArray();

                try
                {
                    service.Endpoint.Behaviors.Add(new MassPayInspectorBehavior());
                    var serviceResult = service.MassPay(ref customSecurityHeaderType, massPayReq);

                    if (serviceResult.Ack == AckCodeType.Success)
                    {
                        result.IsSuccessful = true;
                        result.Data = new MassPaymentResult();
                        result.Data.IsPaymentSuccessful = true;
                        result.Data.CorrelationID = serviceResult.CorrelationID;
                        return result;
                    } else {
                        result.IsSuccessful = true;
                        result.Data = new MassPaymentResult();
                        result.Data.IsPaymentSuccessful = false;
                        result.Data.ErrorMessage = serviceResult.Errors[0].ShortMessage;
                        result.Data.ErrorDescription = serviceResult.Errors[0].LongMessage;
                        result.Data.ErrorCode = serviceResult.Errors[0].ErrorCode;
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    ex.ToExceptionless()
                    .SetMessage("An error occurred while attempting the PayPal API call")
                    .MarkAsCritical()
                    .Submit();
                    //Send Email
                    result.IsSuccessful = false;
                    result.Error = new ProcessingError("An error occurred while attempting the PayPal API call", "An error occurred while attempting the PayPal API call", true, false);
                    return result;
                }
            }
            catch (Exception ex)
            {
                ex.ToExceptionless()
                    .SetMessage("An error occurred while attempting to setup PayPal mass payment")
                    .MarkAsCritical()
                    .Submit();
                result.Error = new ProcessingError("An error occurred while attempting to setup PayPal mass payment", "An error occurred while attempting to setup PayPal mass payment", true, false);
                result.IsSuccessful = false;
                return result;
            }

            return result;
        }
    }
}

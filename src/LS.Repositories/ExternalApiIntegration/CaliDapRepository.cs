using System;
using System.Threading.Tasks;
using System.Configuration;
using Common.Logging;
using System.Configuration;
using LS.Core;
using LS.Domain.ExternalApiIntegration.CaliforniaDap;
using LS.Domain.ExternalApiIntegration.Interfaces;
using LS.Repositories.CaliforniaLifelineDireactApplicationService;
using LS.Repositories.ExternalApiIntegration.Logging;
using LS.Utilities;

namespace LS.Repositories.ExternalApiIntegration
{
    public class CaliDapRepository : ILifelineApplicationRepository
    {
        private static readonly string ApiName = "CaliDap";

        //Todo: Tenant-Specific refactor
        private static readonly string Ocn = ConfigurationSettings.AppSettings["DAPOcn"].ToString();
        private static readonly string UserName = ConfigurationSettings.AppSettings["DAPUsername"].ToString();
        private static readonly string Password = ConfigurationSettings.AppSettings["DAPPassword"].ToString();

        private ILog Logger { get; set; }

        public CaliDapRepository()
        {
            Logger = LoggerFactory.GetLogger(GetType());
        }

        public async Task<DataAccessResult<ICheckStatusResponse>> CheckCustomerStatusAsync(ICheckStatusRequestData request)
        {
            var dataAccessResult = new DataAccessResult<ICheckStatusResponse> {IsSuccessful = true};

            try
            {
                var client = new DirectApplicationWrapperServiceSoapClient();
                client.Endpoint.Behaviors.Add(new CaliDapCheckStatusInspectorBehavior());

                var header = new DirectApplicationRequestSoapHeader
                {
                    Ocn = Ocn,
                    Username = UserName,
                    Password = Password,
                    //Todo: This is the OrderId from original app. 
                    TransactionId = "1"
                };

                try {
                    var response = await client.CheckCustomerStatusAsync(header, header.Ocn, CaliDapRequestConstants.AssignedTelephoneNumber, CaliDapRequestConstants.NamePrefix,
                                request.FirstName, request.MiddleInitial, request.LastName, CaliDapRequestConstants.NameSuffix,
                                request.ServiceAddress1, request.ServiceAddress2, request.ServiceAddressCity,
                                request.ServiceAddressState, request.ServiceAddressZip5, request.ServiceAddressZip4,
                                request.PriorULTSTelephoneNumber,
                                CaliDapRequestConstants.TtyQualifiedSecondLine,
                                CaliDapRequestConstants.ActivityType, request.Ssn4, request.DateOfBirth);
                    var responseObject = response.ToCaliforniaDapCheckStatusResponseData();

                    dataAccessResult.Data = responseObject;

                    return dataAccessResult;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    dataAccessResult.Error =new ProcessingError("Communication error with Californina DAP.","Communication error with Californina DAP.",true,false);
                    dataAccessResult.IsSuccessful = false;

                    return dataAccessResult;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                dataAccessResult.Error = ErrorValues.GENERIC_EXTERNAL_API_RESPONSE_ERROR;
                dataAccessResult.IsSuccessful = false;

                return dataAccessResult;
            }
        }

        //public async Task<DataAccessResult<ISubmitApplicationResponse>> SubmitApplicationAsync(ISubmitApplicationRequestData request)
        //{
        //    var dataAccessResult = new DataAccessResult<ISubmitApplicationResponse> { IsSuccessful = true };
        //    try
        //    {
        //        var client = new DirectApplicationWrapperServiceSoapClient();
        //        client.Endpoint.Behaviors.Add(new CaliDapSubmitInspectorBehavior());

        //        var header = new DirectApplicationRequestSoapHeader
        //        {
        //            Ocn = Ocn,
        //            Username = UserName,
        //            Password = Password,
        //            //Todo: This is the OrderId from original app. 
        //            TransactionId = "1"
        //        };

        //        var response = await client.DirectApplicationRequestAsync(header, header.Ocn, request.SubscriberAccountNumber,
        //                    CaliDapRequestConstants.CustomerCode, request.AssignedTelephoneNumber, CaliDapRequestConstants.NamePrefix,
        //                    request.FirstName, request.MiddleInitial, request.LastName, CaliDapRequestConstants.NameSuffix,
        //                    request.ServiceAddress1, request.ServiceAddress1, request.ServiceAddressCity,
        //                    request.ServiceAddressState, request.ServiceAddressZip5, request.ServiceAddressZip4,
        //                    request.BillingFirstName, request.BillingMiddleInitial, request.BillingLastName,
        //                    request.BillingAddress1, request.BillingAddress2,
        //                    CaliDapRequestConstants.BillingAddress3,
        //                    CaliDapRequestConstants.BillingAddress4,
        //                    CaliDapRequestConstants.BillingAddress5, request.BillingCity, request.BillingState,
        //                    request.BillingZip5, request.BillingZip4, request.ContactPhoneNumber,
        //                    CaliDapRequestConstants.LanguageCode,
        //                    CaliDapRequestConstants.BrailleLargeFont, request.UltsServiceStartDate,
        //                    CaliDapRequestConstants.UltsRecertificationDate,
        //                    request.PriorULTSTelephoneNumber,
        //                    CaliDapRequestConstants.DisconnectDate,
        //                    CaliDapRequestConstants.TtyQualifiedSecondLine, request.DriversLicenseNumber,
        //                    CaliDapRequestConstants.ActivityType, CaliDapRequestConstants.TribalLand,
        //                    CaliDapRequestConstants.ServiceType, CaliDapRequestConstants.RateGroup,
        //                    CaliDapRequestConstants.ModifiedEffectiveDate,
        //                    CaliDapRequestConstants.DirectApplication, request.Ssn4, request.DateOfBirth);

        //        var responseObject = response.ToCaliforniaDapSubmitApplicationResponseData();

        //        dataAccessResult.Data = responseObject;
        //        dataAccessResult.Data.AssignedPhoneNumber = request.AssignedTelephoneNumber;
        //        return dataAccessResult;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error(ex);
        //        dataAccessResult.Error = ErrorValues.GENERIC_EXTERNAL_API_RESPONSE_ERROR;
        //        dataAccessResult.IsSuccessful = false;

        //        return dataAccessResult;
        //    }
        //}
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Common.Logging;
using LS.Domain;
using LS.Domain.ExternalApiIntegration.CaliforniaDap;
using LS.Domain.ExternalApiIntegration.Interfaces;
using LS.Repositories.CaliforniaLifelineDireactApplicationService;
using LS.Utilities;

namespace LS.Repositories.ExternalApiIntegration
{
    public static class LifelineApplicationConverters
    {
        private static readonly string ResponseIdentifier = "Response";
        private static readonly string StatusCodeIdentifier = "StatusCode";
        private static readonly string LifelineDBIDIdentifier = "DocumentID";
        private static readonly string StatusDescriptionIdentifier = "StatusDescription";
        private static readonly string ErrorsIdentifier = "Errors";
        private static readonly string ErrorIdentifier = "Error";
        private static readonly string ErrorCodeIdentifier = "ErrorCode";
        private static readonly string ErrorDescriptionIdentifier = "ErrorDescription";
        private static readonly string CustomerStatusResponseIdentifier = "CustomerStatusResponse";

        private static readonly ILog Logger = LoggerFactory.GetLogger(typeof(LifelineApplicationConverters));

        public static ICheckStatusResponse ToCaliforniaDapCheckStatusResponseData(
            this CheckCustomerStatusResponse response)
        {
            try
            {
                var xDoc = XDocument.Parse(response.CheckCustomerStatusResult);

                var rows = from x in xDoc.Descendants(CustomerStatusResponseIdentifier)
                           select new
                           {
                               ErrorCode = (string)x.Element(ErrorCodeIdentifier),
                               ErrorDescription = (string)x.Element(ErrorDescriptionIdentifier),
                               StatusCode = (string)x.Element(StatusCodeIdentifier),
                               StatusDescription = (string)x.Element(StatusDescriptionIdentifier)
                           };

                var row = rows.ToList()[0];

                var responseData = new CaliDapCheckStatusResponse
                {
                    ErrorCode = row.ErrorCode,
                    ErrorDescription = row.ErrorDescription,
                    StatusCode = row.StatusCode,
                    StatusDescription = row.StatusDescription
                };

                if (responseData.StatusCode == "0") {
                    responseData.EnrollmentType = EnrollmentType.New;
                } else if (responseData.StatusCode == "1") {
                    responseData.EnrollmentType = EnrollmentType.Reconnect;
                    responseData.ErrorCode = "ReconnectCustomer";
                    responseData.ErrorDescription = "This customer currently has Lifeline service and needs to be reconnected.";
                } else if (responseData.StatusCode == "2" || responseData.StatusCode == "4") {
                    responseData.EnrollmentType = EnrollmentType.Transfer;
                } else if (responseData.StatusCode == "3") {
                    responseData.EnrollmentType = EnrollmentType.Existing;
                    responseData.ErrorCode = "ExistingCustomer";
                    responseData.ErrorDescription = "This customer is currently a Budget customer and cannot sign up for an additional account.";
                } else {
                    responseData.EnrollmentType = EnrollmentType.Invalid;
                    if (responseData.ErrorCode == "" || responseData.ErrorCode == null) {
                        responseData.ErrorCode = "UnknownError";
                        responseData.ErrorDescription = "There was an error verifying this customer with California. (Status Code: " + responseData.StatusCode + ")";
                    }
                }

                if (responseData.ErrorDescription != "" && responseData.ErrorDescription != null)
                {
                    responseData.Errors = new List<string> { responseData.ErrorDescription };
                }

                return responseData;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occurred while parsing the xml from CaliDap Check Status Response", ex);

                // TODO: Figure out what a default response should look like, and create and return that here
                return new CaliDapCheckStatusResponse();
            }

        }

        public static CaliDapSubmitApplicationResponse ToCaliforniaDapSubmitApplicationResponseData(this DirectApplicationRequestResponse response)
        {
            try
            {
                var xDoc = XDocument.Parse(response.DirectApplicationRequestResult);

                var statusRows = from x in xDoc.Descendants(ResponseIdentifier)
                                 select new
                                 {
                                     DocumentID = (string)x.Element(LifelineDBIDIdentifier),
                                     StatusCode = (string)x.Element(StatusCodeIdentifier),
                                     StatusDescription = (string)x.Element(StatusDescriptionIdentifier)
                                 };

                var status = statusRows.ToList()[0];

                var returnObject = new CaliDapSubmitApplicationResponse
                {
                    DocumentID = status.DocumentID,
                    StatusCode = status.StatusCode,
                    StatusDescription = status.StatusDescription
                };

                var errors = xDoc.Descendants(ErrorsIdentifier);
                var errorRows = from x in errors.Descendants(ErrorIdentifier)
                               select new
                               {
                                   ErrorCode = (string)x.Element(ErrorCodeIdentifier),
                                   ErrorDescription = (string)x.Element(ErrorDescriptionIdentifier)
                               };
         
                foreach (var errorElement in errorRows.Where(errorElement => !string.IsNullOrEmpty(errorElement.ErrorDescription)))
                {
                    returnObject.Errors.Add(errorElement.ErrorDescription);
                }

                return returnObject;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occurred while parsing the xml from CaliDap Submit Application Response", ex);
                // TODO: Figure out what a default response should look like, and create and return that here
            }

            return new CaliDapSubmitApplicationResponse();
        }
    }
}

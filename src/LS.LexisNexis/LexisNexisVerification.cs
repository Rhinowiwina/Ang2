using System;
using System.Configuration;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common.Logging;
using LS.Core;
using LS.Domain;
using LS.LexisNexis.LexisNexisService;
using LS.LexisNexis.Logging;
using LS.Utilities;
using System.Collections.Generic;
using Exceptionless;
using Exceptionless.Models;
namespace LS.LexisNexis {
    public class LexisNexisVerification {
        protected ILog Logger { get; set; }

        public LexisNexisVerification() {
            Logger = LoggerFactory.GetLogger(GetType());
        }
        public ServiceProcessingResult<LexisNexisResponseDto> Verify(IOrder order) {
            DateTime dateOfBirth;
            DateTime.TryParse(order.UnencryptedDateOfBirth, out dateOfBirth); // TODO: handle inability to convert to DateTime
            return Verify(order.FirstName, order.LastName, order.ServiceAddressStreet1, order.ServiceAddressCity, order.ServiceAddressState, order.ServiceAddressZip, dateOfBirth, order.UnencryptedSsn, order.ContactPhoneNumber);
        }

        public ServiceProcessingResult<LexisNexisResponseDto> Verify(string firstName, string lastName, string streetAddress1, string city, string state, string zip, DateTime dob, string ssn, string homePhone) {
            var user = new User {
                ReferenceCode = "Budget Mobile",
                BillingCode = "Signup",
                GLBPurpose = "5",
                DLPurpose = "3"
            };

            var options = new FlexIDOption {
                IncludeAllRiskIndicators = true,
                DOBMatch = new DOBMatchOptions {
                    MatchType = DOBMatchType.FuzzyCCYYMMDD,
                    MatchYearRadius = 3,
                    MatchYearRadiusSpecified = true
                }
            };
            var searchBy = new FlexIDSearchBy {
                Name = new Name {
                    First = firstName,
                    Last = lastName
                },
                Address = new Address {
                    StreetAddress1 = streetAddress1,
                    City = city,
                    State = state,
                    Zip5 = zip
                },
                DOB = new Date {
                    Year = (short)dob.Year,
                    Month = (short)dob.Month,
                    Day = (short)dob.Day,
                    YearSpecified = true,
                    MonthSpecified = true,
                    DaySpecified = true
                }
            };
            if (ssn.Length > 4) {
                searchBy.SSN = ssn;
            } else {
                searchBy.SSNLast4 = ssn;
            }

            if (!string.IsNullOrEmpty(homePhone)) {
                homePhone = Regex.Replace(homePhone, "[^0-9]*", "");
                searchBy.HomePhone = homePhone;
            }
            return MakeCall(user, options, searchBy);
        }

        /// <summary>
        /// Used to prevent 2 POSTs for one request
        /// </summary>
        /// <param name="user"></param>
        /// <param name="options"></param>
        /// <param name="searchBy"></param>
        /// <returns></returns>
        private ServiceProcessingResult<LexisNexisResponseDto> MakeCall(User user, FlexIDOption options, FlexIDSearchBy searchBy) {
            var service = new WsIdentityServiceSoapClient();
            service.Endpoint.Behaviors.Add(new FlexIdInspectorBehavior());
            service.ClientCredentials.UserName.UserName = "BUDXML"; //ConfigurationManager.AppSettings["LexisNexisUserName"];
            service.ClientCredentials.UserName.Password = "Bu17Dt42"; //ConfigurationManager.AppSettings["LexisNexisPassword"];
            HttpRequestMessageProperty httpRequestProperty = new HttpRequestMessageProperty();
            httpRequestProperty.Headers[HttpRequestHeader.Authorization] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(service.ClientCredentials.UserName.UserName + ":" + service.ClientCredentials.UserName.Password));
            try {
                using (OperationContextScope scope = new OperationContextScope(service.InnerChannel)) {
                    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;
                    var result = new ServiceProcessingResult<LexisNexisResponseDto>();
                    var response = service.FlexID(user, options, searchBy);
                    // TODO: See if we can make this reusable so we don't just make this one call
                    var summary = response.Result.VerifiedElementSummary;
                    //response.Header.TransactionId

                    var riskIndicatorList = new List<RiskIndicatorDto>();

                    foreach (var risk in response.Result.ComprehensiveVerification.RiskIndicators) {
                        var riskIndicatorConverted = new RiskIndicatorDto {
                           RiskCode = risk.RiskCode,
                           Description = risk.Description,
                           Sequence = risk.Sequence
                        };
                        riskIndicatorList.Add(riskIndicatorConverted);
                    }

                    result.IsSuccessful = true;
                    result.Data = new LexisNexisResponseDto {
                        NameAddressSSN = response.Result.NameAddressSSNSummary,
                        DOB = summary.DOB,
                        DOBMatchLevel = summary.DOBMatchLevel,
                        LexId = response.Result.UniqueId,
                        TransactionID = response.Header.TransactionId,
                        RiskIndicators = riskIndicatorList
                    };

                    return result;
                }
            } catch (Exception ex) {
                ex.ToExceptionless()
                  .SetMessage("An error occurred while calling the LexisNexis service")
                  .MarkAsCritical()
                  .Submit();
                //Logger.Error("An error occurred while calling the LexisNexis service", ex);
                return new ServiceProcessingResult<LexisNexisResponseDto> {
                    IsSuccessful = false,
                    Data = null,
                    Error = new ProcessingError("Failed to contact verification services", "Failed to verify user details", true, false)
                };
            }
        }
    }
}

using System;
using System.Configuration;
using System.Threading.Tasks;
using Common.Logging;
using LS.Core;
using LS.Domain;
using LS.Domain.ExternalApiIntegration.Interfaces;
using LS.Domain.ExternalApiIntegration.TexasSolix;
using LS.Repositories.ExternalApiIntegration.Logging;
using LS.Repositories.TexasSolix;
using LS.Utilities;
using Numero3.EntityFramework.Implementation;
using System.Collections.Generic;
using LS.Domain.ExternalApiIntegration;

namespace LS.Repositories.ExternalApiIntegration {
    public class TexasSolixRepository : ILifelineApplicationRepository {
        private static readonly string ApiName = "TexasSolix";
        private static readonly string UserName = ConfigurationSettings.AppSettings["TexasSolixUserName"];
        private static readonly string Password = ConfigurationSettings.AppSettings["TexasSolixPassword"];
        private static readonly string Login = ConfigurationSettings.AppSettings["TexasSolixUserLogin"];

        private ILog Logger { get; set; }

        public TexasSolixRepository() {
            Logger = LoggerFactory.GetLogger(GetType());
        }

        public async Task<DataAccessResult<ICheckStatusResponse>> CheckCustomerStatusAsync(ICheckStatusRequestData request) {
            var dataAccessResult = new DataAccessResult<ICheckStatusResponse> { IsSuccessful = true };
            dataAccessResult.Data = new TexasSolixCheckStatusResponse();
            dataAccessResult.Data.Errors = new List<string>();

            try {
                var client = new HHSCClient();
                client.Endpoint.Behaviors.Add(new TexasSolixCheckStatusInspectorBehavior());

                var response = await client.VerifyAsync(request.LastName, request.Ssn4, request.DateOfBirth, request.ServiceAddressZip5.Substring(0, 5), UserName, Password, Login);
                if (response.ErrorCode != null && response.ErrorCode != "0") {
                    dataAccessResult.IsSuccessful = false;
                    dataAccessResult.Error = new ProcessingError("Unable to contact the Solix system for verification", "Unable to contact the Solix system for verification", true, false);
                    Logger.Error(response.ErrorDescription);
                    return dataAccessResult;
                }

                var dbContextScopeFactory = new DbContextScopeFactory();

                using (var dbContextScope = dbContextScopeFactory.Create()) {
                    var lifelineProgramRepo = new LifelineProgramRepository();
                    var lifelineProgramResult =
                        await lifelineProgramRepo.GetWhereAsync(lp => lp.Id == request.LifelineProgramId);

                    var lifelineProgram = lifelineProgramResult.Data.ProgramName;

                    if (lifelineProgram == "Medicaid" || lifelineProgram == "Food Stamps" || lifelineProgram == "Temporary Assistance for Needy Families") {
                        if (response.EligibleForDiscount.ToString() == "Not-Eligible" && response.ReserveStatus.ToString() == "Cannot Reserve") {
                            var createCustomerRequestData = new TexasSolixCreateCustomerRequestData() {
                                LastName = request.LastName,
                                FirstName = request.FirstName,
                                Ssn4 = request.Ssn4,
                                DateOfBirth = request.DateOfBirth,
                                ServiceAddress1 = request.ServiceAddress1,
                                ServiceAddress2 = request.ServiceAddress2,
                                ServiceAddressCity = request.ServiceAddressCity,
                                ServiceAddressState = request.ServiceAddressState,
                                ServiceAddressZip4 = request.ServiceAddressZip4,
                                ServiceAddressZip5 = request.ServiceAddressZip5,
                                ConfirmationNumber = response.ConfirmationNumber
                            };

                            var createCustomer = await CreateCustomer(createCustomerRequestData);

                            if (createCustomer.IsSuccessful && createCustomer.Data.SuccessStatus) {
                                // Assume that we are good to go. This is where the confirmation number would be set.
                                dataAccessResult.Data.EnrollmentType = EnrollmentType.New;
                            } else {
                                dataAccessResult.IsSuccessful = true;
                                dataAccessResult.Data.Errors.Add("Solix: This person could not be created in the Solix system.");

                                return dataAccessResult;
                            }
                        } else if (response.EligibleForDiscount.ToString() == "Eligible") {
                            if (response.ReserveStatus.ToString() != "Can Reserve") {
                                dataAccessResult.IsSuccessful = true;
                                dataAccessResult.Data.Errors.Add("Solix: This person cannot be reserved.  Customer must contact Solix to dispute.");

                                return dataAccessResult;
                            }

                            dataAccessResult.Data.EnrollmentType = EnrollmentType.New;
                            // Assume that we are good to go. This is where the confirmation number would be set.
                        } else if (response.ReserveStatus.ToString() == "Not-Eligible") {
                            dataAccessResult.IsSuccessful = true;
                            dataAccessResult.Data.Errors.Add("Solix: This person is not eligible for the Lifeline Credit. Recheck Name, SSN, DOB, or Beneficiary Information.");
                            return dataAccessResult;
                        } else {
                            dataAccessResult.IsSuccessful = true;
                            dataAccessResult.Data.Errors.Add("Solix: Details for this person could not be found. Recheck Name, SSN, DOB, or Beneficiary Information.");
                            return dataAccessResult;
                        }
                    } else {
                        if (response.ReceivedDiscount.ToString() == "True") {
                            dataAccessResult.IsSuccessful = true;
                            dataAccessResult.Data.Errors.Add("Solix: This person is already receiving a Lifeline discount from another company.");

                            return dataAccessResult;
                        }

                        dataAccessResult.Data.EnrollmentType = EnrollmentType.New;
                    }

                    return dataAccessResult;
                }
            } catch (Exception ex) {
                Logger.Error(ex);
                dataAccessResult.Error = ErrorValues.GENERIC_EXTERNAL_API_RESPONSE_ERROR;
                dataAccessResult.IsSuccessful = false;

                return dataAccessResult;
            }
        }

        //public async Task<DataAccessResult<ISubmitApplicationResponse>> SubmitApplicationAsync(ISubmitApplicationRequestData request) {
        //    var dataAccessResult = new DataAccessResult<ISubmitApplicationResponse> { IsSuccessful = true };

        //    return dataAccessResult;
        //}

        public async Task<DataAccessResult<TexasSolixCreateCustomerResponse>> CreateCustomer(TexasSolixCreateCustomerRequestData request) {
            var dataAccessResult = new DataAccessResult<TexasSolixCreateCustomerResponse> { IsSuccessful = true };
            try {
                var clientCreate = new HHSCClient();
                clientCreate.Endpoint.Behaviors.Add(new TexasSolixCreateInspectorBehavior());

                var response = await clientCreate.CreateCustomerAsync(request.LastName, request.FirstName, request.Ssn4, request.DateOfBirth, request.ServiceAddress1 + request.ServiceAddress2, request.ServiceAddressCity, request.ServiceAddressZip5.Substring(0, 5), request.ConfirmationNumber, UserName, Password, Login);
                if (response.ErrorCode != null && response.ErrorCode != "0") {
                    dataAccessResult.IsSuccessful = false;
                    dataAccessResult.Error = new ProcessingError("Unable to contact the Solix system for creation", "Unable to contact the Solix system for creation", true, false);
                    Logger.Error("Solix Error Creating Customer - Description: " + response.ErrorDescription + " ErrorCode: " + response.ErrorCode);
                    return dataAccessResult;
                }

                dataAccessResult.Data = new TexasSolixCreateCustomerResponse {
                    ConfirmationNumber = response.ConfirmationNumber,
                    SuccessStatus = response.SuccessStatus,
                    ErrorCode = response.ErrorCode,
                    ErrorDescription = response.ErrorDescription
                };

                return dataAccessResult;
            } catch (Exception ex) {
                Logger.Error(ex);
                dataAccessResult.Error = ErrorValues.GENERIC_EXTERNAL_API_RESPONSE_ERROR;
                dataAccessResult.IsSuccessful = false;

                return dataAccessResult;
            }
        }

        private static ApiLogEntry CreateInitialCreateCustomerApiLogEntry(string input) {
            return new ApiLogEntry {
                Api = ApiName,
                DateStarted = DateTime.UtcNow,
                Function = ApiFunctions.TexasSolixCreateCustomer,
                Input = input
            };
        }

        private static void FinishLogEntry(ApiLogEntry logEntry, string content) {
            logEntry.Response = content;
            logEntry.DateEnded = DateTime.UtcNow;

            var logEntryRepository = new ApiLogEntryRepository();
            logEntryRepository.Add(logEntry);
        }
    }
}

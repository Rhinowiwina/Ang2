using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Logging;
using LS.Core;
using LS.Core.Interfaces;
using LS.Domain.ExternalApiIntegration;
using LS.Domain;
using LS.Utilities;
using LS.BudgetMobile.Logging;
using LS.BudgetMobile.BudgetMobileTestApi;
using LS.Repositories;
using Exceptionless;
using Exceptionless.Models;
//TODO: Move these functions into CompanyAPI.cs

namespace LS.BudgetMobile {
    public class CompanyProviderValidation : ICompanyProviderValidation {
        private static readonly string ApplicationId = "LA1953";
        private static readonly string ApplicationPassword = "uxwri5ph";
        private static readonly int LocationId = 32342;

        private ILog Logger { get; set; }

        public CompanyProviderValidation() {
            Logger = LoggerFactory.GetLogger(GetType());
        }

        public async Task<ValidationResult<IValidatedAddress>> ScrubAddress(IValidatedAddress validatedAddress) {
            var result = new ValidationResult<IValidatedAddress> { Errors = new List<string>() };
            try {
                var service = new LifelineServicesSoapClient();
                service.Endpoint.Behaviors.Add(new CAMsScrubInspectorBehavior());

                var credentials = new Credentials_Address_Standardize() {
                    Address = validatedAddress.Street1,
                    Address2 = validatedAddress.Street2,
                    ApplicationID = ApplicationId,
                    ApplicationPassword = ApplicationPassword,
                    City = validatedAddress.City,
                    State = validatedAddress.State,
                    LocationID = LocationId,
                    Zip = validatedAddress.Zip,
                    BypassRestraints = validatedAddress.BypassRestraints
                };

                var serviceResult = await service.Address_StandardizeAsync(credentials);

                //var serviceResult = await service.Address_VerifyLifelineAsync(credentials);
                if (serviceResult.Address_StandardizeResult.IsError) {
                    result.IsValid = false;
                    result.Errors.Add(serviceResult.Address_StandardizeResult.Errors.ErrorMessage);
                    return result;
                }
                result.Data = new ValidatedAddress {
                    City = serviceResult.Address_StandardizeResult.Validated_City,
                    State = serviceResult.Address_StandardizeResult.Validated_State,
                    Street1 = serviceResult.Address_StandardizeResult.Validated_Address1,
                    Street2 = serviceResult.Address_StandardizeResult.Validated_Address2,
                    Zip = serviceResult.Address_StandardizeResult.Validated_Zip,
                    Id = -1
                };
                result.IsValid = true;
            } catch (Exception exception) {
                exception.ToExceptionless()
                  .SetMessage("An exception was thrown while scrubbing address for Arrow")
                  .MarkAsCritical()
                  .Submit();
                //Logger.Error("An exception was thrown while scrubbing address for Budget Mobile", exception);
                result.Errors.Add("An error occurred while attempting to scrub the customer's address");
                result.IsValid = false;
            }

            return result;
        }

        public async Task<ValidationResult<IValidatedAddress>> ValidateAddress(IValidatedAddress validatedAddress, bool hoh) {
            var result = new ValidationResult<IValidatedAddress> { Errors = new List<string>() };

            try {
                var service = new LifelineServicesSoapClient();
                service.Endpoint.Behaviors.Add(new CAMsValidateAddressInspectorBehavior());

                //Todo: Add user name here
                var credentials = new Credentials_Address_VerifyLifeline() {
                    Address = validatedAddress.Street1,
                    Address2 = validatedAddress.Street2,
                    City = validatedAddress.City,
                    LocationID = LocationId,
                    State = validatedAddress.State,
                    Zip = validatedAddress.Zip,
                    ApplicationPassword = ApplicationPassword,
                    ApplicationID = ApplicationId,
                    Lifeline = true,
                    HOHC = hoh
                };

                var serviceResult = await service.Address_VerifyLifelineAsync(credentials);
                if (serviceResult.Address_VerifyLifelineResult.IsError) {
                    result.IsValid = false;
                    if (serviceResult.Address_VerifyLifelineResult.Errors.ErrorCode == 8) {
                        result.Errors.Add("Duplicate Address (LSA)");
                    }
                    result.Errors.Add(serviceResult.Address_VerifyLifelineResult.Errors.ErrorMessage);
                    return result;
                }
                result.Data = new ValidatedAddress {
                    City = serviceResult.Address_VerifyLifelineResult.Validated_City,
                    State = serviceResult.Address_VerifyLifelineResult.Validated_State,
                    Street1 = serviceResult.Address_VerifyLifelineResult.Validated_Address1,
                    Street2 = serviceResult.Address_VerifyLifelineResult.Validated_Address2,
                    Zip = serviceResult.Address_VerifyLifelineResult.Validated_Zip,
                    Id = Int32.Parse(serviceResult.Address_VerifyLifelineResult.Validated_AddressId)
                };
                result.IsValid = true;
            } catch (Exception exception) {
                exception.ToExceptionless()
                    .SetMessage("An exception was thrown while calling validate address for Budget Mobile")
                    .MarkAsCritical()
                    .Submit();

                //Logger.Error("An exception was thrown while calling validate address for Budget Mobile", exception);
                result.Errors.Add("An error occurred while attempting to validate the customer's address");
                result.IsValid = false;
            }

            return result;
        }
    }
}

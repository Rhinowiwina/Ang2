using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LS.Core;
using LS.Domain;
using LS.Domain.ExternalApiIntegration;
using LS.Domain.ExternalApiIntegration.Interfaces;
using LS.Domain.ExternalApiIntegration.PuertoRico;
using LS.Repositories.ExternalApiIntegration;
using LS.Services.Factories;

namespace LS.Services.ExternalApiIntegration
{
    public class PuertoRicoLifelineApplicationService : BaseLifelineApplicationService<PuertoRicoCheckStatusRequestData, PuertoRicoSubmitRequestData>
    {
        private static readonly string EnrollTransactionType = "enroll";
        private string _loggedInUserId;
        private static readonly int DuplicatePhoneNumberRetryLimit = 5;
      
        public PuertoRicoLifelineApplicationService(string loggedInUserId)
        {
            _repository = new PuertoRicoRepository();
            _loggedInUserId = loggedInUserId;
        }
           private ILifelineApplicationRepository _repository;
       

        protected override ILifelineApplicationRepository Repository
        {
            get { return _repository ?? (_repository = new PuertoRicoRepository()); }
            set { _repository = value; }
        }

        protected override ServiceProcessingResult<PuertoRicoCheckStatusRequestData> ConvertToCheckStatusRequestData(ICheckStatusRequestData requestData)
        {
            return GetPuertoRicoRequestDataFor(requestData);                   
        }

        //protected override ServiceProcessingResult<PuertoRicoSubmitRequestData> ConvertToSubmitRequestData(ISubmitApplicationRequestData requestData)
        //{
        //    return GetPuertoRicoRequestDataFor(requestData);
        //}


        protected override async Task<ServiceProcessingResult<ICheckStatusResponse>> SendCheckStatusRequestAsync(PuertoRicoCheckStatusRequestData requestData)
        {
         //var result = await Repository.CheckCustomerStatusAsync(requestData); //Use this if an api is ever created.
            var result = new DataAccessResult<ICheckStatusResponse>();
            result.Data = new BaseCheckStatusResponse();
            result.Data.EnrollmentType = EnrollmentType.New;
            result.Data.ServiceAddressIsRural = false;
            result.IsSuccessful = true;

           // Puerto Rico is bypassed until an api is created.
            
            //if (ResultRequiresRuralFlag(result))
            //{
            //    requestData.PrimaryRuralFlag = "1";
            //    result = await Repository.CheckCustomerStatusAsync(requestData);
            //    result.Data.ServiceAddressIsRural = true;
            //}

            //if (ResultRequiresHohFlag(result))
            //{
            //    if (requestData.IsHohFlag)
            //    {
            //        requestData.IehFlag = "1";
            //        requestData.IehCertificationDate = requestData.TransactionEffectiveDate;
            //        requestData.IehRecertificationDate = requestData.ServiceReverificationDate;

            //        result = await Repository.CheckCustomerStatusAsync(requestData);
            //    }
            //}

            if (result.Data != null)
            {
                //if (requestData.TpivBypass && IsOnlyTpivErrors(result.Data.Errors))
                //{
                //    result.Data.Errors = new List<string>();
                //    result.Data.IsSuccessful = true;
                //}
                //else if (result.Data.Errors != null && IsOnlyTpivErrors(result.Data.Errors))
                //{
                //    foreach (var error in result.Data.Errors)
                //    {
                //        if (error == "Subscriber name or SSN4 could not be validated." || error == "Subscriber identity could not be found.")
                //        {
                //            result.Data.TpivBypassAvailable = false;
                //            break;
                //        } else {
                //            result.Data.TpivBypassAvailable = IsTpivBypassAllowed(_loggedInUserId);
                //        }
                //    }
                //}
                result.Data.Hoh = requestData.IsHohFlag;
            }

            return result.ToServiceProcessingResult(new ProcessingError("", "", false));
        }

        //protected override async Task<ServiceProcessingResult<ISubmitApplicationResponse>> SendSubmitRequestAsync(PuertoRicoSubmitRequestData requestData)
        //{
        //    if (requestData.EnrollmentType == EnrollmentType.Transfer)
        //    {
        //        return await SendTransferRequestAsync(requestData);
        //    }

        //    var result = await Repository.SubmitApplicationAsync(requestData);
        //    result.Data.AssignedPhoneNumber = requestData.AssignedPhoneNumber;

        //    if (ResultRequiresHohFlag(result))
        //    {
        //        if (requestData.IsHohFlag)
        //        {
        //            requestData.IehFlag = "1";
        //            requestData.IehCertificationDate = requestData.TransactionEffectiveDate;
        //            requestData.IehRecertificationDate = requestData.ServiceReverificationDate;

        //            result = await Repository.SubmitApplicationAsync(requestData);
        //        }
        //    }

        //    if (result.Data != null && result.Data.Errors != null
        //    && result.Data.Errors.Count == 1 && result.Data.Errors[0] == PuertoRicoErrorCodes.ExpectedErrorCodes[PuertoRicoErrorCodes.DuplicatePhoneNumber])
        //    {
        //        var PuertoRicoPhoneNumberDataService = new PuertoRicoPhoneNumberDataService();
        //        var generateNumberResult = PuertoRicoPhoneNumberDataService.GetNextValidPuertoRicoPhoneNumber(requestData.CompanyId);
        //        requestData.AssignedPhoneNumber = null;
                
        //        //Todo: Add a retry loop
        //        if (!generateNumberResult.IsSuccessful)
        //        {
        //            result.IsSuccessful = false;
        //            result.Error = ErrorValues.GENERIC_PuertoRico_PHONE_NUMBER_GENERATION_ERROR;
        //            return result.ToServiceProcessingResult(result.Error);
        //        }

        //        requestData.AssignedPhoneNumber = generateNumberResult.Data;

        //        result = await Repository.SubmitApplicationAsync(requestData);
        //        //Retry with new phone number
        //    }

        //    if (ResultRequiresRuralFlag(result))
        //    {
        //        requestData.PrimaryRuralFlag = "1";
        //        result = await Repository.SubmitApplicationAsync(requestData);
        //        result.Data.ServiceAddressIsRural = true;
        //    }

        //    if (result.Data != null && requestData.TpivBypass && IsOnlyTpivErrors(result.Data.Errors))
        //    {
        //        var PuertoRicoRepository = (PuertoRicoRepository) Repository;
        //        var PuertoRicoApiResponse = (PuertoRicoApiResponse)result.Data;
        //        var resolutionRequestData = new ResolutionRequestData
        //        {
        //            ResolutionId = PuertoRicoApiResponse.ResolutionId,
        //            Description = CreateResolutionRequestDescription(requestData, PuertoRicoApiResponse)
        //        };
        //        var submitResolutionRequest = await PuertoRicoRepository.SubmitResolutionRequest(resolutionRequestData);
        //        return submitResolutionRequest.ToServiceProcessingResult(new ProcessingError("", "", false));
        //    }

        //    return result.ToServiceProcessingResult(new ProcessingError("", "", false));
        //}

        //private async Task<ServiceProcessingResult<ISubmitApplicationResponse>> SendTransferRequestAsync(
        //    PuertoRicoSubmitRequestData requestData)
        //{
        //    var repository = (PuertoRicoRepository) Repository;
        //    var result = await repository.SubmitTransferAsync(requestData);

        //    if (ResultRequiresHohFlag(result))
        //    {
        //        requestData.IehFlag = "1";
        //        requestData.IehCertificationDate = requestData.TransactionEffectiveDate;
        //        requestData.IehRecertificationDate = requestData.ServiceReverificationDate;

        //        result = await repository.SubmitTransferAsync(requestData);
        //    }

        //    if (DuplicatePhoneNumberErrorExists(result))
        //    {
        //        requestData.AssignedPhoneNumber = null;
        //        requestData.AssignedTelephoneNumber = null;
        //        var requestDataResult = GetPuertoRicoRequestDataFor(requestData);
        //        if (!requestDataResult.IsSuccessful)
        //        {
        //            //Todo: Handle retry error
        //        }
        //        requestDataResult.Data.LifelineProgramId = requestData.LifelineProgramId;
        //        requestDataResult.Data.CompanyId = requestData.CompanyId;
        //        result.Data.AssignedPhoneNumber = requestDataResult.Data.AssignedPhoneNumber;

        //        result = await Repository.SubmitApplicationAsync(requestDataResult.Data);
        //        //Retry with new phone number
        //    }

        //    if (result.Data != null && requestData.TpivBypass && IsOnlyTpivErrors(result.Data.Errors))
        //    {
        //        var PuertoRicoRepository = (PuertoRicoRepository)Repository;
        //        var PuertoRicoApiResponse = (PuertoRicoApiResponse)result.Data;
        //        var resolutionRequestData = new ResolutionRequestData
        //        {
        //            ResolutionId = PuertoRicoApiResponse.ResolutionId,
        //            Description = CreateResolutionRequestDescription(requestData, PuertoRicoApiResponse)
        //        };
        //        var submitResolutionRequest = await PuertoRicoRepository.SubmitResolutionRequest(resolutionRequestData);
        //        return submitResolutionRequest.ToServiceProcessingResult(new ProcessingError("", "", false));
        //    }

        //    return result.ToServiceProcessingResult(new ProcessingError("", "", false));
        //}

        private static ServiceProcessingResult<PuertoRicoCheckStatusRequestData> GetPuertoRicoRequestDataFor(ICheckStatusRequestData request)
        {
            var companyDataService = new CompanyDataService();
            var lifeLineProgramDataService = new LifelineProgramDataService();
            var PuertoRicoPhoneNumberDataService = new PuertoRicoPhoneNumberDataService();

            var getCompanyResult = companyDataService.GetWithSacEntries(request.CompanyId);
            if (!getCompanyResult.IsSuccessful)
            {
                return new ServiceProcessingResult<PuertoRicoCheckStatusRequestData>
                {
                    Error = new ProcessingError(string.Format("An error occurred while retrieving company with Id of {0}", request.CompanyId),
                            "An error occurred while processing your request. Please contact support if problem persists", true),
                    IsSuccessful = false,
                };
            }
            var getLifelineProgramResult = lifeLineProgramDataService.Get(request.LifelineProgramId);
            if (!getLifelineProgramResult.IsSuccessful)
            {
                return new ServiceProcessingResult<PuertoRicoCheckStatusRequestData>
                {
                    Error = new ProcessingError(string.Format("An error occurred while retrieving Lifeline Program with Id of {0}", request.LifelineProgramId),
                            "An error occurred while processing your request. Please contact support if problem persists", true),
                    IsSuccessful = false,
                };
            }

            var company = getCompanyResult.Data;
            var lifelineProgram = getLifelineProgramResult.Data;

            if (company.SacEntries.Count == 0)
            {
                return new ServiceProcessingResult<PuertoRicoCheckStatusRequestData>
                {
                    Error = ErrorValues.NO_VALID_SAC_NUMBER_ENTRY_FOR_COMPANY,
                    IsSuccessful = false
                };
            }

            string generatedPhoneNumber = null;

            // NOTE: We don't care what the phone number is when we verify ONLY on submission

            //if (!string.IsNullOrEmpty(request.AssignedTelephoneNumber))
            //{
            //    generatedPhoneNumber = request.AssignedTelephoneNumber;
            //}
            //else
            //{
            //    var getNextValidPhoneNumberResult = PuertoRicoPhoneNumberDataService.GetNextValidPuertoRicoPhoneNumber(company.Id);
            //    if (!getNextValidPhoneNumberResult.IsSuccessful)
            //    {
            //        return new ServiceProcessingResult<PuertoRicoCheckStatusRequestData>
            //        {
            //            Error = getNextValidPhoneNumberResult.Error,
            //            IsSuccessful = false
            //        };
            //    }

            //    generatedPhoneNumber = getNextValidPhoneNumberResult.Data;
            //}
                
            var PuertoRicoRequestData = new PuertoRicoCheckStatusRequestData
            {
                //Sac = company.SacEntries.First(s => s.StateCode == request.ServiceAddressState).SacNumber,
                TransactionType = EnrollTransactionType,
                LastName = request.LastName,
                FirstName = request.FirstName,
                MiddleInitial = request.MiddleInitial,
                AssignedTelephoneNumber = generatedPhoneNumber,
                AssignedPhoneNumber = generatedPhoneNumber,
                Ssn4 = request.Ssn4,
                DateOfBirth = request.DateOfBirth,
                ServiceAddress1 = request.ServiceAddress1,
                ServiceAddressCity = request.ServiceAddressCity,
                ServiceAddressState = request.ServiceAddressState,
                ServiceAddressZip4 = request.ServiceAddressZip4,
                ServiceAddressZip5 = request.ServiceAddressZip5,
                //EligibilityCode = lifelineProgram.PuertoRicoEligibilityCode,
                TransactionEffectiveDate = DateTime.UtcNow.ToString("MM/dd/yyyy"),
                QualifyingBeneficiaryDateOfBirth = request.QualifyingBeneficiaryDateOfBirth,
                QualifyingBeneficiaryFirstName = request.QualifyingBeneficiaryFirstName,
                QualifyingBeneficiaryLastName = request.QualifyingBeneficiaryLastName,
                QualifyingBeneficiaryLast4Ssn = request.QualifyingBeneficiaryLast4Ssn,
                IsHohFlag = request.IsHohFlag,
                CompanyId = request.CompanyId,
                LifelineProgramId = request.LifelineProgramId,
                TpivBypass = request.TpivBypass,
                PrimaryRuralFlag = "0",
                PriorULTSTelephoneNumber = request.PriorULTSTelephoneNumber
            };

            PuertoRicoRequestData.ServiceInitializationDate = PuertoRicoRequestData.TransactionEffectiveDate;

            return new ServiceProcessingResult<PuertoRicoCheckStatusRequestData>
            {
                Data = PuertoRicoRequestData,
                IsSuccessful = true
            };
        }

        private static ServiceProcessingResult<PuertoRicoSubmitRequestData> GetPuertoRicoRequestDataFor(ISubmitApplicationRequestData request)
        {
            var companyDataService = new CompanyDataService();
            var lifeLineProgramDataService = new LifelineProgramDataService();
            var PuertoRicoPhoneNumberDataService = new PuertoRicoPhoneNumberDataService();

            var getCompanyResult = companyDataService.GetWithSacEntries(request.CompanyId);
            if (!getCompanyResult.IsSuccessful)
            {
                return new ServiceProcessingResult<PuertoRicoSubmitRequestData>
                {
                    Error = new ProcessingError(string.Format("An error occurred while retrieving company with Id of {0}", request.CompanyId),
                            "An error occurred while processing your request. Please contact support if problem persists", true),
                    IsSuccessful = false,
                };
            }
            var getLifelineProgramResult = lifeLineProgramDataService.Get(request.LifelineProgramId);
            if (!getLifelineProgramResult.IsSuccessful)
            {
                return new ServiceProcessingResult<PuertoRicoSubmitRequestData>
                {
                    Error = new ProcessingError(string.Format("An error occurred while retrieving Lifeline Program with Id of {0}", request.LifelineProgramId),
                            "An error occurred while processing your request. Please contact support if problem persists", true),
                    IsSuccessful = false,
                };
            }

            var company = getCompanyResult.Data;
            var lifelineProgram = getLifelineProgramResult.Data;

            if (company.SacEntries.Count == 0)
            {
                return new ServiceProcessingResult<PuertoRicoSubmitRequestData>
                {
                    Error = ErrorValues.NO_VALID_SAC_NUMBER_ENTRY_FOR_COMPANY,
                    IsSuccessful = false
                };
            }

            string generatedPhoneNumber;

            if (!string.IsNullOrEmpty(request.AssignedTelephoneNumber))
            {
                generatedPhoneNumber = request.AssignedTelephoneNumber;
            }
            else
            {
                var getNextValidPhoneNumberResult = PuertoRicoPhoneNumberDataService.GetNextValidPuertoRicoPhoneNumber(company.Id);
                if (!getNextValidPhoneNumberResult.IsSuccessful)
                {
                    return new ServiceProcessingResult<PuertoRicoSubmitRequestData>
                    {
                        Error = getNextValidPhoneNumberResult.Error,
                        IsSuccessful = false
                    };
                }

                generatedPhoneNumber = getNextValidPhoneNumberResult.Data;
            }
            
            var PuertoRicoRequestData = new PuertoRicoSubmitRequestData
            {
                Sac = company.SacEntries.First(s => s.StateCode == request.ServiceAddressState).SacNumber,
                TransactionType = EnrollTransactionType,
                LastName = request.LastName,
                FirstName = request.FirstName,
                MiddleInitial = request.MiddleInitial,
                AssignedTelephoneNumber = generatedPhoneNumber,
                AssignedPhoneNumber = generatedPhoneNumber,
                Ssn4 = request.Ssn4,
                DateOfBirth = request.DateOfBirth,
                ServiceAddress1 = request.ServiceAddress1,
                ServiceAddress2 = request.ServiceAddress2,
                ServiceAddressCity = request.ServiceAddressCity,
                ServiceAddressState = request.ServiceAddressState,
                ServiceAddressZip4 = request.ServiceAddressZip4,
                ServiceAddressZip5 = request.ServiceAddressZip5,
                BillingAddress1 = request.BillingAddress1,
                BillingAddress2 = request.BillingAddress2,
                BillingCity = request.BillingCity,
                BillingFirstName = request.BillingFirstName,
                BillingLastName = request.BillingLastName,
                BillingMiddleInitial = request.BillingMiddleInitial,
                BillingState = request.BillingState,
                BillingZip4 = request.BillingZip4,
                BillingZip5 = request.BillingZip5,
                //EligibilityCode = lifelineProgram.PuertoRicoEligibilityCode,
                TransactionEffectiveDate = DateTime.UtcNow.ToString("MM/dd/yyyy"),
                QualifyingBeneficiaryDateOfBirth = request.QualifyingBeneficiaryDateOfBirth,
                QualifyingBeneficiaryFirstName = request.QualifyingBeneficiaryFirstName,
                QualifyingBeneficiaryLastName = request.QualifyingBeneficiaryLastName,
                QualifyingBeneficiaryLast4Ssn = request.QualifyingBeneficiaryLast4Ssn,
                IsHohFlag = request.IsHohFlag,
                CompanyId = request.CompanyId,
                LifelineProgramId = request.LifelineProgramId,
                EnrollmentType = request.EnrollmentType,
                TpivBypass = request.TpivBypass,
                TpivBypassDobDocument = request.TpivBypassDobDocument,
                TpivBypassDobLast4Digits = request.TpivBypassDobLast4Digits,
                TpivBypassSignature = request.TpivBypassSignature,
                TpivBypassSsnDocument = request.TpivBypassSsnDocument,
                TpivBypassSsnLast4Digits = request.TpivBypassSsnLast4Digits,
                PrimaryRuralFlag = "0"
            };

            PuertoRicoRequestData.ServiceInitializationDate = PuertoRicoRequestData.TransactionEffectiveDate;

            return new ServiceProcessingResult<PuertoRicoSubmitRequestData>
            {
                Data = PuertoRicoRequestData,
                IsSuccessful = true
            };
        }

        private static bool ResultRequiresHohFlag(DataAccessResult<ISubmitApplicationResponse> result)
        {
            return result.Data != null && result.Data.Errors != null && result.Data.Errors.Count == 1 &&
                   result.Data.Errors[0] == PuertoRicoErrorCodes.ExpectedErrorCodes[PuertoRicoErrorCodes.DuplicatePrimaryAddress];
        }

        private static bool ResultRequiresHohFlag(DataAccessResult<ICheckStatusResponse> result)
        {

            return result.Data != null && result.Data.Errors != null && result.Data.Errors.Count == 1 &&
                   result.Data.Errors[0] == PuertoRicoErrorCodes.ExpectedErrorCodes[PuertoRicoErrorCodes.DuplicatePrimaryAddress];
        }

        private static bool ResultRequiresRuralFlag(DataAccessResult<ICheckStatusResponse> result)
        {
            if (result.Data == null || result.Data.Errors == null)
            {
                return false;
            }

            var errorListWithoutAmsFailures = new List<string>(result.Data.Errors);
            errorListWithoutAmsFailures.RemoveAll(e => e == PuertoRicoErrorCodes.ExpectedErrorCodes[PuertoRicoErrorCodes.AmsFailure]);
            return errorListWithoutAmsFailures.Count == 0;
        }

        private static bool ResultRequiresRuralFlag(DataAccessResult<ISubmitApplicationResponse> result)
        {
            if (result.Data == null || result.Data.Errors == null)
            {
                return false;
            }

            var errorListWithoutAmsFailures = new List<string>(result.Data.Errors);
            errorListWithoutAmsFailures.RemoveAll(e => e == PuertoRicoErrorCodes.ExpectedErrorCodes[PuertoRicoErrorCodes.AmsFailure]);
            return errorListWithoutAmsFailures.Count == 0;
        }

        private static bool IsOnlyTpivErrors(IEnumerable<string> errorList)
        {
            return errorList.All( e =>
                e == PuertoRicoErrorCodes.ExpectedErrorCodes[PuertoRicoErrorCodes.TpivFail] || 
                e == PuertoRicoErrorCodes.ExpectedErrorCodes[PuertoRicoErrorCodes.TpivFailedOnDob] ||
                e == PuertoRicoErrorCodes.ExpectedErrorCodes[PuertoRicoErrorCodes.TpivFailedOnIdentity] || 
                e == PuertoRicoErrorCodes.ExpectedErrorCodes[PuertoRicoErrorCodes.TpivFailedOnSsn] ||
                e == PuertoRicoErrorCodes.ExpectedErrorCodes[PuertoRicoErrorCodes.FailedTpiv]);
        }

        private bool IsTpivBypassAllowed(string loggedInUserId)
        {
            var applicationUserDataService = new ApplicationUserDataService();
            var getResult = applicationUserDataService.Get(loggedInUserId);
            if (!getResult.IsSuccessful)
            {
                Logger.Error("Failed to get user with Id of " + loggedInUserId + " during TPIV permission check");
                return false;
            }
            var loggedInUser = getResult.Data;
            return loggedInUser.PermissionsBypassTpiv;
        }

        private string CreateResolutionRequestDescription(PuertoRicoRequestData requestData, PuertoRicoApiResponse PuertoRicoApiResponse)
        {
            return string.Format("Resolution ID: {0}.  Agent Name/Employee ID: {1}" +
                                 " (ID: {2}).  I have reviewed the subscriber's" +
                                 " {3} to confirm the full name and date of birth and the " +
                                 "subscriber's {4} to confirm the full name and Social Security " +
                                 "Number.  Certification Statement: I certify that my submission of data to the " +
                                 "National Lifeline Accountability Database is accurate to the best of my knowledge " +
                                 "and any willful false statements made may subject me to federal criminal " +
                                 "prosecution and penalties, as well as civil penalties.", PuertoRicoApiResponse.ResolutionId, requestData.TpivBypassSignature,
                                 _loggedInUserId, requestData.TpivBypassDobDocument, requestData.TpivBypassSsnDocument);
        }

        private static bool IsPhoneNumberDuplicate(DataAccessResult<ICheckStatusResponse> result)
        {
            return result.Data.Errors != null && result.Data.Errors.Any(e => e == PuertoRicoErrorCodes.ExpectedErrorCodes[PuertoRicoErrorCodes.DuplicatePhoneNumber]);
        }

        private static bool DuplicatePhoneNumberErrorExists(DataAccessResult<ICheckStatusResponse> result)
        {
            return result.Data != null && result.Data.Errors != null &&
                   result.Data.Errors.Any(e => e == PuertoRicoErrorCodes.ExpectedErrorCodes[PuertoRicoErrorCodes.DuplicatePhoneNumber]);
        }

        private static bool DuplicatePhoneNumberErrorExists(DataAccessResult<ISubmitApplicationResponse> result)
        {
            return result.Data != null && result.Data.Errors != null &&
                    result.Data.Errors.Any(e => e == PuertoRicoErrorCodes.ExpectedErrorCodes[PuertoRicoErrorCodes.DuplicatePhoneNumber]);
        }
    }
}

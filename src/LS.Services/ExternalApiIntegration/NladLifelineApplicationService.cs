using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LS.Core;
using LS.Domain;
using LS.Domain.ExternalApiIntegration;
using LS.Domain.ExternalApiIntegration.Interfaces;
using LS.Domain.ExternalApiIntegration.Nlad;
using LS.Repositories.ExternalApiIntegration;
using LS.Services.Factories;

namespace LS.Services.ExternalApiIntegration
{
    public class NladLifelineApplicationService : BaseLifelineApplicationService<NladCheckStatusRequestData, NladSubmitRequestData>
    {
        private static readonly string EnrollTransactionType = "enroll";
        private string _loggedInUserId;
        private static readonly int DuplicatePhoneNumberRetryLimit = 5;

        public NladLifelineApplicationService(string loggedInUserId)
        {
            _repository = new NladRepository();
            _loggedInUserId = loggedInUserId;
        }
        
        private ILifelineApplicationRepository _repository;

        protected override ILifelineApplicationRepository Repository
        {
            get { return _repository ?? (_repository = new NladRepository()); }
            set { _repository = value; }
        }

        protected override ServiceProcessingResult<NladCheckStatusRequestData> ConvertToCheckStatusRequestData(ICheckStatusRequestData requestData)
        {
            return GetNladRequestDataFor(requestData);
        }

        //protected override ServiceProcessingResult<NladSubmitRequestData> ConvertToSubmitRequestData(ISubmitApplicationRequestData requestData)
        //{
        //    return GetNladRequestDataFor(requestData);
        //}


        protected override async Task<ServiceProcessingResult<ICheckStatusResponse>> SendCheckStatusRequestAsync(NladCheckStatusRequestData requestData)
        {
            var result = await Repository.CheckCustomerStatusAsync(requestData);
            if (!result.IsSuccessful) {
                return result.ToServiceProcessingResult(result.Error);
            }
            //this function ends up checking if error has a "address failed validation error" if so it removes error sets rural flag to true to okay validation with a second pass. If more than one error it does not set flag and fails order.
            //RW removed "address failed validation error" code so we could do a manual by pass 5/17/2016
            if (ResultRequiresRuralFlag(result)) {
                requestData.PrimaryRuralFlag = "1";
                result = await Repository.CheckCustomerStatusAsync(requestData);
                result.Data.ServiceAddressIsRural = true;
            }
//----------------------------------------------------------------
            if (ResultRequiresHohFlag(result)) {
                if (requestData.IsHohFlag)
                {
                    requestData.IehFlag = "1";
                    requestData.IehCertificationDate = requestData.TransactionEffectiveDate;
                    requestData.IehRecertificationDate = requestData.ServiceReverificationDate;

                    result = await Repository.CheckCustomerStatusAsync(requestData);
                }
            }
            //Cam's address validation has been ran, if blocked we will not get to here. If by passed will need to recognize and by pass for NLAD
            if (result.Data != null) {
                var SSNFailed = false;
                var DOBFailed = false;
                var AddressFailed = false;
                 if (result.Data.Errors != null) {
                    foreach (var error in result.Data.Errors) {
                        if (error == "Subscriber name or SSN4 could not be validated.") {
                            if (!requestData.TpivBypass)
                                { SSNFailed = true; }
                        }
                        if (error == "Subscriber date of birth could not be validated.") {
                            if (!requestData.TpivBypass)
                                { DOBFailed = true; }
                        }
                        if(error=="Address failed validation")
                            {
                            //set bypass
                            if (!requestData.HasServiceAddressBypass)
                                {
                                AddressFailed = true;
                                
                                }
                            }
                    }
                    if (!SSNFailed && !DOBFailed) { //If we did not get a SSN/DOB specific error, check for generic errors
                        foreach (var error in result.Data.Errors) {
                            if (error == "Subscriber failed third-party identity verification." || error== "Subscriber identity could not be found.") {
                                SSNFailed = true;
                                DOBFailed = true;
                            }
                        }
                    }
                    //this has been set or cleared by this point
                        if (SSNFailed && DOBFailed)
                                {
                                result.Data.TpivBypassFailureType = "BOTH";
                                }
                            else if (SSNFailed)
                                {
                                result.Data.TpivBypassFailureType = "SSN";
                                }
                            else if (DOBFailed)
                                {
                                result.Data.TpivBypassFailureType = "DOB";
                                }


                    //make sure there are no other errors

                    if (IsOnlyTpivAndAddressErrors(result.Data.Errors))
                        {
                        //check this && (!string.IsNullOrEmpty(requestData.TpivBypassSsnDocument)) && (!string.IsNullOrEmpty(requestData.TpivBypassDobDocument))
                        if (requestData.TpivBypass && requestData.HasServiceAddressBypass)
                            {
                            result.Data.Errors = new List<string>();
                            if (result.Data.EnrollmentType != EnrollmentType.Transfer)
                                { result.Data.EnrollmentType = EnrollmentType.New; }
                            result.Data.IsSuccessful = true;
                            }
                        else {
                            if (requestData.HasServiceAddressBypass)
                                {
                                result.Data.Errors = RemoveAddressErrors(result.Data.Errors);
                                }

                            if (!requestData.HasServiceAddressBypass && AddressFailed)
                            {
                            result.Data.AddressBypassAvailable = true;
                          
                            }
                            if (requestData.TpivBypass)
                                {
                                result.Data.Errors = RemoveTpivErrors(result.Data.Errors);

                                }
                            if (!requestData.TpivBypass && (SSNFailed || DOBFailed))
                                {
                                result.Data.TpivBypassAvailable = IsTpivBypassAllowed(_loggedInUserId);
                              
                                }
                                                        
                            if (String.IsNullOrEmpty(_loggedInUserId)&& requestData.TpivBypass)
                                {
                                result.Data.TpivBypassAvailable = false;
                                result.Data.IsSuccessful = false;
                                }


                             }
                        }
                    else
                        { //there are other errors fail it
                        result.Data.IsSuccessful = false;
                      
                        }

                  
                }
                result.Data.Hoh = requestData.IsHohFlag;
            }

            return result.ToServiceProcessingResult(new ProcessingError("", "", false));
        }

        //protected override async Task<ServiceProcessingResult<ISubmitApplicationResponse>> SendSubmitRequestAsync(NladSubmitRequestData requestData)
        ////{
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
        //    && result.Data.Errors.Count == 1 && result.Data.Errors[0] == NladErrorCodes.ExpectedErrorCodes[NladErrorCodes.DuplicatePhoneNumber])
        //    {
        //        var nladPhoneNumberDataService = new NladPhoneNumberDataService();
        //        var generateNumberResult = nladPhoneNumberDataService.GetNextValidNladPhoneNumber(requestData.CompanyId);
        //        requestData.AssignedPhoneNumber = null;

        //        //Todo: Add a retry loop
        //        if (!generateNumberResult.IsSuccessful)
        //        {
        //            result.IsSuccessful = false;
        //            result.Error = ErrorValues.GENERIC_NLAD_PHONE_NUMBER_GENERATION_ERROR;
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
        //        var nladRepository = (NladRepository) Repository;
        //        var nladApiResponse = (NladApiResponse)result.Data;
        //        var resolutionRequestData = new ResolutionRequestData
        //        {
        //            ResolutionId = nladApiResponse.ResolutionId,
        //            Description = CreateResolutionRequestDescription(requestData, nladApiResponse)
        //        };
        //        var submitResolutionRequest = await nladRepository.SubmitResolutionRequest(resolutionRequestData);
        //        return submitResolutionRequest.ToServiceProcessingResult(new ProcessingError("", "", false));
        //    }

        //    return result.ToServiceProcessingResult(new ProcessingError("", "", false));
        //}

        //private async Task<ServiceProcessingResult<ISubmitApplicationResponse>> SendTransferRequestAsync(
        //    NladSubmitRequestData requestData)
        //{
        //    var repository = (NladRepository) Repository;
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
        //        var requestDataResult = GetNladRequestDataFor(requestData);
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
        //        var nladRepository = (NladRepository)Repository;
        //        var nladApiResponse = (NladApiResponse)result.Data;
        //        var resolutionRequestData = new ResolutionRequestData
        //        {
        //            ResolutionId = nladApiResponse.ResolutionId,
        //            Description = CreateResolutionRequestDescription(requestData, nladApiResponse)
        //        };
        //        var submitResolutionRequest = await nladRepository.SubmitResolutionRequest(resolutionRequestData);
        //        return submitResolutionRequest.ToServiceProcessingResult(new ProcessingError("", "", false));
        //    }

        //    return result.ToServiceProcessingResult(new ProcessingError("", "", false));
        //}

        private static ServiceProcessingResult<NladCheckStatusRequestData> GetNladRequestDataFor(ICheckStatusRequestData request)
        {
            var companyDataService = new CompanyDataService();
            var lifeLineProgramDataService = new LifelineProgramDataService();
            var nladPhoneNumberDataService = new NladPhoneNumberDataService();

            var getCompanyResult = companyDataService.GetWithSacEntries(request.CompanyId);
            if (!getCompanyResult.IsSuccessful)
            {
                return new ServiceProcessingResult<NladCheckStatusRequestData>
                {
                    Error = new ProcessingError(string.Format("An error occurred while retrieving company with Id of {0}", request.CompanyId),
                            "An error occurred while processing your request. Please contact support if problem persists", true),
                    IsSuccessful = false,
                };
            }
            var getLifelineProgramResult = lifeLineProgramDataService.Get(request.LifelineProgramId);
            if (!getLifelineProgramResult.IsSuccessful)
            {
                return new ServiceProcessingResult<NladCheckStatusRequestData>
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
                return new ServiceProcessingResult<NladCheckStatusRequestData>
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
            //    var getNextValidPhoneNumberResult = nladPhoneNumberDataService.GetNextValidNladPhoneNumber(company.Id);
            //    if (!getNextValidPhoneNumberResult.IsSuccessful)
            //    {
            //        return new ServiceProcessingResult<NladCheckStatusRequestData>
            //        {
            //            Error = getNextValidPhoneNumberResult.Error,
            //            IsSuccessful = false
            //        };
            //    }

            //    generatedPhoneNumber = getNextValidPhoneNumberResult.Data;
            //}
            //Make sure only last 4 digits are sent to Nlad, Some states want full stored in DB.
            if (request.Ssn4.Length > 4)
            {
                request.Ssn4 = request.Ssn4.Substring(request.Ssn4.Length - 4);
            }
            var nladRequestData = new NladCheckStatusRequestData
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
                ServiceAddressCity = request.ServiceAddressCity,
                ServiceAddressState = request.ServiceAddressState,
                ServiceAddressZip4 = request.ServiceAddressZip4,
                ServiceAddressZip5 = request.ServiceAddressZip5,
                EligibilityCode = lifelineProgram.NladEligibilityCode,
                TransactionEffectiveDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MM/dd/yyyy"),
                QualifyingBeneficiaryDateOfBirth = request.QualifyingBeneficiaryDateOfBirth,
                QualifyingBeneficiaryFirstName = request.QualifyingBeneficiaryFirstName,
                QualifyingBeneficiaryLastName = request.QualifyingBeneficiaryLastName,
                QualifyingBeneficiaryLast4Ssn = request.QualifyingBeneficiaryLast4Ssn,
                IsHohFlag = request.IsHohFlag,
                CompanyId = request.CompanyId,
                LifelineProgramId = request.LifelineProgramId,
                TpivBypassSsnDocument=request.TpivBypassSsnDocument,
                TpivBypassDobDocument=request.TpivBypassDobDocument,
                TpivBypass = request.TpivBypass,
                HasServiceAddressBypass=request.HasServiceAddressBypass,
                PrimaryRuralFlag = "0",
                PriorULTSTelephoneNumber = request.PriorULTSTelephoneNumber
            };

            nladRequestData.ServiceInitializationDate = nladRequestData.TransactionEffectiveDate;

            return new ServiceProcessingResult<NladCheckStatusRequestData>
            {
                Data = nladRequestData,
                IsSuccessful = true
            };
        }

        //private static ServiceProcessingResult<NladSubmitRequestData> GetNladRequestDataFor(ISubmitApplicationRequestData request)
        //{
        //    var companyDataService = new CompanyDataService();
        //    var lifeLineProgramDataService = new LifelineProgramDataService();
        //    var nladPhoneNumberDataService = new NladPhoneNumberDataService();

        //    var getCompanyResult = companyDataService.GetWithSacEntries(request.CompanyId);
        //    if (!getCompanyResult.IsSuccessful)
        //    {
        //        return new ServiceProcessingResult<NladSubmitRequestData>
        //        {
        //            Error = new ProcessingError(string.Format("An error occurred while retrieving company with Id of {0}", request.CompanyId),
        //                    "An error occurred while processing your request. Please contact support if problem persists", true),
        //            IsSuccessful = false,
        //        };
        //    }
        //    var getLifelineProgramResult = lifeLineProgramDataService.Get(request.LifelineProgramId);
        //    if (!getLifelineProgramResult.IsSuccessful)
        //    {
        //        return new ServiceProcessingResult<NladSubmitRequestData>
        //        {
        //            Error = new ProcessingError(string.Format("An error occurred while retrieving Lifeline Program with Id of {0}", request.LifelineProgramId),
        //                    "An error occurred while processing your request. Please contact support if problem persists", true),
        //            IsSuccessful = false,
        //        };
        //    }

        //    var company = getCompanyResult.Data;
        //    var lifelineProgram = getLifelineProgramResult.Data;

        //    if (company.SacEntries.Count == 0)
        //    {
        //        return new ServiceProcessingResult<NladSubmitRequestData>
        //        {
        //            Error = ErrorValues.NO_VALID_SAC_NUMBER_ENTRY_FOR_COMPANY,
        //            IsSuccessful = false
        //        };
        //    }

        //    string generatedPhoneNumber;

        //    if (!string.IsNullOrEmpty(request.AssignedTelephoneNumber))
        //    {
        //        generatedPhoneNumber = request.AssignedTelephoneNumber;
        //    }
        //    else
        //    {
        //        var getNextValidPhoneNumberResult = nladPhoneNumberDataService.GetNextValidNladPhoneNumber(company.Id);
        //        if (!getNextValidPhoneNumberResult.IsSuccessful)
        //        {
        //            return new ServiceProcessingResult<NladSubmitRequestData>
        //            {
        //                Error = getNextValidPhoneNumberResult.Error,
        //                IsSuccessful = false
        //            };
        //        }

        //        generatedPhoneNumber = getNextValidPhoneNumberResult.Data;
        //    }

        //    var nladRequestData = new NladSubmitRequestData
        //    {
        //        Sac = company.SacEntries.First(s => s.StateCode == request.ServiceAddressState).SacNumber,
        //        TransactionType = EnrollTransactionType,
        //        LastName = request.LastName,
        //        FirstName = request.FirstName,
        //        MiddleInitial = request.MiddleInitial,
        //        AssignedTelephoneNumber = generatedPhoneNumber,
        //        AssignedPhoneNumber = generatedPhoneNumber,
        //        Ssn4 = request.Ssn4,
        //        DateOfBirth = request.DateOfBirth,
        //        ServiceAddress1 = request.ServiceAddress1,
        //        ServiceAddress2 = request.ServiceAddress2,
        //        ServiceAddressCity = request.ServiceAddressCity,
        //        ServiceAddressState = request.ServiceAddressState,
        //        ServiceAddressZip4 = request.ServiceAddressZip4,
        //        ServiceAddressZip5 = request.ServiceAddressZip5,
        //        BillingAddress1 = request.BillingAddress1,
        //        BillingAddress2 = request.BillingAddress2,
        //        BillingCity = request.BillingCity,
        //        BillingFirstName = request.BillingFirstName,
        //        BillingLastName = request.BillingLastName,
        //        BillingMiddleInitial = request.BillingMiddleInitial,
        //        BillingState = request.BillingState,
        //        BillingZip4 = request.BillingZip4,
        //        BillingZip5 = request.BillingZip5,
        //        EligibilityCode = lifelineProgram.NladEligibilityCode,
        //        TransactionEffectiveDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("MM/dd/yyyy"),
        //        QualifyingBeneficiaryDateOfBirth = request.QualifyingBeneficiaryDateOfBirth,
        //        QualifyingBeneficiaryFirstName = request.QualifyingBeneficiaryFirstName,
        //        QualifyingBeneficiaryLastName = request.QualifyingBeneficiaryLastName,
        //        QualifyingBeneficiaryLast4Ssn = request.QualifyingBeneficiaryLast4Ssn,
        //        IsHohFlag = request.IsHohFlag,
        //        CompanyId = request.CompanyId,
        //        LifelineProgramId = request.LifelineProgramId,
        //        EnrollmentType = request.EnrollmentType,
        //        TpivBypass = request.TpivBypass,
        //        TpivBypassDobDocument = request.TpivBypassDobDocument,
        //        TpivBypassDobLast4Digits = request.TpivBypassDobLast4Digits,
        //        TpivBypassSignature = request.TpivBypassSignature,
        //        TpivBypassSsnDocument = request.TpivBypassSsnDocument,
        //        TpivBypassSsnLast4Digits = request.TpivBypassSsnLast4Digits,
        //        PrimaryRuralFlag = "0"
        //    };

        //    nladRequestData.ServiceInitializationDate = nladRequestData.TransactionEffectiveDate;

        //    return new ServiceProcessingResult<NladSubmitRequestData>
        //    {
        //        Data = nladRequestData,
        //        IsSuccessful = true
        //    };
        //}

        private static bool ResultRequiresHohFlag(DataAccessResult<ISubmitApplicationResponse> result)
        {
            var returnVar = false;

            if (result.Data != null && result.Data.Errors != null && result.Data.Errors.Count >= 1) {
                foreach (var error in result.Data.Errors) {
                    if (error == NladErrorCodes.ExpectedErrorCodes[NladErrorCodes.DuplicatePrimaryAddress]) {
                        returnVar = true;
                    }
                }
            }

            return returnVar;
        }

        private static bool ResultRequiresHohFlag(DataAccessResult<ICheckStatusResponse> result)
        {
            var returnVar = false;

            if (result.Data != null && result.Data.Errors != null && result.Data.Errors.Count >= 1) {
                foreach (var error in result.Data.Errors) {
                    if (error == NladErrorCodes.ExpectedErrorCodes[NladErrorCodes.DuplicatePrimaryAddress]) {
                        returnVar = true;
                    }
                }
            }

            return returnVar;
        }

        private static bool ResultRequiresRuralFlag(DataAccessResult<ICheckStatusResponse> result)
        {
            if (result.Data == null || result.Data.Errors == null || result.Data.Errors.Count==0)
            {
                return false;
            }

            var errorListWithoutAmsFailures = new List<string>(result.Data.Errors);
            //errorListWithoutAmsFailures.RemoveAll(e => e == NladErrorCodes.ExpectedErrorCodes[NladErrorCodes.AmsFailure]);
            return errorListWithoutAmsFailures.Count == 0;
        }

        private static bool ResultRequiresRuralFlag(DataAccessResult<ISubmitApplicationResponse> result)
        {
            if (result.Data == null || result.Data.Errors == null)
            {
                return false;
            }

            var errorListWithoutAmsFailures = new List<string>(result.Data.Errors);
            errorListWithoutAmsFailures.RemoveAll(e => e == NladErrorCodes.ExpectedErrorCodes[NladErrorCodes.AmsFailure]);
            return errorListWithoutAmsFailures.Count == 0;
        }

        private static bool IsOnlyTpivAndAddressErrors(IEnumerable<string> errorList)
        {
            return errorList.All( e =>
                e == NladErrorCodes.ExpectedErrorCodes[NladErrorCodes.TpivFail] || 
                e == NladErrorCodes.ExpectedErrorCodes[NladErrorCodes.TpivFailedOnDob] ||
                e == NladErrorCodes.ExpectedErrorCodes[NladErrorCodes.TpivFailedOnIdentity] || 
                e == NladErrorCodes.ExpectedErrorCodes[NladErrorCodes.TpivFailedOnSsn] ||
                e == NladErrorCodes.ExpectedErrorCodes[NladErrorCodes.FailedTpiv]||
                e == NladErrorCodes.ExpectedErrorCodes[NladErrorCodes.AmsFailure]||
                e == NladErrorCodes.ExpectedErrorCodes[NladErrorCodes.InvalidAddress]);
        }
  private List<string> RemoveTpivErrors(List<string> errorList) {
            errorList.RemoveAll(item => item == NladErrorCodes.ExpectedErrorCodes[NladErrorCodes.FailedTpiv] ||
            item == NladErrorCodes.ExpectedErrorCodes[NladErrorCodes.TpivFailedOnSsn] ||
            item== NladErrorCodes.ExpectedErrorCodes[NladErrorCodes.TpivFailedOnIdentity] ||
            item == NladErrorCodes.ExpectedErrorCodes[NladErrorCodes.TpivFail] || 
            item== NladErrorCodes.ExpectedErrorCodes[NladErrorCodes.TpivFailedOnDob]);
            return errorList;
            }
        private List<string> RemoveAddressErrors(List<string> errorList) {
            errorList.RemoveAll(item => item == NladErrorCodes.ExpectedErrorCodes[NladErrorCodes.FailedTpiv] ||
            item == NladErrorCodes.ExpectedErrorCodes[NladErrorCodes.AmsFailure] ||
            item == NladErrorCodes.ExpectedErrorCodes[NladErrorCodes.InvalidAddress]);
            return errorList;
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

        private string CreateResolutionRequestDescription(NladRequestData requestData, NladApiResponse nladApiResponse)
        {
            return string.Format("Resolution ID: {0}.  Agent Name/Employee ID: {1}" +
                                 " (ID: {2}).  I have reviewed the subscriber's" +
                                 " {3} to confirm the full name and date of birth and the " +
                                 "subscriber's {4} to confirm the full name and Social Security " +
                                 "Number.  Certification Statement: I certify that my submission of data to the " +
                                 "National Lifeline Accountability Database is accurate to the best of my knowledge " +
                                 "and any willful false statements made may subject me to federal criminal " +
                                 "prosecution and penalties, as well as civil penalties.", nladApiResponse.ResolutionId, requestData.TpivBypassSignature,
                                 _loggedInUserId, requestData.TpivBypassDobDocument, requestData.TpivBypassSsnDocument);
        }

        private static bool IsPhoneNumberDuplicate(DataAccessResult<ICheckStatusResponse> result)
        {
            return result.Data.Errors != null && result.Data.Errors.Any(e => e == NladErrorCodes.ExpectedErrorCodes[NladErrorCodes.DuplicatePhoneNumber]);
        }

        private static bool DuplicatePhoneNumberErrorExists(DataAccessResult<ICheckStatusResponse> result)
        {
            return result.Data != null && result.Data.Errors != null &&
                   result.Data.Errors.Any(e => e == NladErrorCodes.ExpectedErrorCodes[NladErrorCodes.DuplicatePhoneNumber]);
        }

        private static bool DuplicatePhoneNumberErrorExists(DataAccessResult<ISubmitApplicationResponse> result)
        {
            return result.Data != null && result.Data.Errors != null &&
                    result.Data.Errors.Any(e => e == NladErrorCodes.ExpectedErrorCodes[NladErrorCodes.DuplicatePhoneNumber]);
        }
    }
}

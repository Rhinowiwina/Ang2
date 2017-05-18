using System;
using System.Threading.Tasks;
using LS.Core;
using LS.Domain.ExternalApiIntegration.CaliforniaDap;
using LS.Domain.ExternalApiIntegration.Interfaces;
using LS.Repositories.ExternalApiIntegration;

namespace LS.Services.ExternalApiIntegration
{
    public class CaliDapLifelineApplicationService : BaseLifelineApplicationService<CaliDapCheckStatusRequestData, CaliDapSubmitApplicationRequestData>
    {
        public CaliDapLifelineApplicationService() : base()
        {
            _repository = new CaliDapRepository();
        }

        private ILifelineApplicationRepository _repository;
        protected override ILifelineApplicationRepository Repository
        {
            get { return _repository ?? (_repository = new CaliDapRepository()); }
            set { _repository = value; }
        }

        protected override ServiceProcessingResult<CaliDapCheckStatusRequestData> ConvertToCheckStatusRequestData(ICheckStatusRequestData requestData)
        {
            return new ServiceProcessingResult<CaliDapCheckStatusRequestData>
            {
                Data = GetCaliDapCheckStatusRequestDataFrom(requestData),
                IsSuccessful = true
            };
        }

        //protected override ServiceProcessingResult<CaliDapSubmitApplicationRequestData> ConvertToSubmitRequestData(ISubmitApplicationRequestData requestData)
        //{
        //    string generatedPhoneNumber;
        //    if (!string.IsNullOrEmpty(requestData.AssignedTelephoneNumber))
        //    {
        //        generatedPhoneNumber = requestData.AssignedTelephoneNumber;
        //    }
        //    else
        //    {
        //        var caliPhoneNuymberDataService = new CaliPhoneNumberDataService();
        //        var getPhoneNumberResult = caliPhoneNuymberDataService.GetNextValidCaliPhoneNumber(requestData.CompanyId);
        //        if (!getPhoneNumberResult.IsSuccessful)
        //        {
        //            return new ServiceProcessingResult<CaliDapSubmitApplicationRequestData>
        //            {
        //                IsSuccessful = false,
        //                Error = new ProcessingError("A valid phone number could not be generated", "Please try again later, if problem persists contact support", true)
        //            };
        //        }

        //        generatedPhoneNumber = getPhoneNumberResult.Data;
        //    }

        //    requestData.AssignedTelephoneNumber = generatedPhoneNumber;
        //    return new ServiceProcessingResult<CaliDapSubmitApplicationRequestData>
        //    {
        //        Data = GetCaliDapSubmitApplicationRequestData(requestData),
        //        IsSuccessful = true
        //    };
        //}
        
        protected override async Task<ServiceProcessingResult<ICheckStatusResponse>> SendCheckStatusRequestAsync(CaliDapCheckStatusRequestData requestData)
        {
            var result = await Repository.CheckCustomerStatusAsync(requestData);
           
                 return result.ToServiceProcessingResult(new ProcessingError(result.Error==null?"": result.Error.UserHelp,result.Error==null?"":result.Error.UserMessage, false));
         
        }

        //protected override async Task<ServiceProcessingResult<ISubmitApplicationResponse>> SendSubmitRequestAsync(CaliDapSubmitApplicationRequestData requestData)
        //{
        //    var result = await Repository.SubmitApplicationAsync(requestData);

        //    return result.ToServiceProcessingResult(new ProcessingError("", "", false));
        //}

        private static CaliDapCheckStatusRequestData GetCaliDapCheckStatusRequestDataFrom(
            ICheckStatusRequestData requestData)
        {
            return new CaliDapCheckStatusRequestData
            {
                AssignedTelephoneNumber = requestData.AssignedTelephoneNumber,
                PriorULTSTelephoneNumber = requestData.PriorULTSTelephoneNumber,
                CompanyId = requestData.CompanyId, 
                DateOfBirth = requestData.DateOfBirth,
                FirstName = requestData.FirstName,
                IsHohFlag = requestData.IsHohFlag,
                LastName = requestData.LastName,
                LifelineProgramId = requestData.LifelineProgramId,
                MiddleInitial = requestData.MiddleInitial,
                ServiceAddress1 = requestData.ServiceAddress1,
                ServiceAddress2 = requestData.ServiceAddress2,
                ServiceAddressCity = requestData.ServiceAddressCity,
                ServiceAddressState = requestData.ServiceAddressState,
                ServiceAddressZip4 = requestData.ServiceAddressZip4,
                ServiceAddressZip5 = requestData.ServiceAddressZip5,
                Ssn4 = requestData.Ssn4,
                QualifyingBeneficiaryDateOfBirth = requestData.QualifyingBeneficiaryDateOfBirth,
                QualifyingBeneficiaryFirstName = requestData.QualifyingBeneficiaryFirstName,
                QualifyingBeneficiaryLast4Ssn = requestData.QualifyingBeneficiaryLast4Ssn,
                QualifyingBeneficiaryLastName = requestData.QualifyingBeneficiaryLastName,
            };
        }

        private static CaliDapSubmitApplicationRequestData GetCaliDapSubmitApplicationRequestData(
            ISubmitApplicationRequestData requestData) {
            return new CaliDapSubmitApplicationRequestData
                {
                AssignedTelephoneNumber = requestData.AssignedTelephoneNumber,
                BillingAddress1 = requestData.BillingAddress1,
                BillingAddress2 = requestData.BillingAddress2,
                BillingCity = requestData.BillingCity,
                BillingFirstName = requestData.BillingFirstName,
                BillingLastName = requestData.BillingLastName,
                BillingMiddleInitial = requestData.BillingMiddleInitial,
                BillingState = requestData.BillingState,
                BillingZip4 = requestData.BillingZip4,
                BillingZip5 = requestData.BillingZip5,
                ServiceAddress1 = requestData.ServiceAddress1,
                ServiceAddress2 = requestData.ServiceAddress2,
                ServiceAddressCity = requestData.ServiceAddressCity,
                ServiceAddressState = requestData.ServiceAddressState,
                ServiceAddressZip4 = requestData.ServiceAddressZip4,
                ServiceAddressZip5 = requestData.ServiceAddressZip5,
                ContactPhoneNumber = requestData.ContactPhoneNumber,
                Ssn4 = requestData.Ssn4,
                DateOfBirth = requestData.DateOfBirth,
                QualifyingBeneficiaryDateOfBirth = requestData.QualifyingBeneficiaryDateOfBirth,
                QualifyingBeneficiaryFirstName = requestData.QualifyingBeneficiaryFirstName,
                QualifyingBeneficiaryLast4Ssn = requestData.QualifyingBeneficiaryLast4Ssn,
                QualifyingBeneficiaryLastName = requestData.QualifyingBeneficiaryLastName,
                FirstName = requestData.FirstName,
                LastName = requestData.LastName,
                IsHohFlag = requestData.IsHohFlag,
                };
            }
        }
}

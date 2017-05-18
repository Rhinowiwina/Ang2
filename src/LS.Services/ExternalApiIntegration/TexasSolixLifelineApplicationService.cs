using System.Threading.Tasks;
using LS.Core;
using LS.Domain.ExternalApiIntegration.TexasSolix;
using LS.Domain.ExternalApiIntegration.Interfaces;
using LS.Repositories.ExternalApiIntegration;

namespace LS.Services.ExternalApiIntegration
{
    public class TexasSolixLifelineApplicationService : BaseLifelineApplicationService<TexasSolixCheckStatusRequestData,TexasSolixSubmitApplicationRequestData> {
        public TexasSolixLifelineApplicationService() {
            _repository = new TexasSolixRepository();
            }

        private ILifelineApplicationRepository _repository;
        protected override ILifelineApplicationRepository Repository {
            get { return _repository ?? (_repository = new TexasSolixRepository()); }
            set { _repository = value; }
            }

        protected override ServiceProcessingResult<TexasSolixCheckStatusRequestData> ConvertToCheckStatusRequestData(ICheckStatusRequestData requestData) {
            return new ServiceProcessingResult<TexasSolixCheckStatusRequestData>
                {
                Data = GetTexasSolixCheckStatusRequestDataFrom(requestData),
                IsSuccessful = true
                };
            }

        //protected override ServiceProcessingResult<TexasSolixSubmitApplicationRequestData> ConvertToSubmitRequestData(ISubmitApplicationRequestData requestData)
        //{
        //    return new ServiceProcessingResult<TexasSolixSubmitApplicationRequestData>
        //    {
        //        Data = GetTexasSolixSubmitApplicationRequestData(requestData),
        //        IsSuccessful = true
        //    };
        //}


        protected override async Task<ServiceProcessingResult<ICheckStatusResponse>> SendCheckStatusRequestAsync(TexasSolixCheckStatusRequestData requestData) {
            var result = await Repository.CheckCustomerStatusAsync(requestData);

            if (result.Data.Errors.Count > 0)
                {
                return result.ToServiceProcessingResult(new ProcessingError("","",false));
                }
            else
                {
                return result.ToServiceProcessingResult(result.Error);
                }

            }

        //protected override async Task<ServiceProcessingResult<ISubmitApplicationResponse>> SendSubmitRequestAsync(TexasSolixSubmitApplicationRequestData requestData)
        //{
        //    var result = await Repository.SubmitApplicationAsync(requestData);

        //    return result.ToServiceProcessingResult(new ProcessingError("", "", false));
        //}

        private static TexasSolixCheckStatusRequestData GetTexasSolixCheckStatusRequestDataFrom(ICheckStatusRequestData requestData) {
            return new TexasSolixCheckStatusRequestData
                {
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
                Ssn4 = requestData.Ssn4
                };
            }
        //private static TexasSolixSubmitApplicationRequestData GetTexasSolixSubmitApplicationRequestData(
        //    ISubmitApplicationRequestData requestData) {
        //    return new TexasSolixSubmitApplicationRequestData
        //        {
        //        AssignedTelephoneNumber = requestData.AssignedTelephoneNumber,
        //        BillingAddress1 = requestData.BillingAddress1,
        //        BillingAddress2 = requestData.BillingAddress2,
        //        BillingCity = requestData.BillingCity,
        //        BillingFirstName = requestData.BillingFirstName,
        //        BillingLastName = requestData.BillingLastName,
        //        BillingMiddleInitial = requestData.BillingMiddleInitial,
        //        BillingState = requestData.BillingState,
        //        BillingZip4 = requestData.BillingZip4,
        //        BillingZip5 = requestData.BillingZip5,
        //        ServiceAddress1 = requestData.ServiceAddress1,
        //        ServiceAddress2 = requestData.ServiceAddress2,
        //        ServiceAddressCity = requestData.ServiceAddressCity,
        //        ServiceAddressState = requestData.ServiceAddressState,
        //        ServiceAddressZip4 = requestData.ServiceAddressZip4,
        //        ServiceAddressZip5 = requestData.ServiceAddressZip5,
        //        ContactPhoneNumber = requestData.ContactPhoneNumber,
        //        Ssn4 = requestData.Ssn4,
        //        DateOfBirth = requestData.DateOfBirth,
        //        QualifyingBeneficiaryDateOfBirth = requestData.QualifyingBeneficiaryDateOfBirth,
        //        QualifyingBeneficiaryFirstName = requestData.QualifyingBeneficiaryFirstName,
        //        QualifyingBeneficiaryLast4Ssn = requestData.QualifyingBeneficiaryLast4Ssn,
        //        QualifyingBeneficiaryLastName = requestData.QualifyingBeneficiaryLastName,
        //        FirstName = requestData.FirstName,
        //        LastName = requestData.LastName,
        //        IsHohFlag = requestData.IsHohFlag,
        //        };
        //    }
        }
    }

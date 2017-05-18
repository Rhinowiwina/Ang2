using System.Threading.Tasks;
using Common.Logging;
using LS.Core;
using LS.Domain;
using LS.Domain.ExternalApiIntegration;
using LS.Domain.ExternalApiIntegration.Interfaces;
using LS.Utilities;

namespace LS.Services.ExternalApiIntegration {
    //public abstract class BaseLifelineApplicationService<TCheckStatusRequestData, TSubmitRequestData> : ILifelineApplicationService
    //     where TCheckStatusRequestData : ICheckStatusRequestData where TSubmitRequestData : ISubmitApplicationRequestData
    public abstract class BaseLifelineApplicationService<TCheckStatusRequestData, TSubmitRequestData> : ILifelineApplicationService 
        where TCheckStatusRequestData : ICheckStatusRequestData
    {
        protected abstract ILifelineApplicationRepository Repository { get; set; }
        protected ILog Logger { get; set; }

        protected abstract ServiceProcessingResult<TCheckStatusRequestData> ConvertToCheckStatusRequestData(ICheckStatusRequestData requestData);

        //protected abstract ServiceProcessingResult<TSubmitRequestData> ConvertToSubmitRequestData(ISubmitApplicationRequestData requestData);

        protected abstract Task<ServiceProcessingResult<ICheckStatusResponse>> SendCheckStatusRequestAsync(TCheckStatusRequestData requestData);

        //protected abstract Task<ServiceProcessingResult<ISubmitApplicationResponse>> SendSubmitRequestAsync(TSubmitRequestData requestData);

        public async Task<ServiceProcessingResult<ICheckStatusResponse>> CheckCustomerStatuseAsync(ICheckStatusRequestData request)
        {
            var requestDataProcessingResult = ConvertToCheckStatusRequestData(request);
            if (!requestDataProcessingResult.IsSuccessful)
            {
                return new ServiceProcessingResult<ICheckStatusResponse>
                {
                    Error = requestDataProcessingResult.Error,
                    IsSuccessful = false
                };
            }

            var requestData = requestDataProcessingResult.Data;

            return await SendCheckStatusRequestAsync(requestData);
        }

        //public async Task<ServiceProcessingResult<ISubmitApplicationResponse>> SubmitApplicationAsync(
        //    ISubmitApplicationRequestData request)
        //{
        //    {
        //        var requestDataProcessingResult = ConvertToSubmitRequestData(request);
        //        if (!requestDataProcessingResult.IsSuccessful)
        //        {
        //            return new ServiceProcessingResult<ISubmitApplicationResponse>
        //            {
        //                Error = requestDataProcessingResult.Error,
        //                IsSuccessful = false
        //            };
        //        }

        //        var requestData = requestDataProcessingResult.Data;

        //        return await SendSubmitRequestAsync(requestData);
        //    }
        //}

        protected BaseLifelineApplicationService()
        {
            Logger = LoggerFactory.GetLogger(GetType());
        }
    }
}

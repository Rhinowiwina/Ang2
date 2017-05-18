using System.Threading.Tasks;
using LS.Core;
using LS.Domain.ExternalApiIntegration;
using LS.Domain.ExternalApiIntegration.Interfaces;
using LS.Services.ExternalApiIntegration;

namespace LS.Services
{
    public class LifelineApplicationDataService
    {
        private readonly string _loggedInUserId;

        public LifelineApplicationDataService(ExternalApi api, string loggedInUserId)
        {
            _loggedInUserId = loggedInUserId;
            Service = GetExternalApiServiceFromEnum(api);
        }

        protected ILifelineApplicationService Service { get; set; }

        public async Task<ServiceProcessingResult<ICheckStatusResponse>> CheckCustomerStatusAsync(ICheckStatusRequestData request)
        {
            return await Service.CheckCustomerStatuseAsync(request);
        }

        //public async Task<ServiceProcessingResult<ISubmitApplicationResponse>> SubmitApplicationAsync(ISubmitApplicationRequestData request)
        //{
        //    return await Service.SubmitApplicationAsync(request);
        //}

        private ILifelineApplicationService GetExternalApiServiceFromEnum(ExternalApi api)
        {
            switch (api)
            {
                 //Todo: Texas Integration
                case ExternalApi.California:
                    return new CaliDapLifelineApplicationService();
                case ExternalApi.Nlad:
                    return new NladLifelineApplicationService(_loggedInUserId);
                case ExternalApi.TexasSolix:
                    return new TexasSolixLifelineApplicationService();
                case ExternalApi.Puerto_Rico:
                    return new PuertoRicoLifelineApplicationService(_loggedInUserId);
                

                default:
                    return new CaliDapLifelineApplicationService();
            }
        }
    }
}

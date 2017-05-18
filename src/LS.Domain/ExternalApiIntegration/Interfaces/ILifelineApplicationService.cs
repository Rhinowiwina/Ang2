using System.Threading.Tasks;
using LS.Core;

namespace LS.Domain.ExternalApiIntegration.Interfaces
{
    public interface ILifelineApplicationService
    {
        Task<ServiceProcessingResult<ICheckStatusResponse>> CheckCustomerStatuseAsync(ICheckStatusRequestData request);
        //Task<ServiceProcessingResult<ISubmitApplicationResponse>> SubmitApplicationAsync(ISubmitApplicationRequestData request);
    }
}

using System.Threading.Tasks;
using LS.Core;

namespace LS.Domain.ExternalApiIntegration.Interfaces
{
    public interface ILifelineApplicationRepository
    {
        Task<DataAccessResult<ICheckStatusResponse>> CheckCustomerStatusAsync(ICheckStatusRequestData request);

        //Task<DataAccessResult<ISubmitApplicationResponse>> SubmitApplicationAsync(ISubmitApplicationRequestData request);
    }
}

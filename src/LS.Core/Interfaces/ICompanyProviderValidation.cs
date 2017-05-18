using System.Threading.Tasks;

namespace LS.Core.Interfaces
{
    public interface ICompanyProviderValidation
    {
        Task<ValidationResult<IValidatedAddress>> ScrubAddress(IValidatedAddress validatedAddress);
        Task<ValidationResult<IValidatedAddress>> ValidateAddress(IValidatedAddress validatedAddress, bool hoh);
    }
}

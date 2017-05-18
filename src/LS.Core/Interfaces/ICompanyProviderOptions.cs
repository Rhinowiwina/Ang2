using System.Collections.Generic;
using System.Threading.Tasks;


namespace LS.Core.Interfaces
{
    public interface ICompanyProviderOptions
    {
        ServiceProcessingResult<IDuplicateCheckResult> DuplicateCheck(string FirstName, string LastName, string DOB, string SSN, string LexID);
    }
}


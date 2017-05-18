using System.Collections.Generic;
using System.Threading.Tasks;
using LS.Core;
using LS.Domain;
using LS.Repositories;

namespace LS.Services
{
    public class ExternalStorageCredentialsDataService : BaseDataService<ExternalStorageCredentials, string>
    {
        //private static readonly string ProofImageType = "Proof";

        public override BaseRepository<ExternalStorageCredentials, string> GetDefaultRepository()
        {
            return new ExternalStorageCredentialsRepository();
        }

        public async Task<ServiceProcessingResult<ExternalStorageCredentials>> GetProofImageStorageCredentialsFromCompanyId(string companyId,string ProofImageType)
        {
            return await GetWhereAsync(sc => sc.CompanyId == companyId && sc.Type == ProofImageType);
        }
    }
}
using System.Threading.Tasks;
using LS.Core;
using LS.Domain;
using LS.Repositories;
using System.Collections.Generic;


namespace LS.Services
{
    public class CompanyTranslationDataService : BaseDataService<CompanyTranslations, string>
    {
        public override BaseRepository<CompanyTranslations, string> GetDefaultRepository()
        {
            return new CompanyTranslationRepository();
        }

        public async Task<ServiceProcessingResult<List<CompanyTranslations>>> GetWhereAsync(string lsProgramID, string Type)
        {
            return await GetAllWhereAsync(p => p.LSID == lsProgramID && p.Type == Type);
        }

        public async Task<ServiceProcessingResult<List<CompanyTranslations>>> GetProviderIDAsync(string lsProgramID)
        {
            return await GetAllWhereAsync(p => p.LSID == lsProgramID && p.Type == "ProviderID");
        }
        public async Task<ServiceProcessingResult<CompanyTranslations>> GetProviderByTranslatedId(int providerId)

        {
            return await GetWhereAsync(p => p.TranslatedID == providerId && p.Type == "ProviderID");
        }
    }
}

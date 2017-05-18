using System.Threading.Tasks;
using LS.Core;
using LS.Domain;
using LS.Repositories;

namespace LS.Services
{
    public class StateSettingsDataService : BaseDataService<StateSettings, string>
    {
        public override BaseRepository<StateSettings, string> GetDefaultRepository()
        {
            return new StateSettingsRepository();
        }

        public async Task<ServiceProcessingResult<StateSettings>> GetStateSettingsByStateCodeAsync(string stateCode)
        {
            return await GetWhereAsync(ss => ss.StateCode == stateCode);
        }
    }
}

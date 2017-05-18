using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class StateSettingsRepository : BaseRepository<StateSettings, string>
    {
        public StateSettingsRepository() : base(new AmbientDbContextLocator())
        {
        }
    }
}

using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class DevDataRepository : BaseRepository<DevData,string>
    {
        public DevDataRepository() : base(new AmbientDbContextLocator())
        {

        }
    }
}

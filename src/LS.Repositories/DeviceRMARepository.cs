using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class DeviceRMARepository : BaseRepository<DeviceRMA,string>
    {
        public DeviceRMARepository() : base(new AmbientDbContextLocator())
        {

        }
    }
}

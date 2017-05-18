using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class DeviceSwapsRepository : BaseRepository<DeviceSwaps, string>
    {
        public DeviceSwapsRepository() : base(new AmbientDbContextLocator())
        {

        }
    }
}

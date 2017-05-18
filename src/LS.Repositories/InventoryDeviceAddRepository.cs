using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class InventoryDeviceAddRepository : BaseRepository<InventoryDeviceAdd, string>
    {
        public InventoryDeviceAddRepository() : base(new AmbientDbContextLocator())
        {
        }
    }
}

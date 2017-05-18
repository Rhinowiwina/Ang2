using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class InventoryCycleCountDeviceRepository : BaseRepository<InventoryCycleCountDevice, string>
    {
        public InventoryCycleCountDeviceRepository() : base(new AmbientDbContextLocator())
        {
        }
    }
}

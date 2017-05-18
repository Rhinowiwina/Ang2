using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class InventoryCycleCountRepository : BaseRepository<InventoryCycleCount, string>
    {
        public InventoryCycleCountRepository() : base(new AmbientDbContextLocator())
        {
        }
    }
}

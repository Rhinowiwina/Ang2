using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class InventoryCycleCountSimRepository : BaseRepository<InventoryCycleCountSim, string>
    {
        public InventoryCycleCountSimRepository() : base(new AmbientDbContextLocator())
        {
        }
    }
}

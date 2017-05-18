using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class InventorySimAddRepository : BaseRepository<InventorySimAdd, string>
    {
        public InventorySimAddRepository() : base(new AmbientDbContextLocator())
        {
        }
    }
}

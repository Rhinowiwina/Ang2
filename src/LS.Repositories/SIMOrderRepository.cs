using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class SIMOrderRepository : BaseRepository<SIMOrder, string>
    {
        public SIMOrderRepository() : base(new AmbientDbContextLocator())
        {

        }
    }
}

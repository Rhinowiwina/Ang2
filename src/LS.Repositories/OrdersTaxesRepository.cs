using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class OrdersTaxesRepository : BaseRepository<OrdersTaxes, string>
    {
        public OrdersTaxesRepository() : base(new AmbientDbContextLocator())
        {
        }
    }
}

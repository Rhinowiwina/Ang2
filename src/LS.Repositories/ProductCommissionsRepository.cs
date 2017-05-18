using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class ProductCommissionsRepository : BaseRepository<ProductCommissions,string>
    {
        public ProductCommissionsRepository() : base(new AmbientDbContextLocator())
        {

        }
    }
}

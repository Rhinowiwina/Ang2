using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class CarriersRepository : BaseRepository<Carriers, string>
    {
        public CarriersRepository() : base(new AmbientDbContextLocator())
        {
        }
    }
}

using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class ZipCodeRepository : BaseRepository<ZipCode, string>
    {
        public ZipCodeRepository() : base(new AmbientDbContextLocator())
        {

        }
    }
}

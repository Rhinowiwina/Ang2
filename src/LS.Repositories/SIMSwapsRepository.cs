using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class SIMSwapsRepository : BaseRepository<SIMSwaps, string>
    {
        public SIMSwapsRepository() : base(new AmbientDbContextLocator())
        {

        }
    }
}

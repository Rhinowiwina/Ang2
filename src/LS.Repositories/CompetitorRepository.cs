using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class CompetitorRepository : BaseRepository<Competitor, string>
    {
        public CompetitorRepository() : base(new AmbientDbContextLocator())
        {
            
        }
    }
}

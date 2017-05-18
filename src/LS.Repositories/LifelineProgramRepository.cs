using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class LifelineProgramRepository : BaseRepository<LifelineProgram, string>
    {
        public LifelineProgramRepository() : base(new AmbientDbContextLocator())
        {
        }
    }
}

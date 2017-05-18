using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class StateProgramRepository : BaseRepository<StateProgram, string>
    {
        public StateProgramRepository() : base(new AmbientDbContextLocator())
        {
        }
    }
}

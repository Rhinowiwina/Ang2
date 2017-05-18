using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class BaseIncomeLevelsRepository : BaseRepository<BaseIncomeLevels, string>
    {
        public BaseIncomeLevelsRepository() : base(new AmbientDbContextLocator())
        {
        }
    }
}

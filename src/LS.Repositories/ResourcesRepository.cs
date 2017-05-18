using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class ResourcesRepository : BaseRepository<Resources, string>
    {
        public ResourcesRepository() : base(new AmbientDbContextLocator())
        {
        }
    }
}

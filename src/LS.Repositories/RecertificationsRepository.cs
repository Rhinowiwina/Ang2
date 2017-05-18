using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class RecertificationsRepository : BaseRepository<Recertifications,string>
    {
        public RecertificationsRepository() : base(new AmbientDbContextLocator())
        {

        }
    }
}

using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class ResponseLogsCGMEHDBRepository : BaseRepository<ResponseLogsCGMEHDB, string>
    {
        public ResponseLogsCGMEHDBRepository() : base(new AmbientDbContextLocator())
        {
        }
    }
}

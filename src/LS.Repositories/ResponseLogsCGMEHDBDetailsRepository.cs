using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class ResponseLogsCGMEHDBDetailsRepository : BaseRepository<ResponseLogsCGMEHDBDetails, string>
    {
        public ResponseLogsCGMEHDBDetailsRepository() : base(new AmbientDbContextLocator())
        {
        }
    }
}

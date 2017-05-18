using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class ExternalStorageCredentialsRepository : BaseRepository<ExternalStorageCredentials, string>
    {
        public ExternalStorageCredentialsRepository() : base(new AmbientDbContextLocator())
        {
        }
    }
}

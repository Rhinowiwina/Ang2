using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class LoginInfoRepository : BaseRepository<LoginInfo, string>
    {
        public LoginInfoRepository() : base(new AmbientDbContextLocator())
        {
        }
    }
}

using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class UserCommissionDetailRepository : BaseRepository<UserCommissionDetail, string>
    {
        public UserCommissionDetailRepository() : base(new AmbientDbContextLocator())
        {
        }
    }
}

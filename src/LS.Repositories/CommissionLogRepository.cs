using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class CommissionLogRepository : BaseRepository<CommissionLog, string>
    {
        public CommissionLogRepository() : base(new AmbientDbContextLocator())
        {
        }
    }
}

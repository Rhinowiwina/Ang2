using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class TenantAccountFulfillmentLogRepository : BaseRepository<TenantAccountFulfillmentLog, string>
    {
        public TenantAccountFulfillmentLogRepository() : base(new AmbientDbContextLocator())
        {
        }


    }
}

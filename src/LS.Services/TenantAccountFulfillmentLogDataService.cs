using System.Collections.Generic;
using System.Threading.Tasks;
using LS.Core;
using LS.Repositories;
using LS.Domain;
using LS.Core.Interfaces;
using LS.Services.Factories;

namespace LS.Services
{
    public class TenantAccountFulfillmentLogDataService : BaseDataService<TenantAccountFulfillmentLog, string>
    {
        public override BaseRepository<TenantAccountFulfillmentLog, string> GetDefaultRepository()
        {
            return new TenantAccountFulfillmentLogRepository();
        }
    }
}

using LS.Core;
using LS.Domain;
using LS.Repositories;

namespace LS.Services
{
    public class ApplicationRoleDataService : BaseDataService<ApplicationRole, string>
    {
        public override BaseRepository<ApplicationRole, string> GetDefaultRepository()
        {
            return new ApplicationRoleRepository();
        }

        public ServiceProcessingResult<ApplicationRole> GetApplicationRoleByRank(int rank)
        {
            return GetWhere(r => r.Rank == rank);
        }
    }
}

using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class RedemptionCodeRepository : BaseRepository<RedemptionCode, string>
    {
        public RedemptionCodeRepository()
            : base(new AmbientDbContextLocator())
        {
        }
    }
}

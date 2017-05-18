using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class StateAgreementRepository : BaseRepository<StateAgreement, string>
    {
        public StateAgreementRepository() : base(new AmbientDbContextLocator())
        {
        }
    }
}

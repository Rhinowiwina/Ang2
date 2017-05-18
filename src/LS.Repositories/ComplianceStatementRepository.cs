using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class ComplianceStatementRepository : BaseRepository<ComplianceStatement, string>
    {
        public ComplianceStatementRepository() : base(new AmbientDbContextLocator())
        {
        }
    }
}

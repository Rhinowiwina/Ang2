using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class ProofDocumentTypeRepository : BaseRepository<ProofDocumentType, string>
    {
        public ProofDocumentTypeRepository() : base(new AmbientDbContextLocator())
        {
        }
    }
}

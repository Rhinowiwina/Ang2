using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class TpivProofDocumentTypeRepository : BaseRepository<TpivProofDocumentType, string>
    {
        public TpivProofDocumentTypeRepository() : base(new AmbientDbContextLocator())
        {
        }
    }
}

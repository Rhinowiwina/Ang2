using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class AddressBypassProofDocumentTypeRepository : BaseRepository<AddressProofDocumentType, string>
    {
        public AddressBypassProofDocumentTypeRepository() : base(new AmbientDbContextLocator())
        {
        }
    }
}

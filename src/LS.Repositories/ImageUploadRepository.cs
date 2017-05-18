using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class ImageUploadRepository : BaseRepository<ImageUpload, string>
    {
        public ImageUploadRepository() : base(new AmbientDbContextLocator())
        {
        }
    }
}

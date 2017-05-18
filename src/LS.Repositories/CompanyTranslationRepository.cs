using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class CompanyTranslationRepository : BaseRepository<CompanyTranslations, string>
    {
        public CompanyTranslationRepository() : base(new AmbientDbContextLocator())
        {
        }
    }
}

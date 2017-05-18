using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    public class CompanyTranslationsEFConfig : EntityTypeConfiguration<CompanyTranslations>
    {
        public CompanyTranslationsEFConfig()
        {
            Property(c => c.Type).HasMaxLength(100);
        }
    }
}

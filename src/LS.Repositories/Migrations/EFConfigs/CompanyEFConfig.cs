using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    public class CompanyEFConfig : EntityTypeConfiguration<Company>
    {
        public CompanyEFConfig()
        {
            HasMany(s => s.Systems)
               .WithMany();
           
            Property(c => c.Name).HasMaxLength(100);
        }
    }
}

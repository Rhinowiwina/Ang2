using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    class CompetitorEFConfig : EntityTypeConfiguration<Competitor>
    {
        public CompetitorEFConfig()
        {
            HasRequired(c => c.Company);

            Property(c => c.Name)
                .HasMaxLength(100);

            Property(c => c.StateCode)
                .HasMaxLength(2);
        }
    }
}

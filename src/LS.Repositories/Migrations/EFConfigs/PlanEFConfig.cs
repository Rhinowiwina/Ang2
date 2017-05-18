using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    class PlanEFConfig : EntityTypeConfiguration<Plan>
    {
        public PlanEFConfig()
        {
            HasRequired(p => p.Company);

            Property(p => p.Name)
                .HasMaxLength(100);

            Property(p => p.StateCode)
                .HasMaxLength(2);

            Property(p => p.Price)
                .HasPrecision(9,2);

            Property(p => p.Minutes)
                .HasColumnType("INT");

            Property(p => p.Texts)
                .HasColumnType("INT");

            Property(p => p.Data)
                .HasColumnType("FLOAT");
        }
    }
}

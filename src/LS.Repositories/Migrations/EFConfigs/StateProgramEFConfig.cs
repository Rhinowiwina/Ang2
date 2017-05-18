using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    class StateProgramEFConfig : EntityTypeConfiguration<StateProgram>
    {
        public StateProgramEFConfig()
        {
            Property(p => p.StateCode)
                .HasMaxLength(2);

            Property(p => p.Name)
                .HasMaxLength(100);
        }
    }
}

using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    class LifelineProgramEFConfig : EntityTypeConfiguration<LifelineProgram>
    {
        public LifelineProgramEFConfig()
        {
            Property(p => p.StateCode)
                .HasMaxLength(2);

            Property(p => p.ProgramName)
                .HasMaxLength(150);
        }
    }
}

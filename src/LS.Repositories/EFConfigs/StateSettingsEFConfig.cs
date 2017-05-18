using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    class StateSettingsEFConfig : EntityTypeConfiguration<StateSettings>
    {
        public StateSettingsEFConfig()
        {
            Property(s => s.StateCode)
                .HasMaxLength(2);

            Property(s => s.SsnType)
                .HasMaxLength(10);
        }
    }
}

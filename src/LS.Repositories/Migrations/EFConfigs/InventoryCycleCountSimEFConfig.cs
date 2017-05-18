using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    class InventoryCycleCountSimEFConfig : EntityTypeConfiguration<InventoryCycleCountSim>
    {
        public InventoryCycleCountSimEFConfig()
        {
            HasRequired(u => u.ApplicationUser);
            Property(c => c.ICCID).HasMaxLength(100);
            Property(c => c.IMSI).HasMaxLength(100);
        }
    }
}

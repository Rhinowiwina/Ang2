using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    class InventoryCycleCountDeviceEFConfig : EntityTypeConfiguration<InventoryCycleCountDevice>
    {
        public InventoryCycleCountDeviceEFConfig()
        {
            HasRequired(u => u.ApplicationUser);
            Property(c => c.Hex).HasMaxLength(100);
            Property(c => c.Dec).HasMaxLength(100);
        }
    }
}

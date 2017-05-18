using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    class InventoryDeviceAddEFConfig : EntityTypeConfiguration<InventoryDeviceAdd>
    {
        public InventoryDeviceAddEFConfig()
        {
            HasRequired(u => u.ApplicationUser);
            HasRequired(c => c.SalesTeam);
            Property(c => c.Hex).HasMaxLength(100);
            Property(c => c.Dec).HasMaxLength(100);
        }
    }
}

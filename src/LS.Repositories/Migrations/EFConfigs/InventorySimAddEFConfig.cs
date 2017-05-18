using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    class InventorySimAddEFConfig : EntityTypeConfiguration<InventorySimAdd>
    {
        public InventorySimAddEFConfig()
        {
            HasRequired(u => u.ApplicationUser);
            HasRequired(c => c.SalesTeam);
            Property(c => c.ICCID).HasMaxLength(100);
            Property(c => c.IMSI).HasMaxLength(100);
        }
    }
}

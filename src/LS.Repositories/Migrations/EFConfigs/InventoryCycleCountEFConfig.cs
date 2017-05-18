using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    class InventoryCycleCountEFConfig : EntityTypeConfiguration<InventoryCycleCount>
    {
        public InventoryCycleCountEFConfig()
        {
            HasRequired(c => c.SalesTeam);
        }
    }
}

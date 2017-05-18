using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    class CarriersEFConfig : EntityTypeConfiguration<Carriers>
    {
        public CarriersEFConfig()
        {
            Property(c => c.Name).HasMaxLength(50);
            Property(c => c.NetworkType).HasMaxLength(50);
            Property(c => c.SortOrder);
        }
    }
}

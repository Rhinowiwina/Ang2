using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    class ProductCommissionEFConfig : EntityTypeConfiguration<ProductCommissions>
    {
        public ProductCommissionEFConfig()
        {
            Property(p => p.ProductType).HasMaxLength(100);
            Property(p => p.RecipientType).HasMaxLength(100);
            Property(p => p.SalesTeamID).HasMaxLength(128);
        }
    }
}

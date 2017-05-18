using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    public class OrdersTaxesEFConfig : EntityTypeConfiguration<OrdersTaxes>
    {
        public OrdersTaxesEFConfig()
        {
            Property(t => t.OrderType).HasMaxLength(10);
            Property(t => t.Description).HasMaxLength(500);
            Property(t => t.Type).HasMaxLength(100);
            Property(t => t.OrderID).HasMaxLength(128);
        }
    }
}

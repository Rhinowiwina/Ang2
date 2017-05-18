using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    public class DeviceOrderEFConfig : EntityTypeConfiguration<DeviceOrder>
    {
        public DeviceOrderEFConfig()
        {
            Property(o => o.Id).HasMaxLength(128);
            Property(o => o.PONumber).HasMaxLength(128);
            Property(o => o.InvoiceNumber).HasMaxLength(128);
            Property(o => o.OrderDate).HasColumnType("DATE");
            Property(o => o.ShipDate).HasColumnType("DATE");
            Property(o => o.AgentDueDate).HasColumnType("DATE");
            Property(o => o.ASGDueDate).HasColumnType("DATE");
            Property(o => o.Level1SalesGroupID).HasMaxLength(128);
            Property(o => o.IsReturned);
        }
    }
}

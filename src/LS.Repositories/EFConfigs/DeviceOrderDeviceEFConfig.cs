using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    public class DeviceOrderDeviceEFConfig : EntityTypeConfiguration<DeviceOrderDevice>
    {
        public DeviceOrderDeviceEFConfig()
        {
            Property(o => o.Id).HasMaxLength(100);
            Property(o => o.OrderID).HasMaxLength(100);
            Property(o => o.IMEI).HasMaxLength(100);
            Property(o => o.PartNumber).HasMaxLength(100);
            Property(o => o.ASGPrice);
            Property(o => o.AgentPrice);
            Property(o => o.IsReturned);
        }
    }
}

using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    class DeviceEFConfig : EntityTypeConfiguration<Device>
    {
        public DeviceEFConfig()
        {
            Property(d => d.Name)
                .HasColumnType("VARCHAR")
                .HasMaxLength(100);

            Property(d => d.Price)
                .HasColumnType("MONEY");
        }
    }
}

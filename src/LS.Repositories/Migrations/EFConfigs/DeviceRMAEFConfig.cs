using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    class DeviceRMAEFConfig : EntityTypeConfiguration<DeviceRMA>
    {
        public DeviceRMAEFConfig()
        {
            Property(c => c.DeviceID).HasMaxLength(150);
            Property(c => c.TeamID).HasMaxLength(128);
            Property(c => c.UserID).HasMaxLength(128);
            Property(c => c.UserID).HasMaxLength(128);
            Property(c => c.Reason).HasMaxLength(200);
            Property(c => c.Notes);
            Property(c => c.Activated).HasMaxLength(128);
        }
    }
}

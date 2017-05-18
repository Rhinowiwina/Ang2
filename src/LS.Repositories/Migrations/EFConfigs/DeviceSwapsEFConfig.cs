using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    class DeviceSwapsEFConfig : EntityTypeConfiguration<DeviceSwaps>
    {
        public DeviceSwapsEFConfig()
        {
            Property(c => c.SalesTeamId).HasMaxLength(128);
            Property(c => c.UserId).HasMaxLength(128);
            Property(c => c.TenantAccountID).HasMaxLength(128);
            Property(c => c.OldDeviceID).HasMaxLength(128);
            Property(c => c.NewDeviceID).HasMaxLength(128);
            Property(c => c.TenantCarrierID).HasMaxLength(128);
            Property(c => c.CompanyID).HasMaxLength(128);
            Property(c => c.Notes);
            Property(c => c.Reason).HasMaxLength(128);
        }
    }
}

using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    public class TenantAccountFulfillmentLogEFConfig : EntityTypeConfiguration<TenantAccountFulfillmentLog>
    {
        public TenantAccountFulfillmentLogEFConfig()
        {
            Property(t => t.TenantAccountID).HasMaxLength(100);
            Property(t => t.CompanyID).HasMaxLength(128);
            Property(t => t.OrderID).HasMaxLength(128);
            Property(t => t.DeviceID).HasMaxLength(128);
            Property(t => t.IMSI).HasMaxLength(100);
            Property(t => t.ProviderID).HasMaxLength(128);
            Property(t => t.Added_By_UserID).HasMaxLength(128);
            Property(t => t.Added_By_TeamID).HasMaxLength(128);
        }
    }
}

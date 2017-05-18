using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    class SIMOrderEFConfig : EntityTypeConfiguration<SIMOrder>
    {
        public SIMOrderEFConfig()
        {
            Property(c => c.SalesTeamID).HasMaxLength(128);
            Property(c => c.CompanyId).HasMaxLength(128);
            Property(c => c.UserID).HasMaxLength(128);
            Property(c => c.Quantity);
            Property(c => c.Total);
            Property(c => c.Cost);
            Property(c => c.FirstName).HasMaxLength(128);
            Property(c => c.LastName).HasMaxLength(128);
            Property(c => c.BillingAddress).HasMaxLength(128);
            Property(c => c.BillingAddress2).HasMaxLength(128);
            Property(c => c.BillingCity).HasMaxLength(128);
            Property(c => c.BillingState).HasMaxLength(128);
            Property(c => c.BillingZip).HasMaxLength(128);
            Property(c => c.ShippingAddress).HasMaxLength(128);
            Property(c => c.ShippingAddress2).HasMaxLength(128);
            Property(c => c.ShippingCity).HasMaxLength(128);
            Property(c => c.ShippingState).HasMaxLength(128);
            Property(c => c.ShippingZip).HasMaxLength(128);
            Property(c => c.DateOrdered);
            Property(c => c.Status).HasMaxLength(128);
            Property(c => c.DateOrdered);
        }
    }
}

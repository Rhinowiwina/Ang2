using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    class CommissionLogEFConfig : EntityTypeConfiguration<CommissionLog>
    {
        public CommissionLogEFConfig()
        {
            Property(c => c.OrderId).IsRequired().HasMaxLength(128);
            HasOptional(c => c.User).WithMany().HasForeignKey(u => u.RecipientUserId);
            HasOptional(c => c.SalesTeam).WithMany().HasForeignKey(u => u.SalesTeamId);
            HasOptional(c => c.Payment).WithMany().HasForeignKey(u => u.PaymentID);
            Property(c => c.RecipientType).IsRequired();
            Property(c => c.OrderType).IsRequired();
        }
    }
}

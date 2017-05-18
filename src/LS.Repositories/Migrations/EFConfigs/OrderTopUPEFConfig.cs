using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    class OrderTopUpEFConfig : EntityTypeConfiguration<OrderTopUp>
    {
        public OrderTopUpEFConfig()
        {
            Property(c => c.UserID).HasMaxLength(128);
            Property(c => c.BudgetMobileID).HasMaxLength(128);
            Property(c => c.Price);
            Property(c => c.TopUpDesc).HasMaxLength(300);
            Property(c => c.TopUpID).HasMaxLength(128);
            Property(c => c.PaymentType).HasMaxLength(100);
            Property(c => c.SalesTeamID).HasMaxLength(128);
        }
    }
}

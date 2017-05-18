using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    public class PaymentTransactionLogEFConfig : EntityTypeConfiguration<PaymentTransactionLog>
    {
        public PaymentTransactionLogEFConfig()
        {
            Property(p => p.Id).HasMaxLength(128);
            Property(p => p.TransactionID);
            Property(p => p.CompanyID).HasMaxLength(128);
            Property(p => p.TransactionType).HasMaxLength(50);
            Property(p => p.OrderType).HasMaxLength(50);
            Property(p => p.MerchantID).HasMaxLength(128);
            Property(p => p.ReferenceTransaction).HasMaxLength(128);
            Property(p => p.Amount);
        }
    }
}

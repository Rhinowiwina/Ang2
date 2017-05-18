using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    class PaymentEFConfig : EntityTypeConfiguration<Payment>
    {
        public PaymentEFConfig()
        {

        }
    }
}

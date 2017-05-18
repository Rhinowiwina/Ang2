using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    class TransactionLogEFConfig : EntityTypeConfiguration<TransactionLog>
    {
        public TransactionLogEFConfig()
        {
           
        }
    }
}

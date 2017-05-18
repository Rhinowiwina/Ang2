using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class PaymentTransactionLogRepository : BaseRepository<PaymentTransactionLog, string>
    {
        public PaymentTransactionLogRepository() : base(new AmbientDbContextLocator())
        {
        }
    }
}

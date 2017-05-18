using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class PaymentRepository : BaseRepository<Payment, string>
    {
        public PaymentRepository() : base(new AmbientDbContextLocator())
        {
        }
    }
}

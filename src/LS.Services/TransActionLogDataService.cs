using LS.Domain;
using LS.Repositories;

namespace LS.Services
{
    public class TransactionLogDataService : BaseDataService<TransactionLog, string>
    {
        public override BaseRepository<TransactionLog, string> GetDefaultRepository()
        {
            return new TransactionLogRepository();
        }
    }
}
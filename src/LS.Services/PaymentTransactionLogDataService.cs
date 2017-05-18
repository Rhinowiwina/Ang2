using System;
using System.Threading.Tasks;
using LS.Core;
using LS.Domain;
using LS.Repositories;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace LS.Services
{
    public class PaymentTransactionLogDataService : BaseDataService<PaymentTransactionLog, string>
    {
        public override BaseRepository<PaymentTransactionLog, string> GetDefaultRepository()
        {
            return new PaymentTransactionLogRepository();
        }
    }
}

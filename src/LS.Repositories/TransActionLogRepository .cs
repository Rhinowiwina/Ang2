using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinqKit;
using LS.Core;
using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class TransactionLogRepository : BaseRepository<TransactionLog, string>
    {
        public TransactionLogRepository() : base(new AmbientDbContextLocator())
        {
        }

      
    }
}

using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Linq;
using LS.Core;
using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class OrderTopUpRepository : BaseRepository<OrderTopUp, string>
    {
        public OrderTopUpRepository() : base(new AmbientDbContextLocator())
        {

        }
    }
}
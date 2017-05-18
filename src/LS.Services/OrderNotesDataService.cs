using LS.Domain;
using LS.Repositories;
using LS.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LS.Utilities;

namespace LS.Services
{
    public class OrderNotesDataService : BaseDataService<OrderNote, string>
    {
        public override BaseRepository<OrderNote, string> GetDefaultRepository()
        {
            return new OrderNotesRepository();
        }
    }
}

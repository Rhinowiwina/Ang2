using System;
using System.Data.Entity;
using System.Linq;
using LS.Core;
using LS.Domain;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class OrderNotesRepository : BaseRepository<OrderNote, string>
    {
        public OrderNotesRepository() : base(new AmbientDbContextLocator())
        {

        }
    }
}

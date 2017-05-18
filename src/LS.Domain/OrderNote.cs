using System;
using LS.Core.Interfaces;

namespace LS.Domain
{
    public class OrderNote : IEntity<string>
    {
        public OrderNote()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string OrderID { get; set; }
        public string Note { get; set; }
        public string AddedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}

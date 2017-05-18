using System;
using LS.Core.Interfaces;
using System.Collections.Generic;
namespace LS.Domain
{
    public class OrdersTaxes : IEntity<string>
    {
        public OrdersTaxes()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string OrderID { get; set; }
        public string OrderType { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}

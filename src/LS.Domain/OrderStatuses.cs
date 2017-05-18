using System;
using LS.Core.Interfaces;

namespace LS.Domain
{
    public class OrderStatuses
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int StatusCode { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}

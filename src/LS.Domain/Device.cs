using System;
using LS.Core.Interfaces;

namespace LS.Domain {
    public class Device : IEntity<string> {
        public Device() {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string CompanyId { get; set; }
        public Company Company { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}

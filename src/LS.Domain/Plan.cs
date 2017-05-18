using System;
using LS.Core.Interfaces;

namespace LS.Domain
{
    public class Plan : IEntity<string>
    {
        public Plan()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string CompanyId { get; set; }
        public Company Company { get; set; }
        public string StateCode { get; set; }
        public decimal Price { get; set; }
        public int Minutes { get; set; }
        public int Texts { get; set; }
        public float Data { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}

using System;
using LS.Core.Interfaces;
using System.Runtime.Serialization;

namespace LS.Domain
{
    public class ZipCode : IEntity<string>
    {
        public ZipCode()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string PostalCode { get; set; }
        public string State { get; set; }
        public string StateAbbreviation { get; set; }
        public string CountyFips { get; set; }
        public string City { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}

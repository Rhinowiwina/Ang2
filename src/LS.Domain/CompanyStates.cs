using System;
using LS.Core.Interfaces;
using System.Runtime.Serialization;

namespace LS.Domain
{
    public class CompanyStates : IEntity<string> {
        public CompanyStates() {
            Id = Guid.NewGuid().ToString();
            }

        public string Id { get; set; }
        public bool IsActive { get; set; }
        public string CompanyID{get;set;}
        public string StateCode { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}

using System;
using LS.Core.Interfaces;

namespace LS.Domain
{
    public class ComplianceStatement : IEntity<string>
    {
        public ComplianceStatement()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string CompanyId { get; set; }
        public Company Company { get; set; }
        public string StateCode { get; set; }
        public string Statement { get; set; }
        public string Statement_ES { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}

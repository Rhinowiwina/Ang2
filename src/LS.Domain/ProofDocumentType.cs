using System;
using LS.Core.Interfaces;

namespace LS.Domain
{
    public class ProofDocumentType : IEntity<string>
    {
        public ProofDocumentType()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string ProofType { get; set; }
        public string Name { get; set; }
        public string StateCode { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}
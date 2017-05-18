using System;
using LS.Core.Interfaces;

namespace LS.Domain
{
    public class TpivProofDocumentType : IEntity<string>
    {
        public TpivProofDocumentType()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string LifelineSystem { get; set; }
        public string LifelineSystemId { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}

using System;
using LS.Core.Interfaces;

namespace LS.Domain.ExternalApiIntegration.PuertoRico
{
    public class PuertoRicoPhoneNumber : IEntity<string>
    {
        public PuertoRicoPhoneNumber()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public bool IsDeleted { get; set; }

        public long Number { get; set; }

        public string CompanyId { get; set; }

        public Company Company { get; set; }

        public bool IsCurrentlyInUse { get; set; }
    }
}

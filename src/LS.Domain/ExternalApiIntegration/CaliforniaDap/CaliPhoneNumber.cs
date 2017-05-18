using System;
using LS.Core.Interfaces;

namespace LS.Domain.ExternalApiIntegration.CaliforniaDap
{
    public class CaliPhoneNumber : IEntity<string>
    {
        public CaliPhoneNumber()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public bool IsDeleted { get; set; }

        public int AreaCode { get; set; }

        public int Number { get; set; }

        public string CompanyId { get; set; }

        public Company Company { get; set; }
    }
}

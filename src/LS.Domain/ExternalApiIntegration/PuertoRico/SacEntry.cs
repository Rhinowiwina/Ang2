using System;
using LS.Core.Interfaces;

namespace LS.Domain.ExternalApiIntegration.PuertoRico
{
    public class SacEntry : IEntity<string>
    {
        public SacEntry()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string StateCode { get; set; }
        public int SacNumber { get; set; }
        public string Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}

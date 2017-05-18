using System;
using LS.Core.Interfaces;

namespace LS.Domain
{
    public class StateSettings : IEntity<string>
    {
        public StateSettings()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string StateCode { get; set; }
        public string SsnType { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}

using System;
using LS.Core.Interfaces;

namespace LS.Domain
{
    public class StateProgram : IEntity<string>
    {
        public StateProgram()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string StateCode { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}

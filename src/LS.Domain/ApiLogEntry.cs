using System;
using LS.Core.Interfaces;

namespace LS.Domain
{
    public class ApiLogEntry : IEntity<string>
    {
        public ApiLogEntry()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string Api { get; set; }
        public string Function { get; set; }
        public string Input { get; set; }
        public string Response { get; set; }
        public Boolean JsonImported { get; set; } = false;
        public DateTime DateStarted { get; set; } = DateTime.UtcNow;
        public DateTime? DateEnded { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime DateModified { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; }
    }
}

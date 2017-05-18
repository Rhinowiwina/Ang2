using System;
using System.ComponentModel.DataAnnotations.Schema;
using LS.Core.Interfaces;

namespace LS.Domain
{
    public class WebApplicationLogEntry : IEntity<Guid?>
    {
        public Guid? Id { get; set; }
        public DateTime? TimeStamp { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public string CallSite { get; set; }
        public string ProcessId { get; set; }
        public string ThreadId { get; set; }
        [NotMapped]
        public DateTime DateCreated { get; set; }
        [NotMapped]
        public DateTime DateModified { get;set; }
        [NotMapped]
        public bool IsDeleted { get; set; }
    }
}

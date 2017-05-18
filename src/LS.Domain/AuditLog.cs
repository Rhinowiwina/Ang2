using System;
using LS.Core.Interfaces;

namespace LS.Domain
{
    public class AuditLog
    {
        public AuditLog() {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }
        public string ModifiedByUserID { get; set; }
        public DateTime DateCreated { get; set; }
        public string TableName { get; set; }
        public string TableRowID { get; set; }
        public string TablePreviousData { get; set; }
        public string Reason { get; set; }
    }
}
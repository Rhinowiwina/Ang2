using System;
using System.Collections.Generic;
using LS.Core.Interfaces;
using LS.Domain.ExternalApiIntegration.Nlad;


namespace LS.Domain
{
    public class ResponseLogsCGMEHDB : IEntity<string>
    {

        public ResponseLogsCGMEHDB()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }
        public string TransactionId { get; set; }
        public string APILogEntriesID { get; set; }
        public DateTime? LogDate { get; set; }
        public Boolean Blacklist { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public virtual ICollection<ResponseLogsCGMEHDBDetails> EhdbDetails { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}

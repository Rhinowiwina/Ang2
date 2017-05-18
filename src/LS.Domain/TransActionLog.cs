using System;
using System.Collections.Generic;
using LS.Core.Interfaces;
using LS.Domain.ExternalApiIntegration.Nlad;


namespace LS.Domain
{
    public class TransactionLog : IEntity<string>
    {
        public TransactionLog()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string SalesTeamId { get; set; }
        public string UserId { get; set; }
        public string Type { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}

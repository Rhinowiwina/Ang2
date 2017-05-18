using System;
using System.Collections.Generic;
using LS.Core.Interfaces;
using LS.Domain.ExternalApiIntegration.Nlad;

namespace LS.Domain
{
    public class Payment : IEntity<string>
    {
        public Payment()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public decimal Amount { get; set; }
        public string Email { get; set; }
        public string TransactionID { get; set; }
        public DateTime? DatePaid { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}

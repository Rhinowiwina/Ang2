using System;
using LS.Core.Interfaces;

namespace LS.Domain
{
    public class CommissionLog : IEntity<string>
    {
        public CommissionLog()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string OrderId { get; set; }
        public ApplicationUser User { get; set; }
        public string RecipientUserId { get; set; }
        public SalesTeam SalesTeam { get; set; }
        public string SalesTeamId { get; set; }
        public decimal? Amount { get; set; }
        public Payment Payment { get; set; }
        public string PaymentID { get; set; }
        public string ProcessID { get; set; }
        public string OrderType { get; set; }
        public string RecipientType { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }

    }
}

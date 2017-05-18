using System;
using LS.Core.Interfaces;

namespace LS.Domain
{
    public class UserCommissionDetail : IEntity<string>
    {
        public UserCommissionDetail()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }

        public Order Order { get; set; }

        public string OrderId { get; set; }

        public ApplicationUser User { get; set; }

        public string UserId { get; set; }

        public SalesTeam SalesTeam { get; set; }

        public string SalesTeamId { get; set; }

        public decimal? Amount { get; set; }

        public Payment Payment { get; set; }

        public string PaymentID { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public bool IsDeleted { get; set; }

    }
}

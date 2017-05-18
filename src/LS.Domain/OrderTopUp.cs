using System;
using LS.Core.Interfaces;
using System.Collections.Generic;
namespace LS.Domain
{
    public class OrderTopUp : IEntity<string>
    {
        public OrderTopUp()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string UserID { get; set; }
        public string SalesTeamID { get; set; }
        public string BudgetMobileID { get; set; }
        public double Price { get; set; }
        public double OrderTotal { get; set; }
        public string TopUpDesc { get; set; }
        public string TopUpID { get; set; }
        public string ReferenceID { get; set; }
        public string CAMsOrderID { get; set; }
        public string PaymentType { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}

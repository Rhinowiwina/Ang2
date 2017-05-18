using System;
using System.Collections;
using System.Collections.Generic;
using LS.Core.Interfaces;

namespace LS.Domain
{
    public class StateAgreement : IEntity<string>
    {
        public StateAgreement()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string StateCode { get; set; }
        public string Agreement { get; set; }
        public string Agreement_ES { get; set; }
        public string StateAgreementParentId { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<TempOrder> TempOrders { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}

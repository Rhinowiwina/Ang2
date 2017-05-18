using LS.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LS.Domain
{
    public class ProductCommissions : IEntity<string>
    {
        public ProductCommissions()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string ProductType { get; set; }
        public decimal? Amount { get; set; }
        public string SalesTeamID { get; set; }
        public string RecipientType { get; set; }
        public string RecipientUserId { get; set; }
        public ApplicationUser RecipientUser { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}

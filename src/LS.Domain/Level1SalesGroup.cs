using System;
using System.Collections.Generic;
using LS.Core.Interfaces;

namespace LS.Domain
{
    public class Level1SalesGroup : IEntity<string>
    {
        public Level1SalesGroup()
        {
            Id = Guid.NewGuid().ToString();
            
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string CompanyId { get; set; }
        public Company Company { get; set; }
        public ICollection<ApplicationUser> Managers { get; set; }
        public ICollection<Level2SalesGroup> ChildSalesGroups { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedByUserId { get; set; }
        public string ModifiedByUserId { get; set; }
        public ApplicationUser CreatedByUser { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}

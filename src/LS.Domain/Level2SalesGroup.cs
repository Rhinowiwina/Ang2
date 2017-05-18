using System;
using System.Collections.Generic;
using LS.Core.Interfaces;

namespace LS.Domain
{
    public class Level2SalesGroup : IEntity<string>
    {
        public Level2SalesGroup()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string CompanyId { get; set; }
        public Company Company { get; set; }
        public ICollection<ApplicationUser> Managers { get; set; }
        public ICollection<Level3SalesGroup> ChildSalesGroups { get; set; }
        public string ParentSalesGroupId { get; set; }
        public Level1SalesGroup ParentSalesGroup { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedByUserId { get; set; }
        public string ModifiedByUserId { get; set; }
        public ApplicationUser CreatedByUser { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}

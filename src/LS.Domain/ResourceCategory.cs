using System;
using System.Collections.Generic;
using LS.Core.Interfaces;
using LS.Domain.ExternalApiIntegration.Nlad;

namespace LS.Domain
{
    public class ResourceCategory : IEntity<string>
    {
        public ResourceCategory()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string ResourceCategoryId { get; set; }
        public string CompanyId { get; set; }
        public int SortOrder { get; set; }
        public string Name { get; set; }
        public ICollection<Resources> Resources { get; set; }
        public ICollection<ResourceCategory>Category2 { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
        
    }
}

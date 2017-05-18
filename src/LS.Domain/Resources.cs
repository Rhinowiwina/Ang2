using System;
using System.Collections.Generic;
using LS.Core.Interfaces;
using LS.Domain.ExternalApiIntegration.Nlad;

namespace LS.Domain
{
    public class Resources : IEntity<string>
    {
        public Resources()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }

        public string CompanyId { get; set; }
        public ResourceCategory ResourceCategory { get; set; }
        public string ResourceCategoryId { get; set; }
        public int SortOrder { get; set; }
        public string Name { get; set; }
        public string AlternateName { get; set; }
        // Is it Url or File
        public string Type { get; set; }
        //location or Filename
        public string Resource { get; set; }
        public string AlternateResource { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }

    }
}
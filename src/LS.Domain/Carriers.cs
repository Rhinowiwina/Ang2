using System;
using LS.Core.Interfaces;

namespace LS.Domain
{
    public class Carriers : IEntity<string>
    {
        public Carriers()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string NetworkType { get; set; }
        public int SortOrder { get; set; }
 
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}

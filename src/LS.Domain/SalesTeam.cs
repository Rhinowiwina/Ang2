using System;
using System.Collections.Generic;
using LS.Core.Interfaces;

namespace LS.Domain
{
    public class SalesTeam : IEntity<string>
    {
        public SalesTeam()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string CompanyId { get; set; }
        public Company Company { get; set; }
        public string ExternalPrimaryId { get; set; }
        public string ExternalDisplayName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string TaxId { get; set; }
        public string PayPalEmail { get; set; }
        public string CycleCountTypeDevice { get; set; }
        public string CycleCountTypeSim { get; set; }
        public string Level3SalesGroupId { get; set; }
        public string SigType { get; set; }
        public Level3SalesGroup Level3SalesGroup { get; set; }
        public string CreatedByUserId { get; set; }
        public string ModifiedByUserId { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }
        public ApplicationUser CreatedByUser { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}

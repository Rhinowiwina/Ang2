using System;
using System.Collections.Generic;
using LS.Core.Interfaces;
using LS.Domain.ExternalApiIntegration.Nlad;


namespace LS.Domain
{
    public class Company : IEntity<string>
    {
        private ICollection<ApplicationUser> _applicationUsers;
        private ICollection<SacEntry> _sacEntries;

        public Company()
        {
            Id = Guid.NewGuid().ToString();
            
        }

        public string Id { get; set; }
        public string Name { get; set; }

        public string CompanyLogoUrl { get; set; }
        public string PrimaryColorHex { get; set; }
        public string SecondaryColorHex { get; set; }
        public bool EmailRequiredForOrder { get; set; }
        public bool ContactPhoneRequiredForOrder { get; set; }
        public string DataImportFilePrefix { get; set; }
        public string OrderStart { get; set; }
        public string OrderEnd { get; set; }

        public string TimeZone { get; set; }

        public string CompanySupportUrl { get; set; }
       
        public int MinToChangeTeam { get; set; }
		public bool ShowHandsetOrders { get; set; }
		public bool ShowReporting { get; set; }
		public bool ShowNewAgentRequestMenu { get; set; }
		public bool DoAddressScrub { get; set; }
		public bool DoWhiteListCheck { get; set; }
		public bool DoPromoCodeCheck { get; set; }
		public decimal MaxCommission { get; set; }
        public string Notes { get; set; }
       
        public virtual ICollection<ApplicationUser> ApplicationUsers
        {
            get { return _applicationUsers ?? (_applicationUsers = new List<ApplicationUser>()); }
            protected set { _applicationUsers = value; }
        }

        public virtual ICollection<SacEntry> SacEntries
        {
            get { return _sacEntries ?? (_sacEntries = new List<SacEntry>()); }
            protected set { _sacEntries = value; }
        }
       
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}

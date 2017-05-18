using System;
using LS.Core.Interfaces;

namespace LS.Domain
{
    public class TenantAccountFulfillmentLog : IEntity<string>
    {
        public TenantAccountFulfillmentLog()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string TenantAccountID { get; set; }
        public string CompanyID { get; set; }
        public string OrderID {get; set;}
        public string DeviceID { get; set; }
        public string IMSI { get; set; }
        public string ProviderID { get; set; }
        public string Added_By_UserID { get; set; }
        public string Added_By_TeamID { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}

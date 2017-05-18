using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LS.Core.Interfaces;
namespace LS.Domain
{
    public class AddressValidation
    {
        public string Id { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
        public bool IsShelter { get; set; }
        public string Reason { get; set;}
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public string ModifiedByUserID { get; set; }
    }
}

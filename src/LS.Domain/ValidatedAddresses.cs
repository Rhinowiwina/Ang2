using LS.Core.Interfaces;
using System.Collections.Generic;

namespace LS.Domain
{
    public class ValidatedAddresses
    {
        public string CompanyID { get; set; }
        public int ValidatedAddressID { get; set; }
        public bool AllowServiceAddressBypass { get; set;}
        public string ServiceAddressStreet1 { get; set; }
        public string ServiceAddressStreet2 { get; set; }
        public string ServiceAddressCity { get; set; }
        public string ServiceAddressState { get; set; }
        public string ServiceAddressZip { get; set; }
        public bool ServiceAddressIsValid { get; set; } = false;
        public string BillingAddressStreet1 { get; set; }
        public string BillingAddressStreet2 { get; set; }
        public string BillingAddressCity { get; set; }
        public string BillingAddressState { get; set; }
        public string BillingAddressZip { get; set; }
        public bool BillingAddressIsValid { get; set; } = false;
        public string ShippingAddressStreet1 { get; set; }
        public string ShippingAddressStreet2 { get; set; }
        public string ShippingAddressCity { get; set; }
        public string ShippingAddressState { get; set; }
        public string ShippingAddressZip { get; set; }
        public bool ShippingAddressIsValid { get; set; } = false;
        public bool Hoh { get; set; }

        public ValidatedAddresses()
        {
            Errors = new List<string>();
        }

        public List<string> Errors { get; set; }


    }
}

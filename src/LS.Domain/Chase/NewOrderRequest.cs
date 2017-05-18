using LS.Core.Interfaces.PayPal;
using System.Collections.Generic;

namespace LS.Domain.Chase
{
    public class NewOrderRequest
    {
        public string companyID { get; set;}
        public string transType { get; set; }
        public string useCustomerRefNum { get; set; }
        public string ccAccountNum { get; set; }
        public string ccExp { get; set; }
        public string ccCardVerifyPresenceInd { get; set; }
        public string ccCardVerifyNum { get; set; }
        public string avsAddress1 { get; set; }
        public string avsAddress2 { get; set; }
        public string avsCity { get; set; }
        public string avsState { get; set; }
        public string avsZip { get; set; }
        public string avsCountryCode { get; set; }
        public string avsName { get; set; }
        public string avsPhone { get; set; }
        public string customerFirstName { get; set; }
        public string customerLastName { get; set; }
        public string customerEmail { get; set; }
        public string customerPhone { get; set; }
        public string orderID { get; set; }
        public double amount { get; set; }
        public string comments { get; set; }
    }
}

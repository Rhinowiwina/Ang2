using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LS.Core.Interfaces;
namespace LS.Domain
{
    public class CustomerSearchData : ICustomerSearchData
    {
        public string BudgetMobileId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MDN { get; set; }
        public string City { get; set; }
        public string StateCode { get; set; }

    }

    public class CustomerSearchResult : ICustomerSearchResult
    {
        public bool IsError { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDescription { get; set; }
        public bool CustomerDetailsAvailable { get; set; }
        public ICollection<ICustomerSearchDetailResult> CustomerDetails { get; set; }

    }

    public class CustomerSearchDetailResult : ICustomerSearchDetailResult
    {
        public long BudgetMobileId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MDN { get; set; }
        public string City { get; set; }
        public string State { get; set; }
    }

   

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Core.Interfaces
{
    public interface ICustomerSearchDetailResult
    {
        long BudgetMobileId { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string MDN { get; set; }
        string City { get; set; }
        string State { get; set; }
    }
}

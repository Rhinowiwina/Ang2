using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LS.Domain;
using LS.Core.Interfaces;
//Relates to ApiCall only has nothing to do with LS Company
namespace LS.ApiBindingModels
{
    public class AccountDetailsBalanceBindingModel
    {
        public ILookupCustomerDetailsResult CustomerDetails { get; set; }

        public AccountBalanceBindingModel AccountBalances { get; set; }
    }
}

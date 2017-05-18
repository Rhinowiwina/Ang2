using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Core.Interfaces
{
    public interface ITopUpProduct
    {
        int ErrorCode { get; set; }
        string ErrorMessage { get; set; }
        string ErrorDescription { get; set; }
        bool IsError { get; set; }
        int TopUpProductID { get; set; }
        int Type { get; set; }
        string Description { get; set; }
        double FaceValue { get; set; }
        int AccountCreditsRequired { get; set; }
        string CommisionCategory { get; set; }
        bool AccountCreditEligible { get; set; }
        bool OfferingType { get; set; }
    }
}

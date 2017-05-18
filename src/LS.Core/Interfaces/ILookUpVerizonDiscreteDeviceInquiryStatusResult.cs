using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Core.Interfaces
{
    public interface ILookUpVerizonDiscreteDeviceInquiryStatusResult {
        int ErrorCode { get; set; }
        string ErrorMessage { get; set; }
        string ErrorDescription { get; set; }
        bool IsError { get; set; }
        string DetailCode { get; set; }
        string DetailCodeMessage { get; set; }
        string activationEligibility { get; set; }
    }
}

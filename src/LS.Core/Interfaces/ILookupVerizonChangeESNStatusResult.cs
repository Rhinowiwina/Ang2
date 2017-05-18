using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Core.Interfaces
{
    public interface ILookupVerizonChangeESNStatusResult
    {
        bool IsError { get; set; }
        int ErrorCode { get; set; }
        string ErrorMessage { get; set; }
        string ErrorDescription { get; set; }
        string DeviceID { get; set; }
        string MDN { get; set; }
        string Order_ID { get; set; }
    }
}

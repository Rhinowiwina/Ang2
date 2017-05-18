using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LS.Core.Interfaces;

namespace LS.Domain
{
    public class LookupVerizonDeactivateDeviceStatusResult : ILookupVerizonDeactivateDeviceStatusResult {
        public bool IsError { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDescription { get; set; }
    }
}

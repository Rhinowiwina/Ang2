using LS.Core.Interfaces;
using System.Collections.Generic;

namespace LS.Domain
{
    public class Verizon_Discrete_DeviceInquiryResult : IVerizon_Discrete_DeviceInquiryResult {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDescription { get; set; }
        public  bool IsError { get; set; }
        public string Success { get; set; }
    }
}

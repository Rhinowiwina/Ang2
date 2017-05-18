using LS.Core.Interfaces;
using System.Collections.Generic;
using System;

namespace LS.Domain
{
    public class LookUpVerizonDiscreteDeviceInquiryStatusResult : ILookUpVerizonDiscreteDeviceInquiryStatusResult
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDescription { get; set; }
        public bool IsError { get; set; }
        public string DetailCode { get; set; }
        public string DetailCodeMessage { get; set; }
        public string activationEligibility { get; set; }
    }
}

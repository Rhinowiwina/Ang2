using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LS.Core.Interfaces;

namespace LS.Domain
{
    public class LookupVerizonChangeESNStatusResult : ILookupVerizonChangeESNStatusResult {
        public bool IsError { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDescription { get; set; }
        public string DeviceID {get; set;}
        public string MDN { get; set; }
        public string Order_ID { get; set; }
    }
}

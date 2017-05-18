using LS.Core.Interfaces;
using System.Collections.Generic;

namespace LS.Domain
{
    public class UpdateDeviceIDResult : IUpdateDeviceIDResult {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDescription { get; set; }
        public bool IsError { get; set; }
    }
}

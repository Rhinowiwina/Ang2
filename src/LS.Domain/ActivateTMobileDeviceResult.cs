using LS.Core.Interfaces;

namespace LS.Domain
{
    public class ActivateTMobileDeviceResult : IActivateTMobileDeviceResult
    {
        public int ErrorCode {get; set;}
        public string ErrorMessage{get; set;}
        public string ErrorDescription{get; set;}
        public bool IsError{get; set;}
        public string MDN { get; set; }
    }
}

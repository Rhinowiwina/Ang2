using LS.Core.Interfaces;

namespace LS.Domain
{
    public class ActivateSprintDeviceResult : IActivateSprintDeviceResult
    {
        public int ErrorCode {get; set;}
        public string ErrorMessage{get; set;}
        public string ErrorDescription{get; set;}
        public bool IsError{get; set;}
        public string MDN { get; set; }
        public string MSID{ get; set; }
        public string CSA { get; set; }
        public string EffectiveDate { get; set; }
        public string SerialNumber { get; set; }
        public string MasterSubsidyLock { get; set; }
    }
}

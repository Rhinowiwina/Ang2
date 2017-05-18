namespace LS.Core.Interfaces
{
    public interface IDeactivateVerizonDeviceResult {
        int ErrorCode { get; set; }
        string ErrorMessage { get; set; }
        string ErrorDescription { get; set; }
        bool IsError { get; set; }
    }
}

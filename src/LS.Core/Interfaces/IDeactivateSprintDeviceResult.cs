namespace LS.Core.Interfaces
{
    public interface IDeactivateSprintDeviceResult {
        int ErrorCode { get; set; }
        string ErrorMessage { get; set; }
        string ErrorDescription { get; set; }
        bool IsError { get; set; }
        string ConfirmMsg { get; set; }
    }
}

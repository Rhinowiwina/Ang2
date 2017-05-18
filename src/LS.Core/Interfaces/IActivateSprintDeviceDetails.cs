namespace LS.Core.Interfaces
{
    public interface IActivateSprintDeviceDetails
    {
        string OrderId { get; set; }
        string DeviceId { get; set; }
        string Zip { get; set; }
        int SpecialProvisioningCode { get; set; }
    }
}

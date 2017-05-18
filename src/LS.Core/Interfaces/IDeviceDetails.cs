namespace LS.Core.Interfaces
{
    public interface IDeviceDetails
    {
        string Type { get; set; }
        int DeviceId { get; set; }
        int Category { get; set; } 
        double SuggestedRetailPrice { get; set; }
    }
}

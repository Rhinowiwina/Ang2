namespace LS.Core.Interfaces
{
    public interface IValidateDevice
    {
        bool IsValid { get; set; }
        string Type { get; set; }
        string HEX { get; set; }
        string DEC { get; set; }
    }
}

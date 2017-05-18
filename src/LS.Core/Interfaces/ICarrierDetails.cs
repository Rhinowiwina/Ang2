namespace LS.Core.Interfaces
{
    public interface ICarrierDetails
    {
        bool ValidForVerizon { get; set; }
        bool ValidForSprint { get; set; }
        bool ValidForTmobile { get; set; } 
    }
}

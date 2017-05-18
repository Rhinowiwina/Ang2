using LS.Core.Interfaces;


namespace LS.Domain
{
    public class CarrierDetails : ICarrierDetails
    {
        public bool ValidForVerizon { get; set; }
        public bool ValidForSprint { get; set; }
        public bool ValidForTmobile { get; set; }
    }
}

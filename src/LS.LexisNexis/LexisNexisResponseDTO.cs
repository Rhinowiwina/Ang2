using System.Collections.Generic;

namespace LS.LexisNexis
{
    public class LexisNexisResponseDto
    {
        public uint NameAddressSSN { get; set; }
        public bool DOB { get; set; }
        public string DOBMatchLevel { get; set; }
        public string LexId { get; set; }
        public string TransactionID { get; set; }
        public List<RiskIndicatorDto> RiskIndicators { get; set; }
    }

    public class RiskIndicatorDto{
        public string RiskCode { get; set; }
        public string Description { get; set; }
        public int Sequence { get; set; }
    }
}

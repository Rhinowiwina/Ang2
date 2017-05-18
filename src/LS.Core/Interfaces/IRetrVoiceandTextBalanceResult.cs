using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Core.Interfaces
{
    public interface IRetrVoiceandTextBalanceResult {
        bool IsError { get; set; }
        int ErrorCode { get; set; }
        string ErrorMessage { get; set; }
        string ErrorDescription { get; set; }
        string WirelessNumber { get; set; }
        int planBalance_Voice { get; set; }
        int planBalance_Text { get; set; }
        int topUpBalance_Voice { get; set; }
        int topUpBalance_Text { get; set; }
        string ServiceEndDate { get; set; }
        bool TopUpExpirationSet { get; set; }
        string TopUpExpiration { get; set; }
        int planBalance_Combo_VS { get; set; }
        int TopUpBalance_Combo_VS { get; set; }
        string planBalance_Data { get; set; }
        string topUpBalance_Data { get; set; }
    }
}

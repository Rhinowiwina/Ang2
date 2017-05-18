using System.Collections.Generic;
namespace LS.Core.Interfaces
{
    public interface ITMobile_SwapIMSIResult {
        int ErrorCode { get; set; }
        string ErrorMessage { get; set; }
        string ErrorDescription { get; set; }
        bool IsError { get; set; }
    }
}

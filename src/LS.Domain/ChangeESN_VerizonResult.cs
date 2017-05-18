using LS.Core.Interfaces;

namespace LS.Domain
{
    public class ChangeESN_VerizonResult : IChangeESN_VerizonResult
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDescription { get; set; }
        public bool IsError { get; set; }
    }
}

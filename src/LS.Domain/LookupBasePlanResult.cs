using LS.Core.Interfaces;

namespace LS.Domain
{
    public class LookupBasePlanResult : ILookupBasePlanResult {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDescription { get; set; }
        public bool IsError { get; set; }
        public bool BasePlanDetailsAvailable { get; set; }
        public string PlanDescription { get; set; }
        public string Voice { get; set; }
        public string Text { get; set; }
        public string Data { get; set; }

    }
}

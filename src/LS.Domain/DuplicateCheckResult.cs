using LS.Core.Interfaces;


namespace LS.Domain
{
    public class DuplicateCheckResult : IDuplicateCheckResult {
        public bool IsDuplicate { get; set; }
    }
}

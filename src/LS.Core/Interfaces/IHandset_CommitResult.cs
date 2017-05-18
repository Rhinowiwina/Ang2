namespace LS.Core.Interfaces
{
    public interface IHandset_CommitResult {
        int ErrorCode { get; set; }
        string ErrorMessage { get; set; }
        string ErrorDescription { get; set; }
        bool IsError { get; set; }
        string HandsetOrderID {get; set;}
    }
}

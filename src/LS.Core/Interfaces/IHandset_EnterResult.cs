using System.Collections.Generic;
namespace LS.Core.Interfaces
{
    public interface IHandset_EnterResult {
        int ErrorCode { get; set; }
        string ErrorMessage { get; set; }
        string ErrorDescription { get; set; }
        bool IsError { get; set; }
        string HandsetOrderID {get; set;}
        List<ITaxItemRow> TaxItem { get; set; }
        List<IHandset> Handset { get; set; }
        int ReferenceID { get; set; }
        double OrderTotal { get; set; }
        bool TaxesApply { get; set; }
    }
}

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LS.Core.Interfaces
{
    public interface IEnterOrderResult
    {
        [DataMember]
        int ReferenceID { get; set; }
        [DataMember]
        int AccountCreditsAwarded { get; set; }
        [DataMember]
        double OrderTotal { get; set; }
        [DataMember]
        string ServicePlanDescription { get; set; }
        [DataMember]
        double ServicePlanCost { get; set; }
        [DataMember]
        bool AdditionalChargesApply { get; set; }
        [DataMember]
        bool TaxesApply { get; set; }

        List<ITaxItemRow> TaxItems { get; set; }
        List<IAdditionalChargeRow> AdditionalCharges { get; set; }
    }
}

using LS.Core.Interfaces;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;

namespace LS.Domain
{
    [DataContract]
    [KnownType(typeof(EnterOrderResult))]
    public class EnterOrderResult : IEnterOrderResult
    {
        [DataMember]
        public int ReferenceID { get; set; }
        [DataMember]
        public int AccountCreditsAwarded { get; set; }
        [DataMember]
        public double OrderTotal { get; set; }
        [DataMember]
        public string ServicePlanDescription { get; set; }
        [DataMember]
        public double ServicePlanCost { get; set; }
        [DataMember]
        public bool AdditionalChargesApply { get; set; }
        [DataMember]
        public bool TaxesApply { get; set; }

        public EnterOrderResult()
        {
            TaxItems = new List<ITaxItemRow>();
            AdditionalCharges = new List<IAdditionalChargeRow>();
        }

        public List<ITaxItemRow> TaxItems { get; set; }
        public List<IAdditionalChargeRow> AdditionalCharges { get; set; }

    
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LS.Core.Interfaces
{
    [DataContract]
    [KnownType(typeof(ITaxItemRow))]
    public class ITaxItemRow
    {
        [DataMember]
        public double TaxItemAmount { get; set; }
        [DataMember]
        public string TaxItemDescription { get; set; }
    }
}

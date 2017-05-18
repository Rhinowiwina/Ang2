using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LS.Core.Interfaces
{
    [DataContract]
    [KnownType(typeof(IAdditionalChargeRow))]
    public class IAdditionalChargeRow
    {
        [DataMember]
        public double AdditionalChargeAmount { get; set; }
        [DataMember]
        public string AdditionalChargeDescription { get; set; }
    }
}

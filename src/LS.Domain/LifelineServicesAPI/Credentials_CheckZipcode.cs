using System;
using System.Collections.Generic;
using LS.Core.Interfaces;
using LS.Domain.ExternalApiIntegration.Nlad;
using System.Runtime.Serialization;

namespace LS.Domain.LifelineServicesAPI
{
    [DataContract]
    public class CheckZipcodeRequest {
        [DataMember]
        public string Zipcode { get; set; }
        [DataMember]
        public string APIKey { get; set; }
        [DataMember]
        public string CompanyID { get; set; }
        [DataMember]
        public string SalesTeamExternalPrimaryId { get; set; }
    }
}

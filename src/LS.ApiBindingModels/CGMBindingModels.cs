using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LS.Domain;
using LS.Domain.ExternalApiIntegration.CGM;
using System;

namespace LS.ApiBindingModels
{
    public class CGMCovertedResponse {
        public virtual ICollection<CGMSubscriberCheck> SubscriberCheck { get; set; }
        public Boolean Blacklist { get; set; }
        public string Transactionid { get; set; }
        public string Token { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string ValidationErrors { get; set; }
        public string ExternalVelocityCheck { get; set; }
    }
}
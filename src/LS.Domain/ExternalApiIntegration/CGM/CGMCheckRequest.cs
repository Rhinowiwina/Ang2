using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Domain.ExternalApiIntegration.CGM
{
    public class CGMCheckRequest
    {
        public string Token { get; set; }
        public string SubscriberFirstName { get; set; }
        public string SubscriberLastName { get; set; }
        public string SubscriberMiddleNameInital { get; set; }
        public string SubscriberSecondLastName { get; set; }
        public string SubscriberLast4ssn { get; set; }
        public string SubscriberDob { get; set; }
        public string SubscriberPrimaryAddress1 { get; set; }
        public string SubscriberPrimaryAddress2 { get; set; }
        public string SubscriberPrimaryCity { get; set; }

        public string SubscriberPrimaryState { get; set; }
        public string SubscriberPrimaryZipCode{ get; set; }
        public string AgentFirstName { get; set; }
        public string AgentLastName { get; set; }
        public string AgentLast4ssn { get; set; }
        public string AgentDob { get; set; }
        public string AgentDeviceId { get; set; }
        
    }
}
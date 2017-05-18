using System;
using LS.Domain.ExternalApiIntegration.Interfaces;

namespace LS.Domain.ExternalApiIntegration
{
    public class NevadaSolixEligibilityRequestData 
    {
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public string Last4Ssn { get; set; }

    }
}

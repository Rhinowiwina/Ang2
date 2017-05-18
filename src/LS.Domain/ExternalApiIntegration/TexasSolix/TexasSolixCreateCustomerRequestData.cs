using System;
using LS.Domain.ExternalApiIntegration.Interfaces;

namespace LS.Domain.ExternalApiIntegration.TexasSolix
{
    public class TexasSolixCreateCustomerRequestData
    {
        public string FirstName { get; set; }
        public string MiddleInitial { get; set; }
        public string LastName { get; set; }
        public string NameSuffix { get; set; }
        public string ServiceAddress1 { get; set; }
        public string ServiceAddress2 { get; set; }
        public string ServiceAddressCity { get; set; }
        public string ServiceAddressState { get; set; }
        public string ServiceAddressZip5 { get; set; }
        public string ServiceAddressZip4 { get; set; }
        public string Ssn4 { get; set; }
        public string DateOfBirth { get; set; }
        public bool IsHohFlag { get; set; }
        public string LifelineProgramId { get; set; }
        public string CompanyId { get; set; }
        public bool TpivBypass { get; set; }
        public string PriorULTSTelephoneNumber { get; set; }
        public string ConfirmationNumber { get; set; }

        public bool RequestIsValid()
        {
            throw new NotImplementedException();
        }
    }
}

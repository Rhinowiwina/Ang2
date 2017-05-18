using LS.Domain.ExternalApiIntegration.Interfaces;

namespace LS.Domain.ExternalApiIntegration.TexasSolix
{
    public class TexasSolixSubmitApplicationRequestData : ISubmitApplicationRequestData {
        public string Ocn { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string TransactionId { get; set; }
        public string SubscriberAccountNumber { get; set; }
        public string AssignedTelephoneNumber { get; set; }
        public string NamePrefix { get; set; }
        public string FirstName { get; set; }
        public string MiddleInitial { get; set; }
        public string LastName { get; set; }
        public string NameSuffix { get; set; }
        public bool HasServiceAddressBypass { get; set; }
        public string ServiceAddress1 { get; set; }
        public string ServiceAddress2 { get; set; }
        public string ServiceAddressCity { get; set; }
        public string ServiceAddressState { get; set; }
        public string ServiceAddressZip5 { get; set; }
        public string ServiceAddressZip4 { get; set; }
        public string ServiceType { get; set; }
        public string ShippingAddress1 { get; set; }
        public string ShippingAddress2 { get; set; }
        public string ShippingAddressCity { get; set; }
        public string ShippingAddressState { get; set; }
        public string ShippingAddressZip5 { get; set; }
        public string ShippingAddressZip4 { get; set; }
        public string BillingFirstName { get; set; }
        public string BillingMiddleInitial { get; set; }
        public string BillingLastName { get; set; }
        public string BillingAddress1 { get; set; }
        public string BillingAddress2 { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingZip5 { get; set; }
        public string BillingZip4 { get; set; }
        public string ContactPhoneNumber { get; set; }
        public string UltsServiceStartDate { get; set; }
        public string DriversLicenseNumber { get; set; }
        public EnrollmentType EnrollmentType { get; set; }
        public bool TpivBypass { get; set; }
        public string TpivBypassSignature { get; set; }
        public string TpivBypassSsnDocument { get; set; }
        public string TpivBypassDobDocument { get; set; }
        public string TpivBypassSsnLast4Digits { get; set; }
        public string TpivBypassDobLast4Digits { get; set; }
        public string Ssn4 { get; set; }
        public string DateOfBirth { get; set; }
        public string QualifyingBeneficiaryFirstName { get; set; }
        public string QualifyingBeneficiaryLastName { get; set; }
        public string QualifyingBeneficiaryLast4Ssn { get; set; }
        public string QualifyingBeneficiaryDateOfBirth { get; set; }
        public bool IsHohFlag { get; set; }
        public string LifelineProgramId { get; set; }
        public string CompanyId { get; set; }
        public string PriorULTSTelephoneNumber { get; set; }

        public bool RequestIsValid() {
            throw new System.NotImplementedException();
            }
        }
    }

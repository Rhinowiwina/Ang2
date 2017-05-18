using System;
using LS.Domain.ExternalApiIntegration.Interfaces;
using Newtonsoft.Json;

namespace LS.Domain.ExternalApiIntegration.Nlad
{
    public class NladRequestData : ISubmitApplicationRequestData {
        [JsonIgnore]
        public string Ocn { get; set; }

        [JsonIgnore]
        public string Username { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        [JsonIgnore]
        public string TransactionId { get; set; }

        [JsonIgnore]
        public string SubscriberAccountNumber { get; set; }

        [JsonIgnore]
        public string UltsTelephoneNumber { get; set; }

        [JsonIgnore]
        public string NamePrefix { get; set; }

        [JsonIgnore]
        public string NameSuffix { get; set; }

        [JsonProperty(PropertyName = "transactionType")]
        public string TransactionType { get; set; }

        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "middleName")]
        public string MiddleInitial { get; set; }

        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "primaryPermanentAddressFlag")]
        public string PrimaryPermanentAddressFlag { get { return "0"; } }

        [JsonIgnore]
        public bool HasServiceAddressBypass { get; set; }
        [JsonIgnore]
        public string ServiceAddressBypassSignature { get; set; }
        [JsonIgnore]
        public string ServiceAddressByPassDocumentProofId { get; set; }
        [JsonProperty(PropertyName = "serviceType")]
        public string ServiceType { get; set; }


        [JsonProperty(PropertyName = "primaryAddress1")]
        public string ServiceAddress1 { get; set; }

        [JsonProperty(PropertyName = "primaryAddress2")]
        public string ServiceAddress2 { get; set; }

        [JsonProperty(PropertyName = "primaryCity")]
        public string ServiceAddressCity { get; set; }

        [JsonProperty(PropertyName = "primaryState")]
        public string ServiceAddressState { get; set; }

        [JsonProperty(PropertyName = "primaryZipCode")]
        public string ServiceAddressCombinedZip {
            get {
                return string.IsNullOrEmpty(ServiceAddressZip4)
                    ? ServiceAddressZip5
                    : ServiceAddressZip5 + "-" + ServiceAddressZip4;
                }
            }

        [JsonIgnore]
        public string ServiceAddressZip5 { get; set; }

        [JsonIgnore]
        public string ServiceAddressZip4 { get; set; }

        [JsonIgnore]
        public string BillingFirstName { get; set; }

        [JsonIgnore]
        public string BillingMiddleInitial { get; set; }

        [JsonIgnore]
        public string BillingLastName { get; set; }

        [JsonProperty(PropertyName = "last4ssn")]
        public string Ssn4 { get; set; }

        [JsonProperty(PropertyName = "dob")]
        public string DateOfBirth { get; set; }

        [JsonIgnore]
        public bool IsHohFlag { get; set; }

        [JsonIgnore]
        public string LifelineProgramId { get; set; }

        [JsonIgnore]
        public string CompanyId { get; set; }

        [JsonIgnore]
        public string AssignedPhoneNumber { get; set; }

        [JsonProperty(PropertyName = "phoneNumber")]
        public string AssignedTelephoneNumber { get; set; }

        [JsonProperty(PropertyName = "phoneNumberInNlad")]
        public string PhoneNumberInNlad { get; set; }

        [JsonProperty(PropertyName = "tribalId")]
        public string TribalId { get; set; }

        [JsonProperty(PropertyName = "sac")]
        public int Sac { get; set; }

        [JsonIgnore]
        public EnrollmentType EnrollmentType { get; set; }

        [JsonProperty(PropertyName = "transactionEffectiveDate")]
        public string TransactionEffectiveDate { get; set; }

        [JsonProperty(PropertyName = "iehCertificationDate")]
        public string IehCertificationDate { get; set; }

        [JsonProperty(PropertyName = "iehRecertificationDate")]
        public string IehRecertificationDate { get; set; }

        [JsonProperty(PropertyName = "mailingAddress1")]
        public string BillingAddress1 { get; set; }

        [JsonProperty(PropertyName = "mailingAddress2")]
        public string BillingAddress2 { get; set; }

        [JsonProperty(PropertyName = "mailingCity")]
        public string BillingCity { get; set; }

        [JsonProperty(PropertyName = "mailingState")]
        public string BillingState { get; set; }

        [JsonProperty(PropertyName = "mailingZipCode")]
        public string BillingAddressZip {
            get {
                return string.IsNullOrEmpty(BillingZip4)
                    ? BillingZip5
                    : BillingZip5 + "-" + BillingZip4;
                }
            }

        [JsonIgnore]
        public string BillingZip5 { get; set; }

        [JsonIgnore]
        public string BillingZip4 { get; set; }

        [JsonIgnore]
        public string ShippingAddress1 { get; set; }

        [JsonIgnore]
        public string ShippingAddress2 { get; set; }

        [JsonIgnore]
        public string ShippingCity { get; set; }

        [JsonIgnore]
        public string ShippingState { get; set; }

        [JsonIgnore]
        public string ShippingZip5 { get; set; }

        [JsonIgnore]
        public string ShippingZip4 { get; set; }

        [JsonProperty(PropertyName = "serviceInitializationDate")]
        public string ServiceInitializationDate { get; set; }

        [JsonProperty(PropertyName = "serviceReverificationDate")]
        public string ServiceReverificationDate { get; set; }

        [JsonProperty(PropertyName = "eligibilityCode")]
        public string EligibilityCode { get; set; }

        [JsonProperty(PropertyName = "bqpLastName")]
        public string QualifyingBeneficiaryLastName { get; set; }

        [JsonProperty(PropertyName = "bqpFirstName")]
        public string QualifyingBeneficiaryFirstName { get; set; }

        [JsonProperty(PropertyName = "bqpMiddleName")]
        public string QualifyingBeneficiaryMiddleName { get; set; }

        [JsonProperty(PropertyName = "bqpDob")]
        public string QualifyingBeneficiaryDateOfBirth { get; set; }

        [JsonProperty(PropertyName = "bqpLast4ssn")]
        public string QualifyingBeneficiaryLast4Ssn { get; set; }

        [JsonProperty(PropertyName = "bqpTribalId")]
        public string BqpTribalId { get; set; }

        [JsonProperty(PropertyName = "linkUpServiceDate")]
        public string LinkUpServiceDate { get; set; }

        [JsonProperty(PropertyName = "lifelineTribalBenefitFlag")]
        public string LifelineTribalBenefitFlag { get { return "0"; } }

        [JsonProperty(PropertyName = "acpFlag")]
        public string AcpFlag { get { return "0"; } }

        [JsonProperty(PropertyName = "etcGeneralUse")]
        public string EtcGeneralUse { get; set; }

        [JsonProperty(PropertyName = "tpivFlag")]
        public string TpivFlag { get { return "0"; } }

        [JsonProperty(PropertyName = "primaryRuralFlag")]
        public string PrimaryRuralFlag { get; set; }

        [JsonProperty(PropertyName = "primaryTribalFlag")]
        public string PrimarTribalFlag { get { return "0"; } }

        [JsonProperty(PropertyName = "iehFlag")]
        public string IehFlag { get; set; }

        [JsonIgnore]
        public string ContactPhoneNumber { get; set; }

        [JsonIgnore]
        public string UltsServiceStartDate { get; set; }

        [JsonIgnore]
        public string DriversLicenseNumber { get; set; }
        [JsonIgnore]
        public bool TpivBypass { get; set; }
        [JsonIgnore]
        public string TpivBypassSsnDocument { get; set; }
        [JsonIgnore]
        public string TpivBypassDobDocument { get; set; }
        [JsonIgnore]
        public string TpivBypassSignature { get; set; }

        [JsonIgnore]
        public string TpivBypassSsnLast4Digits { get; set; }
        [JsonIgnore]
        public string TpivBypassDobLast4Digits { get; set; }
        //California needs nlad don't
        [JsonIgnore]
        public string PriorULTSTelephoneNumber { get; set; }

        public bool RequestIsValid() {
            throw new NotImplementedException();
            }
        }
    }

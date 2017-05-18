using System;
using LS.Domain.ExternalApiIntegration.Interfaces;

namespace LS.Domain.ExternalApiIntegration
{
    public class CheckStatusRequestData : ICheckStatusRequestData
    {
        public string AssignedTelephoneNumber { get; set; }
        public string FirstName { get; set; }
        public string MiddleInitial { get; set; }
        public string LastName { get; set; }
       public bool HasServiceAddressBypass { get; set; }
        public string ServiceAddressBypassSignature { get; set; }
        public string ServiceAddressByPassDocumentProofId { get; set; }
        public string ServiceAddress1 { get; set; }
        public string ServiceAddress2 { get; set; }
        public string ServiceAddressCity { get; set; }
        public string ServiceAddressState { get; set; }
        public string ServiceAddressZip5 { get; set; }
        public string ServiceAddressZip4 { get; set; }
        public string ServiceType { get; set; }
        public string Ssn4 { get; set; }
        public string DateOfBirth { get; set; }
        public string QualifyingBeneficiaryFirstName { get; set; }
        public string QualifyingBeneficiaryLastName { get; set; }
        public string QualifyingBeneficiaryLast4Ssn { get; set; }
        public string QualifyingBeneficiaryDateOfBirth { get; set; }
        public bool IsHohFlag { get; set; }
        public string LifelineProgramId { get; set; }
        public string CompanyId { get; set; }
        public bool TpivBypass { get; set; }
        public string TpivBypassSignature { get; set; }
        public string TpivBypassSsnDocument { get; set; }
        public string TpivBypassDobDocument { get; set; }
        public string TpivBypassSsnLast4Digits { get; set; }
        public string TpivBypassDobLast4Digits { get; set; }
         public string PriorULTSTelephoneNumber { get; set; }
        public string ExternalVelocityCheck { get; set; }//PASS or FAIL
        public bool RequestIsValid()
        {
            throw new NotImplementedException();
        }
    }
}

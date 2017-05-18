namespace LS.Domain.ExternalApiIntegration.Interfaces
{
    public interface ICheckStatusRequestData
    {
        string AssignedTelephoneNumber { get; set; }
        string FirstName { get; set; }
        string MiddleInitial { get; set; }
        string LastName { get; set; }
        string ServiceAddress1 { get; set; }
        string ServiceAddress2 { get; set; }
        string ServiceAddressCity { get; set; }
        string ServiceAddressState { get; set; }
        string ServiceAddressZip5 { get; set; }
        string ServiceAddressZip4 { get; set; }
        string ServiceType { get; set; }
        string Ssn4 { get; set; }
        string DateOfBirth { get; set; }
        string QualifyingBeneficiaryFirstName { get; set; }
        string QualifyingBeneficiaryLastName { get; set; }
        string QualifyingBeneficiaryLast4Ssn { get; set; }
        string QualifyingBeneficiaryDateOfBirth { get; set; }
        bool IsHohFlag { get; set; }
        string LifelineProgramId { get; set; }
        string CompanyId { get; set; }
        bool TpivBypass { get; set; }

        bool HasServiceAddressBypass { get; set; }

        string TpivBypassSsnDocument { get; set; }
        string TpivBypassDobDocument { get; set; }

        string PriorULTSTelephoneNumber { get; set; }
        bool RequestIsValid();
    }
}

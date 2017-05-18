namespace LS.Core.Interfaces
{
    public interface IEnterOrderDetails
    {
        string ApplicationID { get; set; }
        string ApplicationPassword { get; set; }
        int LocationID { get; set; }
        string EmployeeName { get; set; }
        string OrderID { get; set; }
        bool Lifeline { get; set; }
        bool TribalLifeline { get; set; }
        int LifelineProgram { get; set; }
        int CellularPlan { get; set; }
        string Model { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string Gender { get; set; }
        int ValidatedAddressId { get; set; }
        string SSN { get; set; }
        string Contact { get; set; }
        string Email { get; set; }
        string UserName { get; set; }
        string UserPassword { get; set; }
        string ReferringMobileNumber { get; set; }
        string PrequalifiedCode { get; set; }
        string DOB { get; set; }
        string AuthorizationCode { get; set; }
        string EmployeeAcount { get; set; }
        string DeviceID { get; set; }
        string TradeInDeviceID { get; set; }
        bool TemporaryAddress { get; set; }
        string ExternalAgentID { get; set; }
        string BillingAddress { get; set; }
        string BillingAddress2 { get; set; }
        string BillingCity { get; set; }
        string BillingState { get; set; }
        string BillingZip { get; set; }
        string SalesChannel { get; set; }
        string StateRegCode { get; set; }
        bool NoDeviceOnOrder { get; set; }
        bool BypassIdentity { get; set; }
        string BypassIdentityNotes { get; set; }
        string AlternateIDDesc { get; set; }
        string AlternateIDValue { get; set; }
        string Beneficiary { get; set; }
        int AccountCreditAwarded { get; set; }
        string DocumentVerification { get; set; }
        string BeneficiaryFirstName { get; set; }
        string BeneficiaryLastName { get; set; }
        string BeneficiarySSN { get; set; }
        string BeneficiaryDOB { get; set; }
        bool VoiceOnly { get; set; }
        string LifelineProgramValue { get; set; }
        string StateRegCode2 { get; set; }
        string Address { get; set; }
        string Address2 { get; set; }
        string City { get; set; }
        string State { get; set; }
        string Zip { get; set; }
        bool RuralAddress { get; set; }
        bool ByPassDuplicate { get; set; }
        bool HOHC { get; set; }
        string IMSI { get; set; }
        string LexID { get; set; }
    }
}

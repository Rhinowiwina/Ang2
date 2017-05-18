using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using LS.Domain;
using LS.Domain.ExternalApiIntegration.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace LS.ApiBindingModels {
    public class ZipCodeRequestBindingModel {
        public string ApiKey { get; set; }
        public string CompanyId { get; set; } // Should this just be grabbed from the controller?
        public string SalesTeamId { get; set; }
        public string ZipCode { get; set; }
        public string ReturnFormat { get; set; } // What is this for?
    }
    public class OperationTime {
        public string OrderStart { get; set; }
        public string OrderEnd { get; set; }
        public string TimeZone { get; set; }
    }
    public class ReportSearchRequestBindingModel {
        public string Report { get; set; }
        public ICollection<string> Locations { get; set; }
        public ICollection<string> Level1Groups { get; set; }
        public string PONumber { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string MapType { get; set; }
        public string OrderStatus { get; set; }
        public bool ShowUserDetail { get; set; }
        public string DateFilterType { get; set; }
    }

    public class ReportSearchResponseBindingModel {
        public string QueryResult { get; set; }
        public string AddJSON { get; set; }
    }

    [Serializable()]
    public class ZipCodeResponseBindingModel {
        public List<ZipCode> ZipCodeOptions { get; set; }
        public BaseIncomeLevels BaseIncomeLevels { get; set; }
        public ComplianceStatement ComplianceStatement { get; set; }
        public List<Competitor> Competitors { get; set; }
        public string SsnType { get; set; }
        public int IncomeLevelPercentage { get; set; }
        public List<AddressProofDocumentType> AddressBypassProofDocumentTypes { get; set; }
        public List<ProofDocumentType> ProgramProofDocumentTypes { get; set; }
        public List<ProofDocumentType> IdentityProofDocumentTypes { get; set; }
        public List<TpivProofDocumentType> SsnTpivProofDocumentTypes { get; set; }
        public List<TpivProofDocumentType> DobTpivProofDocumentTypes { get; set; }
        public List<LifelineProgram> LifelinePrograms { get; set; }
        public List<StateProgram> StatePrograms { get; set; }
        public bool ContactPhoneRequired { get; set; }
        public bool EmailRequired { get; set; }
        public List<StateAgreement> AgreementStatements { get; set; }

        public List<Plan> Plans { get; set; }
        public BasePlan BasePlan { get; set; }
    }

    public class CheckStatusRequestBindingModel : ICheckStatusRequestData {
        public string Ocn { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string TransactionId { get; set; }
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
        public string TpivBypassSsnDocument { get; set; }
        public string TpivBypassDobDocument { get; set; }
        public string AssignedPhoneNumber { get; set; }
        public string PriorULTSTelephoneNumber { get; set; }

        public bool RequestIsValid() {
            throw new NotImplementedException();
        }
    }

    public class AddressValidationBindingModel {
        public string ServiceAddressStreet1 { get; set; }
        public string ServiceAddressStreet2 { get; set; }
        [Required(ErrorMessage = "Service address city is required.")]
        public string ServiceAddressCity { get; set; }
        [Required(ErrorMessage = "Service address state is required.")]
        public string ServiceAddressState { get; set; }
        [Required(ErrorMessage = "Service address zip is required.")]
        public string ServiceAddressZip { get; set; }
        [Required(ErrorMessage = "Service address residence type is required.")]
        public bool ServiceAddressIsPermanent { get; set; }
        [Required(ErrorMessage = "Billing address street 1 is required.")]
        public string BillingAddressStreet1 { get; set; }
        public string BillingAddressStreet2 { get; set; }
        [Required(ErrorMessage = "Billing address city is required.")]
        public string BillingAddressCity { get; set; }
        [Required(ErrorMessage = "Billing address state is required.")]
        public string BillingAddressState { get; set; }
        [Required(ErrorMessage = "Billing address zip is required.")]
        public string BillingAddressZip { get; set; }
        [Required(ErrorMessage = "Shipping address street 1 is required.")]
        public string ShippingAddressStreet1 { get; set; }
        public string ShippingAddressStreet2 { get; set; }
        [Required(ErrorMessage = "Shipping address city is required.")]
        public string ShippingAddressCity { get; set; }
        [Required(ErrorMessage = "Shipping address state is required.")]
        public string ShippingAddressState { get; set; }
        [Required(ErrorMessage = "Shipping address zip is required.")]
        public string ShippingAddressZip { get; set; }
        public string CompanyID { get; set; }
    }

    public class LifelineProgramAnnualIncome {
        public int HouseholdSized { get; set; }
        public int NumHouseAdult { get; set; }
        public int NumHouseChildren { get; set; }
        public int AverageIncomeAmount { get; set; }
        public string AverageIncomeDuration { get; set; }
        public string AnnualIncomeCustomerInitials { get; set; }
    }


    public class ValidationOrderBindingModel {
        public string OrderId { get; set; }
        public bool HouseholdReceivesLifelineBenefits { get; set; }
        public bool CustomerReceivesLifelineBenefits { get; set; }
        public string QBFirstName { get; set; }
        public string QBLastName { get; set; }
        public string QBSsn { get; set; }
        public string QBDateOfBirth { get; set; }
        [Required(ErrorMessage = "Lifeline Program Id is required.")]
        public string LifelineProgramId { get; set; }
        [Required(ErrorMessage = "Lifeline Program proof type is required.")]
        public string LPProofTypeId { get; set; }
        public string LPProofImageUploadId { get; set; }
        public string LPProofNumber { get; set; }
        public string CurrentLifelinePhoneNumber { get; set; }
        public string StateProgramId { get; set; }
        public string StateProgramNumber { get; set; }
        public string SecondaryStateProgramId { get; set; }
        public string SecondaryStateProgramNumber { get; set; }
        [Required(ErrorMessage = "Customer first name is required.")]
        public string FirstName { get; set; }
        public string MiddleInitial { get; set; }
        [Required(ErrorMessage = "Customer last name is required.")]
        public string LastName { get; set; }
        public string Gender { get; set; }
        [Required(ErrorMessage = "Customer social security number is required.")]
        public string Ssn { get; set; }
        [Required(ErrorMessage = "Customer date of birth is required.")]
        public string DateOfBirth { get; set; }
        public string EmailAddress { get; set; }
        public string ContactPhoneNumber { get; set; }
        [Required(ErrorMessage = "Identity proof type is required.")]
        public string IPProofTypeId { get; set; }
        public string IPProofImageUploadId { get; set; }
        [Required(ErrorMessage = "Service address street 1 is required.")]
        public string ServiceAddressStreet1 { get; set; }
        public string ServiceAddressStreet2 { get; set; }
        [Required(ErrorMessage = "Service address city is required.")]
        public string ServiceAddressCity { get; set; }
        [Required(ErrorMessage = "Service address state is required.")]
        public string ServiceAddressState { get; set; }
        [Required(ErrorMessage = "Service address zip is required.")]
        public string ServiceAddressZip { get; set; }
        [Required(ErrorMessage = "Service address residence type is required.")]
        public bool ServiceAddressIsPermanent { get; set; }
        public bool ServiceAddressIsRural { get; set; }
        [Required(ErrorMessage = "Billing address street 1 is required.")]
        public string BillingAddressStreet1 { get; set; }
        public string BillingAddressStreet2 { get; set; }
        [Required(ErrorMessage = "Billing address city is required.")]
        public string BillingAddressCity { get; set; }
        [Required(ErrorMessage = "Billing address state is required.")]
        public string BillingAddressState { get; set; }
        [Required(ErrorMessage = "Billing address zip is required.")]
        public string BillingAddressZip { get; set; }
        [Required(ErrorMessage = "Shipping address street 1 is required.")]
        public string ShippingAddressStreet1 { get; set; }
        public string ShippingAddressStreet2 { get; set; }
        [Required(ErrorMessage = "Shipping address city is required.")]
        public string ShippingAddressCity { get; set; }
        [Required(ErrorMessage = "Shipping address state is required.")]
        public string ShippingAddressState { get; set; }
        [Required(ErrorMessage = "Shipping address zip is required.")]
        public string ShippingAddressZip { get; set; }
        public bool IsHoh { get; set; }
        [Required(ErrorMessage = "HOH Spouse response is required.")]
        public bool HohSpouse { get; set; }
        [Required(ErrorMessage = "HOH parent response is required.")]
        public bool HohAdultsParent { get; set; }
        [Required(ErrorMessage = "HOH adult child response is required.")]
        public bool HohAdultsChild { get; set; }
        [Required(ErrorMessage = "HOH adult relative response is required.")]
        public bool HohAdultsRelative { get; set; }
        [Required(ErrorMessage = "HOH adult roommate response is required.")]
        public bool HohAdultsRoommate { get; set; }
        [Required(ErrorMessage = "HOH adult other response is required.")]
        public bool HohAdultsOther { get; set; }
        public string HohAdultOtherText { get; set; }
        public bool? HohExpenses { get; set; }
        public bool? HohShareLifeline { get; set; }
        public bool? HohAgreeMultihouse { get; set; }
        public bool HohAgreeViolation { get; set; }
        public ICollection<StateAgreement> AgreementStatements { get; set; }
        public string Signature { get; set; }
        public string SignatureType { get; set; }
        [Required(ErrorMessage = "Location latitude is required.")]
        public float LatitudeCoordinate { get; set; }
        [Required(ErrorMessage = "Location lognitude is required.")]
        public float LongitudeCoordinate { get; set; }
        [Required(ErrorMessage = "Sales team selection is required.")]
        public string SalesTeamId { get; set; }
        public string AssignedPhoneNumber { get; set; }
        public string DocumentID { get; set; }
        public bool TpivBypass { get; set; }
        public string TpivBypassSignature { get; set; }
        public string TpivBypassSsnDocumentId { get; set; }
        public string TpivBypassDobDocumentId { get; set; }
        public string TpivBypassSsnLast4Digits { get; set; }
        public string TpivBypassDobLast4Digits { get; set; }
        public int TenantAddressID { get; set; }

        public string CompanyID { get; set; }
        public string UserID { get; set; }
        public string LifelineEnrollmentType { get; set; }
        public string LifelineProgramAnnualIncome { get; set; }
        public string LoggedInUserFname { get; set; }
        public string LoggedInUserLname { get; set; }
    }

    public class OrderUpdateBindingModel {
        public string Id { get; set; }
        public bool HouseholdReceivesLifelineBenefits { get; set; }
        public bool CustomerReceivesLifelineBenefits { get; set; }
        public string QBFirstName { get; set; }
        public string QBLastName { get; set; }
        public string QBSsn { get; set; }
        public string QBDateOfBirth { get; set; }
        public string LifelineProgramId { get; set; }
        public string LPProofTypeId { get; set; }
        public string LPProofImageUploadId { get; set; }
        public string LPProofNumber { get; set; }
        public string CurrentLifelinePhoneNumber { get; set; }
        public string StateProgramId { get; set; }
        public string StateProgramNumber { get; set; }
        public string SecondaryStateProgramId { get; set; }
        public string SecondaryStateProgramNumber { get; set; }
        public string FirstName { get; set; }
        public string MiddleInitial { get; set; }
        public string LastName { get; set; }
        public string Ssn { get; set; }
        public string DateOfBirth { get; set; }
        public string EmailAddress { get; set; }
        public string ContactPhoneNumber { get; set; }
        public string IPProofTypeId { get; set; }
        public string IPProofImageUploadId { get; set; }
        public string ServiceAddressStreet1 { get; set; }
        public string ServiceAddressStreet2 { get; set; }
        public string ServiceAddressCity { get; set; }
        public string ServiceAddressState { get; set; }
        public string ServiceAddressZip { get; set; }
        public bool ServiceAddressIsPermanent { get; set; }
        public bool ServiceAddressIsRural { get; set; }
        public string BillingAddressStreet1 { get; set; }
        public string BillingAddressStreet2 { get; set; }
        public string BillingAddressCity { get; set; }
        public string BillingAddressState { get; set; }
        public string BillingAddressZip { get; set; }
        public string ShippingAddressStreet1 { get; set; }
        public string ShippingAddressStreet2 { get; set; }
        public string ShippingAddressCity { get; set; }
        public string ShippingAddressState { get; set; }
        public string ShippingAddressZip { get; set; }
        public bool IsHoh { get; set; }
        public bool HohSpouse { get; set; }
        public bool HohAdultsParent { get; set; }
        public bool HohAdultsChild { get; set; }
        public bool HohAdultsRelative { get; set; }
        public bool HohAdultsRoommate { get; set; }
        public bool HohAdultsOther { get; set; }
        public string HohAdultOtherText { get; set; }
        public bool? HohExpenses { get; set; }
        public bool? HohShareLifeline { get; set; }
        public bool? HohAgreeMultihouse { get; set; }
        public bool HohAgreeViolation { get; set; }
        public string Signature { get; set; }
        public string SignatureType { get; set; }
        public float LatitudeCoordinate { get; set; }
        public float LongitudeCoordinate { get; set; }
        public string SalesTeamId { get; set; }
        public string AssignedPhoneNumber { get; set; }
        public string DocumentID { get; set; }
        public bool TpivBypass { get; set; }
        public string TpivBypassSignature { get; set; }
        public string TpivBypassSsnDocumentId { get; set; }
        public string TpivBypassDobDocumentId { get; set; }
        public string TpivBypassSsnLast4Digits { get; set; }
        public string TpivBypassDobLast4Digits { get; set; }
        public int TenantAddressID { get; set; }
        public int TenantReferenceId { get; set; }
        public double PricePlan { get; set; }
        public double PriceTotal { get; set; }
        public string CompanyID { get; set; }
        public string UserID { get; set; }

        public string LifelineEnrollmentType { get; set; }

        public string DeviceModel { get; set; }
        public string DeviceId { get; set; }
        public string FulfillmentType { get; set; }
    }
    public class SubmitOrderBindingModel {
        public string OrderId { get; set; }
        public string ParentOrderId { get; set; }
        public bool HouseholdReceivesLifelineBenefits { get; set; }
        public bool CustomerReceivesLifelineBenefits { get; set; }
        public string QBFirstName { get; set; }
        public string QBLastName { get; set; }
        public string QBSsn { get; set; }

        public string QBDateOfBirth { get; set; }
        [Required(ErrorMessage = "Lifeline Program Id is required.")]
        public string LifelineProgramId { get; set; }
        [Required(ErrorMessage = "Lifeline Program proof type is required.")]
        public string LPProofTypeId { get; set; }
        public string LPProofImageUploadId { get; set; }
        public string LPProofNumber { get; set; }
        public string CurrentLifelinePhoneNumber { get; set; }
        public string StateProgramId { get; set; }
        public string StateProgramNumber { get; set; }
        public string SecondaryStateProgramId { get; set; }
        public string SecondaryStateProgramNumber { get; set; }
        public string Language { get; set; }
        public string CommunicationPreference { get; set; }
        [Required(ErrorMessage = "Customer first name is required.")]
        public string FirstName { get; set; }
        public string MiddleInitial { get; set; }
        [Required(ErrorMessage = "Customer last name is required.")]
        public string LastName { get; set; }
        public string Gender { get; set; }

        [Required(ErrorMessage = "Customer social security number is required.")]
        public string Ssn { get; set; }
        [Required(ErrorMessage = "Customer date of birth is required.")]
        public string DateOfBirth { get; set; }
        public string EmailAddress { get; set; }
        public string ContactPhoneNumber { get; set; }
        // [Required(ErrorMessage = "Identity proof type is required.")]
        public string IPProofTypeId { get; set; }
        public string IPProofImageUploadId { get; set; }
        public string IPProofImageUploadId2 { get; set; }
        [Required(ErrorMessage = "Service address street 1 is required.")]
        public bool HasServiceAddressBypass { get; set; }
        public string ServiceAddressBypassSignature { get; set; }
        public string ServiceAddressByPassImageID { get; set; }
        public string ServiceAddressByPassImageFileName { get; set; }
        public string ServiceAddressByPassDocumentProofId { get; set; }
        public string ServiceAddressStreet1 { get; set; }
        public string ServiceAddressStreet2 { get; set; }
        [Required(ErrorMessage = "Service address city is required.")]
        public string ServiceAddressCity { get; set; }
        [Required(ErrorMessage = "Service address state is required.")]
        public string ServiceAddressState { get; set; }
        [Required(ErrorMessage = "Service address zip is required.")]
        public string ServiceAddressZip { get; set; }
        [Required(ErrorMessage = "Service address residence type is required.")]
        public bool ServiceAddressIsPermanent { get; set; }
        public bool ServiceAddressIsRural { get; set; }

        public string BillingAddressStreet1 { get; set; }
        public string BillingAddressStreet2 { get; set; }

        public string BillingAddressCity { get; set; }

        public string BillingAddressState { get; set; }

        public string BillingAddressZip { get; set; }
        [Required(ErrorMessage = "Shipping address street 1 is required.")]
        public string ShippingAddressStreet1 { get; set; }
        public string ShippingAddressStreet2 { get; set; }
        [Required(ErrorMessage = "Shipping address city is required.")]
        public string ShippingAddressCity { get; set; }
        [Required(ErrorMessage = "Shipping address state is required.")]
        public string ShippingAddressState { get; set; }
        [Required(ErrorMessage = "Shipping address zip is required.")]
        public string ShippingAddressZip { get; set; }
        public bool IsHoh { get; set; }
        [Required(ErrorMessage = "HOH Spouse response is required.")]
        public bool HohSpouse { get; set; }
        [Required(ErrorMessage = "HOH parent response is required.")]
        public bool HohAdultsParent { get; set; }
        [Required(ErrorMessage = "HOH adult child response is required.")]
        public bool HohAdultsChild { get; set; }
        [Required(ErrorMessage = "HOH adult relative response is required.")]
        public bool HohAdultsRelative { get; set; }
        [Required(ErrorMessage = "HOH adult roommate response is required.")]
        public bool HohAdultsRoommate { get; set; }
        [Required(ErrorMessage = "HOH adult other response is required.")]
        public bool HohAdultsOther { get; set; }
        public string HohAdultsOtherText { get; set; }
        public bool? HohExpenses { get; set; }
        public bool? HohShareLifeline { get; set; }
        public bool? HohAgreeMultihouse { get; set; }
        public bool HohAgreeViolation { get; set; }
        public bool? HohPuertoRicoAgreeViolation { get; set; }
        public ICollection<StateAgreement> AgreementStatements { get; set; }
        [Required(ErrorMessage = "Signature is missing.")]
        public string Signature { get; set; }
        [Required(ErrorMessage = "Signature Type is missing.")]
        public string SignatureType { get; set; }
        public string SigFileName { get; set; }//name of file stored on amazon
        [Required(ErrorMessage = "Agreement Initials is missing.")]
        public string Initials { get; set; }
        public string InitialsFileName { get; set; }//Intitials FileName on amazon
        [Required(ErrorMessage = "Location latitude is required.")]
        public float LatitudeCoordinate { get; set; }
        [Required(ErrorMessage = "Location longitude is required.")]
        public float LongitudeCoordinate { get; set; }
        [Required(ErrorMessage = "Sales team selection is required.")]
        public string SalesTeamId { get; set; }
        public string AssignedPhoneNumber { get; set; }
        public string DocumentID { get; set; }
        public bool TpivBypass { get; set; }
        public string TpivBypassType { get; set; }
        public string TpivBypassSignature { get; set; }
        public string TpivBypassSsnDocumentId { get; set; }
        public string TpivSsnImageUploadId { get; set; }
        public string TpivBypassDobDocumentId { get; set; }
        public string TpivBypassSsnLast4Digits { get; set; }
        public string TpivBypassDobLast4Digits { get; set; }
        public string LexID { get; set; }
        public int TenantAddressID { get; set; }
        public int TenantReferenceId { get; set; }
        public double PricePlan { get; set; }
        public double PriceTotal { get; set; }
        public string CompanyID { get; set; }
        public LifelineProgramAnnualIncome LifelineProgramAnnualIncome { get; set; }
        public string ExternalVelocityCheck { get; set; }//PASS or FAIL 
        public string DeviceModel { get; set; }
        [Required(ErrorMessage = "Device Option is required.")]
        public string DeviceId { get; set; }
        public string ByopCarrier { get; set; }
        public string DeviceCompatibility { get; set; }
        public string FulfillmentType { get; set; }
        public string AgentFirstName { get; set; }
        public string AgentLastName { get; set; }
        [Required(ErrorMessage = "Transaction Id required.")]
        public SolixValidationDetails ValidationDetails { get; set; }
        public string TransactionId { get; set; }
        public string TpivCode { get; set; }
        public string TpivRiskIndicators { get; set; }
        public string TpivNasScore { get; set; }
        public string TpivTransactionID { get; set; }
        public string LPProofImageUploadFilename { get; set; }
        public string IPProofImageUploadFilename { get; set; }
        public string IPProofImageUploadFilename2 { get; set; }
        public string TpivSsnImageUploadFilename { get; set; }
        public string SignatureImageFilename { get; set; }
    }

    public class OrderStatusBindingModel {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CustFullName { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }
        public string EmployeeFullName { get; set; }
        public string SalesTeamName { get; set; }
        public string SalesTeamExternalDisplayName { get; set; }
        public string Location { get; set; }
        public string DeviceIdentifier { get; set; }
        public string DeviceId { get; set; }
        public string SimIdentifier { get; set; }
        public string FulfillmentType { get; set; }
        public string TenantAccountID { get; set; }
        public int StatusCode { get; set; }
        public string Status { get; set; }
        public string AccountStatus { get; set; }
        public int ActivationAttempted { get; set; }
        public DateTime? ActivationDate { get; set; }
        public DateTime DateCreated { get; set; }
        public string RTR_Name { get; set; }
        public DateTime RTR_Date { get; set; }
        public string RTR_RejectCode { get; set; }
        public string RTR_Notes { get; set; }
    }

    public class CaliforniaPrecheckBindingModel {
        public string Firstname { get; set; }
        public string MI { get; set; }
        public string Lastname { get; set; }
        public string Dob { get; set; }
        public string SSN { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
        public string CurrentLifelinePhoneNUmber { get; set; }
        public string CompanyID { get; set; }
    }

    public class GetTFVerifyInfoRequestBindingModel {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string ShippingAddress1 { get; set; }
        public string ShippingAddress2 { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingZip { get; set; }
        public string Dob { get; set; }
        public string SsnLast4 { get; set; }
        public string OrderId { get; set; }
        public bool HasServiceAddressBypass { get; set; }
        public int AddressBypassCount { get; set; }
        public bool TpivBypass { get; set; }
        public string TpivBypassType { get; set; }
        public string TpivBypassSsnDocumentId { get; set; }
        public string TpivBypassDobDocumentId { get; set; }
    }
    public class DeviceAddressValidationResponse {
        //Address Validation
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public bool IsValid { get; set; } = false;
        public bool AddressBypassAvailable { get; set; }
    }
    public class DeviceDetailResponse {
        public TFVerifyResponseBindingModel TFVerifyData { get; set; }
        public DeviceAddressValidationResponse ValidatedAddress { get; set; }
        public DeviceAddressValidationResponse ShippingValidatedAddress { get; set; }
    }
    public class ReturnedCarriers {
        public string Name { get; set; }
        public string Code { get; set; }
    }
    public class TFVerifyResponseBindingModel {
        public string IsDuplicate { get; set; }
        public string IsLxNxVerified { get; set; }
        public string LxnxId { get; set; }
        public string LxnxTransactionId { get; set; }
        public string LxnxNameAddressSSNSummary { get; set; }
        public string LxnxRiskIndicators { get; set; }
        public string IsBYOPAvailable { get; set; }
        public string IsRequalification { get; set; }
        public string PreviousMDN { get; set; }
        public string PreviousEnrollmentId { get; set; }
        public List<ReturnedCarriers> ByopCarriers { get; set; }
        public string AgentCommission { get; set; }
        public string IsFreePhoneEligible { get; set; }
        public string ByopPricePlanEnglish { get; set; }
        public string ByopPricePlanSpanish { get; set; }
        public string NewCustomerPricePlanEnglish { get; set; }
        public string NewCustomerPricePlanSpanish { get; set; }
        public string ReturningCustomerPricePlanEnglish { get; set; }
        public string ReturningCustomerPricePlanSpanish { get; set; }
        public string AgentNoCommissionMessageEnglish { get; set; }
        public string AgentNoCommissionMessageSpanish { get; set; }
        public string IsBlacklistedAddress { get; set; }
        public string IsBlacklistedSSN { get; set; }
        public bool TpivBypassAvailable { get; set; }
        public string TpivBypassFailureType { get; set; }
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }

    }

    public class WebcamUploadBindingModel {
        public string DeviceDetails { get; set; }
        public string Base64EncodedImage { get; set; }
        public string UpLoadType { get; set; }
    }
    public class OrderDetailExport {
        public string OrderCode { get; set; }
        public string DateCreated { get; set; }
        public string AccountDate { get; set; }
        public string Level1Name { get; set; }
        public string AgentID { get; set; }
        public string TeamName { get; set; }
        public string EmployeeName { get; set; }
        public string PromoCode { get; set; }
        public string ServiceAddressState { get; set; }
        public string StatusCode { get; set; }
        public string StatusName { get; set; }
        public string DeviceIdentifier { get; set; }
        public string FulfillmentName { get; set; }
        public string ActivationDate { get; set; }
        public string FulfillmentPromoCode { get; set; }
    }

	public class ActivationDetailBindingModel
	{
		public string ESN{ get; set; }
		public string EnrollmentNumber { get; set; }
		public string PromoCode { get; set; }
		public DateTime? ActivationDate { get; set; }
		public DateTime? DateProcessed { get; set; }
		public string DeviceType { get; set; } 
	}
}


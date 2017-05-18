using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LS.Domain;
using LS.Core.Interfaces;
using LS.Domain.ExternalApiIntegration.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Microsoft.Build.Framework;

namespace LS.ApiBindingModels {
    public class SolixAPICreateCustomerRequest {
        public string CreateUserID { get; set; }
        public string FieldActivation { get; set; }
        public string FirstName { get; set; }
        public string Mi { get; set; }
        public string LastName { get; set; }
        public string ResidentialAddress1 { get; set; }
        public string ResidentialAddress2 { get; set; }
        public string ResidentialCity { get; set; }
        public string ResidentialState { get; set; }
        public string ResidentialZip { get; set; }
        public string IsTempAddress { get; set; }
        public string ShippingAddress1 { get; set; }
        public string ShippingAddress2 { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingZip { get; set; }
        public string Email { get; set; }
        public string ContactPhone { get; set; }
        public string DOB { get; set; }
        public string SSNLast4 { get; set; }
        public string DocumentImage1 { get; set; }
        public string DocumentImage2 { get; set; }
        public string DocumentImage3 { get; set; }
        public string DocumentImage4 { get; set; }
        public string DocumentImage5 { get; set; }
        public string DocumentFileName1 { get; set; }
        public string DocumentFileName2 { get; set; }
        public string DocumentFileName3 { get; set; }
        public string DocumentFileName4 { get; set; }
        public string DocumentFileName5 { get; set; }
        public string DocumentFileDescription1 { get; set; }
        public string DocumentFileDescription2 { get; set; }
        public string DocumentFileDescription3 { get; set; }
        public string DocumentFileDescription4 { get; set; }
        public string DocumentFileDescription5 { get; set; }
        public string LxnxID { get; set; }
        public string LxnxNasScore { get; set; }
        public string LxnxRiskIndicators { get; set; }
        public string LxnxTransactionID { get; set; }
        public string WorksheetCA_Q1 { get; set; }
        public string WorksheetCA_Q2 { get; set; }
        public string WorksheetCA_Q2a { get; set; }
        public string WorksheetCA_Q2b { get; set; }
        public string WorksheetCA_Q2c { get; set; }
        public string WorksheetCA_Q2d { get; set; }
        public string WorksheetCA_Q2e { get; set; }
        public string WorksheetCA_Q2other { get; set; }
        public string WorksheetCA_Q3 { get; set; }
        public string WorksheetCA_CertificationF { get; set; }
        public string WorksheetCA_CertificationG { get; set; }
        public string Signature { get; set; }
        public string CustomerInitials { get; set; }
        public string SignedLegalGuardian { get; set; }
        public string Language { get; set; }
        public string Eligibility { get; set; }
        public string ProgramList { get; set; }
        public string HouseholdSize { get; set; }
        public string AnnualHouseholdIncome { get; set; }
        public string HouseholdNumberAdult { get; set; }
        public string HouseholdNumberKids { get; set; }
        //is below used?
        public string PrintType { get; set; }
        public string AlreadyEnrolled { get; set; }
        public string PrivacyLaw { get; set; }
        public string Latitude { get; set; }//new from our values
        public string Longitude { get; set; }//new from our values       
        public string IsBYOP { get; set; }//Y or N echo of isBYOPAvailable
        public string IsRequalification { get; set; }//Y or N echo of isRequalification
        public string RequalificationMDN { get; set; }//new echo of previousMDN
        public string RequalificationAppId { get; set; }//new echo of previousEnrollmentId
        public string IsFreePhoneEligible { get; set; }//new echo of isFreePhoneEligible
        public string Device_type { get; set; }//new - selected Device Type (hard coded values)
        public string Byop_Carrier { get; set; }//new --selected byopCarrierCode

    }

    public class SolixAPICreateCustomerResponse {
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
        public string EnrollmentID { get; set; }
    }

    public class SolixAPILexusNexusCheckRequest {
        public string CreateUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ResidentialAddress1 { get; set; }
        public string ResidentialAddress2 { get; set; }
        public string ResidentialCity { get; set; }
        public string ResidentialZip { get; set; }
        public string Dob { get; set; }
        public string SsnLast4 { get; set; }
        public string LxnxId { get; set; }
    }

    public class SolixAPITracFoneVerificationRequest {
        public string CreateUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ResidentialAddress1 { get; set; }
        public string ResidentialAddress2 { get; set; }
        public string ResidentialCity { get; set; }
        public string ResidentialState { get; set; }
        public string ResidentialZip { get; set; }
        public string Dob { get; set; }
        public string SsnLast4 { get; set; }

        }

    public class SolixAPITracFoneVerificationResponse {
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
        public string ByopCarriers { get; set; }
        public string ByopCarrierCodes { get; set; }
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
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class SolixAPILexusNexusVerificationResponse {
        public string Status { get; set; }
        public string IsDuplicate { get; set; }
        public string IsLxNxVerified { get; set; }
        public string LxnxId { get; set; }
        public string LxnxTransactionId { get; set; }
        public string LxnxNameAddressSSNSummary { get; set; }
        public string LxnxRiskIndicators { get; set; }
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class SolixValidationDetails {
        public string IsBYOP { get; set; }
        public string IsRequalification { get; set; }
        public string IsFreePhoneEligible { get; set;}
        public string RequalificationAppId { get; set;}
        public string RequalificationMDN { get; set;}
        public string AgentCommission { get; set; }
    }
    public class SolixAPILexusNexusCheckResponse {
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
        public string Id { get; set; }
        public string Duplicate { get; set; }
    }
}

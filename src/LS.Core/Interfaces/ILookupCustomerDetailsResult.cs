using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Core.Interfaces
{
    public interface ILookupCustomerDetailsResult
    {
        int ErrorCode { get; set; }
        string ErrorMessage { get; set; }
        string ErrorDescription { get; set; }
        bool IsError { get; set; }
        bool CustomerDetailsAvailable { get; set; }
        string BudgetMobileID { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string Status { get; set; }
        string ProviderID { get; set; }
        string ProviderName { get; set; }
        string AccountCredits { get; set; }
        string MobileNumber { get; set; }
        string SSN { get; set; }
        string Birthdate { get; set; }
        string Address1 { get; set; }
        string Address2 { get; set; }
        string City { get; set; }
        string State { get; set; }
        string ZIP { get; set; }
        string EmailAddress { get; set; }
        string ContactNumber { get; set; }
        string Lifeline { get; set; }
        string Lifeline_Expiration { get; set; }
        string BillingAddress { get; set; }
        string BillingAddress2 { get; set; }
        string BillingCity { get; set; }
        string BillingState { get; set; }
        string BillingZip { get; set; }
        string TopUpExpiration { get; set; }
        string AlternateIDDesc { get; set; }
        string AlternateIDValue { get; set; }
        string Beneficiary { get; set; }
        string Balance { get; set; }
        string DeviceID { get; set; }
        string AccountCompleted { get; set; }
        string DateCompleted { get; set; }
       string VoiceOnly { get; set; }
        string RatePlanPurchaseEligible_Immediate { get; set; }
        string RatePlanPurchaseEligible_Advance { get; set; }
        string ServiceEndDate { get; set; }
        string RatePlanDisplay { get; set; }
        string ServiceStartDate { get; set; }
        string IMEI { get; set; }
        string SIM { get; set; }
        string ServicePlan { get; set; }
        string Employee { get; set; }
        string PrequalificationCode { get; set; }
        string HOHC { get; set; }
        string LastCallDateTime { get; set; }
        string FromOrToNumber { get; set; }
        string StateRegCode { get; set; }
        string DocumentVerification { get; set; }
        string BeneficiaryFirstName { get; set; }
        string BeneficiaryLastName { get; set; }
        string BeneficiaryDOB { get; set; }
        string BeneficiarySSN { get; set; }
        string LifelineCodeName { get; set; }
        string LifelineCode { get; set; }
        string GatewayUse { get; set; }
        string StatusID { get; set; }
        string DirectNonLifeline { get; set; }
        string ProductID { get; set; }
        string RuralAddress { get; set; }
        string StateRegCode2 { get; set; }
        string IMSI { get; set; }
        bool ValidForPlanChange { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LS.Core.Interfaces;
namespace LS.Domain
{
    public class LookupCustomerDetailsResult : ILookupCustomerDetailsResult
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDescription { get; set; }
        public bool IsError { get; set; }
        public bool CustomerDetailsAvailable { get; set; }
        public string BudgetMobileID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Status { get; set; }
        public string ProviderID { get; set; }
        public string ProviderName { get; set; }
        public string AccountCredits { get; set; }
        public string MobileNumber { get; set; }
        public string SSN { get; set; }
        public string Birthdate { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZIP { get; set; }
        public  string EmailAddress { get; set; }
        public string ContactNumber { get; set; }
        public string Lifeline { get; set; }
        public string Lifeline_Expiration { get; set; }
        public string BillingAddress { get; set; }
        public string BillingAddress2 { get; set; }
        public string BillingCity { get; set; }
        public string BillingState{ get; set; }
        public string BillingZip { get; set; }
        public string TopUpExpiration { get; set; }
        public string AlternateIDDesc { get; set; }
        public string AlternateIDValue { get; set; }
        public string Beneficiary { get; set; }
        public string Balance { get; set; }
        public string DeviceID { get; set; }
        public string AccountCompleted { get; set; }
        public string DateCompleted { get; set; }
        public string VoiceOnly { get; set; }
        public string RatePlanPurchaseEligible_Immediate { get; set; }
        public string RatePlanPurchaseEligible_Advance { get; set; }
        public string ServiceEndDate { get; set; }
        public string RatePlanDisplay { get; set; }
        public string ServiceStartDate { get; set; }
        public string IMEI { get; set; }
        public string SIM { get; set; }
        public string ServicePlan { get; set; }
        public string Employee { get; set; }
        public string PrequalificationCode { get; set; }
        public string HOHC { get; set; }
        public string LastCallDateTime { get; set; }
        public string FromOrToNumber { get; set; }
        public string StateRegCode { get; set; }
        public string DocumentVerification { get; set; }
        public string BeneficiaryFirstName { get; set; }
        public string BeneficiaryLastName { get; set; }
        public string BeneficiaryDOB { get; set; }
        public string BeneficiarySSN { get; set; }
        public string LifelineCodeName { get; set; }
        public string LifelineCode { get; set; }
        public string GatewayUse { get; set; }
        public string StatusID { get; set; }
        public string DirectNonLifeline { get; set; }
        public string ProductID { get; set; }
        public string RuralAddress { get; set; }
        public string StateRegCode2 { get; set; }
        public string IMSI { get; set; }
        public bool ValidForPlanChange { get; set; }
    }
}

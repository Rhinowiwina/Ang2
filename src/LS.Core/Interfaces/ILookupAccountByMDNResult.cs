using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Core.Interfaces
{
    public interface ILookupAccountByMDNResult {
        int ErrorCode { get; set; }
        string ErrorMessage { get; set; }
        string ErrorDescription { get; set; }
        bool IsError { get; set; }
        int BudgetMobileId { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        int AccountCredits { get; set; }
        int ProviderID { get; set; }
        string ProviderName { get; set; }
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
         string LifeLineCodeName { get; set; }
         string Lifeline_Expiration { get; set; }
         string BillingAddress { get; set; }
         string BillingAddress2 { get; set; }
         string BillingCity { get; set; }
         string BillingState { get; set; }
         string BillingZip { get; set; }
         int LifelineCode { get; set; }
         string TopUpExpiration { get; set; }
         string LastCallDateTime { get; set; }
         string FromOrToNumber { get; set; }
         string AlternateIDDesc { get; set; }
         string AlternateIDValue { get; set; }
         bool Lifeline { get; set; }
         string Beneficiary { get; set; }
         string LastBasePlanPurchase { get; set; }
         double Balance { get; set; }
         string DeviceID { get; set; }
         bool RestrictUTT { get; set; }
         bool AccountCompleted { get; set; }
         string DateCompleted { get; set; }
         bool VoiceOnly { get; set; }
         bool TT_PendingConversion { get; set; }
         bool TT_ConversionProhibited { get; set; }
         bool RatePlanPurchaseEligible_Immediate { get; set; }
         bool RatePlanPurchaseEligible_Advance { get; set; }
         string ServiceEndDate { get; set; }
         string RatePlanDisplay { get; set; }
         bool ValidForPayment { get; set; }
         bool RecertificationMustUpdate { get; set; }
         bool ValidForRecertification { get; set; }
         bool CommissionableRecertification { get; set; }
         bool ValidForTopup { get; set; }
         int RecertificationCreditEarned { get; set; }
         bool ValidForAccountDetails { get; set; }
         bool RuralAddress { get; set; }
         string StateRegCode2 { get; set; }
         string IMSI { get; set; }
    }
}

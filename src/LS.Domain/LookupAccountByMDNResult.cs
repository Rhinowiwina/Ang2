using LS.Core.Interfaces;

namespace LS.Domain
{
    public class LookupAccountByMDNResult : ILookupAccountByMDNResult
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDescription { get; set; }
        public bool IsError { get; set; }
        public int BudgetMobileId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int AccountCredits { get; set; }
        public int ProviderID { get; set; }
        public string ProviderName { get; set; }
        public string MobileNumber { get; set; }
        public string SSN { get; set; }
        public string Birthdate { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZIP { get; set; }
        public string EmailAddress { get; set; }
        public string ContactNumber { get; set; }
        public string LifeLineCodeName { get; set; }
        public string Lifeline_Expiration { get; set; }
        public string BillingAddress { get; set; }
        public string BillingAddress2 { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingZip { get; set; }
        public int LifelineCode { get; set; }
        public string TopUpExpiration { get; set; }
        public string LastCallDateTime { get; set; }
        public string FromOrToNumber { get; set; }
        public string AlternateIDDesc { get; set; }
        public string AlternateIDValue { get; set; }
        public bool Lifeline { get; set; }
        public string Beneficiary { get; set; }
        public string LastBasePlanPurchase { get; set; }
        public double Balance { get; set; }
        public string DeviceID { get; set; }
        public bool RestrictUTT { get; set; }
        public bool AccountCompleted { get; set; }
        public string DateCompleted { get; set; }
        public bool VoiceOnly { get; set; }
        public bool TT_PendingConversion { get; set; }
        public bool TT_ConversionProhibited { get; set; }
        public bool RatePlanPurchaseEligible_Immediate { get; set; }
        public bool RatePlanPurchaseEligible_Advance { get; set; }
        public string ServiceEndDate { get; set; }
        public string RatePlanDisplay { get; set; }
        public bool ValidForPayment { get; set; }
        public bool RecertificationMustUpdate { get; set; }
        public bool ValidForRecertification { get; set; }
        public bool CommissionableRecertification { get; set; }
        public bool ValidForTopup { get; set; }
        public int RecertificationCreditEarned { get; set; }
        public bool ValidForAccountDetails { get; set; }
        public bool RuralAddress { get; set; }
        public string StateRegCode2 { get; set; }
        public string IMSI { get; set; }
    }
}

namespace LS.Domain.Chase{
    public class NewOrderResponse{
        public ErrorMessage errorMessage {get; set;}
        public string approvalStatus {get; set;}
        public string authorizationCode {get; set;}
        public string avsRespCode {get; set;}
        public string bin {get; set;}
        public string cardBrand {get; set;}
        public string ccAccountNum {get; set;}
        public string countryFraudFilterStatus {get; set;}
        public string ctiAffluentCard {get; set;}
        public string ctiCommercialCard {get; set;}
        public string ctiDurbinExemption {get; set;}
        public string ctiHealthcareCard {get; set;}
        public string ctiIssuingCountry {get; set;}
        public string ctiLevel3Eligible {get; set;}
        public string ctiPINlessDebitCard {get; set;}
        public string ctiPayrollCard { get; set; } 
        public string ctiPrepaidCard { get; set; }
        public string ctiSignatureDebitCard {get; set;}
        public string customerName {get; set;}
        public string customerRefNum {get; set;}
        public string cvvRespCode {get; set;}
        public string debitBillerReferenceNumber {get; set;}
        public string debitPinNetworkID {get; set;}
        public string debitPinSurchargeAmount {get; set;}
        public string debitPinTraceNumber {get; set;}
        public FraudAnalysisResponse fraudAnalysisResponse {get; set;}
        public string fraudScoreProcMsg {get; set;}
        public string fraudScoreProcStatus {get; set;}
        public string giftCardInd {get; set;}
        public string hostAVSRespCode {get; set;}
        public string hostCVVRespCode {get; set;}
        public string hostRespCode {get; set;}
        public string industryType {get; set;}
        public string isoCountryCode {get; set;}
        public string lastRetryDate {get; set;}
        public string mbMicroPaymentDaysLeft {get; set;}
        public string mbMicroPaymentDollarsLeft {get; set;}
        public string mbStatus {get; set;}
        public string mcRecurringAdvCode {get; set;}
        public string merchantID {get; set;}
        public string orderID {get; set;}
        public string partialAuthOccurred {get; set;}
        public string procStatus {get; set;}
        public string procStatusMessage {get; set;}
        public string profileProcStatus {get; set;}
        public string profileProcStatusMsg {get; set;}
        public string redeemedAmount {get; set;}
        public string remainingBalance {get; set;}
        public string requestAmount {get; set;}
        public string respCode {get; set;}
        public string respCodeMessage {get; set;}
        public string respDateTime {get; set;}
        public string retryAttempCount {get; set;}
        public string retryTrace {get; set;}
        public string terminalID {get; set;}
        public string transType {get; set;}
        public string txRefIdx {get; set;}
        public string txRefNum {get; set;}
        public string version {get; set;}
        public string visaVbVRespCode {get; set;}
        public string TransactionLogID { get; set; }
    }
}

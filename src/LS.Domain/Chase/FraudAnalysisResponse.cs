using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Domain.Chase {
    public class FraudAnalysisResponse {
        public string autoDecisionResponse {get; set;}
        public string browserCountry {get; set;}
        public string browserLanguage {get; set;}
        public string cookiesStatus {get; set;}
        public string customerLocalDateTime {get; set;}
        public string customerNetwork {get; set;}
        public string customerRegion {get; set;}
        public string customerTimeZone {get; set;}
        public string deviceCountry {get; set;}
        public string deviceFingerprint {get; set;}
        public string deviceLayers {get; set;}
        public string deviceRegion {get; set;}
        public string flashStatus {get; set;}
        public string fourteenDayVelocity {get; set;}
        public string fraudScoreIndicator {get; set;}
        public string fraudStatusCode {get; set;}
        public string javascriptStatus {get; set;}
        public string kaptchaMatchFlag {get; set;}
        public string mobileDeviceIndicator {get; set;}
        public string mobileDeviceType {get; set;}
        public string mobileWirelessIndicator {get; set;}
        public string numberOfCards {get; set;}
        public string numberOfDevices {get; set;}
        public string numberOfEmails {get; set;}
        public string paymentBrand {get; set;}
        public string pcRemoteIndicator {get; set;}
        public string proxyStatus {get; set;}
        public string riskInquiryTransactionID {get; set;}
        public string riskScore {get; set;}
        public string rulesData {get; set;}
        public string rulesDataLength {get; set;}
        public string sixHourVelocity {get; set;}
        public string voiceDevice {get; set;}
        public string worstCountry {get; set;}
    }
}

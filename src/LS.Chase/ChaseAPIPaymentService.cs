using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using LS.Core;
using LS.Chase.ChaseAPIService;
using LS.Chase.Logging;
using LS.Services;
using LS.Domain;
using LS.Utilities;
using Common.Logging;
using LS.Repositories.DBContext;

namespace LS.Chase {
    public class ChaseAPIPaymentService {

        private ILog Logger { get; set; }
        private static string APIUsername = ConfigurationManager.AppSettings["ChaseUsername"];
        private static string APIPassword = ConfigurationManager.AppSettings["ChasePassword"];
        private static string APIVersion = ConfigurationManager.AppSettings["ChaseVersion"];
        private static string APIBIN = ConfigurationManager.AppSettings["ChaseBIN"];
        private static string APIMercName = ConfigurationManager.AppSettings["ChaseMercName"];
        private static string APICity = ConfigurationManager.AppSettings["ChaseCity"];
        private static string APIPhone = ConfigurationManager.AppSettings["ChasePhone"];
        private static string APIURL = ConfigurationManager.AppSettings["ChaseURL"];
        private static string APIEmail = ConfigurationManager.AppSettings["ChaseEmail"];
        private static string APIMerchantID = ConfigurationManager.AppSettings["ChaseMerchantID"];
        private static string APITerminalID = ConfigurationManager.AppSettings["ChaseTerminalID"];

        private static bool APIAddSoftDesc = false;
        private static bool IsDev = false;
        private static string NewOrderEndpoint = "https://ws.paymentech.net/PaymentechGateway";

        public ChaseAPIPaymentService() {
            Logger = LoggerFactory.GetLogger(GetType());

            if (ConfigurationManager.AppSettings["Environment"] == "DEV") {
                IsDev = true;
            } else {
                IsDev = false;
            }
        }

        public async Task<ServiceProcessingResult<LS.Domain.Chase.NewOrderResponse>> NewOrder(LS.Domain.Chase.NewOrderRequest requestData) {
            var processingResult = new ServiceProcessingResult<LS.Domain.Chase.NewOrderResponse> { IsSuccessful = false };

            var paymentTechClient = new PaymentechClient();
            paymentTechClient.Endpoint.Behaviors.Add(new NewOrderInspectorBehavior());

            var newOrderRequest = new NewOrderRequest();

            newOrderRequest.endppoint = NewOrderEndpoint;
            newOrderRequest.orbitalConnectionUsername = APIUsername;
            newOrderRequest.orbitalConnectionPassword = APIPassword;
            newOrderRequest.version = APIVersion;

            newOrderRequest.industryType = "EC";
            if (IsDev) { newOrderRequest.transType = "A"; } else { newOrderRequest.transType = requestData.transType; }

            var orderAmount = requestData.amount * 100;

            newOrderRequest.bin = APIBIN;
            newOrderRequest.merchantID = APIMerchantID;
            newOrderRequest.terminalID = APITerminalID;
            newOrderRequest.ccAccountNum = requestData.ccAccountNum;
            newOrderRequest.ccExp = requestData.ccExp;
            newOrderRequest.ccCardVerifyPresenceInd = "1";
            newOrderRequest.ccCardVerifyNum = requestData.ccCardVerifyNum;
            newOrderRequest.avsAddress1 = requestData.avsAddress1;
            newOrderRequest.avsAddress2 = requestData.avsAddress2;
            newOrderRequest.avsCity = requestData.avsCity;
            newOrderRequest.avsState = requestData.avsState;
            newOrderRequest.avsZip = requestData.avsZip;
            newOrderRequest.avsCountryCode = requestData.avsCountryCode;
            newOrderRequest.avsName = requestData.avsName;
            newOrderRequest.avsPhone = requestData.avsPhone;
            newOrderRequest.customerName = requestData.customerFirstName + " " + requestData.customerLastName;
            newOrderRequest.customerEmail = requestData.customerEmail;
            newOrderRequest.customerPhone = requestData.customerPhone;
            newOrderRequest.amount = orderAmount.ToString();
            newOrderRequest.comments = requestData.comments;

            if (IsDev) {
                newOrderRequest.amount = "0";
                newOrderRequest.comments = "Should have been" + orderAmount;
            }

            if (APIAddSoftDesc) {
                newOrderRequest.softDescMercName = APIBIN;
                newOrderRequest.softDescMercCity = APIMerchantID;
                newOrderRequest.softDescMercPhone = APITerminalID;
                newOrderRequest.softDescMercURL = APITerminalID;
                newOrderRequest.softDescMercEmail = APIEmail;
            }

            var paymentTransactionLogService = new PaymentTransactionLogDataService();

            var paymentTransactionRequestData = new PaymentTransactionLog {
                Amount = orderAmount,
                CompanyID = requestData.companyID,
                MerchantID = APIMerchantID,
                OrderType = "SIMOrder",
                TransactionType = "Charge"
            };

            //Add transaction log
            var paymentAddTransactionResult = await paymentTransactionLogService.AddAsync(paymentTransactionRequestData);
            if (!paymentAddTransactionResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error adding log to payment transaction. If issue continues please contact support.", "Error adding log to payment transaction.  If issue continues please contact support.", true, false);
                Logger.Error("Error adding payment transaction log (Chase- NewOrder) EX: " + paymentAddTransactionResult.Error.UserMessage);

                return processingResult;
            }

            if (IsDev) {
                newOrderRequest.orderID = "d" + paymentAddTransactionResult.Data.TransactionID.ToString();
            } else {
                newOrderRequest.orderID = paymentAddTransactionResult.Data.TransactionID.ToString();
            }

            // Call API
            var newOrderResult = paymentTechClient.NewOrder(newOrderRequest);
            if (newOrderResult.errorMessage != null) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Chase - " + newOrderResult.errorMessage.errorCode, "Chase - " + newOrderResult.errorMessage.errorCode, true, false);
                return processingResult;
            } else if (newOrderResult.procStatus != "0" && newOrderResult.procStatus != null) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Chase - " + newOrderResult.procStatusMessage, "Chase - " + newOrderResult.procStatusMessage, true, false);
                return processingResult;
            }

            //Update transaction log
            using (var db = new ApplicationDbContext()) {
                db.Configuration.ValidateOnSaveEnabled = false;
                try {
                    var paymentTransactionRow = new PaymentTransactionLog { Id = paymentAddTransactionResult.Data.Id, ReferenceTransaction = newOrderResult.txRefNum, DateModified = DateTime.Now };
                    db.PaymentTransactionLog.Attach(paymentTransactionRow);
                    db.Entry(paymentTransactionRow).Property(p => p.ReferenceTransaction).IsModified = true;
                    db.Entry(paymentTransactionRow).Property(p => p.DateModified).IsModified = true;
                    db.SaveChanges();
                } catch (Exception ex) {
                    Logger.Error("Error updating payment transaction log (Chase - NewOrder)", ex);
                } finally {
                    db.Configuration.ValidateOnSaveEnabled = true;
                }
            }

            var newOrderResultErrorMessageResponseConverted = new LS.Domain.Chase.ErrorMessage();

            if (newOrderResult.errorMessage != null) {
                newOrderResultErrorMessageResponseConverted.errorCode = newOrderResult.errorMessage.errorCode;
                newOrderResultErrorMessageResponseConverted.errorText = newOrderResult.errorMessage.errorText;
            }

            var newOrderResultFraudAnalysisResponseConverted = new LS.Domain.Chase.FraudAnalysisResponse();

            if (newOrderResult.fraudAnalysisResponse != null) {
                newOrderResult.fraudAnalysisResponse.autoDecisionResponse = newOrderResult.fraudAnalysisResponse.autoDecisionResponse;
                newOrderResult.fraudAnalysisResponse.browserCountry = newOrderResult.fraudAnalysisResponse.browserCountry;
                newOrderResult.fraudAnalysisResponse.browserLanguage = newOrderResult.fraudAnalysisResponse.browserLanguage;
                newOrderResult.fraudAnalysisResponse.cookiesStatus = newOrderResult.fraudAnalysisResponse.cookiesStatus;
                newOrderResult.fraudAnalysisResponse.customerLocalDateTime = newOrderResult.fraudAnalysisResponse.customerLocalDateTime;
                newOrderResult.fraudAnalysisResponse.customerNetwork = newOrderResult.fraudAnalysisResponse.customerNetwork;
                newOrderResult.fraudAnalysisResponse.customerRegion = newOrderResult.fraudAnalysisResponse.customerRegion;
                newOrderResult.fraudAnalysisResponse.customerTimeZone = newOrderResult.fraudAnalysisResponse.customerTimeZone;
                newOrderResult.fraudAnalysisResponse.deviceCountry = newOrderResult.fraudAnalysisResponse.deviceCountry;
                newOrderResult.fraudAnalysisResponse.deviceFingerprint = newOrderResult.fraudAnalysisResponse.deviceFingerprint;
                newOrderResult.fraudAnalysisResponse.deviceLayers = newOrderResult.fraudAnalysisResponse.deviceLayers;
                newOrderResult.fraudAnalysisResponse.deviceRegion = newOrderResult.fraudAnalysisResponse.deviceRegion;
                newOrderResult.fraudAnalysisResponse.flashStatus = newOrderResult.fraudAnalysisResponse.flashStatus;
                newOrderResult.fraudAnalysisResponse.fourteenDayVelocity = newOrderResult.fraudAnalysisResponse.fourteenDayVelocity;
                newOrderResult.fraudAnalysisResponse.fraudScoreIndicator = newOrderResult.fraudAnalysisResponse.fraudScoreIndicator;
                newOrderResult.fraudAnalysisResponse.fraudStatusCode = newOrderResult.fraudAnalysisResponse.fraudStatusCode;
                newOrderResult.fraudAnalysisResponse.javascriptStatus = newOrderResult.fraudAnalysisResponse.javascriptStatus;
                newOrderResult.fraudAnalysisResponse.kaptchaMatchFlag = newOrderResult.fraudAnalysisResponse.kaptchaMatchFlag;
                newOrderResult.fraudAnalysisResponse.mobileDeviceIndicator = newOrderResult.fraudAnalysisResponse.mobileDeviceIndicator;
                newOrderResult.fraudAnalysisResponse.mobileDeviceType = newOrderResult.fraudAnalysisResponse.mobileDeviceType;
                newOrderResult.fraudAnalysisResponse.mobileWirelessIndicator = newOrderResult.fraudAnalysisResponse.mobileWirelessIndicator;
                newOrderResult.fraudAnalysisResponse.numberOfCards = newOrderResult.fraudAnalysisResponse.numberOfCards;
                newOrderResult.fraudAnalysisResponse.numberOfDevices = newOrderResult.fraudAnalysisResponse.numberOfDevices;
                newOrderResult.fraudAnalysisResponse.numberOfEmails = newOrderResult.fraudAnalysisResponse.numberOfEmails;
                newOrderResult.fraudAnalysisResponse.paymentBrand = newOrderResult.fraudAnalysisResponse.paymentBrand;
                newOrderResult.fraudAnalysisResponse.pcRemoteIndicator = newOrderResult.fraudAnalysisResponse.pcRemoteIndicator;
                newOrderResult.fraudAnalysisResponse.proxyStatus = newOrderResult.fraudAnalysisResponse.proxyStatus;
                newOrderResult.fraudAnalysisResponse.riskInquiryTransactionID = newOrderResult.fraudAnalysisResponse.riskInquiryTransactionID;
                newOrderResult.fraudAnalysisResponse.riskScore = newOrderResult.fraudAnalysisResponse.riskScore;
                newOrderResult.fraudAnalysisResponse.rulesData = newOrderResult.fraudAnalysisResponse.rulesData;
                newOrderResult.fraudAnalysisResponse.rulesDataLength = newOrderResult.fraudAnalysisResponse.rulesDataLength;
                newOrderResult.fraudAnalysisResponse.sixHourVelocity = newOrderResult.fraudAnalysisResponse.sixHourVelocity;
                newOrderResult.fraudAnalysisResponse.voiceDevice = newOrderResult.fraudAnalysisResponse.voiceDevice;
                newOrderResult.fraudAnalysisResponse.worstCountry = newOrderResult.fraudAnalysisResponse.worstCountry;
            }

            var newOrderResultConverted = new LS.Domain.Chase.NewOrderResponse {
                errorMessage = newOrderResultErrorMessageResponseConverted,
                approvalStatus = newOrderResult.approvalStatus,
                authorizationCode = newOrderResult.authorizationCode,
                avsRespCode = newOrderResult.avsRespCode,
                bin = newOrderResult.bin,
                cardBrand = newOrderResult.cardBrand,
                ccAccountNum = newOrderResult.ccAccountNum,
                countryFraudFilterStatus = newOrderResult.countryFraudFilterStatus,
                ctiAffluentCard = newOrderResult.ctiAffluentCard,
                ctiCommercialCard = newOrderResult.ctiCommercialCard,
                ctiDurbinExemption = newOrderResult.ctiDurbinExemption,
                ctiHealthcareCard = newOrderResult.ctiHealthcareCard,
                ctiIssuingCountry = newOrderResult.ctiIssuingCountry,
                ctiLevel3Eligible = newOrderResult.ctiLevel3Eligible,
                ctiPINlessDebitCard = newOrderResult.ctiPINlessDebitCard,
                ctiPayrollCard = newOrderResult.ctiPayrollCard,
                ctiPrepaidCard = newOrderResult.ctiPrepaidCard,
                ctiSignatureDebitCard = newOrderResult.ctiSignatureDebitCard,
                customerName = newOrderResult.customerName,
                customerRefNum = newOrderResult.customerRefNum,
                cvvRespCode = newOrderResult.cvvRespCode,
                debitBillerReferenceNumber = newOrderResult.debitBillerReferenceNumber,
                debitPinNetworkID = newOrderResult.debitPinNetworkID,
                debitPinSurchargeAmount = newOrderResult.debitPinSurchargeAmount,
                debitPinTraceNumber = newOrderResult.debitPinTraceNumber,
                fraudAnalysisResponse = newOrderResultFraudAnalysisResponseConverted,
                fraudScoreProcMsg = newOrderResult.fraudScoreProcMsg,
                fraudScoreProcStatus = newOrderResult.fraudScoreProcStatus,
                giftCardInd = newOrderResult.giftCardInd,
                hostAVSRespCode = newOrderResult.hostAVSRespCode,
                hostCVVRespCode = newOrderResult.hostCVVRespCode,
                hostRespCode = newOrderResult.hostRespCode,
                industryType = newOrderResult.industryType,
                isoCountryCode = newOrderResult.isoCountryCode,
                lastRetryDate = newOrderResult.lastRetryDate,
                mbMicroPaymentDaysLeft = newOrderResult.mbMicroPaymentDaysLeft,
                mbMicroPaymentDollarsLeft = newOrderResult.mbMicroPaymentDollarsLeft,
                mbStatus = newOrderResult.mbStatus,
                mcRecurringAdvCode = newOrderResult.mcRecurringAdvCode,
                merchantID = newOrderResult.merchantID,
                orderID = newOrderResult.orderID,
                partialAuthOccurred = newOrderResult.partialAuthOccurred,
                procStatus = newOrderResult.procStatus,
                procStatusMessage = newOrderResult.procStatusMessage,
                profileProcStatus = newOrderResult.profileProcStatus,
                profileProcStatusMsg = newOrderResult.profileProcStatusMsg,
                redeemedAmount = newOrderResult.redeemedAmount,
                remainingBalance = newOrderResult.remainingBalance,
                requestAmount = newOrderResult.requestAmount,
                respCode = newOrderResult.respCode,
                respCodeMessage = newOrderResult.respCodeMessage,
                respDateTime = newOrderResult.respDateTime,
                retryAttempCount = newOrderResult.retryAttempCount,
                retryTrace = newOrderResult.retryTrace,
                terminalID = newOrderResult.terminalID,
                transType = newOrderResult.transType,
                txRefIdx = newOrderResult.txRefIdx,
                txRefNum = newOrderResult.txRefNum,
                version = newOrderResult.version,
                visaVbVRespCode = newOrderResult.visaVbVRespCode,
                TransactionLogID = paymentAddTransactionResult.Data.Id
            };

            processingResult.IsSuccessful = true;
            processingResult.Data = newOrderResultConverted;
            return processingResult;
        }

        public async Task<ServiceProcessingResult<LS.Domain.Chase.ReversalResponse>> Reversal(LS.Domain.Chase.ReversalRequest requestData) {
            var processingResult = new ServiceProcessingResult<LS.Domain.Chase.ReversalResponse> { IsSuccessful = false };

            var paymentTechClient = new PaymentechClient();

            var reversalRequest = new ReversalRequest();

            reversalRequest.endppoint = NewOrderEndpoint;
            reversalRequest.orbitalConnectionUsername = APIUsername;
            reversalRequest.orbitalConnectionPassword = APIPassword;
            reversalRequest.version = APIVersion;
            if (IsDev) { reversalRequest.orderID = "d" + requestData.orderID; } else { reversalRequest.orderID = requestData.orderID; }
            reversalRequest.txRefIdx = requestData.txRefIdx;
            reversalRequest.txRefNum = requestData.txRefNum;

            var reversalResult = await paymentTechClient.ReversalAsync(reversalRequest);
            if (reversalResult.ReversalResponse.errorMessage == null) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Chase - " + reversalResult.ReversalResponse.errorMessage.errorText, "Chase - " + reversalResult.ReversalResponse.errorMessage.errorText, true, false);
                return processingResult;
            }

            var reversalResultErrorMessageResponseConverted = new LS.Domain.Chase.ErrorMessage {
                errorCode = reversalResult.ReversalResponse.errorMessage.errorCode,
                errorText = reversalResult.ReversalResponse.errorMessage.errorText
            };

            var reversalResultConverted = new LS.Domain.Chase.ReversalResponse {
                errorMessage = reversalResultErrorMessageResponseConverted,
                approvalStatus = reversalResult.ReversalResponse.approvalStatus,
                bin = reversalResult.ReversalResponse.bin,
                hostRespCode = reversalResult.ReversalResponse.hostRespCode,
                lastRetryDate = reversalResult.ReversalResponse.lastRetryDate,
                merchantID = reversalResult.ReversalResponse.merchantID,
                orderID = reversalResult.ReversalResponse.orderID,
                outstandingAmt = reversalResult.ReversalResponse.outstandingAmt,
                procStatus = reversalResult.ReversalResponse.procStatus,
                procStatusMessage = reversalResult.ReversalResponse.procStatusMessage,
                respCode = reversalResult.ReversalResponse.respCode,
                respDateTime = reversalResult.ReversalResponse.respDateTime,
                retryAttempCount = reversalResult.ReversalResponse.retryAttempCount,
                retryTrace = reversalResult.ReversalResponse.retryTrace,
                terminalID = reversalResult.ReversalResponse.terminalID,
                txRefIdx = reversalResult.ReversalResponse.txRefIdx,
                txRefNum = reversalResult.ReversalResponse.txRefNum,
                version = reversalResult.ReversalResponse.version
            };

            processingResult.IsSuccessful = true;
            processingResult.Data = reversalResultConverted;
            return processingResult;
        }
    }
}
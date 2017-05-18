using System.Collections.Generic;

namespace LS.Domain.ExternalApiIntegration.PuertoRico
{
    public static class PuertoRicoErrorCodes
    {
        public static string DuplicateSubscriber = "DUPLICATE_SUBSCRIBER";
        public static string DuplicateAddress = "DUPLICATE_ADDRESS";
        public static string DuplicatePrimaryAddress = "DUPLICATE_PRIMARY_ADDRESS";
        public static string FailedTpiv = "FAILED_TPIV";
        public static string TpivFail = "TPIV_FAIL";
        public static string TpivFailedOnSsn = "TPIV_FAIL_NAME_SSN4";
        public static string TpivFailedOnDob = "TPIV_FAIL_DOB";
        public static string TpivFailedOnIdentity = "TPIV_FAIL_IDENTITY_NOT_FOUND";
        public static string TpivWebserviceError = "TPIV_WEBSERVICE_ERROR";
        public static string InvalidAddress = "INVALID_ADDRESS";
        public static string AmsFailure = "AMS_FAILURE_ANALYSIS";
        public static string InvalidTransfer = "CANNOT_TRANSFER_WITHIN_60_DAYS";
        public static string DuplicatePhoneNumber = "DUPLICATE_PHONE_NUMBER";

        public static Dictionary<string, string> ExpectedErrorCodes = new Dictionary<string, string>
        {
            {DuplicateSubscriber,"The subscriber in this transaction is a duplicate of another subscriber"},
            {DuplicateAddress,"The primary address in this transaction matches the primary address of another subscriber."},
            {DuplicatePrimaryAddress, "The primary address in this transaction matches the primary address of another subscriber."},
            {FailedTpiv,"Subscriber failed third-party identity verification."},
            {TpivFail, "Subscriber failed third-party identity verification."},
            {TpivFailedOnSsn, "Subscriber name or SSN4 could not be validated."},
            {TpivFailedOnDob, "Subscriber date of birth could not be validated."},
            {TpivFailedOnIdentity, "Subscriber identity could not be found."},
            {TpivWebserviceError, "Third-party identity verification services are currently unavailable. Please try again later."},
            {InvalidAddress,"Address unrecognized (failed Address Matching Service)."},
            {AmsFailure, "Address failed validation"}, //Todo: Handle this error message gracefully
            {InvalidTransfer, "Transfer cannot be processed within 60 days of service initialization"},
            {DuplicatePhoneNumber, "The phone number generated matches an existing number. Please try again"}
        }; 
    }
}

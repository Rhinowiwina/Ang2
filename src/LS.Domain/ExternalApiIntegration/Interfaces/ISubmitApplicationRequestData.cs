using Newtonsoft.Json;

namespace LS.Domain.ExternalApiIntegration.Interfaces
{
    public interface ISubmitApplicationRequestData : ICheckStatusRequestData {
        string AssignedTelephoneNumber { get; set; }
        string SubscriberAccountNumber { get; set; } // Customer's Id in our system
        string BillingFirstName { get; set; }
        string BillingMiddleInitial { get; set; }
        string BillingLastName { get; set; }
        string BillingAddress1 { get; set; }
        string BillingAddress2 { get; set; }
        string BillingCity { get; set; }
        string BillingState { get; set; }
        string BillingZip5 { get; set; }
        string BillingZip4 { get; set; }
        string ContactPhoneNumber { get; set; }
        string UltsServiceStartDate { get; set; }
        string DriversLicenseNumber { get; set; }
        EnrollmentType EnrollmentType { get; set; }
        string TpivBypassSignature { get; set; }
        string TpivBypassSsnDocument { get; set; }
        string TpivBypassDobDocument { get; set; }
        string TpivBypassSsnLast4Digits { get; set; }
        string TpivBypassDobLast4Digits { get; set; }
        string PriorULTSTelephoneNumber { get; set; }
        }
    }

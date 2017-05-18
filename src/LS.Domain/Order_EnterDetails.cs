using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LS.Core.Interfaces;

namespace LS.Domain
{
    public class Order_EnterDeatils : IOrder_EnterDetails {
        public string Order_ID { get; set; }
        public bool Lifeline { get; set; }
        public bool TribalLifeline { get; set; }
        public int Lifeline_Program { get; set; }
        public int Cellular_Plan { get; set; }
        public string Model { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public int Validated_AddressId { get; set; }
        public string SSN { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Gender { get; set; }
        public string UserPassword { get; set; }
        public string ReferringMobileNumber { get; set; }
        public string PrequalifiedCode { get; set; }
        public string DOB { get; set; }
        public string AuthorizationCode { get; set; }
        public string EmployeeAcount { get; set; }
        public string DeviceID { get; set; }
        public string TradeInDeviceID { get; set; }
        public bool TemporaryAddress { get; set; }
        public string ExternalAgentID { get; set; }
        public string BillingAddress { get; set; }
        public string BillingAddress2 { get; set; }
        public string BillingCity { get; set; }
        public string BillingZip { get; set; }
        public string Sales_Channel { get; set; }
        public string StateRegCode { get; set; }
        public bool NoDeviceOnOrder { get; set; }
        public string BypassIdentityNotes { get; set; }
        public string AlternateIDDesc { get; set; }
        public string AlternateIDValue { get; set; }
        public string Beneficiary { get; set; }
        public int AccountCreditAwarded { get; set; }
        public string DocumentVerification { get; set; }
        public string BeneficiaryFirstName { get; set; }
        public string BeneficiaryLastName { get; set; }
        public string BeneficiarySSN { get; set; }
        public string BeneficiaryDOB { get; set; }
        public bool VoiceOnly { get; set; }
        public string Lifeline_ProgramValue { get; set; }
        public string StateRegCode2 { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public bool RuralAddress { get; set; }
        public bool ByPassDuplicate { get; set; }
        public bool HOHC { get; set; }
        public string IMSI { get; set; }
    }
}

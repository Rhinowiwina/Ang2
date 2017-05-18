using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Core.Interfaces
{
    public interface IOrder_EnterDetails {
          string Order_ID { get; set; }
          bool Lifeline { get; set; }
          bool TribalLifeline { get; set; }
          int Lifeline_Program { get; set; }
          int Cellular_Plan { get; set; }
          string Model { get; set; }
          string First_Name { get; set; }
          string Last_Name { get; set; }
          int Validated_AddressId { get; set; }
          string SSN { get; set; }
          string Contact { get; set; }
          string Email { get; set; }
          string UserName { get; set; }
          string UserPassword { get; set; }
          string ReferringMobileNumber { get; set; }
          string PrequalifiedCode { get; set; }
          string DOB { get; set; }
          string AuthorizationCode { get; set; }
          string EmployeeAcount { get; set; }
          string DeviceID { get; set; }
          string TradeInDeviceID { get; set; }
          bool TemporaryAddress { get; set; }
          string ExternalAgentID { get; set; }
          string BillingAddress { get; set; }
          string BillingAddress2 { get; set; }
          string BillingCity { get; set; }
          string BillingZip { get; set; }
          string Sales_Channel { get; set; }
          string StateRegCode { get; set; }
          bool  NoDeviceOnOrder { get; set; }
          string BypassIdentityNotes { get; set; }
          string AlternateIDDesc { get; set; }
          string AlternateIDValue { get; set; }
          string Beneficiary { get; set; }
          int AccountCreditAwarded { get; set; }
          string DocumentVerification { get; set; }
          string BeneficiaryFirstName { get; set; }
          string BeneficiaryLastName { get; set; }
          string BeneficiarySSN { get; set; }
          string BeneficiaryDOB { get; set; }
          bool VoiceOnly { get; set; }
          string Lifeline_ProgramValue { get; set; }
          string StateRegCode2 { get; set; }
          string Address { get; set; }
          string Address2 { get; set; }
          string City { get; set; }
          string State { get; set; }
          string Zip { get; set; }
          bool RuralAddress { get; set; }
          bool ByPassDuplicate { get; set; }
          bool HOHC { get; set; }
          string IMSI { get; set; }
    }
}

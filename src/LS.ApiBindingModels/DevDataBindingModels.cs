using System;

namespace LS.ApiBindingModels
{

    public class DevDataBindingModel
    {
        public string Id { get; set; }
        public DateTime DateSubmitted { get; set; }
        public string OrigState { get; set; }
        public string E_Prg { get; set; }
        public string Num_HouseHold { get; set; }
        public string AuthCode { get; set; }
        public string PreQual { get; set; }
        public string Fname { get; set; }
        public string MI { get; set; }
        public string Lname { get; set; }
        public string SSN_Decrypted { get; set; }
        public string DOB_Decrypted { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public bool BillAsInstall { get; set; }
        public string Bill_Address { get; set; }
        public string Bill_City { get; set; }
        public string Bill_State { get; set; }
        public string Bill_Zip { get; set; }
        public string DayPhone { get; set; }
        public string Avg_Income { get; set; }
        public string Income_Frequency { get; set; }
        public string Qualifying_Beneficiary { get; set; }
        public string Beneficiary_FirstName { get; set; }
        public string Beneficiary_LastName { get; set; }
        public string Beneficiary_SSN_Decrypted { get; set; }
        public string Beneficiary_DOB_Decrypted { get; set; }
        public string HOH_Spouse { get; set; }
        public string HOH_Adults_Parent { get; set; }
        public string HOH_Adults_Child { get; set; }
        public string HOH_Adults_Relative { get; set; }
        public string HOH_Adults_Roommate { get; set; }
        public string HOH_Adults_Other { get; set; }
        public string HOH_Adults_Other_Text { get; set; }
        public string HOH_Expenses { get; set; }
        public string HOH_Share_Lifeline { get; set; }
        public string HOH_Share_Lifeline_Names { get; set; }
        public string HOH_Agree_MultHoues { get; set; }
        public string HOH_Agree_Violation { get; set; }
        public string DocumentType { get; set; }
        public string DocumentNumber { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }

}

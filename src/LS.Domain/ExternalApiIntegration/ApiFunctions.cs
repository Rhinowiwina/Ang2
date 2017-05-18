namespace LS.Domain.ExternalApiIntegration
{
    public static class ApiFunctions
    {
        public static string CheckStatus = "CheckCustomerStatus";
        public static string Submit = "DirectApplicationRequest";

        //Texas Solix 
        public static string TexasSolixVerify = "Verify";
        public static string TexasSolixCreateCustomer = "CreateCustomer";

        //NLAD
        public static string NladVerify = "Verify";
        public static string NladSubmit = "Reserve";
        public static string NladTransfer = "Transfer";
        public static string NladResolutionRequest = "Resolution";

        //CAMs
        public static string ScrubAddress = "Scrub Address";
        public static string ValidateAddress = "ValidateAddress";
        public static string EnterOrderDetails = "Enter Order Details";
        public static string ZipCodeCoverage = "Zip_Coverage";
        public static string GetAvailableDevices = "Devices";
        public static string GetAvailableCarriers = "Zip_Coverage (GetAvailableCarriers)";
        public static string RetrPreactivatedHandsetByDeviceID = "RetrPreactivatedHandsetByDeviceID";
        public static string TMobile_RetrPreactivatedHandsetByDeviceID = "TMobile_RetrPreactivatedHandsetByDeviceID";
        public static string BudgetMobile_CompleteFulfillment = "BudgetMobile_CompleteFulfillment";
        public static string ActivateVerizonDevice = "ActivateVerizonDevice";
        public static string Verizon_RetrSingleDeviceActivationDetails = "Verizon_RetrSingleDeviceActivationDetails";
        public static string ActivateSprintDevice = "ActivateSprintDevice";
        public static string ActivateTMobileDevice = "ActivateTmobileDevice";
        public static string LookupVerizonActivationStatus = "LookupVerizonActivationStatus";
        public static string VerizonRetrDevice = "Verizon_RetrSingleDeviceActivationDetails";
        public static string SprintRetrDevice = "Sprint_RetrSingleDeviceActivationDetails";
        public static string ChangeESN_Verizon = "ChangeESN_Verizon";
        public static string CheckTmobileBalance = "CheckTmobileBalance";
        public static string CheckVerizonBalance = "CheckVerizonBalance";
        public static string DeactivateSprintDevice = "DeactivateSprintDevice";
        public static string DeactivateVerizonDevice = "DeactivateVerizonDevice";
        public static string Handset_Commit = "Handset_Commit";
        public static string Handset_Enter = "Handset_Enter";
        public static string LookUpVerizonDiscreteDeviceInquiryStatus = "LookUpVerizonDiscreteDeviceInquiryStatus";
        public static string LookupAvailableTopUp_Existing = "LookupAvailableTopUp_Existing";
        public static string LookupVerizonChangeESNStatus = "LookupVerizonChangeESNStatus";
        public static string LookupVerizonDeactivateDeviceStatus = "LookupVerizonDeactivateDeviceStatus";
        public static string Order_Commit = "Order_Commit";
        public static string Order_Enter = "Order_Enter";
        public static string OrderSave = "OrderSave";
        public static string RetrVoiceandTextBalance = "RetrVoiceandTextBalance";
        public static string TMobile_SwapIMSI = "TMobile_SwapIMSI";
        public static string TopUp_Commit = "TopUp_Commit";
        public static string TopUp_Enter = "TopUp_Enter";
        public static string Verizon_Discrete_DeviceInquiry = "Verizon_Discrete_DeviceInquiry";
        public static string LookupAccountByMDN = "LookupAccountByMDN";
        public static string UpdateDeviceID = "UpdateDeviceID";
        public static string LookupTopUpDetails = "LookupTopUpDetails";
        public static string LookupTopUpDetailsTotalDue = "LookupTopUpDetailsTotalDue";
        public static string Recertify_BudgetMobile = "Recertify_BudgetMobile";
        public static string LookupCustomerDetails = "LookupCustomerDetails";
        public static string LookupCustomer = "LookupCustomer";
        public static string LookupBasePlan = "LookupBasePlan";
        public static string DuplicateCheck = "DuplicateCheck";

        //Chase API
        public static string NewOrder = "NewOrder";

        //PayPal API
        public static string MassPay = "MassPay";

        //CGM API
        public static string Check = "Check";
        public static string Enroll = "Enroll";
    }
}

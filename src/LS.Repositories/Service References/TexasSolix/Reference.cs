﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LS.Repositories.TexasSolix {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="HHSCResults", Namespace="http://schemas.datacontract.org/2004/07/HHSCService")]
    [System.SerializableAttribute()]
    public partial class HHSCResults : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string BirthdayField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ConfirmationNumberField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string EligibleForDiscountField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ErrorCodeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ErrorDescriptionField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string LastNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MailingZip5Field;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ReceivedDiscountField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ReserveStatusField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string SSN4Field;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string WorksheetNeededField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Birthday {
            get {
                return this.BirthdayField;
            }
            set {
                if ((object.ReferenceEquals(this.BirthdayField, value) != true)) {
                    this.BirthdayField = value;
                    this.RaisePropertyChanged("Birthday");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ConfirmationNumber {
            get {
                return this.ConfirmationNumberField;
            }
            set {
                if ((object.ReferenceEquals(this.ConfirmationNumberField, value) != true)) {
                    this.ConfirmationNumberField = value;
                    this.RaisePropertyChanged("ConfirmationNumber");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string EligibleForDiscount {
            get {
                return this.EligibleForDiscountField;
            }
            set {
                if ((object.ReferenceEquals(this.EligibleForDiscountField, value) != true)) {
                    this.EligibleForDiscountField = value;
                    this.RaisePropertyChanged("EligibleForDiscount");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ErrorCode {
            get {
                return this.ErrorCodeField;
            }
            set {
                if ((object.ReferenceEquals(this.ErrorCodeField, value) != true)) {
                    this.ErrorCodeField = value;
                    this.RaisePropertyChanged("ErrorCode");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ErrorDescription {
            get {
                return this.ErrorDescriptionField;
            }
            set {
                if ((object.ReferenceEquals(this.ErrorDescriptionField, value) != true)) {
                    this.ErrorDescriptionField = value;
                    this.RaisePropertyChanged("ErrorDescription");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string LastName {
            get {
                return this.LastNameField;
            }
            set {
                if ((object.ReferenceEquals(this.LastNameField, value) != true)) {
                    this.LastNameField = value;
                    this.RaisePropertyChanged("LastName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string MailingZip5 {
            get {
                return this.MailingZip5Field;
            }
            set {
                if ((object.ReferenceEquals(this.MailingZip5Field, value) != true)) {
                    this.MailingZip5Field = value;
                    this.RaisePropertyChanged("MailingZip5");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ReceivedDiscount {
            get {
                return this.ReceivedDiscountField;
            }
            set {
                if ((object.ReferenceEquals(this.ReceivedDiscountField, value) != true)) {
                    this.ReceivedDiscountField = value;
                    this.RaisePropertyChanged("ReceivedDiscount");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ReserveStatus {
            get {
                return this.ReserveStatusField;
            }
            set {
                if ((object.ReferenceEquals(this.ReserveStatusField, value) != true)) {
                    this.ReserveStatusField = value;
                    this.RaisePropertyChanged("ReserveStatus");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string SSN4 {
            get {
                return this.SSN4Field;
            }
            set {
                if ((object.ReferenceEquals(this.SSN4Field, value) != true)) {
                    this.SSN4Field = value;
                    this.RaisePropertyChanged("SSN4");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string WorksheetNeeded {
            get {
                return this.WorksheetNeededField;
            }
            set {
                if ((object.ReferenceEquals(this.WorksheetNeededField, value) != true)) {
                    this.WorksheetNeededField = value;
                    this.RaisePropertyChanged("WorksheetNeeded");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ReservationResults", Namespace="http://schemas.datacontract.org/2004/07/HHSCService")]
    [System.SerializableAttribute()]
    public partial class ReservationResults : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string BirthdayField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ConfirmationNumberField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ErrorCodeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ErrorDescriptionField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string LastNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MailingZip5Field;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ReserveConfNoField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ReserveResultField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string SSN4Field;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Birthday {
            get {
                return this.BirthdayField;
            }
            set {
                if ((object.ReferenceEquals(this.BirthdayField, value) != true)) {
                    this.BirthdayField = value;
                    this.RaisePropertyChanged("Birthday");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ConfirmationNumber {
            get {
                return this.ConfirmationNumberField;
            }
            set {
                if ((object.ReferenceEquals(this.ConfirmationNumberField, value) != true)) {
                    this.ConfirmationNumberField = value;
                    this.RaisePropertyChanged("ConfirmationNumber");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ErrorCode {
            get {
                return this.ErrorCodeField;
            }
            set {
                if ((object.ReferenceEquals(this.ErrorCodeField, value) != true)) {
                    this.ErrorCodeField = value;
                    this.RaisePropertyChanged("ErrorCode");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ErrorDescription {
            get {
                return this.ErrorDescriptionField;
            }
            set {
                if ((object.ReferenceEquals(this.ErrorDescriptionField, value) != true)) {
                    this.ErrorDescriptionField = value;
                    this.RaisePropertyChanged("ErrorDescription");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string LastName {
            get {
                return this.LastNameField;
            }
            set {
                if ((object.ReferenceEquals(this.LastNameField, value) != true)) {
                    this.LastNameField = value;
                    this.RaisePropertyChanged("LastName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string MailingZip5 {
            get {
                return this.MailingZip5Field;
            }
            set {
                if ((object.ReferenceEquals(this.MailingZip5Field, value) != true)) {
                    this.MailingZip5Field = value;
                    this.RaisePropertyChanged("MailingZip5");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ReserveConfNo {
            get {
                return this.ReserveConfNoField;
            }
            set {
                if ((object.ReferenceEquals(this.ReserveConfNoField, value) != true)) {
                    this.ReserveConfNoField = value;
                    this.RaisePropertyChanged("ReserveConfNo");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ReserveResult {
            get {
                return this.ReserveResultField;
            }
            set {
                if ((object.ReferenceEquals(this.ReserveResultField, value) != true)) {
                    this.ReserveResultField = value;
                    this.RaisePropertyChanged("ReserveResult");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string SSN4 {
            get {
                return this.SSN4Field;
            }
            set {
                if ((object.ReferenceEquals(this.SSN4Field, value) != true)) {
                    this.SSN4Field = value;
                    this.RaisePropertyChanged("SSN4");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="CreateCustomerResults", Namespace="http://schemas.datacontract.org/2004/07/HHSCService")]
    [System.SerializableAttribute()]
    public partial class CreateCustomerResults : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ConfirmationNumberField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ErrorCodeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ErrorDescriptionField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool SuccessStatusField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ConfirmationNumber {
            get {
                return this.ConfirmationNumberField;
            }
            set {
                if ((object.ReferenceEquals(this.ConfirmationNumberField, value) != true)) {
                    this.ConfirmationNumberField = value;
                    this.RaisePropertyChanged("ConfirmationNumber");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ErrorCode {
            get {
                return this.ErrorCodeField;
            }
            set {
                if ((object.ReferenceEquals(this.ErrorCodeField, value) != true)) {
                    this.ErrorCodeField = value;
                    this.RaisePropertyChanged("ErrorCode");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ErrorDescription {
            get {
                return this.ErrorDescriptionField;
            }
            set {
                if ((object.ReferenceEquals(this.ErrorDescriptionField, value) != true)) {
                    this.ErrorDescriptionField = value;
                    this.RaisePropertyChanged("ErrorDescription");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool SuccessStatus {
            get {
                return this.SuccessStatusField;
            }
            set {
                if ((this.SuccessStatusField.Equals(value) != true)) {
                    this.SuccessStatusField = value;
                    this.RaisePropertyChanged("SuccessStatus");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="TexasSolix.IHHSC")]
    public interface IHHSC {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IHHSC/ServiceStatus", ReplyAction="http://tempuri.org/IHHSC/ServiceStatusResponse")]
        string ServiceStatus();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IHHSC/ServiceStatus", ReplyAction="http://tempuri.org/IHHSC/ServiceStatusResponse")]
        System.Threading.Tasks.Task<string> ServiceStatusAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IHHSC/Verify", ReplyAction="http://tempuri.org/IHHSC/VerifyResponse")]
        LS.Repositories.TexasSolix.HHSCResults Verify(string lastName, string ssn4, string birthday, string mailingZip5, string apiLogin, string apiPassword, string userLogin);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IHHSC/Verify", ReplyAction="http://tempuri.org/IHHSC/VerifyResponse")]
        System.Threading.Tasks.Task<LS.Repositories.TexasSolix.HHSCResults> VerifyAsync(string lastName, string ssn4, string birthday, string mailingZip5, string apiLogin, string apiPassword, string userLogin);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IHHSC/Reserve", ReplyAction="http://tempuri.org/IHHSC/ReserveResponse")]
        LS.Repositories.TexasSolix.ReservationResults Reserve(string lastName, string ssn4, string birthday, string mailingZip5, string confNo, string apiLogin, string apiPassword, string userLogin);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IHHSC/Reserve", ReplyAction="http://tempuri.org/IHHSC/ReserveResponse")]
        System.Threading.Tasks.Task<LS.Repositories.TexasSolix.ReservationResults> ReserveAsync(string lastName, string ssn4, string birthday, string mailingZip5, string confNo, string apiLogin, string apiPassword, string userLogin);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IHHSC/CreateCustomer", ReplyAction="http://tempuri.org/IHHSC/CreateCustomerResponse")]
        LS.Repositories.TexasSolix.CreateCustomerResults CreateCustomer(string lastName, string firstName, string ssn4, string birthday, string residenceStreetAddress, string residenceCity, string residenceZip5, string confirmationNumber, string apiLogin, string apiPassword, string userLogin);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IHHSC/CreateCustomer", ReplyAction="http://tempuri.org/IHHSC/CreateCustomerResponse")]
        System.Threading.Tasks.Task<LS.Repositories.TexasSolix.CreateCustomerResults> CreateCustomerAsync(string lastName, string firstName, string ssn4, string birthday, string residenceStreetAddress, string residenceCity, string residenceZip5, string confirmationNumber, string apiLogin, string apiPassword, string userLogin);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IHHSCChannel : LS.Repositories.TexasSolix.IHHSC, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class HHSCClient : System.ServiceModel.ClientBase<LS.Repositories.TexasSolix.IHHSC>, LS.Repositories.TexasSolix.IHHSC {
        
        public HHSCClient() {
        }
        
        public HHSCClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public HHSCClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public HHSCClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public HHSCClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string ServiceStatus() {
            return base.Channel.ServiceStatus();
        }
        
        public System.Threading.Tasks.Task<string> ServiceStatusAsync() {
            return base.Channel.ServiceStatusAsync();
        }
        
        public LS.Repositories.TexasSolix.HHSCResults Verify(string lastName, string ssn4, string birthday, string mailingZip5, string apiLogin, string apiPassword, string userLogin) {
            return base.Channel.Verify(lastName, ssn4, birthday, mailingZip5, apiLogin, apiPassword, userLogin);
        }
        
        public System.Threading.Tasks.Task<LS.Repositories.TexasSolix.HHSCResults> VerifyAsync(string lastName, string ssn4, string birthday, string mailingZip5, string apiLogin, string apiPassword, string userLogin) {
            return base.Channel.VerifyAsync(lastName, ssn4, birthday, mailingZip5, apiLogin, apiPassword, userLogin);
        }
        
        public LS.Repositories.TexasSolix.ReservationResults Reserve(string lastName, string ssn4, string birthday, string mailingZip5, string confNo, string apiLogin, string apiPassword, string userLogin) {
            return base.Channel.Reserve(lastName, ssn4, birthday, mailingZip5, confNo, apiLogin, apiPassword, userLogin);
        }
        
        public System.Threading.Tasks.Task<LS.Repositories.TexasSolix.ReservationResults> ReserveAsync(string lastName, string ssn4, string birthday, string mailingZip5, string confNo, string apiLogin, string apiPassword, string userLogin) {
            return base.Channel.ReserveAsync(lastName, ssn4, birthday, mailingZip5, confNo, apiLogin, apiPassword, userLogin);
        }
        
        public LS.Repositories.TexasSolix.CreateCustomerResults CreateCustomer(string lastName, string firstName, string ssn4, string birthday, string residenceStreetAddress, string residenceCity, string residenceZip5, string confirmationNumber, string apiLogin, string apiPassword, string userLogin) {
            return base.Channel.CreateCustomer(lastName, firstName, ssn4, birthday, residenceStreetAddress, residenceCity, residenceZip5, confirmationNumber, apiLogin, apiPassword, userLogin);
        }
        
        public System.Threading.Tasks.Task<LS.Repositories.TexasSolix.CreateCustomerResults> CreateCustomerAsync(string lastName, string firstName, string ssn4, string birthday, string residenceStreetAddress, string residenceCity, string residenceZip5, string confirmationNumber, string apiLogin, string apiPassword, string userLogin) {
            return base.Channel.CreateCustomerAsync(lastName, firstName, ssn4, birthday, residenceStreetAddress, residenceCity, residenceZip5, confirmationNumber, apiLogin, apiPassword, userLogin);
        }
    }
}
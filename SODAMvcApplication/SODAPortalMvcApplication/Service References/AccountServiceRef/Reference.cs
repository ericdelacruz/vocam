﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18408
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SODAPortalMvcApplication.AccountServiceRef {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Account", Namespace="http://schemas.datacontract.org/2004/07/SODAwcfService.Models")]
    [System.SerializableAttribute()]
    public partial class Account : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string AddressField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Nullable<System.DateTime> BirthdateField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CityField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CompanyField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CompanyUrlField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ContactNoField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CountryField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Nullable<System.DateTime> DateLoginField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string EmailField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool EmailVerifiedField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FirstNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private char GenderField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private long IdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string LastNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PASSWORDField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int RoleField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private short StatusField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string USERNAMEField;
        
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
        public string Address {
            get {
                return this.AddressField;
            }
            set {
                if ((object.ReferenceEquals(this.AddressField, value) != true)) {
                    this.AddressField = value;
                    this.RaisePropertyChanged("Address");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<System.DateTime> Birthdate {
            get {
                return this.BirthdateField;
            }
            set {
                if ((this.BirthdateField.Equals(value) != true)) {
                    this.BirthdateField = value;
                    this.RaisePropertyChanged("Birthdate");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string City {
            get {
                return this.CityField;
            }
            set {
                if ((object.ReferenceEquals(this.CityField, value) != true)) {
                    this.CityField = value;
                    this.RaisePropertyChanged("City");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Company {
            get {
                return this.CompanyField;
            }
            set {
                if ((object.ReferenceEquals(this.CompanyField, value) != true)) {
                    this.CompanyField = value;
                    this.RaisePropertyChanged("Company");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string CompanyUrl {
            get {
                return this.CompanyUrlField;
            }
            set {
                if ((object.ReferenceEquals(this.CompanyUrlField, value) != true)) {
                    this.CompanyUrlField = value;
                    this.RaisePropertyChanged("CompanyUrl");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ContactNo {
            get {
                return this.ContactNoField;
            }
            set {
                if ((object.ReferenceEquals(this.ContactNoField, value) != true)) {
                    this.ContactNoField = value;
                    this.RaisePropertyChanged("ContactNo");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Country {
            get {
                return this.CountryField;
            }
            set {
                if ((object.ReferenceEquals(this.CountryField, value) != true)) {
                    this.CountryField = value;
                    this.RaisePropertyChanged("Country");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<System.DateTime> DateLogin {
            get {
                return this.DateLoginField;
            }
            set {
                if ((this.DateLoginField.Equals(value) != true)) {
                    this.DateLoginField = value;
                    this.RaisePropertyChanged("DateLogin");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Email {
            get {
                return this.EmailField;
            }
            set {
                if ((object.ReferenceEquals(this.EmailField, value) != true)) {
                    this.EmailField = value;
                    this.RaisePropertyChanged("Email");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool EmailVerified {
            get {
                return this.EmailVerifiedField;
            }
            set {
                if ((this.EmailVerifiedField.Equals(value) != true)) {
                    this.EmailVerifiedField = value;
                    this.RaisePropertyChanged("EmailVerified");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string FirstName {
            get {
                return this.FirstNameField;
            }
            set {
                if ((object.ReferenceEquals(this.FirstNameField, value) != true)) {
                    this.FirstNameField = value;
                    this.RaisePropertyChanged("FirstName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public char Gender {
            get {
                return this.GenderField;
            }
            set {
                if ((this.GenderField.Equals(value) != true)) {
                    this.GenderField = value;
                    this.RaisePropertyChanged("Gender");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long Id {
            get {
                return this.IdField;
            }
            set {
                if ((this.IdField.Equals(value) != true)) {
                    this.IdField = value;
                    this.RaisePropertyChanged("Id");
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
        public string PASSWORD {
            get {
                return this.PASSWORDField;
            }
            set {
                if ((object.ReferenceEquals(this.PASSWORDField, value) != true)) {
                    this.PASSWORDField = value;
                    this.RaisePropertyChanged("PASSWORD");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Role {
            get {
                return this.RoleField;
            }
            set {
                if ((this.RoleField.Equals(value) != true)) {
                    this.RoleField = value;
                    this.RaisePropertyChanged("Role");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public short Status {
            get {
                return this.StatusField;
            }
            set {
                if ((this.StatusField.Equals(value) != true)) {
                    this.StatusField = value;
                    this.RaisePropertyChanged("Status");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string USERNAME {
            get {
                return this.USERNAMEField;
            }
            set {
                if ((object.ReferenceEquals(this.USERNAMEField, value) != true)) {
                    this.USERNAMEField = value;
                    this.RaisePropertyChanged("USERNAME");
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
    [System.Runtime.Serialization.DataContractAttribute(Name="ResetPassword", Namespace="http://schemas.datacontract.org/2004/07/SODAwcfService.Models")]
    [System.SerializableAttribute()]
    public partial class ResetPassword : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int IdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private long UserIdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime dateExField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime dateSentField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool isVerifiedField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string keyField;
        
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
        public int Id {
            get {
                return this.IdField;
            }
            set {
                if ((this.IdField.Equals(value) != true)) {
                    this.IdField = value;
                    this.RaisePropertyChanged("Id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long UserId {
            get {
                return this.UserIdField;
            }
            set {
                if ((this.UserIdField.Equals(value) != true)) {
                    this.UserIdField = value;
                    this.RaisePropertyChanged("UserId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime dateEx {
            get {
                return this.dateExField;
            }
            set {
                if ((this.dateExField.Equals(value) != true)) {
                    this.dateExField = value;
                    this.RaisePropertyChanged("dateEx");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime dateSent {
            get {
                return this.dateSentField;
            }
            set {
                if ((this.dateSentField.Equals(value) != true)) {
                    this.dateSentField = value;
                    this.RaisePropertyChanged("dateSent");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool isVerified {
            get {
                return this.isVerifiedField;
            }
            set {
                if ((this.isVerifiedField.Equals(value) != true)) {
                    this.isVerifiedField = value;
                    this.RaisePropertyChanged("isVerified");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string key {
            get {
                return this.keyField;
            }
            set {
                if ((object.ReferenceEquals(this.keyField, value) != true)) {
                    this.keyField = value;
                    this.RaisePropertyChanged("key");
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
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="AccountServiceRef.IAccountService")]
    public interface IAccountService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAccountService/addAccount", ReplyAction="http://tempuri.org/IAccountService/addAccountResponse")]
        int addAccount(SODAPortalMvcApplication.AccountServiceRef.Account account);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAccountService/addAccount", ReplyAction="http://tempuri.org/IAccountService/addAccountResponse")]
        System.Threading.Tasks.Task<int> addAccountAsync(SODAPortalMvcApplication.AccountServiceRef.Account account);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAccountService/updateAccount", ReplyAction="http://tempuri.org/IAccountService/updateAccountResponse")]
        int updateAccount(SODAPortalMvcApplication.AccountServiceRef.Account Account);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAccountService/updateAccount", ReplyAction="http://tempuri.org/IAccountService/updateAccountResponse")]
        System.Threading.Tasks.Task<int> updateAccountAsync(SODAPortalMvcApplication.AccountServiceRef.Account Account);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAccountService/getAccount", ReplyAction="http://tempuri.org/IAccountService/getAccountResponse")]
        SODAPortalMvcApplication.AccountServiceRef.Account[] getAccount(string UserName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAccountService/getAccount", ReplyAction="http://tempuri.org/IAccountService/getAccountResponse")]
        System.Threading.Tasks.Task<SODAPortalMvcApplication.AccountServiceRef.Account[]> getAccountAsync(string UserName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAccountService/deleteAccount", ReplyAction="http://tempuri.org/IAccountService/deleteAccountResponse")]
        int deleteAccount(string UserName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAccountService/deleteAccount", ReplyAction="http://tempuri.org/IAccountService/deleteAccountResponse")]
        System.Threading.Tasks.Task<int> deleteAccountAsync(string UserName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAccountService/AuthenticateUser", ReplyAction="http://tempuri.org/IAccountService/AuthenticateUserResponse")]
        bool AuthenticateUser(string UserName, string Password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAccountService/AuthenticateUser", ReplyAction="http://tempuri.org/IAccountService/AuthenticateUserResponse")]
        System.Threading.Tasks.Task<bool> AuthenticateUserAsync(string UserName, string Password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAccountService/isUserNameExists", ReplyAction="http://tempuri.org/IAccountService/isUserNameExistsResponse")]
        bool isUserNameExists(string UserName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAccountService/isUserNameExists", ReplyAction="http://tempuri.org/IAccountService/isUserNameExistsResponse")]
        System.Threading.Tasks.Task<bool> isUserNameExistsAsync(string UserName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAccountService/addResetPassword", ReplyAction="http://tempuri.org/IAccountService/addResetPasswordResponse")]
        int addResetPassword(string key, System.DateTime dateSent, System.DateTime dateEx, long UserId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAccountService/addResetPassword", ReplyAction="http://tempuri.org/IAccountService/addResetPasswordResponse")]
        System.Threading.Tasks.Task<int> addResetPasswordAsync(string key, System.DateTime dateSent, System.DateTime dateEx, long UserId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAccountService/getRestPassword", ReplyAction="http://tempuri.org/IAccountService/getRestPasswordResponse")]
        SODAPortalMvcApplication.AccountServiceRef.ResetPassword[] getRestPassword();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAccountService/getRestPassword", ReplyAction="http://tempuri.org/IAccountService/getRestPasswordResponse")]
        System.Threading.Tasks.Task<SODAPortalMvcApplication.AccountServiceRef.ResetPassword[]> getRestPasswordAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAccountService/updateResetPassword", ReplyAction="http://tempuri.org/IAccountService/updateResetPasswordResponse")]
        int updateResetPassword(SODAPortalMvcApplication.AccountServiceRef.ResetPassword resetPass);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAccountService/updateResetPassword", ReplyAction="http://tempuri.org/IAccountService/updateResetPasswordResponse")]
        System.Threading.Tasks.Task<int> updateResetPasswordAsync(SODAPortalMvcApplication.AccountServiceRef.ResetPassword resetPass);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAccountService/deleteResetPassword", ReplyAction="http://tempuri.org/IAccountService/deleteResetPasswordResponse")]
        int deleteResetPassword(int id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAccountService/deleteResetPassword", ReplyAction="http://tempuri.org/IAccountService/deleteResetPasswordResponse")]
        System.Threading.Tasks.Task<int> deleteResetPasswordAsync(int id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAccountService/Authenticate", ReplyAction="http://tempuri.org/IAccountService/AuthenticateResponse")]
        bool Authenticate(string Password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAccountService/Authenticate", ReplyAction="http://tempuri.org/IAccountService/AuthenticateResponse")]
        System.Threading.Tasks.Task<bool> AuthenticateAsync(string Password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAccountService/LogOff", ReplyAction="http://tempuri.org/IAccountService/LogOffResponse")]
        void LogOff(string UserName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAccountService/LogOff", ReplyAction="http://tempuri.org/IAccountService/LogOffResponse")]
        System.Threading.Tasks.Task LogOffAsync(string UserName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAccountService/updatePassword", ReplyAction="http://tempuri.org/IAccountService/updatePasswordResponse")]
        bool updatePassword(long userID, string orig_pass, string new_pass);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAccountService/updatePassword", ReplyAction="http://tempuri.org/IAccountService/updatePasswordResponse")]
        System.Threading.Tasks.Task<bool> updatePasswordAsync(long userID, string orig_pass, string new_pass);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IAccountServiceChannel : SODAPortalMvcApplication.AccountServiceRef.IAccountService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class AccountServiceClient : System.ServiceModel.ClientBase<SODAPortalMvcApplication.AccountServiceRef.IAccountService>, SODAPortalMvcApplication.AccountServiceRef.IAccountService {
        
        public AccountServiceClient() {
        }
        
        public AccountServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public AccountServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public AccountServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public AccountServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public int addAccount(SODAPortalMvcApplication.AccountServiceRef.Account account) {
            return base.Channel.addAccount(account);
        }
        
        public System.Threading.Tasks.Task<int> addAccountAsync(SODAPortalMvcApplication.AccountServiceRef.Account account) {
            return base.Channel.addAccountAsync(account);
        }
        
        public int updateAccount(SODAPortalMvcApplication.AccountServiceRef.Account Account) {
            return base.Channel.updateAccount(Account);
        }
        
        public System.Threading.Tasks.Task<int> updateAccountAsync(SODAPortalMvcApplication.AccountServiceRef.Account Account) {
            return base.Channel.updateAccountAsync(Account);
        }
        
        public SODAPortalMvcApplication.AccountServiceRef.Account[] getAccount(string UserName) {
            return base.Channel.getAccount(UserName);
        }
        
        public System.Threading.Tasks.Task<SODAPortalMvcApplication.AccountServiceRef.Account[]> getAccountAsync(string UserName) {
            return base.Channel.getAccountAsync(UserName);
        }
        
        public int deleteAccount(string UserName) {
            return base.Channel.deleteAccount(UserName);
        }
        
        public System.Threading.Tasks.Task<int> deleteAccountAsync(string UserName) {
            return base.Channel.deleteAccountAsync(UserName);
        }
        
        public bool AuthenticateUser(string UserName, string Password) {
            return base.Channel.AuthenticateUser(UserName, Password);
        }
        
        public System.Threading.Tasks.Task<bool> AuthenticateUserAsync(string UserName, string Password) {
            return base.Channel.AuthenticateUserAsync(UserName, Password);
        }
        
        public bool isUserNameExists(string UserName) {
            return base.Channel.isUserNameExists(UserName);
        }
        
        public System.Threading.Tasks.Task<bool> isUserNameExistsAsync(string UserName) {
            return base.Channel.isUserNameExistsAsync(UserName);
        }
        
        public int addResetPassword(string key, System.DateTime dateSent, System.DateTime dateEx, long UserId) {
            return base.Channel.addResetPassword(key, dateSent, dateEx, UserId);
        }
        
        public System.Threading.Tasks.Task<int> addResetPasswordAsync(string key, System.DateTime dateSent, System.DateTime dateEx, long UserId) {
            return base.Channel.addResetPasswordAsync(key, dateSent, dateEx, UserId);
        }
        
        public SODAPortalMvcApplication.AccountServiceRef.ResetPassword[] getRestPassword() {
            return base.Channel.getRestPassword();
        }
        
        public System.Threading.Tasks.Task<SODAPortalMvcApplication.AccountServiceRef.ResetPassword[]> getRestPasswordAsync() {
            return base.Channel.getRestPasswordAsync();
        }
        
        public int updateResetPassword(SODAPortalMvcApplication.AccountServiceRef.ResetPassword resetPass) {
            return base.Channel.updateResetPassword(resetPass);
        }
        
        public System.Threading.Tasks.Task<int> updateResetPasswordAsync(SODAPortalMvcApplication.AccountServiceRef.ResetPassword resetPass) {
            return base.Channel.updateResetPasswordAsync(resetPass);
        }
        
        public int deleteResetPassword(int id) {
            return base.Channel.deleteResetPassword(id);
        }
        
        public System.Threading.Tasks.Task<int> deleteResetPasswordAsync(int id) {
            return base.Channel.deleteResetPasswordAsync(id);
        }
        
        public bool Authenticate(string Password) {
            return base.Channel.Authenticate(Password);
        }
        
        public System.Threading.Tasks.Task<bool> AuthenticateAsync(string Password) {
            return base.Channel.AuthenticateAsync(Password);
        }
        
        public void LogOff(string UserName) {
            base.Channel.LogOff(UserName);
        }
        
        public System.Threading.Tasks.Task LogOffAsync(string UserName) {
            return base.Channel.LogOffAsync(UserName);
        }
        
        public bool updatePassword(long userID, string orig_pass, string new_pass) {
            return base.Channel.updatePassword(userID, orig_pass, new_pass);
        }
        
        public System.Threading.Tasks.Task<bool> updatePasswordAsync(long userID, string orig_pass, string new_pass) {
            return base.Channel.updatePasswordAsync(userID, orig_pass, new_pass);
        }
    }
}

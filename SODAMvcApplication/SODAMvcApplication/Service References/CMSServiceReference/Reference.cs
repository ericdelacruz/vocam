﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18331
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SODAMvcApplication.CMSServiceReference {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ContentDef", Namespace="http://schemas.datacontract.org/2004/07/SODAwcfService.Models")]
    [System.SerializableAttribute()]
    public partial class ContentDef : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PageCodeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int RegionIdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string SectionNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TypeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ValueField;
        
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
        public string PageCode {
            get {
                return this.PageCodeField;
            }
            set {
                if ((object.ReferenceEquals(this.PageCodeField, value) != true)) {
                    this.PageCodeField = value;
                    this.RaisePropertyChanged("PageCode");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int RegionId {
            get {
                return this.RegionIdField;
            }
            set {
                if ((this.RegionIdField.Equals(value) != true)) {
                    this.RegionIdField = value;
                    this.RaisePropertyChanged("RegionId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string SectionName {
            get {
                return this.SectionNameField;
            }
            set {
                if ((object.ReferenceEquals(this.SectionNameField, value) != true)) {
                    this.SectionNameField = value;
                    this.RaisePropertyChanged("SectionName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Type {
            get {
                return this.TypeField;
            }
            set {
                if ((object.ReferenceEquals(this.TypeField, value) != true)) {
                    this.TypeField = value;
                    this.RaisePropertyChanged("Type");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Value {
            get {
                return this.ValueField;
            }
            set {
                if ((object.ReferenceEquals(this.ValueField, value) != true)) {
                    this.ValueField = value;
                    this.RaisePropertyChanged("Value");
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
    [System.Runtime.Serialization.DataContractAttribute(Name="Contact", Namespace="http://schemas.datacontract.org/2004/07/SODAwcfService.Models")]
    [System.SerializableAttribute()]
    public partial class Contact : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CompanyField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Nullable<System.DateTime> DateLinkExField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string EmailField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int IdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MessageField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PhoneField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PostcodeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool isFreePPTField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Nullable<bool> isVerifiedField;
        
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
        public System.Nullable<System.DateTime> DateLinkEx {
            get {
                return this.DateLinkExField;
            }
            set {
                if ((this.DateLinkExField.Equals(value) != true)) {
                    this.DateLinkExField = value;
                    this.RaisePropertyChanged("DateLinkEx");
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
        public string Message {
            get {
                return this.MessageField;
            }
            set {
                if ((object.ReferenceEquals(this.MessageField, value) != true)) {
                    this.MessageField = value;
                    this.RaisePropertyChanged("Message");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name {
            get {
                return this.NameField;
            }
            set {
                if ((object.ReferenceEquals(this.NameField, value) != true)) {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Phone {
            get {
                return this.PhoneField;
            }
            set {
                if ((object.ReferenceEquals(this.PhoneField, value) != true)) {
                    this.PhoneField = value;
                    this.RaisePropertyChanged("Phone");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Postcode {
            get {
                return this.PostcodeField;
            }
            set {
                if ((object.ReferenceEquals(this.PostcodeField, value) != true)) {
                    this.PostcodeField = value;
                    this.RaisePropertyChanged("Postcode");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool isFreePPT {
            get {
                return this.isFreePPTField;
            }
            set {
                if ((this.isFreePPTField.Equals(value) != true)) {
                    this.isFreePPTField = value;
                    this.RaisePropertyChanged("isFreePPT");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<bool> isVerified {
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
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="CMSServiceReference.ICMS_Service")]
    public interface ICMS_Service {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICMS_Service/addContent", ReplyAction="http://tempuri.org/ICMS_Service/addContentResponse")]
        int addContent(SODAMvcApplication.CMSServiceReference.ContentDef contentdef);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICMS_Service/UpdateContent", ReplyAction="http://tempuri.org/ICMS_Service/UpdateContentResponse")]
        int UpdateContent(SODAMvcApplication.CMSServiceReference.ContentDef contentdef);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICMS_Service/getContent", ReplyAction="http://tempuri.org/ICMS_Service/getContentResponse")]
        SODAMvcApplication.CMSServiceReference.ContentDef[] getContent(string PageCode, string sectionName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICMS_Service/deleteContent", ReplyAction="http://tempuri.org/ICMS_Service/deleteContentResponse")]
        int deleteContent(string PageCode, string sectionName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICMS_Service/Authenticate", ReplyAction="http://tempuri.org/ICMS_Service/AuthenticateResponse")]
        bool Authenticate(string Password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICMS_Service/addContact", ReplyAction="http://tempuri.org/ICMS_Service/addContactResponse")]
        int addContact(SODAMvcApplication.CMSServiceReference.Contact contact);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICMS_Service/getContact", ReplyAction="http://tempuri.org/ICMS_Service/getContactResponse")]
        SODAMvcApplication.CMSServiceReference.Contact[] getContact();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICMS_Service/updateContact", ReplyAction="http://tempuri.org/ICMS_Service/updateContactResponse")]
        int updateContact(SODAMvcApplication.CMSServiceReference.Contact contact);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICMS_Service/deleteContact", ReplyAction="http://tempuri.org/ICMS_Service/deleteContactResponse")]
        int deleteContact(int id);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ICMS_ServiceChannel : SODAMvcApplication.CMSServiceReference.ICMS_Service, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CMS_ServiceClient : System.ServiceModel.ClientBase<SODAMvcApplication.CMSServiceReference.ICMS_Service>, SODAMvcApplication.CMSServiceReference.ICMS_Service {
        
        public CMS_ServiceClient() {
        }
        
        public CMS_ServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public CMS_ServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public CMS_ServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public CMS_ServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public int addContent(SODAMvcApplication.CMSServiceReference.ContentDef contentdef) {
            return base.Channel.addContent(contentdef);
        }
        
        public int UpdateContent(SODAMvcApplication.CMSServiceReference.ContentDef contentdef) {
            return base.Channel.UpdateContent(contentdef);
        }
        
        public SODAMvcApplication.CMSServiceReference.ContentDef[] getContent(string PageCode, string sectionName) {
            return base.Channel.getContent(PageCode, sectionName);
        }
        
        public int deleteContent(string PageCode, string sectionName) {
            return base.Channel.deleteContent(PageCode, sectionName);
        }
        
        public bool Authenticate(string Password) {
            return base.Channel.Authenticate(Password);
        }
        
        public int addContact(SODAMvcApplication.CMSServiceReference.Contact contact) {
            return base.Channel.addContact(contact);
        }
        
        public SODAMvcApplication.CMSServiceReference.Contact[] getContact() {
            return base.Channel.getContact();
        }
        
        public int updateContact(SODAMvcApplication.CMSServiceReference.Contact contact) {
            return base.Channel.updateContact(contact);
        }
        
        public int deleteContact(int id) {
            return base.Channel.deleteContact(id);
        }
    }
}

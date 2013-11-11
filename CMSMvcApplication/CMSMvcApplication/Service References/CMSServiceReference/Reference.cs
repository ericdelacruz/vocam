﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18331
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CMSMvcApplication.CMSServiceReference {
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
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="CMSServiceReference.ICMS_Service")]
    public interface ICMS_Service {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICMS_Service/addContent", ReplyAction="http://tempuri.org/ICMS_Service/addContentResponse")]
        int addContent(CMSMvcApplication.CMSServiceReference.ContentDef contentdef);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICMS_Service/UpdateContent", ReplyAction="http://tempuri.org/ICMS_Service/UpdateContentResponse")]
        int UpdateContent(CMSMvcApplication.CMSServiceReference.ContentDef contentdef);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICMS_Service/getContent", ReplyAction="http://tempuri.org/ICMS_Service/getContentResponse")]
        CMSMvcApplication.CMSServiceReference.ContentDef[] getContent(string PageCode, string sectionName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICMS_Service/deleteContent", ReplyAction="http://tempuri.org/ICMS_Service/deleteContentResponse")]
        int deleteContent(string PageCode, string sectionName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICMS_Service/Authenticate", ReplyAction="http://tempuri.org/ICMS_Service/AuthenticateResponse")]
        bool Authenticate(string Password);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ICMS_ServiceChannel : CMSMvcApplication.CMSServiceReference.ICMS_Service, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CMS_ServiceClient : System.ServiceModel.ClientBase<CMSMvcApplication.CMSServiceReference.ICMS_Service>, CMSMvcApplication.CMSServiceReference.ICMS_Service {
        
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
        
        public int addContent(CMSMvcApplication.CMSServiceReference.ContentDef contentdef) {
            return base.Channel.addContent(contentdef);
        }
        
        public int UpdateContent(CMSMvcApplication.CMSServiceReference.ContentDef contentdef) {
            return base.Channel.UpdateContent(contentdef);
        }
        
        public CMSMvcApplication.CMSServiceReference.ContentDef[] getContent(string PageCode, string sectionName) {
            return base.Channel.getContent(PageCode, sectionName);
        }
        
        public int deleteContent(string PageCode, string sectionName) {
            return base.Channel.deleteContent(PageCode, sectionName);
        }
        
        public bool Authenticate(string Password) {
            return base.Channel.Authenticate(Password);
        }
    }
}

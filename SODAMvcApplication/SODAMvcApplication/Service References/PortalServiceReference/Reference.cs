﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18331
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SODAMvcApplication.PortalServiceReference {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Customer", Namespace="http://schemas.datacontract.org/2004/07/SODAwcfService.Models")]
    [System.SerializableAttribute()]
    public partial class Customer : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Nullable<System.DateTime> DatePurchaseField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Nullable<System.DateTime> DateSubscriptionEndField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Nullable<System.DateTime> DateUpdatedField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private long IdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int LicensesField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Nullable<long> PPIdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private short RecurringTypeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private long SalesCodeIdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private long UserIdField;
        
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
        public System.Nullable<System.DateTime> DatePurchase {
            get {
                return this.DatePurchaseField;
            }
            set {
                if ((this.DatePurchaseField.Equals(value) != true)) {
                    this.DatePurchaseField = value;
                    this.RaisePropertyChanged("DatePurchase");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<System.DateTime> DateSubscriptionEnd {
            get {
                return this.DateSubscriptionEndField;
            }
            set {
                if ((this.DateSubscriptionEndField.Equals(value) != true)) {
                    this.DateSubscriptionEndField = value;
                    this.RaisePropertyChanged("DateSubscriptionEnd");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<System.DateTime> DateUpdated {
            get {
                return this.DateUpdatedField;
            }
            set {
                if ((this.DateUpdatedField.Equals(value) != true)) {
                    this.DateUpdatedField = value;
                    this.RaisePropertyChanged("DateUpdated");
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
        public int Licenses {
            get {
                return this.LicensesField;
            }
            set {
                if ((this.LicensesField.Equals(value) != true)) {
                    this.LicensesField = value;
                    this.RaisePropertyChanged("Licenses");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<long> PPId {
            get {
                return this.PPIdField;
            }
            set {
                if ((this.PPIdField.Equals(value) != true)) {
                    this.PPIdField = value;
                    this.RaisePropertyChanged("PPId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public short RecurringType {
            get {
                return this.RecurringTypeField;
            }
            set {
                if ((this.RecurringTypeField.Equals(value) != true)) {
                    this.RecurringTypeField = value;
                    this.RaisePropertyChanged("RecurringType");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long SalesCodeId {
            get {
                return this.SalesCodeIdField;
            }
            set {
                if ((this.SalesCodeIdField.Equals(value) != true)) {
                    this.SalesCodeIdField = value;
                    this.RaisePropertyChanged("SalesCodeId");
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
    [System.Runtime.Serialization.DataContractAttribute(Name="SalesPerson", Namespace="http://schemas.datacontract.org/2004/07/SODAwcfService.Models")]
    [System.SerializableAttribute()]
    public partial class SalesPerson : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int IdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int RegionIdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private long SalesCodeIdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private long UserIdField;
        
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
        public long SalesCodeId {
            get {
                return this.SalesCodeIdField;
            }
            set {
                if ((this.SalesCodeIdField.Equals(value) != true)) {
                    this.SalesCodeIdField = value;
                    this.RaisePropertyChanged("SalesCodeId");
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
    [System.Runtime.Serialization.DataContractAttribute(Name="SalesCode", Namespace="http://schemas.datacontract.org/2004/07/SODAwcfService.Models")]
    [System.SerializableAttribute()]
    public partial class SalesCode : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime DateCreatedField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Nullable<System.DateTime> DateEndField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal DiscountField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private long IdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Nullable<int> SalesPersonIDField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string Sales_CodeField;
        
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
        public System.DateTime DateCreated {
            get {
                return this.DateCreatedField;
            }
            set {
                if ((this.DateCreatedField.Equals(value) != true)) {
                    this.DateCreatedField = value;
                    this.RaisePropertyChanged("DateCreated");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<System.DateTime> DateEnd {
            get {
                return this.DateEndField;
            }
            set {
                if ((this.DateEndField.Equals(value) != true)) {
                    this.DateEndField = value;
                    this.RaisePropertyChanged("DateEnd");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal Discount {
            get {
                return this.DiscountField;
            }
            set {
                if ((this.DiscountField.Equals(value) != true)) {
                    this.DiscountField = value;
                    this.RaisePropertyChanged("Discount");
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
        public System.Nullable<int> SalesPersonID {
            get {
                return this.SalesPersonIDField;
            }
            set {
                if ((this.SalesPersonIDField.Equals(value) != true)) {
                    this.SalesPersonIDField = value;
                    this.RaisePropertyChanged("SalesPersonID");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Sales_Code {
            get {
                return this.Sales_CodeField;
            }
            set {
                if ((object.ReferenceEquals(this.Sales_CodeField, value) != true)) {
                    this.Sales_CodeField = value;
                    this.RaisePropertyChanged("Sales_Code");
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
    [System.Runtime.Serialization.DataContractAttribute(Name="Price", Namespace="http://schemas.datacontract.org/2004/07/SODAwcfService.Models")]
    [System.SerializableAttribute()]
    public partial class Price : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool ActiveField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool FirstMonthFreeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int IdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal PriceAmtField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal PriceAmt_BField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int RegionIdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal priceAmt_CField;
        
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
        public bool Active {
            get {
                return this.ActiveField;
            }
            set {
                if ((this.ActiveField.Equals(value) != true)) {
                    this.ActiveField = value;
                    this.RaisePropertyChanged("Active");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool FirstMonthFree {
            get {
                return this.FirstMonthFreeField;
            }
            set {
                if ((this.FirstMonthFreeField.Equals(value) != true)) {
                    this.FirstMonthFreeField = value;
                    this.RaisePropertyChanged("FirstMonthFree");
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
        public decimal PriceAmt {
            get {
                return this.PriceAmtField;
            }
            set {
                if ((this.PriceAmtField.Equals(value) != true)) {
                    this.PriceAmtField = value;
                    this.RaisePropertyChanged("PriceAmt");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal PriceAmt_B {
            get {
                return this.PriceAmt_BField;
            }
            set {
                if ((this.PriceAmt_BField.Equals(value) != true)) {
                    this.PriceAmt_BField = value;
                    this.RaisePropertyChanged("PriceAmt_B");
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
        public decimal priceAmt_C {
            get {
                return this.priceAmt_CField;
            }
            set {
                if ((this.priceAmt_CField.Equals(value) != true)) {
                    this.priceAmt_CField = value;
                    this.RaisePropertyChanged("priceAmt_C");
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
    [System.Runtime.Serialization.DataContractAttribute(Name="Region", Namespace="http://schemas.datacontract.org/2004/07/SODAwcfService.Models")]
    [System.SerializableAttribute()]
    public partial class Region : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CurrencyField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int IdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string RegionNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string WebsiteUrlField;
        
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
        public string Currency {
            get {
                return this.CurrencyField;
            }
            set {
                if ((object.ReferenceEquals(this.CurrencyField, value) != true)) {
                    this.CurrencyField = value;
                    this.RaisePropertyChanged("Currency");
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
        public string RegionName {
            get {
                return this.RegionNameField;
            }
            set {
                if ((object.ReferenceEquals(this.RegionNameField, value) != true)) {
                    this.RegionNameField = value;
                    this.RaisePropertyChanged("RegionName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string WebsiteUrl {
            get {
                return this.WebsiteUrlField;
            }
            set {
                if ((object.ReferenceEquals(this.WebsiteUrlField, value) != true)) {
                    this.WebsiteUrlField = value;
                    this.RaisePropertyChanged("WebsiteUrl");
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
    [System.Runtime.Serialization.DataContractAttribute(Name="LicenseConsumption", Namespace="http://schemas.datacontract.org/2004/07/SODAwcfService.Models")]
    [System.SerializableAttribute()]
    public partial class LicenseConsumption : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int ConsumedField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime DateUpdatedField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int IdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private long UserIdField;
        
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
        public int Consumed {
            get {
                return this.ConsumedField;
            }
            set {
                if ((this.ConsumedField.Equals(value) != true)) {
                    this.ConsumedField = value;
                    this.RaisePropertyChanged("Consumed");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime DateUpdated {
            get {
                return this.DateUpdatedField;
            }
            set {
                if ((this.DateUpdatedField.Equals(value) != true)) {
                    this.DateUpdatedField = value;
                    this.RaisePropertyChanged("DateUpdated");
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
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="PortalServiceReference.IPortalService")]
    public interface IPortalService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPortalService/getCustomer", ReplyAction="http://tempuri.org/IPortalService/getCustomerResponse")]
        SODAMvcApplication.PortalServiceReference.Customer[] getCustomer();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPortalService/getByDate", ReplyAction="http://tempuri.org/IPortalService/getByDateResponse")]
        SODAMvcApplication.PortalServiceReference.Customer[] getByDate(System.DateTime dateStart, System.Nullable<System.DateTime> dateEnd);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPortalService/getBySaleCode", ReplyAction="http://tempuri.org/IPortalService/getBySaleCodeResponse")]
        SODAMvcApplication.PortalServiceReference.Customer[] getBySaleCode(string SalesCode);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPortalService/addCustomer", ReplyAction="http://tempuri.org/IPortalService/addCustomerResponse")]
        int addCustomer(SODAMvcApplication.PortalServiceReference.Customer customer);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPortalService/updateCustomer", ReplyAction="http://tempuri.org/IPortalService/updateCustomerResponse")]
        int updateCustomer(SODAMvcApplication.PortalServiceReference.Customer customer);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPortalService/deleteCustomer", ReplyAction="http://tempuri.org/IPortalService/deleteCustomerResponse")]
        int deleteCustomer(long id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPortalService/getSalePerson", ReplyAction="http://tempuri.org/IPortalService/getSalePersonResponse")]
        SODAMvcApplication.PortalServiceReference.SalesPerson[] getSalePerson();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPortalService/addSalesPerson", ReplyAction="http://tempuri.org/IPortalService/addSalesPersonResponse")]
        int addSalesPerson(SODAMvcApplication.PortalServiceReference.SalesPerson salesPerson);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPortalService/updateSalesPerson", ReplyAction="http://tempuri.org/IPortalService/updateSalesPersonResponse")]
        int updateSalesPerson(SODAMvcApplication.PortalServiceReference.SalesPerson salesPerson);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPortalService/deleteSalePerson", ReplyAction="http://tempuri.org/IPortalService/deleteSalePersonResponse")]
        int deleteSalePerson(int ID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPortalService/getSaleCode", ReplyAction="http://tempuri.org/IPortalService/getSaleCodeResponse")]
        SODAMvcApplication.PortalServiceReference.SalesCode[] getSaleCode();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPortalService/addSalesCode", ReplyAction="http://tempuri.org/IPortalService/addSalesCodeResponse")]
        int addSalesCode(SODAMvcApplication.PortalServiceReference.SalesCode salesCode);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPortalService/updateSalsCode", ReplyAction="http://tempuri.org/IPortalService/updateSalsCodeResponse")]
        int updateSalsCode(SODAMvcApplication.PortalServiceReference.SalesCode salesCode);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPortalService/deleteSalesCode", ReplyAction="http://tempuri.org/IPortalService/deleteSalesCodeResponse")]
        int deleteSalesCode(long Id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPortalService/getPrice", ReplyAction="http://tempuri.org/IPortalService/getPriceResponse")]
        SODAMvcApplication.PortalServiceReference.Price[] getPrice();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPortalService/addPrice", ReplyAction="http://tempuri.org/IPortalService/addPriceResponse")]
        int addPrice(SODAMvcApplication.PortalServiceReference.Price price);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPortalService/updatePrice", ReplyAction="http://tempuri.org/IPortalService/updatePriceResponse")]
        int updatePrice(SODAMvcApplication.PortalServiceReference.Price price);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPortalService/deletePrice", ReplyAction="http://tempuri.org/IPortalService/deletePriceResponse")]
        int deletePrice(int Id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPortalService/getRegion", ReplyAction="http://tempuri.org/IPortalService/getRegionResponse")]
        SODAMvcApplication.PortalServiceReference.Region[] getRegion();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPortalService/addRegion", ReplyAction="http://tempuri.org/IPortalService/addRegionResponse")]
        int addRegion(SODAMvcApplication.PortalServiceReference.Region region);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPortalService/updateRegion", ReplyAction="http://tempuri.org/IPortalService/updateRegionResponse")]
        int updateRegion(SODAMvcApplication.PortalServiceReference.Region region);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPortalService/deleteRegion", ReplyAction="http://tempuri.org/IPortalService/deleteRegionResponse")]
        int deleteRegion(int id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPortalService/getLicenseConsumption", ReplyAction="http://tempuri.org/IPortalService/getLicenseConsumptionResponse")]
        SODAMvcApplication.PortalServiceReference.LicenseConsumption[] getLicenseConsumption();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPortalService/addLicenseConsumption", ReplyAction="http://tempuri.org/IPortalService/addLicenseConsumptionResponse")]
        int addLicenseConsumption(SODAMvcApplication.PortalServiceReference.LicenseConsumption license);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPortalService/updateLicenseConsumption", ReplyAction="http://tempuri.org/IPortalService/updateLicenseConsumptionResponse")]
        int updateLicenseConsumption(SODAMvcApplication.PortalServiceReference.LicenseConsumption license);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPortalService/delteLicenseConsumption", ReplyAction="http://tempuri.org/IPortalService/delteLicenseConsumptionResponse")]
        int delteLicenseConsumption(int id);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IPortalServiceChannel : SODAMvcApplication.PortalServiceReference.IPortalService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class PortalServiceClient : System.ServiceModel.ClientBase<SODAMvcApplication.PortalServiceReference.IPortalService>, SODAMvcApplication.PortalServiceReference.IPortalService {
        
        public PortalServiceClient() {
        }
        
        public PortalServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public PortalServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PortalServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PortalServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public SODAMvcApplication.PortalServiceReference.Customer[] getCustomer() {
            return base.Channel.getCustomer();
        }
        
        public SODAMvcApplication.PortalServiceReference.Customer[] getByDate(System.DateTime dateStart, System.Nullable<System.DateTime> dateEnd) {
            return base.Channel.getByDate(dateStart, dateEnd);
        }
        
        public SODAMvcApplication.PortalServiceReference.Customer[] getBySaleCode(string SalesCode) {
            return base.Channel.getBySaleCode(SalesCode);
        }
        
        public int addCustomer(SODAMvcApplication.PortalServiceReference.Customer customer) {
            return base.Channel.addCustomer(customer);
        }
        
        public int updateCustomer(SODAMvcApplication.PortalServiceReference.Customer customer) {
            return base.Channel.updateCustomer(customer);
        }
        
        public int deleteCustomer(long id) {
            return base.Channel.deleteCustomer(id);
        }
        
        public SODAMvcApplication.PortalServiceReference.SalesPerson[] getSalePerson() {
            return base.Channel.getSalePerson();
        }
        
        public int addSalesPerson(SODAMvcApplication.PortalServiceReference.SalesPerson salesPerson) {
            return base.Channel.addSalesPerson(salesPerson);
        }
        
        public int updateSalesPerson(SODAMvcApplication.PortalServiceReference.SalesPerson salesPerson) {
            return base.Channel.updateSalesPerson(salesPerson);
        }
        
        public int deleteSalePerson(int ID) {
            return base.Channel.deleteSalePerson(ID);
        }
        
        public SODAMvcApplication.PortalServiceReference.SalesCode[] getSaleCode() {
            return base.Channel.getSaleCode();
        }
        
        public int addSalesCode(SODAMvcApplication.PortalServiceReference.SalesCode salesCode) {
            return base.Channel.addSalesCode(salesCode);
        }
        
        public int updateSalsCode(SODAMvcApplication.PortalServiceReference.SalesCode salesCode) {
            return base.Channel.updateSalsCode(salesCode);
        }
        
        public int deleteSalesCode(long Id) {
            return base.Channel.deleteSalesCode(Id);
        }
        
        public SODAMvcApplication.PortalServiceReference.Price[] getPrice() {
            return base.Channel.getPrice();
        }
        
        public int addPrice(SODAMvcApplication.PortalServiceReference.Price price) {
            return base.Channel.addPrice(price);
        }
        
        public int updatePrice(SODAMvcApplication.PortalServiceReference.Price price) {
            return base.Channel.updatePrice(price);
        }
        
        public int deletePrice(int Id) {
            return base.Channel.deletePrice(Id);
        }
        
        public SODAMvcApplication.PortalServiceReference.Region[] getRegion() {
            return base.Channel.getRegion();
        }
        
        public int addRegion(SODAMvcApplication.PortalServiceReference.Region region) {
            return base.Channel.addRegion(region);
        }
        
        public int updateRegion(SODAMvcApplication.PortalServiceReference.Region region) {
            return base.Channel.updateRegion(region);
        }
        
        public int deleteRegion(int id) {
            return base.Channel.deleteRegion(id);
        }
        
        public SODAMvcApplication.PortalServiceReference.LicenseConsumption[] getLicenseConsumption() {
            return base.Channel.getLicenseConsumption();
        }
        
        public int addLicenseConsumption(SODAMvcApplication.PortalServiceReference.LicenseConsumption license) {
            return base.Channel.addLicenseConsumption(license);
        }
        
        public int updateLicenseConsumption(SODAMvcApplication.PortalServiceReference.LicenseConsumption license) {
            return base.Channel.updateLicenseConsumption(license);
        }
        
        public int delteLicenseConsumption(int id) {
            return base.Channel.delteLicenseConsumption(id);
        }
    }
}

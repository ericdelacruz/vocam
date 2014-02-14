using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SODAwcfService.Models
{
    public class PayPalCheckOutModel
    {
        public string itemName { get; set; }

        public PayPal.PayPalAPIInterfaceService.Model.CurrencyCodeType? CType { get; set; }

        public string confirmUrl { get; set; }

        public string cancelurl { get; set; }

        public int? Quantity { get; set; }

        public double orderTotalamt { get; set; }

        public double itemTotalamt { get; set; }

        public string OrderDesc { get; set; }

        public string itemDesc { get; set; }

        public string BillingAgreement { get; set; }

        public string ItemAmt { get; set; }

        public int? TaxTotalamt { get; set; }

        public string taxAmt { get; set; }
    }
    public class PayPalConfirmModel
    {

        public string token { get; set; }

        public string payorid { get; set; }

        public string schedDesc { get; set; }

        public DateTime dateStart { get; set; }

        public PayPal.PayPalAPIInterfaceService.Model.CurrencyCodeType? cType { get; set; }

        public decimal PaymenytAmt { get; set; }

        public int? BillingFrequency { get; set; }

        public long? userid { get; set; }

        public long SalesCodeId { get; set; }

        public int Qty { get; set; }

        public string TaxAmt { get; set; }
    }
    
}
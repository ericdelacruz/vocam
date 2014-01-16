using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SODAPortalMvcApplication.ViewModel
{
    public class CustomerModel
    {
        public PortalServiceReference.Customer customer { get; set; }
        public AccountServiceRef.Account account { get; set; }
        public PortalServiceReference.SalesCode salesCode { get; set; }
        public PortalServiceReference.SalesPerson salesPerson { get; set; }
        public PortalServiceReference.Price price { get; set; }

        public SODAPayPalSerRef.PayPalTrans paypal { get; set; }

        public PortalServiceReference.Region rejoin { get; set; }

        public PortalServiceReference.CustomerContract contract { get; set; }
    }

    public class VerifyModel
    {
        public PortalServiceReference.SalesPerson saleperson { get; set; }
        public PortalServiceReference.SalesCode salescode { get; set; }
        public PortalServiceReference.Price price { get; set; }
        public PortalServiceReference.Region region { get; set; }
        public decimal discountedPrice_A { get; set; }
        public decimal discountedPrice_B { get; set; }
        public decimal discountedPrice_C { get; set; }

        public int selectedSubscription { get; set; }
        public int qty { get; set; }
        public bool isDefaultSalesCode { get {return isDefault;}
                                        set { isDefault = value; }
                                        }
        private bool isDefault = false;
        public enum Currency
        {
            USD,
            EUR,
            AUD,
            GBP
        };
      
        public static System.Globalization.CultureInfo getCultureInfo(Currency curenncy)
        {
            switch (curenncy)
            {


                case Currency.GBP: return new System.Globalization.CultureInfo("en-GB");    
                case Currency.EUR:
                    return new System.Globalization.CultureInfo("de-DE");
                case Currency.AUD:
                    return new System.Globalization.CultureInfo("en-AU");
                default://USD default
                    return new System.Globalization.CultureInfo("en-US");
            }
        }

        public static System.Globalization.CultureInfo getCultureInfo(string strCurrency)
        {

            var currency = (Currency)Enum.Parse(typeof(Currency), strCurrency);

            return getCultureInfo(currency);
        }
    }
  
}
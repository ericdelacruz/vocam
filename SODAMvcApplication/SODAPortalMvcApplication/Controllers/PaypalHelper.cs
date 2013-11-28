using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Moolah;
using Moolah.PayPal;
namespace SODAPortalMvcApplication.Controllers
{
    public static class PaypalHelper
    {
        private static string username = "jon_api1.straightarrow.com";
        private static string password = "1384995928";
        private static string signature = "AFcWxV21C7fd0v3bYYYRCpSSRl31ATsYWC6SETdiq-vn09q6FuTpA0Kp";

        public static string checkout(decimal amt, Moolah.PayPal.CurrencyCodeType cType, string itemName, string itemDesc, string itemURL, string cancelurl, string confirmUrl)
        {
            var tempconfig = new PayPalConfiguration(PaymentEnvironment.Test, username, password, signature);
            var gateway = new PayPalExpressCheckout(tempconfig);
            try
            {
                var response = gateway.SetExpressCheckout(new OrderDetails
                {
                    Items = new[]{
                     new OrderDetailsItem 
                        { 
                            Description = itemDesc, 
                            Quantity = 1, 
                            UnitPrice = amt, 
                            ItemUrl = itemURL,
                            IsRecurringPayment = true,
                             Name = itemName
                        }
                 },
                    CurrencyCodeType = cType,
                    OrderTotal = amt,
                    OrderDescription = itemDesc

                }, cancelurl, confirmUrl);

                if (response.Status == PaymentStatus.Failed)
                {
                    throw new Exception(response.FailureMessage);
                }
                return response.RedirectUrl;
            }

            catch (Exception ex)
            {
                throw (ex);
            }

        }
    }
}
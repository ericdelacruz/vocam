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
        private static PayPalConfiguration config = new PayPalConfiguration(PaymentEnvironment.Test, username, password, signature);
        public static string checkout(decimal amt, Moolah.PayPal.CurrencyCodeType cType, string itemName, string itemDesc, string itemURL, string cancelurl, string confirmUrl)
        {
            //var tempconfig = new PayPalConfiguration(PaymentEnvironment.Test, username, password, signature);
            var gateway = new PayPalExpressCheckout(config);
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
        public static string confirmation(long userid, string payorid, string token, decimal amt, decimal recur_amt, Moolah.PayPal.CurrencyCodeType cType, string itemName, string itemDesc, DateTime dateStart)
        {
            var gateway = new PayPalExpressCheckout(config);
            var checkoutDetails = gateway.GetExpressCheckoutDetails(token);
            var response = gateway.DoExpressCheckoutPayment(amt, cType, token, payorid);
            if (response.Status == PaymentStatus.Failed)
            {
                //if (response.IsSystemFailure)
                //    // System failures can indicate issues like a configuration problem
                //    throw new Exception(response.FailureMessage);
                //else
                //    return response.FailureMessage;
                // Non-system failure messages can be shown directly to the customer
                //DisplayError(response.FailureMessage);
                throw new Exception(response.FailureMessage);
            }
            else
            {
                var recurringPaymentsResponse = gateway.CreateRecurringPaymentsProfile(new RecurringProfile
                {
                    Description = itemDesc,
                    BillingPeriod = RecurringPeriod.Month,
                    BillingFrequency = 1,
                    Amount = 10m,
                    StartDate = dateStart,
                    ItemName = itemName,
                    CurrencyCodeType = cType,

                }, token);

                if (recurringPaymentsResponse.IsSystemFailure || recurringPaymentsResponse.Status == PaymentStatus.Failed)
                    throw new Exception(recurringPaymentsResponse.FailureMessage);
                //using (PortalDataSetTableAdapters.PaypalTransTableAdapter paypalAdapter = new PortalDataSetTableAdapters.PaypalTransTableAdapter())
                //{
                //    try
                //    {
                //        paypalAdapter.Insert(userid, response.TransactionReference, recurringPaymentsResponse.ProfileId, true, DateTime.Now);
                //    }
                //    catch (Exception ex)
                //    {
                //        //log to textfile instead
                //    }
                //}
                return response.TransactionReference + ";" + recurringPaymentsResponse.ProfileId;
            }
        }
    }
}
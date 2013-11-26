using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Moolah;
using Moolah.PayPal;
using PayPal.PayPalAPIInterfaceService.Model;
using PayPal.PayPalAPIInterfaceService;
namespace SODAwcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SODAPaypalService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select SODAPaypalService.svc or SODAPaypalService.svc.cs at the Solution Explorer and start debugging.
    public class SODAPaypalService : ISODAPaypalService
    {

        private static string username = "jon_api1.straightarrow.com";
        private static string password = "1384995928";
        private static string signature = "AFcWxV21C7fd0v3bYYYRCpSSRl31ATsYWC6SETdiq-vn09q6FuTpA0Kp";
        private static PayPalConfiguration config = new PayPalConfiguration(PaymentEnvironment.Test, username, password, signature);
        //private static PayPalConfiguration config = new PayPalConfiguration(PaymentEnvironment.Test, "jon_api1.straightarrow.com", "1384995928", "AFcWxV21C7fd0v3bYYYRCpSSRl31ATsYWC6SETdiq-vn09q6FuTpA0Kp");

        public string checkout(decimal amt, Moolah.PayPal.CurrencyCodeType cType, string itemName, string itemDesc, string itemURL, string cancelurl, string confirmUrl)
        {
            var gateway = new PayPalExpressCheckout(config);
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

        public string confirmation(long userid, string payorid, string token, decimal amt, decimal recur_amt, Moolah.PayPal.CurrencyCodeType cType, string itemName, string itemDesc, DateTime dateStart)
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
                using (PortalDataSetTableAdapters.PaypalTransTableAdapter paypalAdapter = new PortalDataSetTableAdapters.PaypalTransTableAdapter())
                {
                    try
                    {
                        paypalAdapter.Insert(userid, response.TransactionReference, recurringPaymentsResponse.ProfileId, true, DateTime.Now);
                    }
                    catch (Exception ex)
                    {
                        //log to textfile instead
                    }
                }
                return response.TransactionReference + ";" + recurringPaymentsResponse.ProfileId;
            }
        }

        public bool cancelSubscription(long userid)
        {
            string profileID = "";
            bool flag = false;
            using (PortalDataSetTableAdapters.PaypalTransTableAdapter paypalAdapter = new PortalDataSetTableAdapters.PaypalTransTableAdapter())
            {
                try
                {
                    var result = (from row in paypalAdapter.GetData()
                                  where row.UserID == userid
                                  select row).First();

                    if ((flag = cancel(result.RPProfile)))
                    {
                        result.Active = false;

                        paypalAdapter.Update(result);
                    }
                }
                catch (Exception ex)
                {
                    //log to textfile maybe
                }
            }
            return flag;
        }
        private bool cancel(string profileid)
        {
            ManageRecurringPaymentsProfileStatusRequestType request =
                new ManageRecurringPaymentsProfileStatusRequestType();
            ManageRecurringPaymentsProfileStatusRequestDetailsType details =
                new ManageRecurringPaymentsProfileStatusRequestDetailsType();

            request.ManageRecurringPaymentsProfileStatusRequestDetails = details;

            details.ProfileID = profileid;

            details.Action = StatusChangeActionType.CANCEL;

            ManageRecurringPaymentsProfileStatusReq wrapper = new ManageRecurringPaymentsProfileStatusReq();
            wrapper.ManageRecurringPaymentsProfileStatusRequest = request;

            Dictionary<string, string> configurationMap = GetAcctAndConfig();
            PayPalAPIInterfaceServiceService service = new PayPalAPIInterfaceServiceService(configurationMap);

            ManageRecurringPaymentsProfileStatusResponseType manageProfileStatusResponse =
                    service.ManageRecurringPaymentsProfileStatus(wrapper);

            string result = setKeyResponseObjects(service, manageProfileStatusResponse);
            return true;
        }

        private string setKeyResponseObjects(PayPalAPIInterfaceServiceService service, ManageRecurringPaymentsProfileStatusResponseType response)
        {
            Dictionary<string, string> responseParams = new Dictionary<string, string>();
            responseParams.Add("API Status", response.Ack.ToString());
            // HttpContext CurrContext = HttpContext.Current;
            //CurrContext.Items.Add("Response_redirectURL", null);
            if (response.Ack.Equals(AckCodeType.FAILURE) ||
                (response.Errors != null && response.Errors.Count() > 0))
            {
                //CurrContext.Items.Add("Response_error", response.Errors);
                throw new Exception("Error");
            }
            else
            {
                //CurrContext.Items.Add("Response_error", null);
                //responseParams.Add("Profile Id", response.ManageRecurringPaymentsProfileStatusResponseDetails.ProfileID);
                return response.ManageRecurringPaymentsProfileStatusResponseDetails.ProfileID;
            }
            //CurrContext.Items.Add("Response_keyResponseObject", responseParams);
            //CurrContext.Items.Add("Response_apiName", "ManageRecurringPaymentsProfileStatus");
            //CurrContext.Items.Add("Response_requestPayload", service.getLastRequest());
            //CurrContext.Items.Add("Response_responsePayload", service.getLastResponse());

        }
        private Dictionary<string, string> GetAcctAndConfig()
        {
            Dictionary<string, string> configMap = new Dictionary<string, string>();

            configMap = GetConfig();

            // Signature Credential
            configMap.Add("account1.apiUsername", username);
            configMap.Add("account1.apiPassword", password);
            configMap.Add("account1.apiSignature", signature);
            // Optional
            // configMap.Add("account1.Subject", "");

            // Sample Certificate Credential
            // configMap.Add("account2.apiUsername", "certuser_biz_api1.paypal.com");
            // configMap.Add("account2.apiPassword", "D6JNKKULHN3G5B8A");
            // configMap.Add("account2.apiCertificate", "resource/sdk-cert.p12");
            // configMap.Add("account2.privateKeyPassword", "password");
            // Optional
            // configMap.Add("account2.Subject", "");
            return configMap;
        }
        private Dictionary<string, string> GetConfig()
        {
            Dictionary<string, string> configMap = new Dictionary<string, string>();

            // Endpoints are varied depending on whether sandbox OR live is chosen for mode
            configMap.Add("mode", "sandbox");

            // These values are defaulted in SDK. If you want to override default values, uncomment it and add your value.
            // configMap.Add("connectionTimeout", "5000");
            // configMap.Add("requestRetries", "2");

            return configMap;
        }




        public GetRecurringPaymentsProfileDetailsResponseType getRecurProfileDetails(long userid)
        {
            GetRecurringPaymentsProfileDetailsResponseType recurringPaymentsProfileDetailsResponse = new GetRecurringPaymentsProfileDetailsResponseType();
            using (PortalDataSetTableAdapters.PaypalTransTableAdapter paypalAdapter = new PortalDataSetTableAdapters.PaypalTransTableAdapter())
            {
                var recur_profile = paypalAdapter.GetData().Select(p => p).Where(p => p.UserID == userid);
                if (recur_profile.Count() > 0)
                {
                    string profileid = recur_profile.First().RPProfile;
                    GetRecurringPaymentsProfileDetailsRequestType request = new GetRecurringPaymentsProfileDetailsRequestType();
                    request.ProfileID = profileid;

                    GetRecurringPaymentsProfileDetailsReq wrapper = new GetRecurringPaymentsProfileDetailsReq();
                    wrapper.GetRecurringPaymentsProfileDetailsRequest = request;

                    // Configuration map containing signature credentials and other required configuration.
                    // For a full list of configuration parameters refer in wiki page 
                    // [https://github.com/paypal/sdk-core-dotnet/wiki/SDK-Configuration-Parameters]
                    Dictionary<string, string> configurationMap = GetAcctAndConfig();

                    // Create the PayPalAPIInterfaceServiceService service object to make the API call
                    PayPalAPIInterfaceServiceService service = new PayPalAPIInterfaceServiceService(configurationMap);

                    try
                    {
                        recurringPaymentsProfileDetailsResponse = service.GetRecurringPaymentsProfileDetails(wrapper);
                    }
                    catch
                    {
                        recurringPaymentsProfileDetailsResponse = null;
                    }

                }
            }
            return recurringPaymentsProfileDetailsResponse;
        }



        
        
    }
}

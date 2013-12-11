using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

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
        //private static PayPalConfiguration config = new PayPalConfiguration(PaymentEnvironment.Test, username, password, signature);
        //private static PayPalConfiguration config = new PayPalConfiguration(PaymentEnvironment.Test, "jon_api1.straightarrow.com", "1384995928", "AFcWxV21C7fd0v3bYYYRCpSSRl31ATsYWC6SETdiq-vn09q6FuTpA0Kp");
        
        public string checkout(decimal amt, CurrencyCodeType CType , string itemName, string itemDesc, string itemURL, string cancelurl, string confirmUrl)
        {
            // Create request object
            SetExpressCheckoutRequestType request = new SetExpressCheckoutRequestType();
            populateRequestObject(request,amt,CType,itemName,itemDesc,itemURL,cancelurl,confirmUrl);

            SetExpressCheckoutReq wrapper = new SetExpressCheckoutReq();
            wrapper.SetExpressCheckoutRequest = request;

            Dictionary<string, string> configurationMap = GetAcctAndConfig();

            // Create the PayPalAPIInterfaceServiceService service object to make the API call
            PayPalAPIInterfaceServiceService service = new PayPalAPIInterfaceServiceService(configurationMap);

            // # API call 
            // Invoke the SetExpressCheckout method in service wrapper object  
            SetExpressCheckoutResponseType setECResponse = service.SetExpressCheckout(wrapper);
            if (setECResponse.Ack.Equals(AckCodeType.FAILURE) ||
                (setECResponse.Errors != null && setECResponse.Errors.Count > 0))
            {
                throw new FaultException("Express Checkout Failed:");
            }

            return "https://www.sandbox.paypal.com/webscr&cmd=" + "_express-checkout&token=" + setECResponse.Token;
        }

        
        public string checkoutModel(Models.PayPalCheckOutModel model)
        {
            SetExpressCheckoutRequestType request = new SetExpressCheckoutRequestType();
            populateRequestObject(request,model);

            SetExpressCheckoutReq wrapper = new SetExpressCheckoutReq();
            wrapper.SetExpressCheckoutRequest = request;

            Dictionary<string, string> configurationMap = GetAcctAndConfig();

            // Create the PayPalAPIInterfaceServiceService service object to make the API call
            PayPalAPIInterfaceServiceService service = new PayPalAPIInterfaceServiceService(configurationMap);

            // # API call 
            // Invoke the SetExpressCheckout method in service wrapper object  
            SetExpressCheckoutResponseType setECResponse = service.SetExpressCheckout(wrapper);
            if (setECResponse.Ack.Equals(AckCodeType.FAILURE) ||
                (setECResponse.Errors != null && setECResponse.Errors.Count > 0))
            {
                throw new FaultException("Express Checkout Failed:");
            }

            return "https://www.sandbox.paypal.com/webscr&cmd=" + "_express-checkout&token=" + setECResponse.Token;
        }

       

        private void populateRequestObject(SetExpressCheckoutRequestType request,Models.PayPalCheckOutModel model)
        {
            SetExpressCheckoutRequestDetailsType ecDetails = new SetExpressCheckoutRequestDetailsType();
            
                ecDetails.ReturnURL = model.confirmUrl;
            
                ecDetails.CancelURL = model.cancelurl;
            

            /* Populate payment requestDetails. 
            * SetExpressCheckout allows parallel payments of upto 10 payments. 
            * This samples shows just one payment.
            */
            PaymentDetailsType paymentDetails = new PaymentDetailsType();
            ecDetails.PaymentDetails.Add(paymentDetails);

            double orderTotal = model.orderTotalamt;
            double itemTotal = model.itemTotalamt;

            
                paymentDetails.OrderDescription = model.OrderDesc;
            

            // Each payment can include requestDetails about multiple items
            // This example shows just one payment item

            PaymentDetailsItemType itemDetails = new PaymentDetailsItemType();
            itemDetails.Name = model.itemName;
            itemDetails.Amount = new BasicAmountType(model.CType, model.ItemAmt);
            itemDetails.Quantity = model.Quantity;
            // Indicates whether an item is digital or physical. For digital goods, this field is required and must be set to Digital. It is one of the following values:
            //   1.Digital
            //   2.Physical
            //  This field is available since version 65.1. 
            itemDetails.ItemCategory = ItemCategoryType.DIGITAL;

            //itemTotal = 1;

            //(Optional) Item description.
            // Character length and limitations: 127 single-byte characters
            // This field is introduced in version 53.0. 

            itemDetails.Description = model.itemDesc;

            paymentDetails.PaymentDetailsItem.Add(itemDetails);

            //orderTotal += itemTotal;
            paymentDetails.ItemTotal = new BasicAmountType(model.CType, itemTotal.ToString());
            paymentDetails.OrderTotal = new BasicAmountType(model.CType, orderTotal.ToString());

         
            BillingCodeType billingCodeType = BillingCodeType.RECURRINGPAYMENTS;
            BillingAgreementDetailsType baType = new BillingAgreementDetailsType(billingCodeType);
            baType.BillingAgreementDescription = model.BillingAgreement;

            ecDetails.BillingAgreementDetails.Add(baType);

            request.SetExpressCheckoutRequestDetails = ecDetails;
        }
       

        public string confirmation(long userid, string payorid, string token, decimal amt, decimal recur_amt, CurrencyCodeType cType, string itemName, string itemDesc, DateTime dateStart)
        {
             var details = getExpressCheckoutDetails(token);
             DoExpressCheckoutPaymentResponseType doECResponse = DoExpressCheckOut(payorid, token, details);


            var response =  CreateRecurringProfile(token, amt, cType, itemDesc, dateStart);
           
            if (response.Ack.Equals(AckCodeType.FAILURE) ||
             (response.Errors != null && response.Errors.Count > 0))
            {
                throw new FaultException("Error on creating profile");
            }

            using (PortalDataSetTableAdapters.PaypalTransTableAdapter paypalAdapter = new PortalDataSetTableAdapters.PaypalTransTableAdapter())
            {
                try
                {
                    //paypalAdapter.Insert(userid, doECResponse.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].TransactionID, response.CreateRecurringPaymentsProfileResponseDetails.ProfileID, true, DateTime.Now);
                }
                catch (Exception ex)
                {
                    //log to textfile instead
                }
            }
            return doECResponse.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].TransactionID + ";" + response.CreateRecurringPaymentsProfileResponseDetails.ProfileID;
        }
        public string confirmationModel(Models.PayPalConfirmModel model)
        {
            var details = getExpressCheckoutDetails(model.token);
            DoExpressCheckoutPaymentResponseType doECResponse = DoExpressCheckOut(details,model);
             //DoExpressCheckoutPaymentResponseType doECResponse = DoExpressCheckOut(model.payorid, model.token, details);

            var response = CreateRecurringProfile(model);
            
            if (response.Ack.Equals(AckCodeType.FAILURE) ||
             (response.Errors != null && response.Errors.Count > 0))
            {
                throw new FaultException("Error on creating profile");
            }

            using (PortalDataSetTableAdapters.PaypalTransTableAdapter paypalAdapter = new PortalDataSetTableAdapters.PaypalTransTableAdapter())
            {
                try
                {
                    //paypalAdapter.Insert(model.userid,doECResponse.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].TransactionID, response.CreateRecurringPaymentsProfileResponseDetails.ProfileID, true, DateTime.Now,model.SalesCodeId,model.PaymenytAmt,model.Qty);
                    paypalAdapter.Insert(model.userid.Value,doECResponse.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].TransactionID, response.CreateRecurringPaymentsProfileResponseDetails.ProfileID, true, DateTime.Now,model.SalesCodeId,model.PaymenytAmt,model.Qty);
                }
                catch (Exception ex)
                {
                    //log to textfile instead
                }
                finally
                {
                    paypalAdapter.Connection.Close();
                }
            }
            return doECResponse.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].TransactionID + ";" + response.CreateRecurringPaymentsProfileResponseDetails.ProfileID;
        }

        private CreateRecurringPaymentsProfileResponseType CreateRecurringProfile(Models.PayPalConfirmModel model)
        {
            CreateRecurringPaymentsProfileRequestType request = new CreateRecurringPaymentsProfileRequestType();

            CreateRecurringPaymentsProfileRequestDetailsType profileDetails = new CreateRecurringPaymentsProfileRequestDetailsType();
            request.CreateRecurringPaymentsProfileRequestDetails = profileDetails;

            profileDetails.Token = model.token;

            RecurringPaymentsProfileDetailsType rpProfileDetails =
               new RecurringPaymentsProfileDetailsType(model.dateStart.ToString("yyyy-MM-ddTHH:mm:ss"));
           
            profileDetails.RecurringPaymentsProfileDetails = rpProfileDetails;

            // (Required) Describes the recurring payments schedule, including the regular payment period, whether there is a trial period, and the number of payments that can fail before a profile is suspended.
            ScheduleDetailsType scheduleDetails = new ScheduleDetailsType();
            scheduleDetails.Description = model.schedDesc;

            BasicAmountType paymentAmount = new BasicAmountType(model.cType, Convert.ToDouble(model.PaymenytAmt).ToString());

            BillingPeriodType period = BillingPeriodType.MONTH;

            BillingPeriodDetailsType paymentPeriod = new BillingPeriodDetailsType(period, model.BillingFrequency, paymentAmount);


            scheduleDetails.PaymentPeriod = paymentPeriod;

            profileDetails.ScheduleDetails = scheduleDetails;

            CreateRecurringPaymentsProfileReq wrapper = new CreateRecurringPaymentsProfileReq();
            wrapper.CreateRecurringPaymentsProfileRequest = request;

            // Configuration map containing signature credentials and other required configuration.
            // For a full list of configuration parameters refer in wiki page 
            // [https://github.com/paypal/sdk-core-dotnet/wiki/SDK-Configuration-Parameters]
            Dictionary<string, string> configurationMap = GetAcctAndConfig();

            // Create the PayPalAPIInterfaceServiceService service object to make the API call
            PayPalAPIInterfaceServiceService service = new PayPalAPIInterfaceServiceService(configurationMap);

            // # API call 
            // Invoke the CreateRecurringPaymentsProfile method in service wrapper object  
            return service.CreateRecurringPaymentsProfile(wrapper);
        }

        private DoExpressCheckoutPaymentResponseType DoExpressCheckOut(GetExpressCheckoutDetailsResponseType details, Models.PayPalConfirmModel model)
        {
            DoExpressCheckoutPaymentRequestType request = new DoExpressCheckoutPaymentRequestType();
            DoExpressCheckoutPaymentRequestDetailsType requestDetails = new DoExpressCheckoutPaymentRequestDetailsType();
            request.DoExpressCheckoutPaymentRequestDetails = requestDetails;

            requestDetails.PaymentDetails = details.GetExpressCheckoutDetailsResponseDetails.PaymentDetails;
            requestDetails.Token = model.token;
            requestDetails.PayerID = model.payorid;
            requestDetails.PaymentAction = PaymentActionCodeType.SALE;

            Dictionary<string, string> configurationMap = GetAcctAndConfig();
            PayPalAPIInterfaceServiceService service = new PayPalAPIInterfaceServiceService(configurationMap);
            // Invoke the API
            DoExpressCheckoutPaymentReq wrapper = new DoExpressCheckoutPaymentReq();
            wrapper.DoExpressCheckoutPaymentRequest = request;
            // # API call 
            // Invoke the DoExpressCheckoutPayment method in service wrapper object
            DoExpressCheckoutPaymentResponseType doECResponse = service.DoExpressCheckoutPayment(wrapper);

            if (doECResponse.Ack.Equals(AckCodeType.FAILURE) ||
              (doECResponse.Errors != null && doECResponse.Errors.Count > 0))
            {
                throw new FaultException("Confirmation Failed");
            }
            return doECResponse;
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
        public Models.RecuringProfileDetails getRecurProfileDetails(long userid)
        {
            return new Models.RecuringProfileDetails() { profileStatus = getRecurProfileDetailsfromPayPal(userid).GetRecurringPaymentsProfileDetailsResponseDetails.ProfileStatus };
        }

        private CreateRecurringPaymentsProfileResponseType CreateRecurringProfile(string token, decimal amt, CurrencyCodeType cType, string itemDesc, DateTime dateStart)
        {
            CreateRecurringPaymentsProfileRequestType request = new CreateRecurringPaymentsProfileRequestType();

            CreateRecurringPaymentsProfileRequestDetailsType profileDetails = new CreateRecurringPaymentsProfileRequestDetailsType();
            request.CreateRecurringPaymentsProfileRequestDetails = profileDetails;

            profileDetails.Token = token;

            RecurringPaymentsProfileDetailsType rpProfileDetails =
               new RecurringPaymentsProfileDetailsType(dateStart.ToString("yyyy-MM-ddTHH:mm:ss"));

            profileDetails.RecurringPaymentsProfileDetails = rpProfileDetails;

            // (Required) Describes the recurring payments schedule, including the regular payment period, whether there is a trial period, and the number of payments that can fail before a profile is suspended.
            ScheduleDetailsType scheduleDetails = new ScheduleDetailsType();
            scheduleDetails.Description = itemDesc;

            BasicAmountType paymentAmount = new BasicAmountType(cType, Convert.ToDouble(amt).ToString());

            BillingPeriodType period = BillingPeriodType.MONTH;

            BillingPeriodDetailsType paymentPeriod = new BillingPeriodDetailsType(period, 1, paymentAmount);


            scheduleDetails.PaymentPeriod = paymentPeriod;

            profileDetails.ScheduleDetails = scheduleDetails;

            CreateRecurringPaymentsProfileReq wrapper = new CreateRecurringPaymentsProfileReq();
            wrapper.CreateRecurringPaymentsProfileRequest = request;

            // Configuration map containing signature credentials and other required configuration.
            // For a full list of configuration parameters refer in wiki page 
            // [https://github.com/paypal/sdk-core-dotnet/wiki/SDK-Configuration-Parameters]
            Dictionary<string, string> configurationMap = GetAcctAndConfig();

            // Create the PayPalAPIInterfaceServiceService service object to make the API call
            PayPalAPIInterfaceServiceService service = new PayPalAPIInterfaceServiceService(configurationMap);

            // # API call 
            // Invoke the CreateRecurringPaymentsProfile method in service wrapper object  
            return service.CreateRecurringPaymentsProfile(wrapper);
        }

        private DoExpressCheckoutPaymentResponseType DoExpressCheckOut(string payorid, string token, GetExpressCheckoutDetailsResponseType details)
        {
            DoExpressCheckoutPaymentRequestType request = new DoExpressCheckoutPaymentRequestType();
            DoExpressCheckoutPaymentRequestDetailsType requestDetails = new DoExpressCheckoutPaymentRequestDetailsType();
            request.DoExpressCheckoutPaymentRequestDetails = requestDetails;

            requestDetails.PaymentDetails = details.GetExpressCheckoutDetailsResponseDetails.PaymentDetails;
            requestDetails.Token = token;
            requestDetails.PayerID = payorid;
            requestDetails.PaymentAction = PaymentActionCodeType.SALE;

            Dictionary<string, string> configurationMap = GetAcctAndConfig();
            PayPalAPIInterfaceServiceService service = new PayPalAPIInterfaceServiceService(configurationMap);
            // Invoke the API
            DoExpressCheckoutPaymentReq wrapper = new DoExpressCheckoutPaymentReq();
            wrapper.DoExpressCheckoutPaymentRequest = request;
            // # API call 
            // Invoke the DoExpressCheckoutPayment method in service wrapper object
            DoExpressCheckoutPaymentResponseType doECResponse = service.DoExpressCheckoutPayment(wrapper);

            if (doECResponse.Ack.Equals(AckCodeType.FAILURE) ||
              (doECResponse.Errors != null && doECResponse.Errors.Count > 0))
            {
                throw new FaultException("Confirmation Failed");
            }
            return doECResponse;
        }

        private GetExpressCheckoutDetailsResponseType getExpressCheckoutDetails(string token)
        {
            // Create request object
            GetExpressCheckoutDetailsRequestType request = new GetExpressCheckoutDetailsRequestType();
            // (Required) A timestamped token, the value of which was returned by SetExpressCheckout response.
            // Character length and limitations: 20 single-byte characters
            request.Token = token;
            // Invoke the API
            GetExpressCheckoutDetailsReq wrapper = new GetExpressCheckoutDetailsReq();
            wrapper.GetExpressCheckoutDetailsRequest = request;

            // Configuration map containing signature credentials and other required configuration.
            // For a full list of configuration parameters refer in wiki page 
            // [https://github.com/paypal/sdk-core-dotnet/wiki/SDK-Configuration-Parameters]
            Dictionary<string, string> configurationMap = GetAcctAndConfig();

            // Create the PayPalAPIInterfaceServiceService service object to make the API call
            PayPalAPIInterfaceServiceService service = new PayPalAPIInterfaceServiceService(configurationMap);

            // # API call 
            // Invoke the GetExpressCheckoutDetails method in service wrapper object
            return service.GetExpressCheckoutDetails(wrapper);
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




        public GetRecurringPaymentsProfileDetailsResponseType getRecurProfileDetailsfromPayPal(long userid)
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
        private void populateRequestObject(SetExpressCheckoutRequestType request, decimal amt, CurrencyCodeType CType, string itemName, string itemDesc, string itemURL, string cancelurl, string confirmUrl)
        {
            SetExpressCheckoutRequestDetailsType ecDetails = new SetExpressCheckoutRequestDetailsType();
            if (itemURL != string.Empty)
            {
                ecDetails.ReturnURL = confirmUrl;
            }
            if (cancelurl != string.Empty)
            {
                ecDetails.CancelURL = cancelurl;
            }

            /* Populate payment requestDetails. 
            * SetExpressCheckout allows parallel payments of upto 10 payments. 
            * This samples shows just one payment.
            */
            PaymentDetailsType paymentDetails = new PaymentDetailsType();
            ecDetails.PaymentDetails.Add(paymentDetails);

            double orderTotal = Convert.ToDouble(amt);
            double itemTotal = Convert.ToDouble(amt);

            //if (itemDesc != string.Empty)
            //{
            //    paymentDetails.OrderDescription = itemDesc;
            //}

            // Each payment can include requestDetails about multiple items
            // This example shows just one payment item

            PaymentDetailsItemType itemDetails = new PaymentDetailsItemType();
            itemDetails.Name = itemName;
            itemDetails.Amount = new BasicAmountType(CType, Convert.ToDouble(amt).ToString());
            itemDetails.Quantity = 1;
            // Indicates whether an item is digital or physical. For digital goods, this field is required and must be set to Digital. It is one of the following values:
            //   1.Digital
            //   2.Physical
            //  This field is available since version 65.1. 
            itemDetails.ItemCategory = ItemCategoryType.DIGITAL;

            //itemTotal = 1;

            //(Optional) Item description.
            // Character length and limitations: 127 single-byte characters
            // This field is introduced in version 53.0. 

            itemDetails.Description = itemDesc;

            paymentDetails.PaymentDetailsItem.Add(itemDetails);

            //orderTotal += itemTotal;
            paymentDetails.ItemTotal = new BasicAmountType(CType, itemTotal.ToString());
            paymentDetails.OrderTotal = new BasicAmountType(CType, orderTotal.ToString());

            //(Required) Type of billing agreement. For recurring payments,
            //this field must be set to RecurringPayments. 
            //In this case, you can specify up to ten billing agreements. 
            //Other defined values are not valid.
            //Type of billing agreement for reference transactions. 
            //You must have permission from PayPal to use this field. 
            //This field must be set to one of the following values:
            //   1. MerchantInitiatedBilling - PayPal creates a billing agreement 
            //      for each transaction associated with buyer.You must specify 
            //      version 54.0 or higher to use this option.
            //   2. MerchantInitiatedBillingSingleAgreement - PayPal creates a 
            //      single billing agreement for all transactions associated with buyer.
            //      Use this value unless you need per-transaction billing agreements. 
            //      You must specify version 58.0 or higher to use this option.
            BillingCodeType billingCodeType = BillingCodeType.RECURRINGPAYMENTS;
            BillingAgreementDetailsType baType = new BillingAgreementDetailsType(billingCodeType);
            baType.BillingAgreementDescription = itemDesc;

            ecDetails.BillingAgreementDetails.Add(baType);

            request.SetExpressCheckoutRequestDetails = ecDetails;
        }



        public IEnumerable<Models.PayPalTrans> getPayPalTrans()
        {

            PortalDataSetTableAdapters.PaypalTransTableAdapter paypalAdapter = new PortalDataSetTableAdapters.PaypalTransTableAdapter();
            var paypalList = paypalAdapter.GetData().ToList();
            paypalAdapter.Dispose();
            return paypalList.Select(p=>new Models.PayPalTrans()
             {
                Id = p.Id,
                ECTransID = p.ECTransID,
                RPPProfile = p.RPProfile,
                Active = p.Active,
                dateLog = p.DateLog,
                SalesCodeID = p.SalesCodeId,
                TotalAmt = p.TotalAmt,
                Qty = p.Qty

            });
           
          
            
        }


        public Models.RecuringProfileDetails getRecurProfileDetailsByTransID(string TransID)
        {
            GetRecurringPaymentsProfileDetailsResponseType recurringPaymentsProfileDetailsResponse = new GetRecurringPaymentsProfileDetailsResponseType();
            using (PortalDataSetTableAdapters.PaypalTransTableAdapter paypalAdapter = new PortalDataSetTableAdapters.PaypalTransTableAdapter())
            {
                var recur_profile = paypalAdapter.GetData().Select(p => p).Where(p => p.ECTransID == TransID);
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
            return new Models.RecuringProfileDetails() { profileStatus = recurringPaymentsProfileDetailsResponse.GetRecurringPaymentsProfileDetailsResponseDetails.ProfileStatus };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SODAPortalMvcApplication.Controllers
{
    public class userController : Controller
    {
        PortalServiceReference.PortalServiceClient portalClient = new PortalServiceReference.PortalServiceClient();
        AccountServiceRef.AccountServiceClient AccountClient = new AccountServiceRef.AccountServiceClient();
        SODAPayPalSerRef.SODAPaypalServiceClient paypalClient = new SODAPayPalSerRef.SODAPaypalServiceClient();

        private static string defaultRegion = "AU";//for debugging on localhost set your region here
        //
        // GET: /user/
        private static string BILLING_AGREEMENT_FORMAT = "You will be bill every {0} month/s";
        /// <summary>
        /// Controller action for Index. init account details and displayes the current subscription
        /// </summary>
        /// <param name="u">Username if redirection from different region</param>
        /// <param name="p">password if redirection from different region </param>
        /// <returns></returns>
        [RequireHttps]
        public ActionResult Index(string u, string p)
        {
            //authenticate when username if not null
            if (!string.IsNullOrEmpty(u))
            {
                
                if (AccountClient.AuthenticateUser(u, EncDec.DecryptString(p)))
                {
                    Session["Username"] = u;
                }
            }

            string username = Session["Username"] != null ? Session["Username"].ToString() : null;

            if (username == null)
                return RedirectToAction("index", "home");
            else
            {
                // this will contain the details of the customer
                var customer = getCustomerData(username);


                //First time user, redirect to purchasing page
                if (customer.Count() == 0)
                {

                    return RedirectToAction("indexpurchase");
                }
                else
                {

                    //checks if customer is in the correct region, else redirect to proper region url
                    if (Request.Url.Host != "localhost" && Request.Url.Host != customer.First().rejoin.WebsiteUrl.Replace("www", "portal"))
                    {

                        
                        return Redirect("https://" + customer.First().rejoin.WebsiteUrl.Replace("www", "portal") + Url.Action("Index", "user", new { u = username, p = customer.First().account.PASSWORD }));
                    }

                        
                    Session.Add("CustomerData", customer);
                        //Check Customer Recurring profile if still active, 
                        foreach(var cust in customer)
                        {
                            //if it has 0 remaining days
                            if (getRemainingDays(cust.customer) == 0)
                            {
                                //is recurring profile is still active then renew - extend the date of subscription depending on the type of subscription
                                if(isPayPalRecurActive(cust.paypal.ECTransID))
                                {
                                    var old_customer = cust.customer;
                                    switch(cust.customer.RecurringType)
                                    {
                                        //monthly
                                        case 1: cust.customer.DateSubscriptionEnd = cust.customer.DateSubscriptionEnd.Value.AddMonths(1);
                                            break;
                                        //3 months
                                        case 2: cust.customer.DateSubscriptionEnd = cust.customer.DateSubscriptionEnd.Value.AddMonths(3);
                                            break;
                                        //6 months
                                        case 3: cust.customer.DateSubscriptionEnd = cust.customer.DateSubscriptionEnd.Value.AddMonths(6);
                                            break;
                                    }
                                    //log
                                    AuditLoggingHelper.LogUpdateAction(Session["Username"].ToString(), old_customer, cust.customer, portalClient);
                                    //update to DB
                                    portalClient.updateCustomer(cust.customer);
                                }
                                
                               
                            }          
                        }
                        //this is for purchasing additional licenses
                        if (Session["SalesCode"] == null)
                        {
                                
                                var verifyModel = getDefaultVerifyViewModel();
                                Session.Add("SalesCode", verifyModel.First());
                                
                            
                        }
                        
                        return View(customer);
                    //}
                }
               
            }
           
        }
        private double getRemainingDays(PortalServiceReference.Customer c)
        {
            //set to 0 if less days is less than 0
            return (c.DateSubscriptionEnd.Value - DateTime.Now).TotalDays > 0 ? (c.DateSubscriptionEnd.Value - DateTime.Now).TotalDays : 0;
        }
        /// <summary>
        /// Post action for index controller. This purchase addtional licenses
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult index(FormCollection collection)
        {
            
            //no subscription selected
            if(collection["subscription"] == null)
            {
                ModelState.AddModelError("", "Please select subscrption");
                return View();
            }
            int qty = 0;
            //qty is not number
            if (!int.TryParse(collection["quantity"], out qty))
            {
                var customer = getCustomerData(Session["Username"].ToString());
            
                TempData["errorQuantity"] = "Invalid Quantity";
           

                return View(customer);
            }

            try
            {

                

                //compute client date given dateTimeOffset which is init by a js function located in the _layout
                DateTime clientDateTime = toClientTime(collection["dateTimeOffset"]);
                Session.Add("ClientDateTime", clientDateTime);
            }
            catch (System.Security.SecurityException)
            {
                //set the client date to server time if security exception
                Session.Add("ClientDateTime", DateTime.Now);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            
            var VerifyModel = Session["SalesCode"] as ViewModel.VerifyModel;
            VerifyModel.selectedSubscription = int.Parse(collection["subscription"]);
            VerifyModel.qty = !string.IsNullOrEmpty(collection["quantity"]) ? int.Parse(collection["quantity"]) : 1;
            Session["SalesCode"] = VerifyModel;
            return RedirectToAction("termsinit");
        }

        private DateTime toClientTime(string strTimeZoneOffset)
        {


            return (DateTime.UtcNow.Add(new TimeSpan(long.Parse(strTimeZoneOffset))));
        }

        private bool isPayPalRecurActive(string ECTRans)
        {
            try
            {
                var response = paypalClient.getRecurProfileDetailsByTransID(ECTRans);
                return response.profileStatus.Value == SODAPayPalSerRef.RecurringPaymentsProfileStatusType.ACTIVEPROFILE;
            }
            catch
            {
                return false;
            }
           
        }
        protected override void Dispose(bool disposing)
        {
            portalClient.Close();
            AccountClient.Close();
            paypalClient.Close();
            
            base.Dispose(disposing);
        }
        protected override IAsyncResult BeginExecute(System.Web.Routing.RequestContext requestContext, AsyncCallback callback, object state)
        {
            var region = portalClient.getRegion().Where(r => r.WebsiteUrl == requestContext.HttpContext.Request.Url.Host.Replace("portal", "www") || (requestContext.HttpContext.Request.Url.Host == "localhost" && r.RegionName == defaultRegion)).FirstOrDefault();
            if (region != null)
            {
                 //init merchant paypal account setting
                paypalClient.initPayPalAccountSettings(region.Id);
            }
            //gain access to account service methods
            AccountClient.Authenticate("myS0D@P@ssw0rd");
            return base.BeginExecute(requestContext, callback, state);
        }
        
        private bool isPayPalRecurActive(long userid)
        {

            var response = paypalClient.getRecurProfileDetails(userid);
           
            return response.profileStatus.Value == SODAPayPalSerRef.RecurringPaymentsProfileStatusType.ACTIVEPROFILE;
        }

        private IEnumerable<ViewModel.CustomerModel> getCustomerData(string username)
        {
            //customer detail if the customer used a sales code when they purchased or subscribed
            var customer = from cust in portalClient.getCustomer()
                           join user in AccountClient.getAccount(username) on cust.UserId equals user.Id
                           join salescode in portalClient.getSaleCode() on cust.SalesCodeId.Value equals salescode.Id
                           join sp in portalClient.getSalePerson() on salescode.SalesPersonID equals sp.Id
                           join p in portalClient.getPrice() on sp.RegionId equals p.RegionId
                           join PPT in paypalClient.getPayPalTrans() on cust.PPId equals PPT.Id
                           join r in portalClient.getRegion() on p.RegionId equals r.Id
                           join contract in portalClient.getCustomerContract() on user.Id equals contract.UserId
                           where ( r.DefaultSalesCodeId != salescode.Id) 
                           select new ViewModel.CustomerModel() { account = user, customer = cust, salesCode = salescode, salesPerson = sp, price = p, paypal = PPT, rejoin = r,contract = contract };
            //did not use a sales code. The difference is that the basis for the sales code will be default salescodeid in the region 
            var default_customer = from cust in portalClient.getCustomer()
                                   join user in AccountClient.getAccount(username) on cust.UserId equals user.Id
                                   join salescode in portalClient.getSaleCode() on cust.SalesCodeId.Value equals salescode.Id

                                   join r in portalClient.getRegion() on salescode.Id equals r.DefaultSalesCodeId

                                   join PPT in paypalClient.getPayPalTrans() on cust.PPId equals PPT.Id
                                   join p in portalClient.getPrice() on r.Id equals p.RegionId
                                   join contract in portalClient.getCustomerContract() on user.Id equals contract.UserId
                                  
                                   select new ViewModel.CustomerModel() { account = user, customer = cust, salesCode = salescode, paypal = PPT, rejoin = r, price = p, contract = contract };
            //Union the both customer with sales code and with "default" sales code, else just get either who has value
            return customer.Count() > 0 && default_customer.Count() > 0 ? customer.Union(default_customer) :
                customer.Count() > 0 && default_customer.Count() == 0 ? customer : default_customer;
           
        }

        /// <summary>
        /// Controller action for the purchasing page
        /// </summary>
        /// <param name="salescode"></param>
        /// <returns></returns>
        [RequireHttps]
        [AllowAnonymous]
        public ActionResult indexpurchase(string salescode)
        {
            //set session and tempdata for salescode to default salecode if null
            if (TempData["SalesCode"] == null)
            {
                Session.Add("SalesCode", getDefaultVerifyViewModel().First());
               
                TempData["SalesCode"] = getDefaultVerifyViewModel().First();
                TempData["DefaultSalesCode"] = true;
            }
            else if ((TempData["SalesCode"] as ViewModel.VerifyModel).salescode.Sales_Code == getDefaultVerifyViewModel().First().salescode.Sales_Code)
            {
                TempData["DefaultSalesCode"] = true;
            }
            else
            {
                TempData["DefaultSalesCode"] = null;
            }
            return View();
        }
        [HttpPost]
        [RequireHttps]
        public ActionResult indexpurchase(FormCollection collection)
        {

            if (collection["subscription"] == null)
            {
                return RedirectToAction("indexpurchase");
            }
            int qty = 0;
            if (!int.TryParse(collection["quantity"], out qty))
            {
                ModelState.AddModelError("", "Invalid Quantity");
                //TempData["error"] = "Invalid Quantity";
                if ((Session["SalesCode"] as ViewModel.VerifyModel).salescode.Sales_Code == getDefaultVerifyViewModel().First().salescode.Sales_Code)
                {
                    TempData["DefaultSalesCode"] = true;
                }
                else
                {
                    TempData["DefaultSalesCode"] = null;
                }
                return View();
            }
            var VerifyModel = Session["SalesCode"] as ViewModel.VerifyModel;
            VerifyModel.selectedSubscription = int.Parse(collection["subscription"]);
            VerifyModel.qty = !string.IsNullOrEmpty(collection["quantity"]) ? int.Parse(collection["quantity"]) : 1;
            Session["SalesCode"] = VerifyModel;
            
            //return Session["NewAccount"] != null? RedirectToAction("termsinit"):RedirectToAction("purchasedetails","home");

            if(Session["Username"] !=null && !AccountClient.isUserNameExists(Session["Username"].ToString()))
            return RedirectToAction("termsinit");
            else
                return RedirectToAction("purchasedetails","home");
        }
        private IEnumerable<ViewModel.VerifyModel> getDefaultVerifyViewModel()
        {
            int RegionId = getRegionId();
            //get salecode details of the default sales code set on the region setting
            var salescodeList = from r in portalClient.getRegion()
                            join sc in portalClient.getSaleCode() on r.DefaultSalesCodeId equals sc.Id
                            join p in portalClient.getPrice() on r.Id equals p.RegionId
                            where r.Id == RegionId
                            select new ViewModel.VerifyModel()
                            {
                                price = p,
                                salescode = sc,
                                region = r,
                                
                                discountedPrice_A = p.PriceAmt - sc.Less_monthly,
                                discountedPrice_B = p.PriceAmt_B - sc.Less_3months,
                                discountedPrice_C =  p.priceAmt_C -sc.Less_6months,
                                isDefaultSalesCode = true
                            };
            
            
            return salescodeList;
        }

        private int getRegionId()
        {
            return portalClient.getRegion().Where(r => r.WebsiteUrl == Request.Url.Host.Replace("portal", "www") || (Request.Url.Host == "localhost" && r.RegionName == defaultRegion)).First().Id;
            //return portalClient.getRegion().Where(r => r.WebsiteUrl == Request.Url.Host.Replace("portal", "www")).Count() > 0? portalClient.getRegion().Where(r => r.WebsiteUrl == Request.Url.Host.Replace("portal", "www")).First().Id:12;
        }
        /// <summary>
        /// Verify Salecode function for additional licenses page. Same as verify but redirect to index instead of purchase page
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult reverify(FormCollection collection)
        {
            
            
            string salescode = collection["SalesCode"];
            //get details
            var salescodeList = getVerifyViewModel(salescode);
            //if no result, error else set the add result to session
            if (salescodeList.Count() > 0)
            {
                TempData["error"] = null;
                Session.Add("SalesCode", salescodeList.First());
            }
            else
            {
                TempData["error"] = "Sales Code: " + collection["SalesCode"] + " doesn't exists";
                Session["SalesCode"] = null;
            }
            return RedirectToAction("index");
        }
        /// <summary>
        /// Verify sales code action for the purchase page/indexpurchase
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult verify(FormCollection collection)
        {
            string salescode = collection["SalesCode"];
            //get sales code details
            var salescodeList = getVerifyViewModel(salescode);
            //if no result, error else set the add result to session
            if (salescodeList.Count() > 0)
            {
                TempData["error"] = null;
                Session.Add("SalesCode", salescodeList.First());
                TempData.Add("SalesCode", salescodeList.First());
            }
            else
            {
                TempData["error"] = "Sales Code: " + collection["SalesCode"] + " doesn't exists"; 
                Session["SalesCode"] = null;
                TempData["SalesCode"] = null;
            }
            return RedirectToAction("indexpurchase");
        }

        private IEnumerable<ViewModel.VerifyModel> getVerifyViewModel(string salescode)
        {
            var websiteURL = Request.Url.Host.Replace("portal", "www") != "localhost" ? Request.Url.Host.Replace("portal", "www") : "www.sac-iis.com";
            //get SalesCode Details. Notes SalesCode depends on region.
            var salescodeList = from sp in portalClient.getSalePerson()
                                join sc in portalClient.getSaleCode() on sp.SalesCodeId equals sc.Id
                                join p in portalClient.getPrice() on sp.RegionId equals p.RegionId
                                join r in portalClient.getRegion() on sp.RegionId equals r.Id
                                where sc.Sales_Code.ToLower() == salescode.Trim().ToLower() &&  r.WebsiteUrl == websiteURL
                                select new ViewModel.VerifyModel() { price = p, saleperson = sp, salescode = sc, region = r,
                                
                                discountedPrice_A = p.PriceAmt - sc.Less_monthly,
                                discountedPrice_B = p.PriceAmt_B - sc.Less_3months,
                                discountedPrice_C = p.priceAmt_C - sc.Less_6months,
                                 isDefaultSalesCode = false
                                };
           
            return salescodeList;
            
        }
        /// <summary>
        /// Terms and condition page for the purchase and additional purchase actions
        /// </summary>
        /// <returns></returns>
        public ActionResult termsinit()
        {
            //redirect to home page id session expired
            if (Session["Username"] == null)
                return RedirectToAction("index", "home");
            else
            {
                //if Session["NewAccount"} is null get account details using Session["Username"] 
                var account =Session["NewAccount"] == null? AccountClient.getAccount(Session["Username"].ToString()).Select(a => a).First():Session["NewAccount"] as AccountServiceRef.Account;

                    return View(account);
                
            }
            
        }
        [HttpPost]
        public ActionResult termsinit(FormCollection collection)
        {
            //Redirect to checkout page if user agreed to the terms and conditions
            if(collection["theCheck"] == "yes")
            {
                return RedirectToAction("checkout");
            }
            else
            {
                ModelState.AddModelError("", "You must agree on the Term and Conditions to continue");
            }
            var account = Session["NewAccount"] == null ? AccountClient.getAccount(Session["Username"].ToString()).Select(a => a).First() : Session["NewAccount"] as AccountServiceRef.Account;
            return View(account);
        }
        /// <summary>
        /// Displays if the purchase is successfull or cancelled
        /// </summary>
        /// <param name="stat"></param>
        /// <returns></returns>
        [RequireHttps]
        public ActionResult paymentstatus(string stat)
        {
            ViewBag.paymentSuccess = (Session["Username"] != null || Session["NewAccount"] != null) && stat == "success";
            //if not sucess, clear customer session data
            if (stat != "success")
                Session.Remove("CustomerData");
            else//Success
            {
                //If new account, email customer the account details
                if(Session["NewAccount"] != null)
                {
                    var new_accnt = Session["NewAccount"] as AccountServiceRef.Account;
                    
                   
                    emailCustomer(new_accnt);
                    Session.Remove("NewAccount");
                    Session.Add("ReturnUrl", Url.Action("index"));//for change password
                    ViewBag.isNewAccount = true;
                    
                }
                Session.Remove("SalesCode");
            }
            return View();
        }

        private void emailCustomer(AccountServiceRef.Account account)
        {
            var CustomerName = account.FirstName + " " + account.LastName;
            var ContractRecord = from contract in portalClient.getCustomerContract()
                                 join accnt in AccountClient.getAccount(account.USERNAME) on contract.UserId equals accnt.Id
                                 select contract;

            var ContractEndDate = ContractRecord.First().DateEnd;
            var VerfiyModel = Session["SalesCode"] as ViewModel.VerifyModel;
            ViewData.Add("CustomerName", CustomerName);
            ViewData.Add("ContractEndDate", ContractEndDate);
            ViewData.Add("Qty", VerfiyModel.qty);
            ViewData.Add("Username", Session["Username"].ToString());
            string body = EmailHelper.ToHtml("emailaccount", ViewData, this.ControllerContext);
            //EmailHelper.SendEmail("test@sac-iis.com", Session["Username"].ToString(), "Account Details", body);
            string from = Request.Url.Host != "localhost" ? "no-reply" + Request.Url.Host.Replace("portal.", "@") : "test@sac-iis.com";
            string subject = "Your login details to Safety On Demand";
            EmailHelper.SendEmail(new System.Net.Mail.MailAddress(from, "Safety on Demand"), new System.Net.Mail.MailAddress(Session["Username"].ToString()), subject, body, true, null);
        }
        [RequireHttps]
        public ActionResult profile()
        {
            if (Session["Username"] == null)
                return RedirectToAction("index", "home");
            var cust = Session["CustomerData"] as IEnumerable<ViewModel.CustomerModel>;
            return View(cust.OrderByDescending(c=>c.customer.DateSubscriptionEnd).First());
        }
        [RequireHttps]
        public ActionResult editProfile()
        {
            if (Session["Username"] != null)
            {
                var userAcnt = from accnt in AccountClient.getAccount(Session["Username"].ToString())
                               select new ViewModel.UserModel() { Email = accnt.Email, Company = accnt.Company, Password = accnt.PASSWORD, Contact = accnt.ContactNo, FirtName = accnt.FirstName, LastName = accnt.LastName, CompanyUrl = accnt.CompanyUrl };

                var customer = from cust in portalClient.getCustomer()
                               join accnt in AccountClient.getAccount(Session["Username"].ToString()) on cust.UserId equals accnt.Id
                               select cust;

                //if (customer.Count() > 0)
                    //ViewBag.ContractEndDate = customer.First().DateSubscriptionEnd;
                ViewBag.ContractEndDate = portalClient.getCustomerContract().Where(contract => contract.UserId == customer.First().UserId).FirstOrDefault().DateEnd;
                return View(userAcnt.First());
            }
            else
            {
                return RedirectToAction("index", "Home");
            }
        }
        [RequireHttps]
        [HttpPost]
        public ActionResult editprofile(ViewModel.UserModel user)
        {
            ModelState.Remove("ConfirmPassword");
            if (ModelState.IsValid)
            {
                var accnt = AccountClient.getAccount(user.Email).First();
                accnt.FirstName = user.FirtName;
                accnt.LastName = user.LastName;
                accnt.Company = user.Company;
                accnt.ContactNo = user.Contact;
                accnt.CompanyUrl = user.CompanyUrl;
                AccountClient.updateAccount(accnt);
                Session["CustomerData"] = getCustomerData(user.Email);
                return RedirectToAction("profile");
            }
                return View(user);

        }
        [RequireHttps]
        public ActionResult checkout()
        {
            //redirect to index if session expired
            if (Session["Username"] != null && Session["SalesCode"] != null)
            {
                

                //init checkout object/model then call checkout service method
                string redirectURL = paypalClient.checkoutModel(initCheckoutModel());
            
                //redirect the user to paypal page. if the user cancels, paypal redirects to cancel action else if success, redirects to confirm action
                return Redirect(redirectURL);

            }
            else
                return RedirectToAction("index");
             
        }

        private SODAPayPalSerRef.PayPalCheckOutModel initCheckoutModel()
        {
            var VerifyModel = Session["SalesCode"] as ViewModel.VerifyModel;
            SODAPayPalSerRef.PayPalCheckOutModel ppc = new SODAPayPalSerRef.PayPalCheckOutModel();
            //if user cancel transaction, paypal redirects to cancelurl
            ppc.cancelurl = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("cancel");
            //if customer finished paying in paypal, paypal redirects to confirm url
            ppc.confirmUrl = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("confirm");
            ppc.CType = (SODAPayPalSerRef.CurrencyCodeType)Enum.Parse(typeof(SODAPayPalSerRef.CurrencyCodeType), VerifyModel.region.Currency);
            ppc.Quantity = VerifyModel.qty;
            ppc.OrderDesc = "Order Description here.";
            
            decimal roundedDCPrice = 0;
            //set the pricing for the paypal page depending on the type of subscrption selected
            switch(VerifyModel.selectedSubscription)
            {
                case 1://monthly
                    /*round to 2 decimal places to prevent overflow. Paypal will not accept like 1.234 to must be rounded off to 1.23*/
                    roundedDCPrice = Math.Round( VerifyModel.discountedPrice_A,2);
                    ppc.ItemAmt = string.Format("{0:0.00}", roundedDCPrice);
                    ppc.itemDesc = "SODA Monthly Recurring Subscription";
                    ppc.itemName = "Monthly Recurring Subscription";
                    
                    ppc.itemTotalamt =  Convert.ToDouble(roundedDCPrice * ppc.Quantity);
                    
                    ppc.orderTotalamt = ppc.itemTotalamt;
                   
                    ppc.BillingAgreement = string.Format(BILLING_AGREEMENT_FORMAT, 1);
                   
                    break;
                case 2://3 months
                    roundedDCPrice = Math.Round(VerifyModel.discountedPrice_B,2);
                    ppc.ItemAmt = string.Format("{0:0.00}",roundedDCPrice);
                    ppc.itemDesc = "SODA 3 Months Recurring Subscription";
                    ppc.itemName = "3 Months Recurring Subscription";
                    ppc.itemTotalamt = Convert.ToDouble(roundedDCPrice * ppc.Quantity);
                    ppc.orderTotalamt = ppc.itemTotalamt;
                    ppc.BillingAgreement = string.Format(BILLING_AGREEMENT_FORMAT, 3);
                    break;
                case 3: //6 months
                     roundedDCPrice = Math.Round(VerifyModel.discountedPrice_C,2);
                    ppc.ItemAmt = string.Format("{0:0.00}",roundedDCPrice);
                    ppc.itemDesc = "SODA 6 Months Recurring Subscription";
                    ppc.itemName = "6 Months Recurring Subscription";
                    ppc.itemTotalamt = Convert.ToDouble(roundedDCPrice * ppc.Quantity);
                    ppc.orderTotalamt = ppc.itemTotalamt;
                    ppc.BillingAgreement = string.Format(BILLING_AGREEMENT_FORMAT, 6);
                    break;
                default:
                    throw new Exception("Invalid subscription type");
            }
            return ppc;
        }
        /// <summary>
        /// Cancel purchase from paypal
        /// </summary>
        /// <returns></returns>
        public ActionResult cancel()
        {
            return RedirectToAction("paymentstatus", new { stat = "cancel" }) ;
        }
        [RequireHttps]
        public ActionResult confirm(string token, string payerid)
        {
            
            
            var VerfiyModel = Session["SalesCode"] as ViewModel.VerifyModel;
            //if not existing account,create new
            if(Session["NewAccount"] != null)
            {
                var new_accnt = Session["NewAccount"] as AccountServiceRef.Account;
                AuditLoggingHelper.LogCreateAction(new_accnt.USERNAME, new_accnt, portalClient);
                AccountClient.addAccount(new_accnt);

               
            }
            //get account details
             var accnt = from account in AccountClient.getAccount(Session["Username"].ToString())
                        select account;
            //confirm to paypal that we have recieve payment and create recurring profile    
            string result = paypalClient.confirmationModel(initConfirmModel(VerfiyModel, accnt.First().Id, token, payerid));
            //ini new customer record
            var new_cust = new PortalServiceReference.Customer()
                {
                    UserId = accnt.First().Id,
                    DatePurchase = new Nullable<DateTime>(DateTime.Now),
                    //DateSubscriptionEnd = new Nullable<DateTime>(DateTime.Now.AddMonths(6)),
                    DateSubscriptionEnd = getDateSubscriptionEnd(VerfiyModel.selectedSubscription, VerfiyModel.price.FirstMonthFree),
                    SalesCodeId = VerfiyModel.salescode.Id,
                    DateUpdated = DateTime.Now,
                    Licenses = VerfiyModel.qty,
                    RecurringType = Convert.ToInt16(VerfiyModel.selectedSubscription),
                    PPId = paypalClient.getPayPalTrans().Select(p => p).Where(p => p.ECTransID == result.Split(';')[0]).First().Id
                };
            //log customer
                AuditLoggingHelper.LogCreateAction(Session["Username"].ToString(), new_cust, portalClient);
            //add to db
                portalClient.addCustomer(new_cust);
                //if new account, add contract. else update existing contract
                if (Session["NewAccount"] != null)
                {
                   //init customer contract
                    var new_cust_contract = new PortalServiceReference.CustomerContract()
                    {
                       
                        DateEnd = (Session["ClientDateTime"] as Nullable<DateTime>).Value.AddMonths(6),
                        
                        DateStart = (Session["ClientDateTime"] as Nullable<DateTime>).Value,
                        UserId = accnt.First().Id
                    };
                    //log contract
                    AuditLoggingHelper.LogCreateAction(Session["Username"].ToString(), new_cust_contract, portalClient);
                    //add to db
                    portalClient.addCustomerContract(new_cust_contract);
                }
                else
                {

                    var old_contract = portalClient.getCustomerContract().Where(c => c.UserId == accnt.First().Id).First();
                    var new_cust_contract = new PortalServiceReference.CustomerContract()
                    {
                        //DateEnd = DateTime.Now.AddMonths(6),
                        DateEnd = (Session["ClientDateTime"] as Nullable<DateTime>).Value.AddMonths(6),
                        //DateStart = DateTime.Now,
                        DateStart = (Session["ClientDateTime"] as Nullable<DateTime>).Value,
                        UserId = accnt.First().Id,
                        Id = old_contract.Id
                    };
                    AuditLoggingHelper.LogUpdateAction(Session["Username"].ToString(), old_contract, new_cust_contract, portalClient);
                    portalClient.updateCustomerContract(new_cust_contract);
                    //AuditLoggingHelper.LogCreateAction(Session["Username"].ToString(), new_cust_contract, portalClient);
                    //portalClient.addCustomerContract(new_cust_contract);
                }
                return RedirectToAction("paymentstatus",new{stat="success"});
            
        }

        private DateTime? getDateSubscriptionEnd(int RecurringType, bool FirstMonthFree)
        {
            switch(RecurringType)
            {
                case 1: return FirstMonthFree?DateTime.Now.AddMonths(2):DateTime.Now.AddMonths(1);
                case 2: return FirstMonthFree?DateTime.Now.AddMonths(4):DateTime.Now.AddMonths(3);
                default: return FirstMonthFree?DateTime.Now.AddMonths(7): DateTime.Now.AddMonths(6);
            }
        }

        private SODAPayPalSerRef.PayPalConfirmModel initConfirmModel(ViewModel.VerifyModel VerfiyModel, long userid, string token, string payerid)
        {
            //var VerfiyModel = Session["SalesCode"] as ViewModel.VerifyModel;

            SODAPayPalSerRef.PayPalConfirmModel ppConfirm = new SODAPayPalSerRef.PayPalConfirmModel();
            ppConfirm.userid = userid;
            ppConfirm.token = token;
            ppConfirm.payorid = payerid;
            ppConfirm.cType = (SODAPayPalSerRef.CurrencyCodeType)Enum.Parse(typeof(SODAPayPalSerRef.CurrencyCodeType), VerfiyModel.region.Currency);
            ppConfirm.SalesCodeId = VerfiyModel.salescode.Id;

            switch(VerfiyModel.selectedSubscription)
            {
                case 1://monthly
                    ppConfirm.dateStart = VerfiyModel.price.FirstMonthFree ? DateTime.Now.AddMonths(2) : DateTime.Now.AddMonths(1);
                    ppConfirm.PaymenytAmt = Math.Round(VerfiyModel.discountedPrice_A,2) * VerfiyModel.qty;
                    ppConfirm.schedDesc = string.Format(BILLING_AGREEMENT_FORMAT, 1);
                    ppConfirm.BillingFrequency = 1;
                    break;
                case 2://3 months
                    ppConfirm.dateStart = VerfiyModel.price.FirstMonthFree ? DateTime.Now.AddMonths(4) : DateTime.Now.AddMonths(3);
                    ppConfirm.PaymenytAmt = Math.Round(VerfiyModel.discountedPrice_B,2) * VerfiyModel.qty;
                    ppConfirm.schedDesc = string.Format(BILLING_AGREEMENT_FORMAT, 3);
                    ppConfirm.BillingFrequency = 3;
                    break;
                case 3: // 6 months
                     ppConfirm.dateStart = VerfiyModel.price.FirstMonthFree ? DateTime.Now.AddMonths(7) : DateTime.Now.AddMonths(6);
                    ppConfirm.PaymenytAmt = Math.Round( VerfiyModel.discountedPrice_C,2) * VerfiyModel.qty;
                    ppConfirm.schedDesc = string.Format(BILLING_AGREEMENT_FORMAT, 6);
                    ppConfirm.BillingFrequency = 6;
                    break;


            }
            return ppConfirm;
        }
        /// <summary>
        /// Cancels the recurring profile and updates the subscription end to current
        /// </summary>
        /// <param name="transid"></param>
        /// <returns></returns>
        public ActionResult unsubscribe(string transid)
        {
            
             
            if (Session["CustomerData"] != null)
             {
                //get the details of the selected subscription to be canceled 
                var customerViewModel = (Session["CustomerData"] as IEnumerable<ViewModel.CustomerModel>).Where(c=> c.paypal.ECTransID == transid);

                 var customRecord = customerViewModel.First().customer;
                //set the subscription end to current date
                 customRecord.DateSubscriptionEnd = DateTime.Now;


                    //cancel recurring profile using transaction id
                    paypalClient.cancelSubscription(transid);
                    //log
                     AuditLoggingHelper.LogUpdateAction(Session["Username"].ToString(), customerViewModel.First().customer, customRecord, portalClient);
                    //update customer
                     portalClient.updateCustomer(customRecord);
                     //get current state of license cosump for logging
                     var old_licenseConsumption = portalClient.getLicenseConsumption().Where(lc => lc.UserId == customerViewModel.First().account.Id).FirstOrDefault();
                     if (old_licenseConsumption != null)
                     {
                         //init updated LC
                         var new_licenseConsumption = new PortalServiceReference.LicenseConsumption()
                         {
                             Id = old_licenseConsumption.Id,
                             Consumed = 0,
                             DateUpdated = DateTime.Now,
                             UserId = old_licenseConsumption.UserId
                         };
                         //log LC
                         AuditLoggingHelper.LogUpdateAction(Session["Username"].ToString(), old_licenseConsumption, new_licenseConsumption, portalClient);
                         //update to DB
                         portalClient.updateLicenseConsumption(new_licenseConsumption);
                     }
                 }
                 
                 
                 return RedirectToAction("index");
                 
             
        }


        [RequireHttps]
        public ActionResult downloads()
        {
            //redirect to home page if session expired
            if (Session["Username"] == null)
                return RedirectToAction("index", "home");
            else
            return View();
        }
        [RequireHttps]
        public ActionResult termsconditions()
        {
            //redirect to home page if session expired
            if (Session["Username"] == null)
                return RedirectToAction("index", "home");
            //get Customer COntract details
            var CustomerContract = portalClient.getCustomerContract().Where(contract => contract.UserId == AccountClient.getAccount(Session["Username"].ToString()).First().Id);
            ViewBag.ContractDate = CustomerContract.First().DateStart;
            return View();
        }

        public ActionResult faqs()
        {
            return View();
        }
        /// <summary>
        /// Download video player
        /// </summary>
        /// <returns></returns>
        [RequireHttps]
        public FileStreamResult StreamFileFromDisk()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "Content/download/";
            //get the filename of the airplayer to be downloaded depending on the region setting
            var region = portalClient.getRegion().Where(r => r.WebsiteUrl == Request.Url.Host.Replace("portal", "www") || (Request.Url.Host == "localhost" && r.RegionName ==defaultRegion)).First();
            string filename = region.AirPlayerFileName;
            return File(new System.IO.FileStream(path + filename, System.IO.FileMode.Open), "application/vnd.adobe.air-application-installer-package+zip", filename);
        }

       
    }
}

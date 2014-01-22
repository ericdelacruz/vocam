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
        //
        // GET: /user/
        private static string BILLING_AGREEMENT_FORMAT = "You will be bill every {0} month/s";

        public ActionResult Index()
        {
            string username = Session["Username"] != null? Session["Username"].ToString():null;
            if (username == null)
                return RedirectToAction("index", "home");
            else
            {
                //if (Session["CustomerData"] == null)
                //{
                    var customer = getCustomerData(username);
                    
                    
                    //First time user 
                    if (customer.Count() == 0)
                    {

                        return RedirectToAction("indexpurchase");
                    }
                    else
                    {

                        if (Request.Url.Host != "localhost" && Request.Url.Host != customer.First().rejoin.WebsiteUrl.Replace("www", "portal"))
                        {
                            
                            return Redirect("http://" + customer.First().rejoin.WebsiteUrl.Replace("www", "portal") +Url.Action("Index","user"));
                        }

                        Session.Add("CustomerData", customer);
                        foreach(var cust in customer)
                        {
                            if (getRemainingDays(cust.customer) == 0)
                            {
                                if(isPayPalRecurActive(cust.paypal.ECTransID))
                                {
                                    switch(cust.customer.RecurringType)
                                    {
                                        case 1: cust.customer.DateSubscriptionEnd = cust.customer.DateSubscriptionEnd.Value.AddMonths(1);
                                            break;
                                        case 2: cust.customer.DateSubscriptionEnd = cust.customer.DateSubscriptionEnd.Value.AddMonths(3);
                                            break;
                                        case 3: cust.customer.DateSubscriptionEnd = cust.customer.DateSubscriptionEnd.Value.AddMonths(6);
                                            break;
                                    }
                                    portalClient.updateCustomer(cust.customer);
                                }
                                
                               
                            }          
                        }
                        if (Session["SalesCode"] == null)
                        {
                           
                                var verifyModel = getDefaultVerifyViewModel();
                                Session.Add("SalesCode", verifyModel.First());
                                
                            
                        }
                        //else
                        //{
                            
                        //    TempData["DefaultSalesCode"] = null;
                        //}
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
      
        [HttpPost]
        public ActionResult index(FormCollection collection)
        {
            
            if(collection["subscription"] == null)
            {
                ModelState.AddModelError("", "Please select subscrption");
                return View();
            }
            int qty = 0;
            if (!int.TryParse(collection["quantity"], out qty))
            {
                var customer = getCustomerData(Session["Username"].ToString());
                //ModelState.AddModelError("", "Invalid Quantity");
                TempData["errorQuantity"] = "Invalid Quantity";
                //if ((Session["SalesCode"] as ViewModel.VerifyModel).salescode.Sales_Code == getDefaultVerifyViewModel().First().salescode.Sales_Code)
                //{
                //    TempData["DefaultSalesCode"] = true;
                //}
                //else
                //{
                //    TempData["DefaultSalesCode"] = null;
                //}

                return View(customer);
            }

            try
            {

                TimeZoneInfo info = DateHelper.getTimeZoneInto(collection["tz_info"]);

                Session.Add("ClientDateTime", DateHelper.UTCtoLocal(DateTime.UtcNow, collection["tz_info"]));
            }
            catch (System.Security.SecurityException)
            {
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
        private bool isPayPalRecurActive(string ECTRans)
        {
            var response = paypalClient.getRecurProfileDetailsByTransID(ECTRans);

            return response.profileStatus.Value == SODAPayPalSerRef.RecurringPaymentsProfileStatusType.ACTIVEPROFILE;
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
            var region = portalClient.getRegion().Where(r => r.WebsiteUrl == requestContext.HttpContext.Request.Url.Host.Replace("portal", "www") || (requestContext.HttpContext.Request.Url.Host == "localhost" && r.RegionName == "AU")).FirstOrDefault();
            if (region != null)
            {
                paypalClient.initPayPalAccountSettings(region.Id);
            }
            return base.BeginExecute(requestContext, callback, state);
        }
        
        private bool isPayPalRecurActive(long userid)
        {

            var response = paypalClient.getRecurProfileDetails(userid);
           
            return response.profileStatus.Value == SODAPayPalSerRef.RecurringPaymentsProfileStatusType.ACTIVEPROFILE;
        }

        private IEnumerable<ViewModel.CustomerModel> getCustomerData(string username)
        {
            var customer = from cust in portalClient.getCustomer()
                           join user in AccountClient.getAccount(username) on cust.UserId equals user.Id
                           join salescode in portalClient.getSaleCode() on cust.SalesCodeId.Value equals salescode.Id
                           join sp in portalClient.getSalePerson() on cust.SalesCodeId equals sp.SalesCodeId
                           join p in portalClient.getPrice() on sp.RegionId equals p.RegionId
                           join PPT in paypalClient.getPayPalTrans() on cust.PPId equals PPT.Id
                           join r in portalClient.getRegion() on p.RegionId equals r.Id
                           join contract in portalClient.getCustomerContract() on user.Id equals contract.UserId
                           where PPT.Active == true
                           select new ViewModel.CustomerModel() { account = user, customer = cust, salesCode = salescode, salesPerson = sp, price = p, paypal = PPT, rejoin = r,contract = contract };
           
            var default_customer = from cust in portalClient.getCustomer()
                                   join user in AccountClient.getAccount(username) on cust.UserId equals user.Id
                                   join salescode in portalClient.getSaleCode() on cust.SalesCodeId.Value equals salescode.Id

                                   join r in portalClient.getRegion() on salescode.Id equals r.DefaultSalesCodeId

                                   join PPT in paypalClient.getPayPalTrans() on cust.PPId equals PPT.Id
                                   join p in portalClient.getPrice() on r.Id equals p.RegionId
                                   join contract in portalClient.getCustomerContract() on user.Id equals contract.UserId
                                   where PPT.Active == true
                                   select new ViewModel.CustomerModel() { account = user, customer = cust, salesCode = salescode, paypal = PPT, rejoin = r, price = p, contract = contract };

            return customer.Count() > 0 && default_customer.Count() > 0 ? customer.Union(default_customer) :
                customer.Count() > 0 && default_customer.Count() == 0 ? customer : default_customer;
           
        }
        public ActionResult indexpurchase(string salescode)
        {
            //if (Session["Username"] == null)
            //    return RedirectToAction("index", "home");

            if (Session["SalesCode"] == null)
            {
                Session.Add("SalesCode", getDefaultVerifyViewModel().First());
                TempData["DefaultSalesCode"] = true;
            }
            else if((Session["SalesCode"] as ViewModel.VerifyModel).salescode.Sales_Code == getDefaultVerifyViewModel().First().salescode.Sales_Code)
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
            
            return Session["NewAccount"] != null? RedirectToAction("termsinit"):RedirectToAction("purchasedetails","home");
        }
        private IEnumerable<ViewModel.VerifyModel> getDefaultVerifyViewModel()
        {
            int RegionId = getRegionId();
            var salescodeList = from r in portalClient.getRegion()
                            join sc in portalClient.getSaleCode() on r.DefaultSalesCodeId equals sc.Id
                            join p in portalClient.getPrice() on r.Id equals p.RegionId
                            where r.Id == RegionId
                            select new ViewModel.VerifyModel()
                            {
                                price = p,
                                salescode = sc,
                                region = r,
                                discountedPrice_A = p.PriceAmt - (p.PriceAmt * sc.Discount),
                                discountedPrice_B = p.PriceAmt_B - (p.PriceAmt_B * sc.Discount),
                                discountedPrice_C = p.priceAmt_C - (p.priceAmt_C * sc.Discount),
                                 isDefaultSalesCode = true
                            };
            
            //ViewBag.DefaultSalesCode = true;
            return salescodeList;
        }

        private int getRegionId()
        {
            //int RegionId = portalClient.getRegion().Where(r => r.WebsiteUrl == Request.Url.Host.Replace("portal", "www")).First().Id;
            return portalClient.getRegion().Where(r => r.WebsiteUrl == Request.Url.Host.Replace("portal", "www")).Count() > 0? portalClient.getRegion().Where(r => r.WebsiteUrl == Request.Url.Host.Replace("portal", "www")).First().Id:12;
        }
       
        [HttpPost]
        public ActionResult reverify(FormCollection collection)
        {
            
            
            string salescode = collection["SalesCode"];
            var salescodeList = getVerifyViewModel(salescode);

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
        [HttpPost]
        public ActionResult verify(FormCollection collection)
        {
            string salescode = collection["SalesCode"];
            var salescodeList = getVerifyViewModel(salescode);

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
            return RedirectToAction("indexpurchase");
        }

        private IEnumerable<ViewModel.VerifyModel> getVerifyViewModel(string salescode)
        {
            var websiteURL = Request.Url.Host.Replace("portal", "www") != "localhost" ? Request.Url.Host.Replace("portal", "www") : "www.safetyondemand.com.au";
            //get SalesCode Details. Notes SalesCode depends on region.
            var salescodeList = from sp in portalClient.getSalePerson()
                                join sc in portalClient.getSaleCode() on sp.SalesCodeId equals sc.Id
                                join p in portalClient.getPrice() on sp.RegionId equals p.RegionId
                                join r in portalClient.getRegion() on sp.RegionId equals r.Id
                                where sc.Sales_Code == salescode.Trim() &&  r.WebsiteUrl == websiteURL
                                select new ViewModel.VerifyModel() { price = p, saleperson = sp, salescode = sc, region = r,
                                discountedPrice_A = p.PriceAmt - (p.PriceAmt * sc.Discount),
                                discountedPrice_B = p.PriceAmt_B - (p.PriceAmt_B * sc.Discount),
                                discountedPrice_C = p.priceAmt_C - (p.priceAmt_C * sc.Discount),
                                 isDefaultSalesCode = false
                                };
           
            return salescodeList;
            
        }

        public ActionResult termsinit()
        {
            if (Session["Username"] == null)
                return RedirectToAction("index", "home");
            else
            {
                
                var account =Session["NewAccount"] == null? AccountClient.getAccount(Session["Username"].ToString()).Select(a => a).First():Session["NewAccount"] as AccountServiceRef.Account;

                    return View(account);
                
            }
            
        }
        [HttpPost]
        public ActionResult termsinit(FormCollection collection)
        {
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
        public ActionResult paymentstatus(string stat)
        {
            ViewBag.paymentSuccess = (Session["Username"] != null || Session["NewAccount"] != null) && stat == "success";
            if (stat != "success")
                Session.Remove("CustomerData");
            else//Success
            {
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
      
        public ActionResult profile()
        {
            if (Session["Username"] == null)
                return RedirectToAction("index", "home");
            var cust = Session["CustomerData"] as IEnumerable<ViewModel.CustomerModel>;
            return View(cust.OrderByDescending(c=>c.customer.DateSubscriptionEnd).First());
        }
        
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

        public ActionResult checkout()
        {
            //ViewModel.CustomerModel customer = new ViewModel.CustomerModel();
            if (Session["Username"] != null && Session["SalesCode"] != null)
            {
                

                //var salesPersonCodePrice = Session["SalesCode"] as ViewModel.VerifyModel;
                string redirectURL = paypalClient.checkoutModel(initCheckoutModel());
                //string redirectURL = PaypalHelper.checkout(price, Moolah.PayPal.CurrencyCodeType.AUD, itemname, itemDesc, itemURL, cancelURl, confirmURL);

                return Redirect(redirectURL);

            }
            else
                return RedirectToAction("index");
             
        }

        private SODAPayPalSerRef.PayPalCheckOutModel initCheckoutModel()
        {
            var VerifyModel = Session["SalesCode"] as ViewModel.VerifyModel;
            SODAPayPalSerRef.PayPalCheckOutModel ppc = new SODAPayPalSerRef.PayPalCheckOutModel();
            ppc.cancelurl = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("cancel"); ;
            ppc.confirmUrl = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("confirm");
            ppc.CType = (SODAPayPalSerRef.CurrencyCodeType)Enum.Parse(typeof(SODAPayPalSerRef.CurrencyCodeType), VerifyModel.region.Currency);
            ppc.Quantity = VerifyModel.qty;
            ppc.OrderDesc = "Order Description here.";
            
            switch(VerifyModel.selectedSubscription)
            {
                case 1:
                    ppc.ItemAmt = string.Format("{0:0.00}", VerifyModel.discountedPrice_A);
                    ppc.itemDesc = "SODA Monthly Recurring Subscription";
                    ppc.itemName = "Monthly Recurring Subscription";
                    ppc.itemTotalamt = Convert.ToDouble(VerifyModel.discountedPrice_A * ppc.Quantity);
                    ppc.orderTotalamt = ppc.itemTotalamt;
                    ppc.BillingAgreement = string.Format(BILLING_AGREEMENT_FORMAT, 1);
                   
                    break;
                case 2:
                     ppc.ItemAmt = string.Format("{0:0.00}",VerifyModel.discountedPrice_B);
                    ppc.itemDesc = "SODA 3 Months Recurring Subscription";
                    ppc.itemName = "3 Months Recurring Subscription";
                    ppc.itemTotalamt = Convert.ToDouble(VerifyModel.discountedPrice_B * ppc.Quantity);
                    ppc.orderTotalamt = ppc.itemTotalamt;
                    ppc.BillingAgreement = string.Format(BILLING_AGREEMENT_FORMAT, 3);
                    break;
                case 3: ppc.ItemAmt = string.Format("{0:0.00}",VerifyModel.discountedPrice_C);
                    ppc.itemDesc = "SODA 6 Months Recurring Subscription";
                    ppc.itemName = "6 Months Recurring Subscription";
                    ppc.itemTotalamt = Convert.ToDouble(VerifyModel.discountedPrice_C * ppc.Quantity);
                    ppc.orderTotalamt = ppc.itemTotalamt;
                    ppc.BillingAgreement = string.Format(BILLING_AGREEMENT_FORMAT, 6);
                    break;
                default:
                    throw new Exception("Invalid subscription type");
            }
            return ppc;
        }

        public ActionResult cancel()
        {
            return RedirectToAction("paymentstatus", new { stat = "cancel" }) ;
        }
        public ActionResult confirm(string token, string payerid)
        {
            
           
            var VerfiyModel = Session["SalesCode"] as ViewModel.VerifyModel;
            if(Session["NewAccount"] != null)
            {
                var new_accnt = Session["NewAccount"] as AccountServiceRef.Account;
                AccountClient.addAccount(new_accnt);

               
            }
             var accnt = from account in AccountClient.getAccount(Session["Username"].ToString())
                        select account;
                
            string result = paypalClient.confirmationModel(initConfirmModel(VerfiyModel, accnt.First().Id, token, payerid));
                portalClient.addCustomer(new PortalServiceReference.Customer()
                {
                    UserId = accnt.First().Id,
                    DatePurchase = new Nullable<DateTime>(DateTime.Now),
                    //DateSubscriptionEnd = new Nullable<DateTime>(DateTime.Now.AddMonths(6)),
                    DateSubscriptionEnd = getDateSubscriptionEnd(VerfiyModel.selectedSubscription,VerfiyModel.price.FirstMonthFree),
                    SalesCodeId = VerfiyModel.salescode.Id,
                     DateUpdated = DateTime.Now,
                     Licenses = VerfiyModel.qty,
                     RecurringType = Convert.ToInt16(VerfiyModel.selectedSubscription),
                     PPId =  paypalClient.getPayPalTrans().Select(p=>p).Where(p=>p.ECTransID == result.Split(';')[0]).First().Id
                });
                if (Session["NewAccount"] != null)
                portalClient.addCustomerContract(new PortalServiceReference.CustomerContract()
                {
                    //DateEnd = DateTime.Now.AddMonths(6),
                    DateEnd = (Session["ClientDateTime"] as Nullable<DateTime>).Value.AddMonths(6),
                    //DateStart = DateTime.Now,
                    DateStart = (Session["ClientDateTime"] as Nullable<DateTime>).Value,
                    UserId = accnt.First().Id
                });

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
                case 1:
                    ppConfirm.dateStart = VerfiyModel.price.FirstMonthFree ? DateTime.Now.AddMonths(2) : DateTime.Now.AddMonths(1);
                    ppConfirm.PaymenytAmt = VerfiyModel.discountedPrice_A * VerfiyModel.qty;
                    ppConfirm.schedDesc = string.Format(BILLING_AGREEMENT_FORMAT, 1);
                    ppConfirm.BillingFrequency = 1;
                    break;
                case 2:
                    ppConfirm.dateStart = VerfiyModel.price.FirstMonthFree ? DateTime.Now.AddMonths(4) : DateTime.Now.AddMonths(3);
                    ppConfirm.PaymenytAmt = VerfiyModel.discountedPrice_B * VerfiyModel.qty;
                    ppConfirm.schedDesc = string.Format(BILLING_AGREEMENT_FORMAT, 3);
                    ppConfirm.BillingFrequency = 3;
                    break;
                case 3:
                     ppConfirm.dateStart = VerfiyModel.price.FirstMonthFree ? DateTime.Now.AddMonths(7) : DateTime.Now.AddMonths(6);
                    ppConfirm.PaymenytAmt = VerfiyModel.discountedPrice_C * VerfiyModel.qty;
                    ppConfirm.schedDesc = string.Format(BILLING_AGREEMENT_FORMAT, 6);
                    ppConfirm.BillingFrequency = 6;
                    break;


            }
            return ppConfirm;
        }
        public ActionResult unsubscribe(string transid)
        {
            
             if (Session["CustomerData"] != null)
             {
                 var customerViewModel = (Session["CustomerData"] as IEnumerable<ViewModel.CustomerModel>).Where(c=> c.paypal.ECTransID == transid);

                 var customRecord = customerViewModel.First().customer;
                 customRecord.DateSubscriptionEnd = DateTime.Now;

                 if (paypalClient.cancelSubscription(transid))
                     portalClient.updateCustomer(customRecord);

                 return RedirectToAction("index");
                 
             }
             else
                 return RedirectToAction("index");
        }
        public ActionResult downloads()
        {
            if (Session["Username"] == null)
                return RedirectToAction("index", "home");
            else
            return View();
        }

        public ActionResult termsconditions()
        {
            return View();
        }

        public ActionResult faqs()
        {
            return View();
        }


        public FileStreamResult StreamFileFromDisk()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "Content/download/";
            var region = portalClient.getRegion().Where(r => r.WebsiteUrl == Request.Url.Host.Replace("portal", "www") || (Request.Url.Host == "localhost" && r.RegionName.ToLower()=="au")).First();
            string filename = region.AirPlayerFileName;
            return File(new System.IO.FileStream(path + filename, System.IO.FileMode.Open), "application/vnd.adobe.air-application-installer-package+zip", filename);
        }

       
    }
}

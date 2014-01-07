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

                        if (Request.Url.Host != customer.First().rejoin.WebsiteUrl.Replace("www", "portal"))
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
                                
                                //if(isPayPalRecurActive(customer.First().account.Id))
                                //{
                                //    customer.First().customer.DateSubscriptionEnd = customer.First().customer.DateSubscriptionEnd.Value.AddMonths(1);
                                //    portalClient.updateCustomer(customer.First().customer);
                                //}
                            }          
                        }
                        if (Session["SalesCode"] == null)
                        {
                            var verifyModel = getVerifyViewModel(customer.Last().salesCode.Sales_Code);
                            if (verifyModel.Count() > 0)
                                Session.Add("SalesCode", verifyModel.First());
                            else //set to default sales code
                            {
                                verifyModel = getDefaultVerifyViewModel();
                                Session.Add("SalesCode", verifyModel.First());
                            }
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
        //Verify SC
        [HttpPost]
        public ActionResult index(FormCollection collection)
        {
            
            if(collection["subscription"] == null)
            {
                ModelState.AddModelError("", "Please select subscrption");
                return View();
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
                           where PPT.Active == true
                           select new ViewModel.CustomerModel() { account = user, customer = cust, salesCode = salescode, salesPerson = sp, price = p, paypal = PPT, rejoin = r };
            if (customer.Count() > 0)
            {
                if (TempData["DefaultSalesCode"] != null)
                    TempData.Remove("DefaultSalesCode");
                return customer;
            }//else default
            else
            {
                TempData["DefaultSalesCode"] = true;
                //int RegionId = portalClient.getRegion().Where(r => r.WebsiteUrl == Request.Url.Host.Replace("portal","www")).First().Id;
                customer = from cust in portalClient.getCustomer()
                           join user in AccountClient.getAccount(username) on cust.UserId equals user.Id
                           join salescode in portalClient.getSaleCode() on cust.SalesCodeId.Value equals salescode.Id

                           join r in portalClient.getRegion() on salescode.Id equals r.DefaultSalesCodeId

                           join PPT in paypalClient.getPayPalTrans() on cust.PPId equals PPT.Id
                           join p in portalClient.getPrice() on r.Id equals p.RegionId
                           where PPT.Active == true
                           select new ViewModel.CustomerModel() { account = user, customer = cust, salesCode = salescode, paypal = PPT, rejoin = r, price = p };
                return customer;

            }
        }
        public ActionResult indexpurchase(string salescode)
        {
            if (Session["Username"] == null)
                return RedirectToAction("index", "home");

           
                
            //var salescodeList =salescode != null? getVerifyViewModel(salescode): getDefaultVerifyViewModel();
            

            //if (salescodeList.Count() > 0)
            //{

            //    Session.Add("SalesCode", salescodeList.First());
            //}
            //else
            //    Session["SalesCode"] = null;

            if (Session["SalesCode"] == null)
                Session.Add("SalesCode", getDefaultVerifyViewModel().First());
            return View();
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
                            };
            TempData["DefaultSalesCode"] = true;
            return salescodeList;
        }

        private int getRegionId()
        {
            //int RegionId = portalClient.getRegion().Where(r => r.WebsiteUrl == Request.Url.Host.Replace("portal", "www")).First().Id;
            return portalClient.getRegion().Where(r => r.WebsiteUrl == Request.Url.Host.Replace("portal", "www")).Count() > 0? portalClient.getRegion().Where(r => r.WebsiteUrl == Request.Url.Host.Replace("portal", "www")).First().Id:12;
        }
        [HttpPost]
        public ActionResult indexpurchase(FormCollection collection)
        {
            
            if(collection["subscription"] == null)
            {
                return RedirectToAction("indexpurchase");
            }
            
            var VerifyModel = Session["SalesCode"] as ViewModel.VerifyModel;
            VerifyModel.selectedSubscription = int.Parse(collection["subscription"]);
            VerifyModel.qty = !string.IsNullOrEmpty(collection["quantity"])?int.Parse(collection["quantity"]):1;
            Session["SalesCode"] = VerifyModel;
            return RedirectToAction("termsinit");
        }
        [HttpPost]
        public ActionResult reverify(FormCollection collection)
        {
            //var verifymodel = getVerifyViewModel(salescode);
            //TempData["VerifiedSC"] = false;
            //if (verifymodel.Count() > 0)
            //{
            //    TempData["VerifiedSC"] = true;
            //    Session.Add("SalesCode", verifymodel.First());
            //}
           
            //return RedirectToAction("index");
            
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
            var salescodeList = from sp in portalClient.getSalePerson()
                                join sc in portalClient.getSaleCode() on sp.SalesCodeId equals sc.Id
                                join p in portalClient.getPrice() on sp.RegionId equals p.RegionId
                                join r in portalClient.getRegion() on sp.RegionId equals r.Id
                                where sc.Sales_Code == salescode.Trim()
                                select new ViewModel.VerifyModel() { price = p, saleperson = sp, salescode = sc, region = r,
                                discountedPrice_A = p.PriceAmt - (p.PriceAmt * sc.Discount),
                                discountedPrice_B = p.PriceAmt_B - (p.PriceAmt_B * sc.Discount),
                                discountedPrice_C = p.priceAmt_C - (p.priceAmt_C * sc.Discount),
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

        public ActionResult paymentstatus(string stat)
        {
            ViewBag.paymentSuccess = (Session["Username"] != null || Session["NewAccount"] != null) && stat == "success";
            if (stat != "success" && Session["CustomerData"] != null)
                Session.Remove("CustomerData");
            else//Success
            {
                if(Session["NewAccount"] != null)
                {
                    var new_accnt = Session["NewAccount"] as AccountServiceRef.Account;
                    
                   
                    emailCustomer(new_accnt);
                    Session.Remove("NewAccount");
                    //Session.Add("Username", new_accnt.USERNAME);
                }
            }
            return View();
        }

        private void emailCustomer(AccountServiceRef.Account account)
        {
            var CustomerName = account.FirstName + " " + account.LastName;
            ViewData.Add("CustomerName", CustomerName);
            
            string body = EmailHelper.ToHtml("emailaccount", ViewData, this.ControllerContext);
            EmailHelper.SendEmail("test@sac-iis.com", Session["Username"].ToString(), "Account Details", body);
        }
      
        public ActionResult profile()
        {
            if (Session["Username"] == null)
                return RedirectToAction("index", "home");
            var cust = Session["CustomerData"] as IEnumerable<ViewModel.CustomerModel>;
            return View(cust.Last());
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

                if (customer.Count() > 0)
                    ViewBag.ContractEndDate = customer.First().DateSubscriptionEnd;

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
                

                var salesPersonCodePrice = Session["SalesCode"] as ViewModel.VerifyModel;
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
            return View();
        }

        public ActionResult termsconditions()
        {
            return View();
        }


        public FileStreamResult StreamFileFromDisk()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "Content/download/";
            string filename = "Sample Pictures.zip";
            return File(new System.IO.FileStream(path + filename, System.IO.FileMode.Open), "application/zip","TestFileDownload");
        }

       
    }
}

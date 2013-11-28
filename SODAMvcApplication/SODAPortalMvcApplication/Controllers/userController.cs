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

        public ActionResult Index()
        {
            string username = Session["Username"] != null? Session["Username"].ToString():null;
            if (username == null)
                return RedirectToAction("index", "home");
            else
            {
                if (Session["CustomerData"] == null)
                {
                    var customer = getCustomerData(username);

                    //if (customer.First().customer.DatePurchase != null || customer.First().customer.DatePurchase.Value.Year == 1901)//default value from dataset
                    //First time user 
                    if (customer.Count() == 0)
                    {

                        return RedirectToAction("indexpurchase");
                    }
                    else
                    {


                        Session.Add("CustomerData", customer.First());
                        if(customer.First().customer.DateSubscriptionEnd.Value.Month >=  DateTime.Now.Month)
                        {
                            if(isPayPalRecurActive(customer.First().account.Id))
                            {
                                customer.First().customer.DateSubscriptionEnd = customer.First().customer.DateSubscriptionEnd.Value.AddMonths(1);
                                portalClient.updateCustomer(customer.First().customer);
                            }
                        }
                        return View(customer.First());
                    }
                }
                else
                {
                    var customer = getCustomerData(username);
                    Session["CustomerData"] = customer.First();
                    return View(customer.First());
                }
            }
           
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
                           join salescode in portalClient.getSaleCode() on cust.SalesCodeId equals salescode.Id
                           join sp in portalClient.getSalePerson() on cust.SalesCodeId equals sp.SalesCodeId
                           join p in portalClient.getPrice() on sp.RegionId equals p.RegionId
                           select new ViewModel.CustomerModel() { account = user, customer = cust, salesCode = salescode, salesPerson = sp, price = p };
            return customer;
        }
        public ActionResult indexpurchase()
        {
            
            return View();
        }

        public ActionResult reverify(string salescode)
        {
            var verifymodel = getVerifyViewModel(salescode);
            TempData["VerifiedSC"] = false;
            if (verifymodel.Count() > 0)
            {
                TempData["VerifiedSC"] = true;
                Session.Add("SalesCode", verifymodel.First());
            }
           
            return RedirectToAction("index");
        }
        [HttpPost]
        public ActionResult verify(FormCollection collection)
        {
            string salescode = collection["SalesCode"];
            var salescodeList = getVerifyViewModel(salescode);

            if (salescodeList.Count() > 0)
                Session.Add("SalesCode",salescodeList.First());
            else
                Session["SalesCode"] = null;

            return RedirectToAction("indexpurchase");
        }

        private IEnumerable<ViewModel.VerifyModel> getVerifyViewModel(string salescode)
        {
            var salescodeList = from sp in portalClient.getSalePerson()
                                join sc in portalClient.getSaleCode() on sp.SalesCodeId equals sc.Id
                                join p in portalClient.getPrice() on sp.RegionId equals p.RegionId
                                where sc.Sales_Code == salescode.Trim()
                                select new ViewModel.VerifyModel() { price = p, saleperson = sp, salescode = sc };
            return salescodeList;
        }

        public ActionResult termsinit()
        {
            
            return View();
        }

        public ActionResult paymentstatus(string stat)
        {
            ViewBag.paymentSuccess = Session["Username"] != null && stat == "success";
           
            return View();
        }
      
        public ActionResult profile()
        {
            ViewModel.CustomerModel cust = Session["CustomerData"] as ViewModel.CustomerModel;
            return View(cust);
        }
        
        public ActionResult editProfile()
        {
            if (Session["Username"] != null)
            {
                var userAcnt = from accnt in AccountClient.getAccount(Session["Username"].ToString())
                               select new ViewModel.UserModel() { Email = accnt.Email, Company = accnt.Company, Password = accnt.PASSWORD, Contact = accnt.ContactNo, FirtName = accnt.FirstName, LastName = accnt.LastName };

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

                AccountClient.updateAccount(accnt);
                Session["CustomerData"] = getCustomerData(user.Email).First();
                return RedirectToAction("profile");
            }
                return View(user);

        }

        public ActionResult checkout()
        {
            ViewModel.CustomerModel customer = new ViewModel.CustomerModel();
            if (Session["Username"] != null && Session["SalesCode"] != null)
            {
                var accnt = from account in AccountClient.getAccount(Session["Username"].ToString())
                            select account;
                var salesPersonCodePrice = Session["SalesCode"] as ViewModel.VerifyModel;

                customer.account = accnt.First();
                customer.salesPerson = salesPersonCodePrice.saleperson;
                customer.salesCode = salesPersonCodePrice.salescode;
                customer.price = salesPersonCodePrice.price;

                Session.Add("CustomerData", customer);

                string itemname = "SODA Subscription";
                string itemDesc = string.Concat("Soda Subscription payment for ", customer.account.FirstName + " " + customer.account.LastName);
                string itemURL = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("checkout");
                string cancelURl = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("cancel");
                string confirmURL = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("confirm");
                decimal price = customer.price.FirstMonthFree ? customer.price.PriceAmt * 5 : customer.price.PriceAmt * 6;

                if ((customer.salesCode.Discount * 100) > 0)
                {
                    price = price - (customer.salesCode.Discount * 100);
                }

                string redirectURL = paypalClient.checkout(price, SODAPayPalSerRef.CurrencyCodeType.AUD, itemname, itemDesc, itemURL, cancelURl, confirmURL);
                //string redirectURL = PaypalHelper.checkout(price, Moolah.PayPal.CurrencyCodeType.AUD, itemname, itemDesc, itemURL, cancelURl, confirmURL);
                return Redirect(redirectURL);

            }
            else
                return RedirectToAction("index");
             
        }

        public ActionResult cancel()
        {
            return RedirectToAction("paymentstatus", new { stat = "cancel" }) ;
        }
        public ActionResult confirm(string token, string payerid)
        {
            ViewModel.CustomerModel customer = new ViewModel.CustomerModel();
            if (Session["CustomerData"] != null)
            {
                customer = Session["CustomerData"] as ViewModel.CustomerModel;

                

                string itemname = "SODA Subscription";
                string itemDesc = string.Concat("Soda Subscription payment for ", customer.account.FirstName + " " + customer.account.LastName);
                string itemURL = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("checkout");
                decimal price = customer.price.FirstMonthFree ? customer.price.PriceAmt * 5 : customer.price.PriceAmt * 6;

                if ((customer.salesCode.Discount * 100) > 0)
                {
                    price = price - (customer.salesCode.Discount * 100);
                }

                paypalClient.confirmation(customer.account.Id, payerid, token, price, customer.price.PriceAmt, SODAPayPalSerRef.CurrencyCodeType.AUD, itemname, itemDesc, DateTime.Now.AddMonths(6));
                
                portalClient.addCustomer(new PortalServiceReference.Customer()
                {
                    UserId = customer.account.Id,
                    DatePurchase = new Nullable<DateTime>(DateTime.Now),
                    DateSubscriptionEnd = new Nullable<DateTime>(DateTime.Now.AddMonths(6)),
                    SalesCodeId = customer.salesCode.Id
                });
                return RedirectToAction("paymentstatus",new{stat="success"});
            }
            else
                return RedirectToAction("index");
        }
        public ActionResult unsubscribe()
        {
             ViewModel.CustomerModel customer = new ViewModel.CustomerModel();
             if (Session["CustomerData"] != null)
             {
                 customer = Session["CustomerData"] as ViewModel.CustomerModel;
                 
                 customer.customer.DateSubscriptionEnd = DateTime.Now;

                 if (paypalClient.cancelSubscription(customer.account.Id))
                     portalClient.updateCustomer(customer.customer);

                 return RedirectToAction("index");
                 
             }
             else
                 return RedirectToAction("index");
        }
    }
}

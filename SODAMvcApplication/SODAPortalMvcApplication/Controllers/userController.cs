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

                        return View(customer.First());
                    }
                }
                else
                {
                    
                    return View(Session["CustomerData"]);
                }
            }
           
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
        [HttpPost]
        public ActionResult verify(FormCollection collection)
        {
            string salescode = collection["SalesCode"];
            var salescodeList = from sp in portalClient.getSalePerson()
                                join sc in portalClient.getSaleCode() on sp.SalesCodeId equals sc.Id
                                join p in portalClient.getPrice() on sp.RegionId equals p.RegionId
                                where sc.Sales_Code == salescode.Trim()
                                select new ViewModel.VerifyModel() { price = p, saleperson = sp, salescode = sc };

            if (salescodeList.Count() > 0)
                Session.Add("SalesCode",salescodeList.First());
            else
                Session["SalesCode"] = null;

            return RedirectToAction("indexpurchase");
        }

        public ActionResult termsinit()
        {
            
            return View();
        }

        public ActionResult paymentstatus(string stat)
        {
            if((ViewBag.paymentSuccess =Session["Username"] != null && stat == "success"))
            {
                var user = from acnt in AccountClient.getAccount(Session["Username"].ToString())
                           select acnt;
                long SalesCode = (Session["SalesCode"] as ViewModel.VerifyModel).salescode.Id;

                portalClient.addCustomer(new PortalServiceReference.Customer()
                {
                    UserId = user.First().Id,
                     DatePurchase = new Nullable<DateTime>(DateTime.Now),
                      DateSubscriptionEnd = new Nullable<DateTime>(DateTime.Now.AddMonths(6)),
                       SalesCodeId = SalesCode
                });
            }
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
    }
}

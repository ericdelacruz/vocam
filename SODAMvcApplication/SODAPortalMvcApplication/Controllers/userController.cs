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
            if (username != null)
                return RedirectToAction("index", "home");
            else
            {
                var customer = getCustomerData(username);

                //if (customer.First().customer.DatePurchase != null || customer.First().customer.DatePurchase.Value.Year == 1901)//default value from dataset
                 if (customer.Count() == 0)
                {
                    
                    return RedirectToAction("indexpurchase");
                }
                else
                 {
                    TempData["CustomerData"] = customer.First();
                     return View();
                 }
            }
           
        }

        private IEnumerable<ViewModel.CustomerModel> getCustomerData(string username)
        {
            var customer = from cust in portalClient.getCustomer()
                           join user in AccountClient.getAccount("") on cust.UserId equals user.Id
                           join salescode in portalClient.getSaleCode() on cust.SalesCodeId equals salescode.Id
                           where username == user.USERNAME
                           select new ViewModel.CustomerModel() { account = user, customer = cust, salesCode = salescode };
            return customer;
        }
        public ActionResult indexpurchase()
        {
            
            return View();
        }

        public ActionResult verify(string salescode)
        {
            var salescodeList = from sp in portalClient.getSalePerson()
                                join sc in portalClient.getSaleCode() on sp.SalesCodeId equals sc.Id
                                join p in portalClient.getPrice() on sp.RegionId equals p.RegionId
                                where sc.Sales_Code == salescode.Trim()
                                select new ViewModel.VerifyModel() { price = p, saleperson = sp, salescode = sc };

            if (salescodeList.Count() > 0)
                TempData["VerifyData"] = salescodeList.First();
            else
                TempData["VerifyData"] = null;

            return RedirectToAction("indexpurchase");
        }

        public ActionResult termsCondition()
        {
            if(TempData["SaleCodeVerified"] != null && (TempData["SaleCodeVerified"] as Nullable<bool>) == true)
            {
                return View();
            }
            else
            {
                return RedirectToAction("indexpurchase");
            }
        }

        public ActionResult paymentsuccess()
        {
            return View();
        }

        public ActionResult profile()
        {
            ViewModel.CustomerModel cust = TempData["CustomerData"] as ViewModel.CustomerModel;
            return View(cust);
        }
        
        public ActionResult editProfile()
        {
            if (Session["Username"] != null)
            {
                var userAcnt = from accnt in AccountClient.getAccount(Session["Username"].ToString())
                               select new ViewModel.UserModel() { Email = accnt.Email, Company = accnt.Company, Password = accnt.PASSWORD, Contact = accnt.ContactNo, FirtName = accnt.FirstName, LastName = accnt.LastName };


                return View(userAcnt);
            }
            else
            {
                return RedirectToAction("index", "Home");
            }
        }

        [HttpPost]
        public ActionResult editprofile(ViewModel.UserModel user)
        {
             
        }
    }
}

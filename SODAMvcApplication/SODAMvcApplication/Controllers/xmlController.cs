using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SODAMvcApplication.Models.XML;
namespace SODAMvcApplication.Controllers
{
    public class xmlController : Controller
    {

        private PortalServiceReference.PortalServiceClient portalClient = new PortalServiceReference.PortalServiceClient();
        private CategoriesServiceReference.CatListingServiceClient catClient = new CategoriesServiceReference.CatListingServiceClient();
        private AccountServiceReference.AccountServiceClient account = new AccountServiceReference.AccountServiceClient();
        
        protected override void Dispose(bool disposing)
        {
            catClient.Close();
            account.Close();
            portalClient.Close();
            base.Dispose(disposing);
        }
        //
        // GET: /xml/
        public ActionResult Index()
        {
            return View();
        }
         
        public ActionResult validate(string username, string password)
        {
            if(!string.IsNullOrEmpty(username.Trim()) && !string.IsNullOrEmpty(password.Trim()) && account.AuthenticateUser(username,password))
            {
                var customer = from accnt in account.getAccount(username)
                               join cust in portalClient.getCustomer() on accnt.Id equals cust.UserId
                               select cust;
                

                if(customer.Count() > 0)
                {
                    int daysleft = 0;
                    if (customer.First().DateSubscriptionEnd.HasValue)
                        daysleft = ((TimeSpan)(DateTime.Now - customer.First().DateSubscriptionEnd.Value)).Days;
                    return View(new Users() { authorized = true,daysleft=daysleft, shownews = true });
                }
                else
                {
                    return View(new Users() { authorized = false, daysleft = 0, shownews = true });
                }
            }
            else
            {
                return View(new Users() { authorized = false, daysleft = 0, shownews = true });
            }
        }
        public ActionResult channels()
        {
            var channels = catClient.get_Categories();
            return View(channels);
        }
        public ActionResult titles()
        {
            var titles = catClient.get();
            
            return View(titles);
        }
    }
}

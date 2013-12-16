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
                               where cust.DateSubscriptionEnd > DateTime.Now
                               orderby cust.DateSubscriptionEnd descending
                               select cust;
                                
                                        

                if(customer.Count() > 0)
                {
                    int daysleft = 0;
                    int consumed = 0;
                    var maxActiveLicenses = customer.Sum(c => c.Licenses);

                    var LicenseConsumption = portalClient.getLicenseConsumption().Where(lc => lc.UserId == customer.First().UserId);
                    if (LicenseConsumption.Count() > 0)
                        consumed = LicenseConsumption.First().Consumed;
                    if (customer.First().DateSubscriptionEnd.HasValue)
                        daysleft = ((TimeSpan)(customer.First().DateSubscriptionEnd.Value - DateTime.Now)).Days;
                    return View(new Users() { authorized = true,daysleft=daysleft, shownews = true,PCLicenses=maxActiveLicenses, PCLicenseConsumed = consumed });
                }
                else
                {
                    return View(new Users() { authorized = false, daysleft = 0, shownews = true, PCLicenses = 0, PCLicenseConsumed = 0 });
                }
            }
            else
            {
                return View(new Users() { authorized = false, daysleft = 0, shownews = true, PCLicenses = 0, PCLicenseConsumed = 0 });
            }
        }
        public ActionResult channels()
        {
            var channels = catClient.get_Categories();
            return View(channels);
        }

        public ActionResult titles(string id)
        {
            int regionId = id != null? portalClient.getRegion().Where(r => r.RegionName.ToLower() == id.ToLower().Trim()).First().Id:12; //default to 12 AU 
            var titles =  catClient.get().Where(title=> title.RegionId == regionId);
            
            return View(titles);
        }
    }
}

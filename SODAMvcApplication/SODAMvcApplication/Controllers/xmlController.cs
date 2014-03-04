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

        protected override IAsyncResult BeginExecute(System.Web.Routing.RequestContext requestContext, AsyncCallback callback, object state)
        {
            //securty measure. 
            account.Authenticate("myS0D@P@ssw0rd");
            return base.BeginExecute(requestContext, callback, state);
        }
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
        //Validate user. This method is for the videoplayer (air)  
        public ActionResult validate(string username, string password)
        {
            
            if(!string.IsNullOrEmpty(username.Trim()) && !string.IsNullOrEmpty(password.Trim()) && account.AuthenticateUser(username,password))
            {
                var customer = getCustomer(username);
                                
                                        
                
                if(customer.Count() > 0)
                {
                    int daysleft = 0;
                    int consumed = 0;
                    
                    
                    var maxActiveLicenses = customer.Sum(c => c.Licenses);
                   
                    var LicenseConsumption = portalClient.getLicenseConsumption().Where(lc => lc.UserId == customer.First().UserId);
                    //set consumed if LicenseConsumption exists
                    if (LicenseConsumption.Count() > 0)
                        consumed = LicenseConsumption.First().Consumed;
                    //get and set the days remaining 
                    if (customer.First().DateSubscriptionEnd.HasValue)
                        daysleft = ((TimeSpan)(customer.First().DateSubscriptionEnd.Value - DateTime.Now)).Days;
                    return View(new Users() { authorized = true,daysleft=daysleft, shownews = false,PCLicenses=maxActiveLicenses, PCLicenseConsumed = consumed,CompanyWebsite=getWebsiteUrl(customer) });
                }
                else
                {
                    return View(new Users() { authorized = false, daysleft = 0, shownews = false, PCLicenses = 0, PCLicenseConsumed = 0, CompanyWebsite = "" });
                }
            }
            else
            {
                return View(new Users() { authorized = false, daysleft = 0, shownews = false, PCLicenses = 0, PCLicenseConsumed = 0, CompanyWebsite = "" });
            }
        }

        private IEnumerable<PortalServiceReference.Customer> getCustomer(string username)
        {
            var customer = from accnt in account.getAccount(username)
                           join cust in portalClient.getCustomer() on accnt.Id equals cust.UserId

                           where cust.DateSubscriptionEnd > DateTime.Now
                           orderby cust.DateSubscriptionEnd descending
                           select cust;
            return customer;
        }

        private string getWebsiteUrl(IEnumerable<PortalServiceReference.Customer> customer)
        {
            
            //get region setting by salescode
            var nonDefault = from salesCode in portalClient.getSaleCode()
                             join salesPerson in portalClient.getSalePerson() on salesCode.SalesPersonID equals salesPerson.Id
                             join region in portalClient.getRegion() on salesPerson.RegionId equals region.Id
                             where customer.First().SalesCodeId == salesCode.Id
                             select region;
            //if the customer used a salescode, else use the default salescode as a basis for getting the region 
            if (nonDefault.Count() > 0)
            {
                return "http://" + nonDefault.First().WebsiteUrl;
            }
            else
            {
                var Default = from salesCode in portalClient.getSaleCode()
                              join region in portalClient.getRegion() on salesCode.Id equals region.DefaultSalesCodeId
                              where customer.First().SalesCodeId == salesCode.Id 
                              select region;
                return "http://" + Default.First().WebsiteUrl;
            }
        }
        
        
        public ActionResult channels()
        {
            var channels = catClient.get_Categories().Where(c=> c.CategoryId != 1);
            return View(channels);
        }
        //id is RegionName
        public ActionResult titles(string id)
        {
            int regionId = id != null? portalClient.getRegion().Where(r => r.RegionName.ToLower() == id.ToLower().Trim()).First().Id:12; //default to 12 AU-for testing 
            //get title by region name
            var titles = catClient.get().Where(title => title.RegionId == regionId).OrderBy(title => title.Title);
            
            return View(titles);
        }
        //Id is the region name
        public ActionResult news(string id,string username)
        {
          var _account = account.getAccount(username);
          var region = portalClient.getRegion().Where(r => r.RegionName.ToLower() == id.ToLower().Trim());
          
            if(_account.Count() > 0 && region.Count() > 0)
            {
                var customer = getCustomer(username);
                ViewBag.isExistsUser = true;
                ViewBag.WebsiteURL =  getWebsiteUrl(customer);
            }
            else
            {
                ViewBag.isExistsUser = null;
            }

            return View();
        }
    }
}

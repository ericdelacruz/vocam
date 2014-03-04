using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SODAMvcApplication.Controllers
{
    public class drmController : Controller
    {
        //
        // GET: /drm/

        public ActionResult Index()
        {
            return View();
        }
        //
        //Get:/drm/authenticate?username=&password=&contentid
        public ActionResult authenticate(string username,string password, string contentid)
        {
            AccountServiceReference.AccountServiceClient accountClient = new AccountServiceReference.AccountServiceClient();
            //authenticate the service to allow access to methods
            accountClient.Authenticate("myS0D@P@ssw0rd");

            PortalServiceReference.PortalServiceClient portalClient = new PortalServiceReference.PortalServiceClient();
            CategoriesServiceReference.CatListingServiceClient catClient = new CategoriesServiceReference.CatListingServiceClient();
            //Error if contentid doesn't exists and the user is not authenticated, then show error state 
            if (catClient.get().Where(title => title.FileName == contentid).Count() == 0 && !accountClient.AuthenticateUser(username, password))
                return View(new SODAMvcApplication.Models.drmModel() { Authorized = false, AccessExpirationDays = 0 });
            else
            {
                
                var customer = from c in portalClient.getCustomer()
                               join a in accountClient.getAccount(username) on c.UserId equals a.Id
                               orderby c.DateSubscriptionEnd descending
                               select new SODAMvcApplication.Models.drmModel() { Authorized = true, AccessExpirationDays = Convert.ToInt32(getDays(c)) };
                //close service connections
                accountClient.Close();
                portalClient.Close();
                catClient.Close();
                
                    if(customer.Count() > 0)
                    return View(customer.First());
                    else
                    {
                        return View(new SODAMvcApplication.Models.drmModel() { Authorized = false, AccessExpirationDays = 0 });
                    }
                
            }
           
            
        }

        private static double getDays(PortalServiceReference.Customer c)
        {
            //set to 0 if less days is less than 0
            return (c.DateSubscriptionEnd.Value - DateTime.Now).TotalDays>0?(c.DateSubscriptionEnd.Value - DateTime.Now).TotalDays:0;
        }
    }
}

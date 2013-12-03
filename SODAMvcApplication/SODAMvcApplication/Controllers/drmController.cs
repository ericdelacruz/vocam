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

        public ActionResult authenticate(string username,string password, string contentid)
        {
            AccountServiceReference.AccountServiceClient accountClient = new AccountServiceReference.AccountServiceClient();
            PortalServiceReference.PortalServiceClient portalClient = new PortalServiceReference.PortalServiceClient();

            if(accountClient.AuthenticateUser(username,password))
            {
                var customer = from c in portalClient.getCustomer()
                               join a in accountClient.getAccount(username) on c.UserId equals a.Id
                               select c;
                if (customer.Count() > 0)
                {
                    return View(customer.First());
                }
                else
                    return View();
                
            }
            else
            {
                return View();
            }
        }
    }
}

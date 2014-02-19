using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
namespace CMSMvcApplication.Controllers
{
    public class HomeController : Controller
    {
       
        private AccountServiceReference.AccountServiceClient accountClient = new AccountServiceReference.AccountServiceClient();
        //
        // GET: /Home/
        protected override IAsyncResult BeginExecute(System.Web.Routing.RequestContext requestContext, AsyncCallback callback, object state)
        {
            accountClient.Authenticate("myS0D@P@ssw0rd");
            return base.BeginExecute(requestContext, callback, state);
        }
        public ActionResult Index()
        {
            
            if (Session["Username"] != null)
                return View();
            else
                return RedirectToAction("login");

           
        }
        public ActionResult login()
        {
            return View();
        }
        protected override void Dispose(bool disposing)
        {
            accountClient.Close();
            base.Dispose(disposing);
        }
        [HttpPost]
        public ActionResult login(FormCollection collection)
        {
            if(accountClient.isUserNameExists(collection["Username"]) && accountClient.AuthenticateUser(collection["Username"],collection["Password"]))
            {
                if (accountClient.getAccount(collection["Username"]).First().Role <= 1)
                {
                    Session.Add("Username", collection["Username"]);
                    return RedirectToAction("index");
                }
                else
                {
                    //error no access
                    return View(collection);
                }
            }
            else
            {
                //error
                ViewBag.AccessDenied = true;
                return View(collection);
            }
        }
        public ActionResult logout()
        {
            if(Session["Username"]==null)
                return RedirectToAction("login");

            string username = Session["Username"].ToString();

            accountClient.LogOff(username);
            Session["Username"] = null;
            return RedirectToAction("login");
        }
        //
        // GET: /Home/Profile
        public ActionResult Profile()
        {
            if (Session["Username"] != null)
            {
                var accnt = accountClient.getAccount(Session["Username"].ToString()).Select(a => a).First();
                return View(accnt);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult editprofile()
        {
            if (Session["Username"] != null)
            {
                var accnt = accountClient.getAccount(Session["Username"].ToString()).Select(a => a).First();
                return View(accnt);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public ActionResult editprofile(AccountServiceReference.Account accnt)
        {
            if(Session["Username"] !=null)
            {
                var old_accnt = accountClient.getAccount(Session["Username"].ToString()).Select(a => a).First();

                var new_accnt = accountClient.getAccount(Session["Username"].ToString()).Select(a => a).First();
                
                new_accnt.FirstName = accnt.FirstName;
                new_accnt.LastName = accnt.LastName;
                new_accnt.Company = accnt.Company;
                new_accnt.ContactNo = accnt.ContactNo;
                new_accnt.Address = accnt.Address;
                new_accnt.Email = accnt.Email;
                new_accnt.Role = 1;
                accnt.USERNAME = accnt.Email;
                new_accnt.Id = accnt.Id;
                logUpdatedAccount(old_accnt, new_accnt);
                accountClient.updateAccount(new_accnt);
                return RedirectToAction("profile");
            }
            else
            {
                return RedirectToAction("index");
            }
        }

        private void logUpdatedAccount(AccountServiceReference.Account old_accnt, AccountServiceReference.Account new_accnt)
        {
            PortalServiceReference.PortalServiceClient portalClient = new PortalServiceReference.PortalServiceClient();
            AuditLoggingHelper.LogUpdateAction(Session["Username"].ToString(), old_accnt, new_accnt, portalClient);
            portalClient.Close();
        }
    }
}

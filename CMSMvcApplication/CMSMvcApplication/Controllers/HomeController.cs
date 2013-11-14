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
        private static string portalURL = ConfigurationManager.AppSettings["portalURL"].ToString();
        private AccountServiceReference.AccountServiceClient accountClient = new AccountServiceReference.AccountServiceClient();
        //
        // GET: /Home/

        public ActionResult Index(string user)
        {
            AccountServiceReference.Account account;
            if (!string.IsNullOrEmpty(user) && (account = accountClient.getAccount(user).First()).Status == 1 && account.Role == 0)
            {
                Session.Add("Username", user);
                return View();
            }
            else
                return Redirect(portalURL);
        }

        public ActionResult logout()
        {
            string username = Session["Username"].ToString();

            accountClient.LogOff(username);

            return Redirect(portalURL);
        }
        //
        // GET: /Home/Profile
        public ActionResult Profile()
        {
            return View();
        }
    }
}

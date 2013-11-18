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

        public ActionResult Index()
        {
            
            //AccountServiceReference.Account account;
            //if (!string.IsNullOrEmpty(user) && (account = accountClient.getAccount(user).First()).Status == 1 && account.Role == 0)
            //{
            //    Session.Add("Username", user);
            //    return View();
            //}
            //else
            //    return Redirect(portalURL);
            if (Session["Username"] != null)
                return View();
            else
                return RedirectToAction("login");
        }
        public ActionResult login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult login(FormCollection collection)
        {
            if(accountClient.isUserNameExists(collection["Username"]) && accountClient.AuthenticateUser(collection["Username"],collection["Password"]))
            {
                if (accountClient.getAccount(collection["Username"]).First().Role == 1)
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
            return View();
        }
    }
}

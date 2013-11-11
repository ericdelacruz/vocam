using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SODAPortalMvcApplication.Controllers
{
    public class HomeController : Controller
    {
        private AccountServiceReference.AccountServiceClient accountClient = new AccountServiceReference.AccountServiceClient();
        private const string CMS_URL = "";
        public ActionResult Index()
        {
          

            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(SODAwcfService.Models.Account model)
        {
            try
            {
                if(accountClient.AuthenticateUser(model.USERNAME, model.PASSWORD))
                {
                    //see AccountService CS
                    switch(accountClient.getAccount(model.USERNAME).First().Role)
                    {
                        case 0: return RedirectToAction("Index", "Admin");
                        case 1: return Redirect(string.Format(CMS_URL,model.USERNAME));//make sure to add username parameter to CMS
                        case 2: return RedirectToAction("Index", "Sales");
                        default: return RedirectToAction("Index","User");

                    }
                }
                else
                {
                     
                    return View("Index") ;
                }
            }
             catch(Exception ex)
            {
                 throw(ex);
            }

            return View("Index");

        }
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}

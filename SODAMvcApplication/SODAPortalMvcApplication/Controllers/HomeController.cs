using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SODAPortalMvcApplication.Models;
namespace SODAPortalMvcApplication.Controllers
{
    public class HomeController : Controller
    {
        private AccountServiceRef.AccountServiceClient accountClient = new AccountServiceRef.AccountServiceClient();
        private static string CMSURL = System.Configuration.ConfigurationManager.AppSettings["CMSURL"].ToString() == ""? "http://localhost:56146/":
                                       System.Configuration.ConfigurationManager.AppSettings["CMSURL"].ToString();
        
        public ActionResult Index()
        {
           
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Index(FormCollection collection)
        {

            if (accountClient.AuthenticateUser(collection["Username"], collection["Password"]))
            {
                Session.Add("Username", collection["Username"]);
                AccountServiceRef.Account account = accountClient.getAccount(collection["Username"]).First();
                switch (account.Role)
                {
                    case 0: return RedirectToAction("Index", "Admin");

                    case 1: return Redirect(string.Format(CMSURL, collection["Username"]));
                    case 2: return RedirectToAction("Index", "Sales");
                    case 3:
                        if(account.EmailVerified)
                        return RedirectToAction("Index", "User");
                        else
                        {
                            Session["Username"] = null;
                            ViewBag.loginError = "Email not yet verified";
                            return View(collection);
                        }

                }
            }
            else
            {// If we got this far, something failed, redisplay form
                ModelState.AddModelError("", "The user name or password provided is incorrect.");
                return RedirectToAction("index");
            }
                return View(collection);
        }
        public ActionResult registration()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult registration(ViewModel.UserModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    accountClient.addAccount(new AccountServiceRef.Account(){
                         USERNAME = model.Email,
                         PASSWORD = model.Password,
                         Role = 3,
                         Status = 1,
                         Company = model.Company,
                         ContactNo = model.Contact,
                         Email = model.Email,
                         FirstName = model.FirtName,
                         LastName = model.LastName
                           
                    });

                    sendEmailVerification(model);
                    TempData["EmailSent"] = true;
                    return RedirectToAction("Index", "User");
                }
                catch
                {
                    ModelState.AddModelError("", "Error");
                }
            }
            return View(model);
        }

        private void sendEmailVerification(ViewModel.UserModel model)
        {
            
        }
        [HttpPost]
        public ActionResult EmailVerify(string cid)
        {
            var account = from accnt in accountClient.getAccount("")
                          where accnt.Id == long.Parse(cid)
                          select accnt;

            account.First().EmailVerified = true;

            accountClient.updateAccount(account.First());

            return View();
        }
        public ActionResult logout(string username)
        {
            if (Session["Username"] == null)
                return RedirectToAction("index");

            accountClient.LogOff(username);
            Session.RemoveAll();
            return RedirectToAction("index");
        }
       

       
    }
}

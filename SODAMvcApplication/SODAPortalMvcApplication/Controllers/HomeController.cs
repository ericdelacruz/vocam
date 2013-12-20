using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SODAPortalMvcApplication.Models;
using System.IO;
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
        protected override void Dispose(bool disposing)
        {
            accountClient.Close();
            base.Dispose(disposing);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Index(FormCollection collection)
        {

            if (accountClient.AuthenticateUser(collection["Username"], collection["Password"].Split(',')[0]))
            {
                Session.Add("Username", collection["Username"]);
                AccountServiceRef.Account account = accountClient.getAccount(collection["Username"]).First();
                switch (account.Role)
                {
                    case 0: return RedirectToAction("Index", "Admin");

                    case 1: return Redirect(string.Format(CMSURL, collection["Username"]));

                    case 2:
                        if (account.EmailVerified)
                        return RedirectToAction("Index", "Sales");
                        else
                        {
                            Session["Username"] = null;
                            ViewBag.loginError = "Email not yet verified";
                            return View(collection);
                        }
                    case 3:
                        
                        return RedirectToAction("Index", "User");
                       

                }
            }
            else
            {// If we got this far, something failed, redisplay form
                ModelState.AddModelError("", "The user name or password provided is incorrect.");
                return View("index");
            }
                return View(collection);
        }
        public ActionResult registration()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult registration(ViewModel.UserModel model,FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    accountClient.addAccount(new AccountServiceRef.Account(){
                         USERNAME = model.Email,
                         PASSWORD = model.Password.Split(',')[0],
                         Role = 3,
                         Status = 1,
                         Company = model.Company,
                         ContactNo = model.Contact,
                         Email = model.Email,
                         FirstName = model.FirtName,
                         LastName = model.LastName,
                         Country = collection["country"],
                          CompanyUrl = model.CompanyUrl
                          
                                                     
                    });

                    //sendEmailVerification(model);
                    TempData["EmailSent"] = true;
                    Session.Add("Username", model.Email);
                    return RedirectToAction("Index", "User");
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError("", "Error");
                }
            }
            return View(model);
        }

        private void sendEmailVerification(ViewModel.UserModel model)
        {

            EmailHelper.SendEmail("test@sac-iis.com", model.Email, "Verification", "Click the link to continue." + Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("verify", "home", new { code = EncDec.EncryptData(model.Email) }).Replace("/portal", ""));
        }
        public ActionResult verify(string code)
        {
            string username = EncDec.DecryptString(code);
            if(!string.IsNullOrEmpty(username))
            {
                var user = from accnt in accountClient.getAccount(username)
                           select accnt;

                if(user.Count() >0)
                {
                    AccountServiceRef.Account accnt = user.First();

                    //accnt.EmailVerified = true;

                    accountClient.updateAccount(new AccountServiceRef.Account(){
                        USERNAME = accnt.Email,
                        PASSWORD = accnt.PASSWORD,
                         Role = 3,
                         Status = 1,
                        Company = accnt.Company,
                        ContactNo = accnt.ContactNo,
                        Email = accnt.Email,
                        FirstName = accnt.FirstName,
                        LastName = accnt.LastName,
                          EmailVerified = true
                          });

                    Session.Add("Username", accnt.USERNAME);
                    return RedirectToAction("index", "user");
                }
            }
            return RedirectToAction("index");
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
       
        public ActionResult forgotpassword()
        {
            return View();
        }
       
        [HttpPost]
        [AllowAnonymous]
        public ActionResult forgotpassword(FormCollection collection)
        {
            if(!string.IsNullOrEmpty(collection["Username"]))
            {
                if(accountClient.getAccount(collection["Username"]).Count() > 0)
                {
                    long userid = accountClient.getAccount(collection["Username"]).First().Id;
                    DateTime dateSent = DateTime.Now;
                    DateTime dateEx = dateSent.AddDays(1);
                    
                    string key = EmailHelper.GetMd5Hash(collection["Username"] + ";" + dateSent.ToString());
                    accountClient.addResetPassword(key, dateSent, dateEx, userid);
                    ViewData.Add("resetlink", Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("resetpassword", new { code = key }));
                    string body = EmailHelper.ToHtml("forgotpasswordemail",ViewData, this.ControllerContext);
                    EmailHelper.SendEmail("test@sec-iis.com", collection["Username"], "Reset password", body);
                    TempData["ResetPassSent"] = true;
                    return View();

                }
                else
                {
                    ModelState.AddModelError("", "Error! Account doesn't exist.");
                    return View();
                }
            }
            else
            {
                //error
                return RedirectToAction("forgotpassword");
               
            }
        }

        public ActionResult resetpassword(string code)
        {
            var resetpass = accountClient.getRestPassword().Where(rp => rp.key == code);
           
            if (resetpass.Count() > 0)
            {
                TempData["resetuserid"] = resetpass.First().UserId.ToString();
                return View();
            }
            else
                return Redirect("index");
        }
        [HttpPost]
        public ActionResult resetpassword(FormCollection collection)
        {
              if(collection["Password"] == collection["ConfirmPassword"])
              {
                  long userid = 0;
                  if (long.TryParse(TempData["resetuserid"].ToString(), out userid))
                  {
                      var accnt = accountClient.getAccount("").Where(a => a.Id == userid).First();
                      accountClient.updatePassword(userid, accnt.PASSWORD, collection["Password"]);
                      string redirectLink = getRedirectLinkByRole(accnt.Role);
                      Session.Add("Username", accnt.USERNAME);
                      TempData["PasswordUpdated"] = true;//reuse tempdata for redirection link
                      return View(); 
                  }
                  else
                  {
                      ModelState.AddModelError("", "Error");
                      return View();
                  }
              }
              else
              {
                  ModelState.AddModelError("", "Password doesnot match please try again");
                  return View();
              }
        }

        private string getRedirectLinkByRole(int Role)
        {
            switch(Role)
            {
                case 0: return Url.Action("index","admin");

                //case 1: return Redirect(string.Format(CMSURL, collection["Username"]));

                case 2:
                        return Url.Action("index","sales");
                case 3:

                        return Url.Action("index", "user");
                default:return "";
            }
        }
    }
}

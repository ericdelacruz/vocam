using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SODAPortalMvcApplication.Models;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
namespace SODAPortalMvcApplication.Controllers
{
    public class HomeController : Controller
    {
        private AccountServiceRef.AccountServiceClient accountClient = new AccountServiceRef.AccountServiceClient();
        private PortalServiceReference.PortalServiceClient portaClient = new PortalServiceReference.PortalServiceClient();
        private CMSServiceReference.CMS_ServiceClient cmsClient = new CMSServiceReference.CMS_ServiceClient();
        [RequireHttps]
        public ActionResult Index()
        {
            string PhoneNo = getPhoneNum();
            Session.Add("PhoneNo", PhoneNo);
             
            return View();
        }
        protected override IAsyncResult BeginExecute(System.Web.Routing.RequestContext requestContext, AsyncCallback callback, object state)
        {
            accountClient.Authenticate("myS0D@P@ssw0rd");
            return base.BeginExecute(requestContext, callback, state);
        }
        private string getPhoneNum()
        {
            try
            {
                int RegionId = portaClient.getRegion().Where(r => r.WebsiteUrl == Request.Url.Host.Replace("portal", "www")).First().Id;

                string PhoneNo = cmsClient.getContent("Contact", "PhoneNo").Where(c => c.RegionId == RegionId).First().Value;
                return PhoneNo;
            }
            catch
            {
                return "";
            }
        }
        protected override void Dispose(bool disposing)
        {
            accountClient.Close();
            portaClient.Close();
            cmsClient.Close();
            base.Dispose(disposing);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Index(FormCollection collection)
        {

            string cmsurl = "http://"+Request.Url.Host.Replace("portal","cms");
            if (accountClient.AuthenticateUser(collection["Username"], collection["Password"].Split(',')[0]))
            {
                Session.Add("Username", collection["Username"]);
                AccountServiceRef.Account account = accountClient.getAccount(collection["Username"]).First();
                Session.Add("UserId", account.Id);
                AuditLoggingHelper.LogAuthenticationAction(Session["Username"].ToString(), true, portaClient);
                switch (account.Role)
                {
                    case 0: return RedirectToAction("Index", "Admin");

                    case 1: return Redirect(cmsurl);

                    case 2:
                        if (EncDec.DecryptString(account.PASSWORD) != "safety")
                        return RedirectToAction("Index", "Sales");
                        else if (EncDec.DecryptString(account.PASSWORD) == "safety")
                        {
                            return RedirectToAction("changepassword", new { returnurl=@Url.Action("index","sales") });
                        }
                        else
                        {
                            Session["Username"] = null;
                            ViewBag.loginError = "Email not yet verified";
                            return View(collection);
                        }
                    case 3:
                        if (EncDec.DecryptString(account.PASSWORD) != "safety")
                        return RedirectToAction("Index", "User");
                        else
                        {
                            return RedirectToAction("changepassword", new { returnurl = Url.Action("index", "user") });
                        }

                }
            }
            else
            {// If we got this far, something failed, redisplay form
                ModelState.AddModelError("", "The user name or password provided is incorrect.");
                return View("index");
            }
                return View(collection);
        }
        [RequireHttps]
        public ActionResult registration()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [RequireHttps]
        public ActionResult registration(ViewModel.UserModel model,FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var new_cust = new AccountServiceRef.Account()
                    {
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


                    };
                    AuditLoggingHelper.LogCreateAction(new_cust.USERNAME, new_cust, portaClient);
                    accountClient.addAccount(new_cust);

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
        [RequireHttps]
        public ActionResult purchaseDetails()
        {
            ViewBag.Country = portaClient.getRegion().Where(r => r.WebsiteUrl == Request.Url.Host.Replace("portal", "www") || (Request.Url.Host == "localhost" && r.RegionName == "AU")).FirstOrDefault().RegionName;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [RequireHttps]
        public ActionResult purchaseDetails(ViewModel.UserModel model, FormCollection collection)
        {
            model.Password = "safety";
            model.ConfirmPassword = "safety";
            model.Country = collection["country"];
            if(!IsValidEmail(model.Email))
            {
                ModelState.AddModelError("", "Invalid Email");
            }
            
            if (ModelState.IsValid)
            {
               

                    if (!accountClient.isUserNameExists(model.Email.ToLower()))
                    {//New account will be added upon successful payment. Check paymentstatus method under user controller
                        Session.Add("NewAccount", new AccountServiceRef.Account()
                        {
                            USERNAME = model.Email,
                            PASSWORD = model.Password,
                            Role = 3,
                            Status = 1,
                            Company = model.Company,
                            ContactNo = model.Contact,
                            Email = model.Email,
                            FirstName = model.FirtName,
                            LastName = model.LastName,
                            //Country = collection["country"],
                            Country = model.Country,
                            CompanyUrl = model.CompanyUrl


                        });
                        TempData["EmailSent"] = true;
                        Session.Add("Username", model.Email);

                        DateTime clientDateTime = toClientTime(collection["dateTimeOffset"]);
                        Session.Add("ClientDateTime", clientDateTime);
                             
                        return Session["SalesCode"] == null?RedirectToAction("indexpurchase", "user"):RedirectToAction("termsinit","user");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Email address already exists. Please use a different email address.");
                    }
                        
               
            }
            return View(model);
        }

        private DateTime toClientTime(string strTimeZoneOffset)
        {
            
            
            return (DateTime.UtcNow.Add(new TimeSpan(long.Parse(strTimeZoneOffset))));
        }
        private bool IsValidEmail(string strIn)
        {
            return Regex.IsMatch(strIn, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z");
        }
        private void sendEmailVerification(ViewModel.UserModel model)
        {

            EmailHelper.SendEmail("test@sac-iis.com", model.Email, "Verification", "Click the link to continue." + Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("verify", "home", new { code = EncDec.EncryptData(model.Email) }).Replace("/portal", ""));
        }
        [RequireHttps]
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
        [RequireHttps]
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
            AuditLoggingHelper.LogAuthenticationAction(Session["Username"].ToString(), false, portaClient);
            accountClient.LogOff(username);
            Session.RemoveAll();
            return RedirectToAction("index");
        }
        [RequireHttps]
        public ActionResult forgotpassword()
        {
            return View();
        }
       
        [HttpPost]
        [AllowAnonymous]
        [RequireHttps]
        public ActionResult forgotpassword(FormCollection collection)
        {
            if(!string.IsNullOrEmpty(collection["Username"]))
            {
                if(accountClient.getAccount(collection["Username"]).Count() > 0)
                {
                    //long userid = accountClient.getAccount(collection["Username"]).First().Id;
                    //DateTime dateSent = DateTime.Now;
                    //DateTime dateEx = dateSent.AddDays(1);
                    
                    //string key = EmailHelper.GetMd5Hash(collection["Username"] + ";" + dateSent.ToString());
                    //accountClient.addResetPassword(key, dateSent, dateEx, userid);
                    //ViewData.Add("resetlink", Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("resetpassword", new { code = key }));
                    //string body = EmailHelper.ToHtml("forgotpasswordemail",ViewData, this.ControllerContext);
                    //EmailHelper.SendEmail("test@sec-iis.com", collection["Username"], "Reset password", body);
                    //TempData["ResetPassSent"] = true;
                    var accnt = accountClient.getAccount(collection["Username"]).First();
                    string portalurl = "http://" + Request.Url.Host;
                         //string body = "Hi " + accnt.FirstName + " " + accnt.LastName +". Your password is " + EncDec.DecryptString(accnt.PASSWORD);
                    //string body = "Hi " + accnt.FirstName + " " + accnt.LastName + "," + "\n" + "\n" + "Your password is " + 
                    //              EncDec.DecryptString(accnt.PASSWORD) + "\n" + "\n" + "Copy and paste the site URL for the login page" + "\n" +
                    //              portalurl + "Regards," + "\n" + "SafetyOnDemand.com";
                    ViewData.Add("CustomerName", accnt.FirstName + " " + accnt.LastName);
                    ViewData.Add("Password", EncDec.DecryptString(accnt.PASSWORD));
                    string body = EmailHelper.ToHtml("emailforgotpassword", ViewData, this.ControllerContext);     
                    //EmailHelper.SendEmail("test@sac-iis.com", collection["Username"], "Forgot password", body);
                    string from = Request.Url.Host != "localhost" ? "no-reply" + Request.Url.Host.Replace("portal.", "@") : "test@sac-iis.com";
                    EmailHelper.SendEmail(new System.Net.Mail.MailAddress(from, "Safety On Demand"), new System.Net.Mail.MailAddress(collection["Username"]), "Forgot Password", body, true,null);
                    TempData["ResetPassSent"] = true;



                      
                    return View();

                }
                else
                {
                    ModelState.AddModelError("", "Error! Account does not exist.");
                    return View();
                }
            }
            else
            {
                //error
                return RedirectToAction("forgotpassword");
               
            }
        }

        public ActionResult emailforgotpassword()
        {
            return View();
        }
        [RequireHttps]
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
        [RequireHttps]
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
                  ModelState.AddModelError("", "Password does not match! Please try again.");
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
        [RequireHttps]
        public ActionResult changePassword(string returnurl,string role)
        {
           
            
            if(role == "admin")
            {
                TempData["ReturnUrl"] = Url.Action("index", "admin");
                ViewBag.CancelUrl = Url.Action("index", "admin");
                ViewBag.enableCancel = true;
            }
            else
            {
                TempData["ReturnUrl"] = Session["ReturnUrl"] ?? returnurl;
            }
            //ViewBag.isAdmin = TempData["isAdmin"] ?? null;
            return View();
        }

        [HttpPost]
        [RequireHttps]
        public ActionResult changePassword(ViewModel.changePasswordModel changePasswordModel)
        {
             if(Session["Username"] != null)
             {
                 var account = accountClient.getAccount(Session["Username"].ToString()).First();
                 if (ModelState.IsValid)
                 {
                     accountClient.updatePassword(account.Id, account.PASSWORD, changePasswordModel.Password);

                     emailCustomerNewPassword(changePasswordModel, account);
                     
                     return Redirect(TempData["ReturnUrl"].ToString());
                 }
                 else
                 {
                     return View(changePasswordModel);
                 }
             }
             else
             {
                 return RedirectToAction("Index");
             }
        }

        private void emailCustomerNewPassword(ViewModel.changePasswordModel changePasswordModel, AccountServiceRef.Account account)
        {
            string from = "no-reply" + Request.Url.Host.Replace("portal.", "@");
            string to = account.USERNAME; //username is email 
            ViewData.Add("CustomerName", account.FirstName + " " + account.LastName);
            ViewData.Add("Password", changePasswordModel.Password);
            ViewData.Add("Username", account.USERNAME);
            string body = EmailHelper.ToHtml("changepasswordemail", ViewData, this.ControllerContext);
            EmailHelper.SendEmail(new System.Net.Mail.MailAddress(from, "Safety On Demand"), new System.Net.Mail.MailAddress(to), "Your TrainNow Password Changed", body, true, null);

        }
        public ActionResult changepasswordemail()
        {
            return View();
        }
    }
}

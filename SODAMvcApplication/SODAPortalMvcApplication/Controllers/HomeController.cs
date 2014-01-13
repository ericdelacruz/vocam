using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SODAPortalMvcApplication.Models;
using System.IO;
using System.Text.RegularExpressions;
namespace SODAPortalMvcApplication.Controllers
{
    public class HomeController : Controller
    {
        private AccountServiceRef.AccountServiceClient accountClient = new AccountServiceRef.AccountServiceClient();
        private PortalServiceReference.PortalServiceClient portaClient = new PortalServiceReference.PortalServiceClient();
        private CMSServiceReference.CMS_ServiceClient cmsClient = new CMSServiceReference.CMS_ServiceClient();
     
        public ActionResult Index()
        {
            string PhoneNo = getPhoneNum();
            Session.Add("PhoneNo", PhoneNo);
             
            return View();
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
                switch (account.Role)
                {
                    case 0: return RedirectToAction("Index", "Admin");

                    case 1: return Redirect(cmsurl);

                    case 2:
                        if (account.EmailVerified && EncDec.DecryptString(account.PASSWORD) != "safety")
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
                            return RedirectToAction("changepassword", new { returnurl = @Url.Action("index", "user") });
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
        public ActionResult purchaseDetails()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
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
                try
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
                        return RedirectToAction("indexpurchase", "user");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Email address already exists. Please use a different email address.");
                    }
                        
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error");
                }
            }
            return View(model);
        }
        private bool IsValidEmail(string strIn)
        {
            return Regex.IsMatch(strIn, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z");
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
                    EmailHelper.SendEmail("test@sac-iis.com", collection["Username"], "Forgot password", body);
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

        public ActionResult changePassword(string returnurl)
        {
            TempData["ReturnUrl"] = returnurl;
            return View();
        }

        [HttpPost]
        public ActionResult changePassword(ViewModel.changePasswordModel changePasswordModel)
        {
             if(Session["Username"] != null)
             {
                 var account = accountClient.getAccount(Session["Username"].ToString()).First();
                 if (ModelState.IsValid)
                 {
                     accountClient.updatePassword(account.Id, account.PASSWORD, changePasswordModel.Password);
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
    }
}

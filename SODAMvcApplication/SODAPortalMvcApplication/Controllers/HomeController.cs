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
        private static string defaultRegion = "AU";
        [RequireHttps]
        public ActionResult Index()
        {
            //get phone number depending on what region. Returns string.empty if localhost
            string PhoneNo = getPhoneNum();
            Session.Add("PhoneNo", PhoneNo);
             
            return View();
        }

        protected override IAsyncResult BeginExecute(System.Web.Routing.RequestContext requestContext, AsyncCallback callback, object state)
        {
            //Authenticate the account service so that we can call its methods
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
                //log user to auditlogs
                AuditLoggingHelper.LogAuthenticationAction(Session["Username"].ToString(), true, portaClient);
                //redirection rules for different roles of account
                //0 - admin
                //1-marketer/cms
                //2- sales
                //3 - customer
                switch (account.Role)
                {
                    case 0: return RedirectToAction("Index", "Admin");

                    case 1: return Redirect(cmsurl);//redirect marketer user to cms

                    case 2:
                        //if password is the default password (safety) then redirect to change password
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
                        //if password is the default password (safety) then redirect to change password
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
        //Obselete. See purchasedetails instead
        [RequireHttps]
        public ActionResult registration()
        {
            return View();
        }
        //obselete. see purchasedetails instead
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
            //this sets the default country depending on the regionname. sets to AU if localhost
            ViewBag.Country = portaClient.getRegion().Where(r => r.WebsiteUrl == Request.Url.Host.Replace("portal", "www") || (Request.Url.Host == "localhost" && r.RegionName == defaultRegion)).FirstOrDefault().RegionName;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [RequireHttps]
        public ActionResult purchaseDetails(ViewModel.UserModel model, FormCollection collection)
        {
            //set deafult password to safety
            model.Password = "safety";
            model.ConfirmPassword = "safety";//required for validation of the model
            model.Country = collection["country"];
            //parse email if valid
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
        /// <summary>
        /// Verify account when the user clicks the link in their email
        /// </summary>
        /// <param name="code">Username in encrypted form</param>
        /// <returns></returns>
        [RequireHttps]
        public ActionResult verify(string code)
        {
            string username = EncDec.DecryptString(code);
            if(!string.IsNullOrEmpty(username))
            {
                
                var user = from accnt in accountClient.getAccount(username)
                           select accnt;
                //if user exists then update email verified to true then redirect to the user page.
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
                    
              
                    var accnt = accountClient.getAccount(collection["Username"]).First();
                    string portalurl = "http://" + Request.Url.Host;
                      
                    ViewData.Add("CustomerName", accnt.FirstName + " " + accnt.LastName);
                    ViewData.Add("Password", EncDec.DecryptString(accnt.PASSWORD));
                    //converts emailforgotpassword.cshtml to string variable. to see the format goto emailforgotpassword method then goto view
                    string body = EmailHelper.ToHtml("emailforgotpassword", ViewData, this.ControllerContext);     
                    //if localhost set to test@sac-iis.com
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
        public ActionResult changePassword(string returnurl)
        {
           
            
            
             TempData["ReturnUrl"] = Session["ReturnUrl"] ?? returnurl;
            
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
                 //if model is valid, update the password then email the new password
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
            //convert changepasswordemail.cshtml to variable string. to see the format goto changepasswordemail() then goto view
            string body = EmailHelper.ToHtml("changepasswordemail", ViewData, this.ControllerContext);
            EmailHelper.SendEmail(new System.Net.Mail.MailAddress(from, "Safety On Demand"), new System.Net.Mail.MailAddress(to), "Your TrainNow Password Changed", body, true, null);

        }
        public ActionResult changepasswordemail()
        {
            return View();
        }
    }
}

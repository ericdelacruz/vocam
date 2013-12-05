using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using Recaptcha.Web;
using Recaptcha.Web.Mvc;
using System.Threading.Tasks;
namespace SODAMvcApplication.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        private CMSServiceReference.CMS_ServiceClient cmsServiceClient = new CMSServiceReference.CMS_ServiceClient();
        private string password = "myS0D@P@ssw0rd";

        private const string HOME = "Home";
        private const string CONTACT = "Contact";
        private const string LEARN = "Learn";

        public ActionResult Index()
        {

          
            
              var  lContentDef = cmsServiceClient.getContent(HOME, string.Empty);
            
              
                 
            return View(lContentDef);
        }

        protected override void Dispose(bool disposing)
        {
           cmsServiceClient.Close();
            base.Dispose(disposing);
        }
        //
        //GET: /Contact/
        public ActionResult Contact()
        {
            
            return View();
        }
        [HttpPost]
        public ActionResult contact(CMSServiceReference.Contact contact)
        {
            contact.isFreePPT = false;
            cmsServiceClient.addContact(contact);
            return RedirectToAction("Index");
        }
        //
        //GET: /LearnMore/
         
        public ActionResult LearnMore()
        {
            if (cmsServiceClient.Authenticate(password))
            {
                //error page
            }


            var lContentDef = cmsServiceClient.getContent(LEARN, string.Empty);
           


            return View(lContentDef);
        }
        public ActionResult thankyou()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> contactFreeppt(CMSServiceReference.Contact contact)
        {
            RecaptchaVerificationHelper recaptchaHelper = this.GetRecaptchaVerificationHelper();
            if (String.IsNullOrEmpty(recaptchaHelper.Response))
            {
                ModelState.AddModelError("", "Captcha answer cannot be empty.");
                return View(contact);
            }

            RecaptchaVerificationResult recaptchaResult = await recaptchaHelper.VerifyRecaptchaResponseTaskAsync();

            if (recaptchaResult != RecaptchaVerificationResult.Success)
            {
                ModelState.AddModelError("", "Incorrect captcha answer.");
                return View(contact);
            }
           
            contact.isFreePPT = false;
            cmsServiceClient.addContact(contact);
            EmailHelper.SendEmail("test@yahoo.com", contact.Email, "TEST", "TEST BODY");
            return RedirectToAction("thankyou");
           
        }

    }
}

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
        private string[] filenames = { "USO-AUS-PPS.pps", "USW-AUS-PPS.pps", "WHSE-AUS-PPS.pps" };
        
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
        public async Task<ActionResult> contactFreeppt(CMSServiceReference.Contact contact,FormCollection collection)
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
           
            contact.isFreePPT = true;
            DateTime dateAdded = DateTime.Now;
            contact.isVerified = false;
            string key = EmailHelper.GetMd5Hash(contact.Email.Substring(0, contact.Email.IndexOf('@')) + dateAdded.Ticks.ToString());
            string selected = collection["selectppt"];
            contact.key = key;
            contact.DateLinkEx = dateAdded;
            cmsServiceClient.addContact(contact);
            
            EmailHelper.SendEmail("test@yahoo.com", contact.Email, "TEST", createDownloadPPTLink(key,selected));
            return RedirectToAction("thankyou");
           
        }

        private string createDownloadPPTLink(string key,string selected)
        {
            
            return Request.Url.GetLeftPart(UriPartial.Authority)+ Url.Action("download", new { key = key, selected = selected });
        }
        public ActionResult download(string key, string select)
        {
            var contact = cmsServiceClient.getContact().Where(c => c.key == key);
            if(contact.Count() >0 && (contact.First().DateLinkEx.Value - DateTime.Now).Hours < 1)
            {
                
                return View();
            }
            return RedirectToAction("index");
        }

        public FileStreamResult StreamFileFromDisk(int index)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "Content/download/";
            string filename = filenames[index];
            return File(new System.IO.FileStream(path + filename, System.IO.FileMode.Open),"application/vnd.ms-powerpoint", filename);
        }

    }
}

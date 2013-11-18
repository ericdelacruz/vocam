using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
namespace SODAMvcApplication.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        private static CMSServiceReference.CMS_ServiceClient cmsServiceClient = new CMSServiceReference.CMS_ServiceClient();
        private string password = "myS0D@P@ssw0rd";
        private const string HOME = "Home";
        private const string CONTACT = "Contact";
        private const string LEARN = "Learn";

        public ActionResult Index()
        {

          
            //if (cmsServiceClient.Authenticate(password))
            //{
            //    //error page
            //}

            
              var  lContentDef = cmsServiceClient.getContent(HOME, string.Empty);
            
              
                 
            return View(lContentDef);
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

    }
}

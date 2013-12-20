using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
//using Recaptcha.Web;
//using Recaptcha.Web.Mvc;
using CaptchaMvc.Attributes;
using CaptchaMvc.HtmlHelpers;
using CaptchaMvc.Interface;
using System.Configuration;

namespace SODAMvcApplication.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        private CMSServiceReference.CMS_ServiceClient cmsServiceClient = new CMSServiceReference.CMS_ServiceClient();
        private CategoriesServiceReference.CatListingServiceClient catListingSerivceClient = new CategoriesServiceReference.CatListingServiceClient();
        private PortalServiceReference.PortalServiceClient portalClient = new PortalServiceReference.PortalServiceClient();
        private string Region = ConfigurationManager.AppSettings["Region"].ToString();
        private string password = "myS0D@P@ssw0rd";

        private const string HOME = "Home";
        private const string CONTACT = "Contact";
        private const string LEARN = "Learn";
        private string[] filenames = { "USO-AUS-PPS.pps", "USW-AUS-PPS.pps", "WHSE-AUS-PPS.pps" };
        
        public ActionResult Index()
        {

          
            
              var  lContentDef = cmsServiceClient.getContent(HOME, string.Empty);

              var filterByRegion = from content in lContentDef
                                   join region in portalClient.getRegion() on content.RegionId equals region.Id
                                   where region.RegionName.ToLower() == Region.ToLower()
                                   select content;

              var contact = cmsServiceClient.getContent(CONTACT,"PhoneNo").Where(cms=>cms.RegionId == filterByRegion.First().RegionId).First().Value;
              Session.Add("PortalUrl", filterByRegion.Where(c => c.SectionName == "PortalUrl").First().Value);
              Session.Add("PhoneNo", contact);   
            return View(filterByRegion);
        }

        protected override void Dispose(bool disposing)
        {
           cmsServiceClient.Close();
           catListingSerivceClient.Close();
            portalClient.Close();
            base.Dispose(disposing);
        }
        //
        //GET: /Contact/
        public ActionResult Contact()
        {
            
            return View();
        }

        //GET: /ContactFreePpt/
        public ActionResult contactfreeppt()
        {

            return View();
        }


        //GET: /Sitemap/
        public ActionResult Sitemap()
        {
            var sitemapData = from cat in catListingSerivceClient.get_Categories()
                              where cat.CategoryId > 1
                              orderby ConvertGrade(cat.CategoryId)
                              select new Models.SiteMapModel()
                              {
                                  CatName = cat.CategoryName,
                                  Titles = getTitlesUnderCat(cat.CategoryId),
                                  CatId = cat.CategoryId
                              };

            return View(sitemapData);
        }
        private long ConvertGrade(long CatID)
        {
            //can be database but for now hard code
            return CatID == 22 ? 0 : CatID == 3 ? 1 : 4;
        }
        private List<string> getTitlesUnderCat(long catid)
        {
            var catTitles = from ca in catListingSerivceClient.getCatAssign()
                            join title in catListingSerivceClient.get() on ca.SpecID equals title.Id
                            join region in portalClient.getRegion() on title.RegionId equals region.Id 
                            where ca.CategoryId == catid && region.RegionName == Region
                            select title.Title;
            return catTitles.Count() > 0 ? catTitles.ToList() : null;
        }

        

        //GET: /Legals/
        public ActionResult Legals()
        {

            return View();
        }


        [HttpPost]
        public ActionResult contact(CMSServiceReference.Contact contact)
        {
            contact.isFreePPT = false;
            cmsServiceClient.addContact(contact);
            TempData["MsgSent"] = true;
            return RedirectToAction("contact");
            //return View();
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

            var filterByRegion = from content in lContentDef
                                 join region in portalClient.getRegion() on content.RegionId equals region.Id
                                 where region.RegionName.ToLower() == Region
                                 select content;

            return View(filterByRegion);
        }

        public ActionResult Replyfreeppt()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Replyfreeppt(FormCollection collection)
        {
            return View();
        }
        //public async Task<ActionResult> contactFreeppt(CMSServiceReference.Contact contact,FormCollection collection)
        //{
        [HttpPost]
        public ActionResult contactFreeppt(CMSServiceReference.Contact contact, FormCollection collection)
        {
            //RecaptchaVerificationHelper recaptchaHelper = this.ge;
            //if (String.IsNullOrEmpty(recaptchaHelper.Response))
            //{
            //    ModelState.AddModelError("", "Captcha answer cannot be empty.");
            //    return View(contact);
            //}
           
            //RecaptchaVerificationResult recaptchaResult = recaptchaHelper.VerifyRecaptchaResponse();

            //if (recaptchaResult != RecaptchaVerificationResult.Success)
            //{
            //    ModelState.AddModelError("", "Incorrect captcha answer.");
            //    return View(contact);
            //}
            if (!this.IsCaptchaValid("Captcha is not valid"))
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
            return RedirectToAction("Replyfreeppt");
           
        }

        private string createDownloadPPTLink(string key,string selected)
        {
            
            return Request.Url.GetLeftPart(UriPartial.Authority)+ Url.Action("download", new { key = key, selected = selected });
        }
        public ActionResult download(string key, string selected)
        {
            if(cmsServiceClient.State == System.ServiceModel.CommunicationState.Closed)
            {
                cmsServiceClient.Open();
            }
            var contact = cmsServiceClient.getContact().Where(c => c.key == key);
            int i = 0;
            if(contact.Count() >0 && (contact.First().DateLinkEx.Value - DateTime.Now).Hours < 1)
            {
                switch(selected)
                {
                    case "industrial": i = 0;
                        break;
                    case "office": i = 1; 
                        break;
                    case "warehouse": i=2;
                        break;
                    default: return RedirectToAction("index");

                }
                return View(i);
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

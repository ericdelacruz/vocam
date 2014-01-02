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
        //private PortalServiceReference.PortalServiceClient portalClient = new PortalServiceReference.PortalServiceClient();
        //private string Region = "au";
        private string defaultRegion = "au";
        private string password = "myS0D@P@ssw0rd";

        private const string HOME = "Home";
        private const string CONTACT = "Contact";
        private const string LEARN = "Learn";
        //private string[] filenames = { "USO-AUS-PPS.pps", "USW-AUS-PPS.pps", "WHSE-AUS-PPS.pps" };
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            
            base.Initialize(requestContext);
        }
        public ActionResult Index()
        {


            if (!Request.Url.Host.Contains("www.") && Request.Url.Host != "localhost")
                return Redirect("http://www." + Request.Url.Host);
            else if (Request.Url.Host == "www.sac-iis.com" || Request.Url.Host == "sac-iis.com")
            {
                var websiteUrl = cmsServiceClient.getRegions().Where(r => r.Id == 12).First().WebsiteUrl;
                return Redirect("http://" + websiteUrl);
            }

              var  lContentDef = cmsServiceClient.getContent(HOME, string.Empty);

              var filterByRegion = getContents(lContentDef);
              
               
              var contact = cmsServiceClient.getContent(CONTACT,"PhoneNo").Where(cms=>cms.RegionId == filterByRegion.First().RegionId).First().Value;

              Session.Add("PortalUrl", filterByRegion.Where(c => c.SectionName == "PortalUrl").First().Value);
              Session.Add("PhoneNo", contact);   
             
            return View(filterByRegion);
        }

        private IEnumerable<CMSServiceReference.ContentDef> getContents(CMSServiceReference.ContentDef[] lContentDef)
        {
            var filterByRegion = from content in lContentDef
                                 join region in cmsServiceClient.getRegions() on content.RegionId equals region.Id
                                 where region.WebsiteUrl == Request.Url.Host
                                 select content;
            if(filterByRegion.Count() > 0)
            return filterByRegion;
            else
            {
                return from content in lContentDef
                       join region in cmsServiceClient.getRegions() on content.RegionId equals region.Id
                       where region.RegionName == defaultRegion
                       select content;
            }
        }

        protected override void Dispose(bool disposing)
        {
           cmsServiceClient.Close();
           catListingSerivceClient.Close();
           
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
            var freeppt = from fppt in cmsServiceClient.getFreePPT()
                          join region in cmsServiceClient.getRegions() on fppt.RegionId equals region.Id
                          where region.WebsiteUrl == Request.Url.Host
                          select fppt;
            ViewBag.pptList = freeppt.Select(fppt => new SelectListItem()
            {
                Text = fppt.DisplayText,
                Value = fppt.PPTType,
                Selected = false

            });
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
            var catTitles = getCatTitles(catid);
           

            return catTitles.Count() > 0 ? catTitles.ToList() : null;
        }

        private IEnumerable<string> getCatTitles(long catid)
        {
            var catTitles = from ca in catListingSerivceClient.getCatAssign()
                            join title in catListingSerivceClient.get() on ca.SpecID equals title.Id
                            join region in cmsServiceClient.getRegions() on title.RegionId equals region.Id
                            where ca.CategoryId == catid && region.WebsiteUrl == Request.Url.Host
                            select title.Title;
            if(catTitles.Count() >0)
            return catTitles;
            else
            {
                return from ca in catListingSerivceClient.getCatAssign()
                            join title in catListingSerivceClient.get() on ca.SpecID equals title.Id
                            join region in cmsServiceClient.getRegions() on title.RegionId equals region.Id
                            where ca.CategoryId == catid && region.RegionName == defaultRegion
                            select title.Title;
            }
        }

        

        //GET: /Legals/
        public ActionResult Legals()
        {

            return View();
        }


        [HttpPost]
        public ActionResult contact(CMSServiceReference.Contact contact)
        {
            if (!this.IsCaptchaValid("Captcha is not valid"))
            {
                ModelState.AddModelError("", "Incorrect captcha answer.");
                return View(contact);
            }
            contact.isFreePPT = false;
            cmsServiceClient.addContact(contact);
            //EmailHelper.SendEmail(contact.Email, "SODA Customer Inquiry", "Message sent. A customer representative will contact you shortly.");
            string from = "no_reply_test" + Request.Url.Host.Replace("www.", "@");
            string replyto = "sales_test" + Request.Url.Host.Replace("www.", "@");
            //EmailHelper.SendEmail(contact.Email, "SODA Customer Inquiry", "Message sent. A customer representative will contact you shortly.");
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

            var filterByRegion = getContents(lContentDef);

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
        
        [HttpPost]
        public ActionResult contactFreeppt(CMSServiceReference.Contact contact, FormCollection collection)
        {
            
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
            string from = "no_reply_test" + Request.Url.Host.Replace("www.", "@");
            string Replyto = "sales_test" + Request.Url.Host.Replace("www.", "@");
            EmailHelper.SendEmail(new System.Net.Mail.MailAddress(from, "Safety On Demand"), new System.Net.Mail.MailAddress(contact.Email), "SODA:Free Powerpoint download Link", createDownloadPPTLink(key, selected), false, Replyto,"P@ssw0rd12345");
            //EmailHelper.SendEmail(contact.Email, "SODA:Free Powerpoint download", createDownloadPPTLink(key,selected),null);

            sendFreePPTDetailsToSales(contact);
            return RedirectToAction("Replyfreeppt");
           
        }

        private void sendFreePPTDetailsToSales(CMSServiceReference.Contact contact)
        {
            string from = "no_reply_test" + Request.Url.Host.Replace("www.", "@");
            string to = "sales_test" + Request.Url.Host.Replace("www.", "@");
            ViewData.Add("Contact", contact);
            string body = EmailHelper.ToHtml("email_freeppt_details", ViewData, this.ControllerContext);
            string subject = "SODA:Free PPT Customer Details";
            EmailHelper.SendEmail(new System.Net.Mail.MailAddress(from, "Safety On Demand"), new System.Net.Mail.MailAddress(to), subject, body, false, null,"P@ssw0rd12345");
        }
        public ActionResult email_freeppt_details()
        {
            return View();
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
           
            if(contact.Count() >0 && (contact.First().DateLinkEx.Value - DateTime.Now).Hours < 1)
            {
              
                ViewBag.selected = selected;
                return View();
            }
            return RedirectToAction("index");
        }

       
        public FileStreamResult StreamFileFromDisk(string selected)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "Content/download/";
            var freeppt = from fppt in cmsServiceClient.getFreePPT()
                          join region in cmsServiceClient.getRegions() on fppt.RegionId equals region.Id
                          where region.WebsiteUrl == Request.Url.Host && fppt.PPTType == selected
                          select fppt;
            string filename = "";
            if(freeppt.Count() > 0)
            {
                filename = freeppt.First().FileName;
            }
            else
            {
                //error
                //return RedirectToAction("index");
            }
            //string filename = ;
            return File(new System.IO.FileStream(path + filename, System.IO.FileMode.Open), "application/vnd.ms-powerpoint", filename);
        }

    }
}

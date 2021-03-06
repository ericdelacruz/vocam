﻿using System;
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
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SODAMvcApplication.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        private CMSServiceReference.CMS_ServiceClient cmsServiceClient = new CMSServiceReference.CMS_ServiceClient();
        private CategoriesServiceReference.CatListingServiceClient catListingSerivceClient = new CategoriesServiceReference.CatListingServiceClient();
      
        private string defaultRegion = "AU";
        
        private string password = "myS0D@P@ssw0rd";

        private const string HOME = "Home";
        private const string CONTACT = "Contact";
        private const string LEARN = "Learn";

       
      
        public ActionResult Index()
        {


            if (!Request.Url.Host.Contains("www.") && Request.Url.Host != "localhost")
                return Redirect("http://www." + Request.Url.Host);

              
              var  lContentDef = cmsServiceClient.getContent(HOME, string.Empty);

              var filterByRegion = getContents(lContentDef);
              
               
              var contact = cmsServiceClient.getContent(CONTACT,"PhoneNo").Where(cms=>cms.RegionId == filterByRegion.First().RegionId).First().Value;
              
            
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
            else//for testing purpose only
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

            initFreePPtViewBag();
            return View();
        }

        private void initFreePPtViewBag()
        {
            var freeppt = from fppt in cmsServiceClient.getFreePPT()
                          join region in cmsServiceClient.getRegions() on fppt.RegionId equals region.Id
                          where region.WebsiteUrl == Request.Url.Host
                          select fppt;
            //For debugging purpose (localhost)
            if (freeppt.Count() == 0 && Request.Url.Host == "localhost")
            {
                freeppt = from fppt in cmsServiceClient.getFreePPT()
                          join region in cmsServiceClient.getRegions() on fppt.RegionId equals region.Id
                          where region.RegionName == defaultRegion
                          select fppt;
            }

            ViewBag.pptList = freeppt.Select(fppt => new SelectListItem()
            {
                Text = fppt.DisplayText,
                Value = fppt.PPTType,
                Selected = false

            });
        }

        [HttpPost]
        public ActionResult contactFreeppt(CMSServiceReference.Contact contact, FormCollection collection)
        {

            bool isValid = true;
            /*validations*/
            if (!this.IsCaptchaValid("Captcha is not valid"))
            {
                ModelState.AddModelError("", "Incorrect captcha answer.");
                //return View(contact);
                isValid = false;
            }
           
            if (!RegExHelper.IsValidEmail(contact.Email))
            {
                ModelState.AddModelError("", "Please enter a valid email.");
                //return (View(contact));
                isValid = false;
            }
            else if (!RegExHelper.IsValidPhone(contact.Phone))
            {
                ModelState.AddModelError("", "Please enter a valid Phone Number");
               // return (View(contact));
                isValid = false;
            }
            else if (!RegExHelper.isValidPostal(contact.Postcode))
            {
                ModelState.AddModelError("", "Please enter a valid Postal Code");
                //return (View(contact));
                isValid = false;
            }

            //Send download link if email not yet exisiting and validation is ok else error. Also notifies Sales
            if (cmsServiceClient.getContact().Where(c => c.Email.Trim() == contact.Email.Trim()).Count() == 0 && isValid)
            {
                sendDownloadLinkToCustomer(contact, collection);

                sendCustomerDetailsToSales(contact, "SODA:Free PPT Customer Details");
                return RedirectToAction("Replyfreeppt");
            }
            else
            {
                ModelState.AddModelError("", "Email already exists! Sorry you can only download once.");
                
            }

           
            initFreePPtViewBag();
            return View(contact);
        }

        //GET: /Sitemap/
        public ActionResult Sitemap()
        {
           
            var sitemapData = from cat in catListingSerivceClient.get_Categories()
                              where cat.CategoryId > 1 && cat.CategoryName.Trim() != "My Favorites" && cat.CategoryName.Trim() != "Downloads" 
                              orderby ConvertGrade(cat.CategoryId)
                              select new Models.SiteMapModel()
                              {
                                  CatName = cat.CategoryName,
                                  Titles = getTitlesUnderCat(cat.CategoryId),//For each cattegory get Titles
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
            else//for debugging purpose
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
            /*Validation*/
            if (!this.IsCaptchaValid("Captcha is not valid"))
            {
                ModelState.AddModelError("", "Incorrect captcha answer.");
                return View(contact);
            }
            if(!RegExHelper.IsValidEmail(contact.Email))
            {
                ModelState.AddModelError("", "Please enter a valid email.");
                return (View(contact));
            }
            else if(!RegExHelper.IsValidPhone(contact.Phone))
            {
                ModelState.AddModelError("", "Please enter a valid Phone Number");
                return (View(contact));
            }
            else if(!RegExHelper.isValidPostal(contact.Postcode))
            {
                ModelState.AddModelError("", "Please enter a valid Postal Code");
                return (View(contact));
            }
            
            contact.isFreePPT = false;
            cmsServiceClient.addContact(contact);
            //send email to customer and sales
            string from =Request.Url.Host != "localhost"? "no-reply" + Request.Url.Host.Replace("www.", "@"):"test@sac-iis.com";
            string replyto = Request.Url.Host != "localhost"? "sales" + Request.Url.Host.Replace("www.", "@"):"sales@safetyondemand.com.au";
           
            ViewData.Add("Contact", contact);
            string body = EmailHelper.ToHtml("contactemail",ViewData,this.ControllerContext);
            EmailHelper.SendEmail(new System.Net.Mail.MailAddress(from, "Safety On Demand"), new System.Net.Mail.MailAddress(contact.Email),"SODA Customer Inquiry", body,true, replyto);
            sendCustomerDetailsToSales(contact, "SODA:Customer Inquiry");
            TempData["MsgSent"] = true;
            return RedirectToAction("contact");
            
        }

       
        //
        //GET: /LearnMore/
         
        public ActionResult LearnMore()
        {
            


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
        
       

        private void sendDownloadLinkToCustomer(CMSServiceReference.Contact contact, FormCollection collection)
        {
            contact.isFreePPT = true;
            DateTime dateAdded = DateTime.Now;
            contact.isVerified = false;
            //key is email plus dateadded.Ticks in MD5 hash format
            string key = EmailHelper.GetMd5Hash(contact.Email.Substring(0, contact.Email.IndexOf('@')) + dateAdded.Ticks.ToString());
            string selected = collection["selectppt"];
            contact.key = key;
            contact.DateLinkEx = dateAdded;
            cmsServiceClient.addContact(contact);
            string from = Request.Url.Host != "localhost" ? "no-reply" + Request.Url.Host.Replace("www.", "@") : "test@sac-iis.com";
            string Replyto = Request.Url.Host != "localhost" ? "sales_test" + Request.Url.Host.Replace("www.", "@") : "sales_test@safetyondemand.com.au";
            ViewData.Add("Contact", contact);
            ViewData.Add("key", key);
            ViewData.Add("selected", collection["selectppt"]);
            string body = EmailHelper.ToHtml("email_freeppt_customer", ViewData, this.ControllerContext);
            EmailHelper.SendEmail(new System.Net.Mail.MailAddress(from, "Safety On Demand"), new System.Net.Mail.MailAddress(contact.Email), "SODA:Free Powerpoint download Link", body, false, Replyto,"P@ssw0rd12345");
            //EmailHelper.SendEmail(contact.Email, "SODA:Free Powerpoint download", createDownloadPPTLink(key,selected),null);
        }
        public ActionResult email_freeppt_customer()
        {
            return View();
        }
        private void sendCustomerDetailsToSales(CMSServiceReference.Contact contact,string subject)
        {
            
            string from = Request.Url.Host != "localhost" ? "no-reply" + Request.Url.Host.Replace("www.", "@") : "no-reply@sac-iis.com";
            string to = Request.Url.Host != "localhost" ? "sales" + Request.Url.Host.Replace("www.", "@") : "sales_test@sac-iis.com";
            
            
            if(ViewData["Contact"] == null)
            ViewData.Add("Contact", contact);
            string body = EmailHelper.ToHtml("email_freeppt_details", ViewData, this.ControllerContext);
            //string subject = "SODA:Free PPT Customer Details";
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
            //get free ppt details depending on the region
            var freeppt = from fppt in cmsServiceClient.getFreePPT()
                          join region in cmsServiceClient.getRegions() on fppt.RegionId equals region.Id
                          where region.WebsiteUrl == Request.Url.Host && fppt.PPTType.ToLower().Trim() == selected.ToLower().Trim()
                          select fppt;
            //for debugging purposes only
            if (freeppt.Count() == 0 && Request.Url.Host == "localhost")
            {
                freeppt = from fppt in cmsServiceClient.getFreePPT()
                          join region in cmsServiceClient.getRegions() on fppt.RegionId equals region.Id
                          where region.RegionName == defaultRegion && fppt.PPTType.ToLower().Trim() == selected.ToLower().Trim()
                          select fppt;
            }
            //if contact has a match and not yet expired then verify and update contact
            if(contact.Count() >0 && (contact.First().DateLinkEx.Value - DateTime.Now).Hours < 1 && contact.First().isVerified == false)
            {
              
                ViewBag.selected = selected;
                ViewBag.pptName = freeppt.First().DisplayText; 
                //update isverified flag to true
                var orig_contact = contact.First();
                orig_contact.isVerified = true;
                cmsServiceClient.updateContact(orig_contact);
                
                return View();
            }
            return RedirectToAction("index");
        }

        //link to stream or download file
        public FileStreamResult StreamFileFromDisk(string selected)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "Content/download/";
            //get the filename of the select ppt file to be downloaded by the customer. 
            var freeppt = from fppt in cmsServiceClient.getFreePPT()
                          join region in cmsServiceClient.getRegions() on fppt.RegionId equals region.Id
                          where (region.WebsiteUrl == Request.Url.Host || (Request.Url.Host =="localhost" && region.Id ==12)) && fppt.PPTType == selected
                          select fppt;
            string filename = "";
            if(freeppt.Count() > 0)
            {
                filename = freeppt.First().FileName;
            }
            //if freeppt.First().Url is null then look the file in the local directory.
            if(string.IsNullOrEmpty(freeppt.First().Url))
            return File(new System.IO.FileStream(path + filename, System.IO.FileMode.Open), "application/vnd.ms-powerpoint", filename);
            else
                return File(new System.IO.FileStream(freeppt.First().Url, System.IO.FileMode.Open), "application/vnd.ms-powerpoint", filename);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMSMvcApplication.Controllers
{
    public class PagesController : Controller
    {
        private CMSServiceReference.CMS_ServiceClient cmsService = new CMSServiceReference.CMS_ServiceClient();
        
        //
        // GET: /Pages/

        public ActionResult Index()
        {
            if (Session["Username"] == null)
                return RedirectToAction("login", "Home");

            return View();
        }
        //
        //GET: /Pages/Home
        public ActionResult EditHome()
        {
            if (Session["Username"] == null)
                return RedirectToAction("login", "Home");
            var HomeContent = from homeContent in cmsService.getContent("Home", string.Empty)
                              select homeContent;
            return View(HomeContent);
        }
        [HttpPost]
        public ActionResult EditHome(FormCollection collection)
        {
            //UpdateContent("header_banner_bg_IMG", "href", collection["TOP_BG"]);
            UpdateImageContent(cmsService.getContent("Home","header_banner_bg_IMG").First(),"TOP_BG");
                
            
            UpdateHomeContent("header_banner_Title", "text", collection["Top_Headline1"]);
            UpdateHomeContent("header_banner_txt", "text", collection["Top_SubHeadline2"]);

            UpdateImageContent(cmsService.getContent("Home", "thumbPost_thum_Pic_href").First(), "FP1_Thumb");   
            UpdateHomeContent("thumbPost_thum_Title", "text", collection["FTitle1"]);
            UpdateHomeContent("thumbPost_thum_details", "text", collection["fDesc1"]);
            UpdateHomeContent("thumbPost_thum_href", "href", collection["FURL1"]);

            //UpdateContent("thumbPost_thum_Pic_href", "href", collection["FP2_Thumb"]);
            UpdateImageContent(cmsService.getContent("Home", "thumbPost_thum2_Title").First(), "FP2_Thumb");
            UpdateHomeContent("thumbPost_thum2_Title", "text", collection["FTitle2"]);
            UpdateHomeContent("thumbPost_thum2_Details", "text", collection["fDesc2"]);

            UpdateHomeContent("thumbPost_thum2_href", "href", collection["FURL1"]);

            UpdateImageContent(cmsService.getContent("Home", "thumbPost_thum3_Title").First(), "FP3_Thumb");
            UpdateHomeContent("thumbPost_thum3_Title", "text", collection["FTitle3"]);
            UpdateHomeContent("thumbPost_thum3_Details", "text", collection["fDesc3"]);
            UpdateHomeContent("thumbPost_thum3_href", "href", collection["FURL3"]);

            UpdateHomeContent("footer_banner_Title", "text", collection["Bottom_Headline2"]);
            UpdateHomeContent("footer_banner_Desc", "text", collection["Bottom_Subheadline2"]);
            return RedirectToAction("Index");
        }

        private void UpdateImageContent(CMSServiceReference.ContentDef contentDef, string index)
        {
            if (contentDef.Value != Request.Files[index].FileName && !string.IsNullOrEmpty(Request.Files[index].FileName))
            {
                UpdateHomeContent(contentDef.SectionName, contentDef.Type, string.Concat(Request.Url.GetLeftPart(UriPartial.Authority),FileTransferHelper.UploadImage(Request.Files[index],Server)));
            }
        }

        private void UpdateHomeContent(string sectionName, string strType, string strValue)
        {
            cmsService.UpdateContent(new CMSServiceReference.ContentDef()
            {
                PageCode = "Home",
                SectionName = sectionName,
                Value = strValue.Replace("\n","<br/>").Trim(),
                Type = strType
            });
        }

       

       
        //
        //GET /Pages/Contact
        public ActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Contact(FormCollection collection)
        {
            return View();
        }
        //
        //GET: /Pages/EditLearnMore
        public ActionResult EditLearnMore()
        {
            var LearnContent = from content in cmsService.getContent("Learn",string.Empty)
                               select content;
            return View(LearnContent);
        }

        [HttpPost]
        public ActionResult EditLearnMore(FormCollection collection)
        {
            UpdateLearnContent("LearnMore_Title", "text", collection["Title"]);
            UpdateLearnContent("LearnMore_Desc", "text", collection["Desc"]);
            return RedirectToAction("Index");
        }

        private void UpdateLearnContent(string sectionName, string strType, string strValue)
        {
            cmsService.UpdateContent(new CMSServiceReference.ContentDef()
            {
                PageCode = "learn",
                SectionName = sectionName,
                Value = strValue.Replace("\n", "<br/>"),
                Type = strType
            });
        }
    }
}

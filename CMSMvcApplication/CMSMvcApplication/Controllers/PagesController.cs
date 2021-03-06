﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMSMvcApplication.Controllers
{
    public class PagesController : Controller
    {
        private CMSServiceReference.CMS_ServiceClient cmsService = new CMSServiceReference.CMS_ServiceClient();
        private PortalServiceReference.PortalServiceClient portalClient = new PortalServiceReference.PortalServiceClient();
        //
        // GET: /Pages/

        public ActionResult Index()
        {
            //redirect to login page if session expired
            if (Session["Username"] == null)
                return RedirectToAction("login", "Home");
            ViewBag.defaultRegion = portalClient.getRegion().Where(r => r.RegionName.ToLower() == "au").First();
            return View();
        }
        protected override void Dispose(bool disposing)
        {
            cmsService.Close();
            portalClient.Close();
            base.Dispose(disposing);
        }
        //
        //GET: /Pages/Home
        public ActionResult EditHome(string region)
        {
            //id is region name
            if (Session["Username"] == null)
                return RedirectToAction("login", "Home");
            //if region is null default to au
            if (region == null)
                region = "au";
            var Selected_region = initRegion(region);
            //get content data for home filter by the region selected
            var HomeContent = from homeContent in cmsService.getContent("Home", string.Empty)
                              join r in Selected_region on homeContent.RegionId equals r.Id                          
                              select homeContent;
            return View(HomeContent);
        }

        private IEnumerable<PortalServiceReference.Region> initRegion(string region)
        {
            
            var regionList = portalClient.getRegion();
            //for region dropdown box
            ViewBag.RegionList = regionList.Select(r => new SelectListItem()
            {
                Text = r.RegionName,
                Value = r.RegionName,
                Selected = r.RegionName.ToLower() == region.ToLower()
            });

            var Selected_region = regionList.Where(r => r.RegionName.ToLower() == region.ToLower());
            //if region name exists, set to session["Region"]
            if (Selected_region.Count() > 0)
            {
                Session["Region"] = Selected_region.First();
            }
            return Selected_region;
        }
        [HttpPost]
        public ActionResult EditHome(FormCollection collection)
        {
            /*Update contents*/
           
            UpdateImageContent(cmsService.getContent("Home","header_banner_bg_IMG").First(),"TOP_BG");

            UpdateHomeContent("previewNowBtn_href", "href", collection["PREVURL"]);
            UpdateHomeContent("header_banner_Title", "text", collection["Top_Headline1"]);
            UpdateHomeContent("header_banner_txt", "text", collection["Top_SubHeadline2"]);

            UpdateImageContent(cmsService.getContent("Home", "thumbPost_thum_Pic_href").First(), "FP1_Thumb");   
            UpdateHomeContent("thumbPost_thum_Title", "text", collection["FTitle1"]);
            UpdateHomeContent("thumbPost_thum_details", "text", collection["fDesc1"]);
            UpdateHomeContent("thumbPost_thum_href", "href", collection["FURL1"]);

           
            UpdateImageContent(cmsService.getContent("Home", "thumbPost_thum2_Pic_href").First(), "FP2_Thumb");
            UpdateHomeContent("thumbPost_thum2_Title", "text", collection["FTitle2"]);
            UpdateHomeContent("thumbPost_thum2_Details", "text", collection["fDesc2"]);

            UpdateHomeContent("thumbPost_thum2_href", "href", collection["FURL2"]);

            UpdateImageContent(cmsService.getContent("Home", "thumbPost_thum3_Pic_href").First(), "FP3_Thumb");
            UpdateHomeContent("thumbPost_thum3_Title", "text", collection["FTitle3"]);
            UpdateHomeContent("thumbPost_thum3_Details", "text", collection["fDesc3"]);
            UpdateHomeContent("thumbPost_thum3_href", "href", collection["FURL3"]);

            UpdateHomeContent("footer_banner_Title", "text", collection["Bottom_Headline2"]);
            UpdateHomeContent("footer_banner_Desc", "text", collection["Bottom_Subheadline2"]);
            return RedirectToAction("Index");
        }
        /// <summary>
        /// Upload and update the contents that are image type
        /// </summary>
        /// <param name="contentDef">contentdef data</param>
        /// <param name="index">Index in the Request.files</param>
        private void UpdateImageContent(CMSServiceReference.ContentDef contentDef, string index)
        {
            //if the filename is not the same and request file is not null, then proceed with the update
            if (contentDef.Value != Request.Files[index].FileName && !string.IsNullOrEmpty(Request.Files[index].FileName))
            {
                //uploads first then update home content
                UpdateHomeContent(contentDef.SectionName, contentDef.Type, string.Concat(Request.Url.GetLeftPart(UriPartial.Authority),FileTransferHelper.UploadImage(Request.Files[index],Server)));
            }
        }
        /// <summary>
        /// Update the ContentDef for Home
        /// </summary>
        /// <param name="sectionName">Section of the Home Page to be updated</param>
        /// <param name="strType">Type of Content</param>
        /// <param name="strValue">Value of the content</param>
        private void UpdateHomeContent(string sectionName, string strType, string strValue)
        {
            //get the current contedef to be updated
            var old_content = cmsService.getContent("Home", sectionName).First();
            //init content to be updated
            var new_Content = new CMSServiceReference.ContentDef()
            {
                PageCode = "Home",
                SectionName = sectionName,
                //replaced next line char to <br/>
                Value = strValue.Replace("\n", "<br/>").Trim(),
                Type = strType,
                RegionId = (Session["Region"] as PortalServiceReference.Region).Id
            };
            //log
            AuditLoggingHelper.LogUpdateAction(Session["Username"].ToString(), old_content, new_Content, portalClient);
            //update content
            cmsService.UpdateContent(new_Content);
        }

       

       
        //
        //GET /Pages/Contact
        public ActionResult editcontact(string region)
        {
           //default region to au
            if (region == null)
                region = "au";
            
            var Selected_region = initRegion(region);
            //get the phone no depending on the region
            var phoneNoList = from content in cmsService.getContent("Contact","PhoneNo")
                              join r in Selected_region on content.RegionId equals r.Id
                              select content;
            ViewBag.phoneno = phoneNoList.Count() >0 ? phoneNoList.First().Value : "";
            
            return View();
        }
        [HttpPost]
        public ActionResult editcontact(string region,FormCollection collection)
        {
            string phoneno = collection["PhoneNo"];
            //update phone no
            var old_content = cmsService.getContent("Contact", "PhoneNo").First();
            var new_content = new CMSServiceReference.ContentDef()
            {
                PageCode = "Contact",
                SectionName = "PhoneNo",
                Value = phoneno,
                Type = "text",
                RegionId = (Session["Region"] as PortalServiceReference.Region).Id

            };
            AuditLoggingHelper.LogUpdateAction(Session["Username"].ToString(), old_content, new_content, portalClient);
            cmsService.UpdateContent(new_content);
            return RedirectToAction("index");
        }
        //
        //GET: /Pages/EditLearnMore
        public ActionResult EditLearnMore(string region)
        {
            if (region == null)
                region = "au";
            var Selected_region = initRegion(region);
            //get learn more content by region
            var LearnContent = from content in cmsService.getContent("Learn",string.Empty)
                               join r in Selected_region on content.RegionId equals r.Id
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
        /// <summary>
        /// Update the content of Learn More
        /// </summary>
        /// <param name="sectionName">Section of the learn more page to be updated</param>
        /// <param name="strType">Type of content</param>
        /// <param name="strValue">Value of the content</param>
        private void UpdateLearnContent(string sectionName, string strType, string strValue)
        {
            var old_content = cmsService.getContent("learn", sectionName).First();
            var new_content = new CMSServiceReference.ContentDef()
            {
                PageCode = "learn",
                SectionName = sectionName,
                Value = strValue.Replace("\n", "<br/>"),
                Type = strType,
                RegionId = (Session["Region"] as PortalServiceReference.Region).Id

            };
            AuditLoggingHelper.LogUpdateAction(Session["Username"].ToString(), old_content, new_content, portalClient);
            cmsService.UpdateContent(new_content);
        }
    }
}

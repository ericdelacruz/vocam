using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMSMvcApplication.Controllers
{
    public class powerpointController : Controller
    {

        CMSServiceReference.CMS_ServiceClient cmsClient = new CMSServiceReference.CMS_ServiceClient();
        PortalServiceReference.PortalServiceClient portalClient = new PortalServiceReference.PortalServiceClient();//for resusing region adapter and logging purposes

        //
        // GET: /powerpoint/

        public ActionResult Index()
        {
            //get the free ppt list depending on the region
            var freePptList = from fppt in cmsClient.getFreePPT()
                              join region in portalClient.getRegion() on fppt.RegionId equals region.Id
                              select new CMSMvcApplication.ViewModels.FreePPTModel() { DisplayName = fppt.DisplayText,RegionName = region.RegionName, Id = fppt.Id };
            return View(freePptList);
        }
        public ActionResult Create()
        {
            //region list for the dropdown box
            var regionList = portalClient.getRegion().Select(r => new SelectListItem()
            {
                Text = r.RegionName,
                Value = r.Id.ToString(),
                Selected = false
            });
            ViewBag.regionList = regionList;
            return View();
        }
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
           //upload file and get the file dir
            var fileloc = uploadFile(Request.Files["pptFile"],null);
            var fileUrl = string.Concat(AppDomain.CurrentDomain.BaseDirectory, fileloc);
            //init free ppt object to be added
            var new_freeppt = new CMSServiceReference.FreePPT()
            {
                DisplayText = collection["DisplayText"],
                FileName = Request.Files["pptFile"].ContentLength >0? Request.Files["pptFile"].FileName:"",
                PPTType = collection["ppttype"],
                RegionId = int.Parse(collection["region"]),
                 Url = fileUrl
            };
            //log 
            AuditLoggingHelper.LogCreateAction(Session["Username"].ToString(), new_freeppt, portalClient);
            //add to DB
            cmsClient.addfreePPT(new_freeppt);
            return RedirectToAction("index");
        }

        private string uploadFile(HttpPostedFileBase httpPostedFileBase,string fileToBeDel)
        {
            if(!string.IsNullOrEmpty(fileToBeDel))
            {
                FileTransferHelper.DeleteFile(fileToBeDel, Server);
            }
            return FileTransferHelper.UploadFile(httpPostedFileBase, Server);
            
        }
        public ActionResult Edit(int id)
        {
            //get the freeppt data to be edited
            var freePPT = cmsClient.getFreePPT().Where(ppt => ppt.Id == id).First();
            //get region list for the dropdownbox
            var regionSelList = portalClient.getRegion().Select(r => new SelectListItem()
            {
                Text = r.RegionName,
                Value = r.Id.ToString(),
                Selected = freePPT.RegionId == r.Id
            });
            ViewBag.regionList = regionSelList;
            return View(freePPT);
        }
        [HttpPost]
        public ActionResult Edit(int id,FormCollection collection)
        {
            //get the current freeppt data
            var old_freePPT = cmsClient.getFreePPT().Where(ppt => ppt.Id == id).First();
            //upload and get the file directry
            var fileloc = Request.Files["pptFile"].ContentLength >0? uploadFile(Request.Files["pptFile"],old_freePPT.FileName):"";
            var fileUrl = Request.Files["pptFile"].ContentLength > 0 ? string.Concat(AppDomain.CurrentDomain.BaseDirectory, fileloc) : old_freePPT.Url;
            //init freeppt to be updated
            var new_freePPT = new CMSServiceReference.FreePPT()
            {
                DisplayText = collection["DisplayText"],
                FileName = Request.Files["pptFile"].ContentLength > 0 ? Request.Files["pptFile"].FileName : old_freePPT.FileName,
                PPTType = collection["ppttype"],
                RegionId = int.Parse(collection["region"]),
                Id = old_freePPT.Id,
                Url = fileUrl
            };
            //log
            AuditLoggingHelper.LogUpdateAction(Session["Username"].ToString(), old_freePPT, new_freePPT, portalClient);
            
            cmsClient.updateFreePPT(new_freePPT);
            return RedirectToAction("index");
        }
        public ActionResult delete(int id)
        {
           //get the free ptt to be deleted
            var old_freePPT = cmsClient.getFreePPT().Where(ppt => ppt.Id == id).First();
            FileTransferHelper.DeleteFile(old_freePPT.FileName, Server);
            //log
            AuditLoggingHelper.LogDeleteAction(Session["Username"].ToString(), old_freePPT, portalClient);
            cmsClient.deleteFreePPT(id);
            return RedirectToAction("index");

        }
        //public void deleteppt(int id)
        //{
        //    var old_freePPT = cmsClient.getFreePPT().Where(ppt => ppt.Id == id).First();
        //    FileTransferHelper.DeleteFile(old_freePPT.FileName, Server);

        //}
        protected override void Dispose(bool disposing)
        {
            cmsClient.Close();
            base.Dispose(disposing);
        }
        protected override IAsyncResult BeginExecute(System.Web.Routing.RequestContext requestContext, AsyncCallback callback, object state)
        {
            if (requestContext.HttpContext.Session["Username"] == null)
                requestContext.HttpContext.Response.Redirect("~/home");
            return base.BeginExecute(requestContext, callback, state);



        }
    }
}

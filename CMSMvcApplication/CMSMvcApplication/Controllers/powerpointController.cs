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
            var freePptList = from fppt in cmsClient.getFreePPT()
                              join region in portalClient.getRegion() on fppt.RegionId equals region.Id
                              select new CMSMvcApplication.ViewModels.FreePPTModel() { DisplayName = fppt.DisplayText,RegionName = region.RegionName, Id = fppt.Id };
            return View(freePptList);
        }
        public ActionResult Create()
        {
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
            var fileloc = uploadFile(Request.Files["pptFile"],null);
            var fileUrl = string.Concat(AppDomain.CurrentDomain.BaseDirectory, fileloc);

            var new_freeppt = new CMSServiceReference.FreePPT()
            {
                DisplayText = collection["DisplayText"],
                FileName = Request.Files["pptFile"].ContentLength >0? Request.Files["pptFile"].FileName:"",
                PPTType = collection["ppttype"],
                RegionId = int.Parse(collection["region"]),
                 Url = fileUrl
            };
            AuditLoggingHelper.LogCreateAction(Session["Username"].ToString(), new_freeppt, portalClient);
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
            var freePPT = cmsClient.getFreePPT().Where(ppt => ppt.Id == id).First();
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
            var old_freePPT = cmsClient.getFreePPT().Where(ppt => ppt.Id == id).First();
            
            var fileloc = Request.Files["pptFile"].ContentLength >0? uploadFile(Request.Files["pptFile"],old_freePPT.FileName):"";
            var fileUrl = Request.Files["pptFile"].ContentLength > 0 ? string.Concat(AppDomain.CurrentDomain.BaseDirectory, fileloc) : old_freePPT.Url;

            var new_freePPT = new CMSServiceReference.FreePPT()
            {
                DisplayText = collection["DisplayText"],
                FileName = Request.Files["pptFile"].ContentLength > 0 ? Request.Files["pptFile"].FileName : old_freePPT.FileName,
                PPTType = collection["ppttype"],
                RegionId = int.Parse(collection["region"]),
                Id = old_freePPT.Id,
                Url = fileUrl
            };
            AuditLoggingHelper.LogUpdateAction(Session["Username"].ToString(), old_freePPT, new_freePPT, portalClient);
            cmsClient.updateFreePPT(new_freePPT);
            return RedirectToAction("index");
        }
        public ActionResult delete(int id)
        {
            var old_freePPT = cmsClient.getFreePPT().Where(ppt => ppt.Id == id).First();
            FileTransferHelper.DeleteFile(old_freePPT.FileName, Server);
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

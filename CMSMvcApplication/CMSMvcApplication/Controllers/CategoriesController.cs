using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
namespace CMSMvcApplication.Controllers
{
    public class CategoriesController : Controller
    {
        CatListingServiceReference.CatListingServiceClient catListingClient = new CatListingServiceReference.CatListingServiceClient();
        PortalServiceReference.PortalServiceClient portalClient = new PortalServiceReference.PortalServiceClient();
        //
        // GET: /Categories/
       
        public ActionResult Index(int? page)
        {
            if (Session["Username"] == null)
                return RedirectToAction("login", "Home");
            //get categories 
            var catList = from cat in catListingClient.get_Categories()
                          where cat.CategoryId != 1
                         
                          select cat;
            //default region to au
            ViewBag.defaultRegion = portalClient.getRegion().Where(r => r.RegionName.ToLower() == "au").First();

            int pageMaxSize = 15;
            int pageNumber = page ?? 1;
            return View(catList.ToPagedList(pageNumber,pageMaxSize));
        }

      
        // GET: /Categories/Create
        protected override void Dispose(bool disposing)
        {
            catListingClient.Close();
            portalClient.Close();
            base.Dispose(disposing);
        }
        public ActionResult Create()
        {
            if (Session["Username"] == null)
                return RedirectToAction("login", "Home");
            return View();
        }

        //
        // POST: /Categories/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            
            try
            {
                //init new category to be added
                var new_cat = new CatListingServiceReference.Category()
                {
                    CategoryName = collection["CategoryTitle"].Replace("\n", "<br/>"),
                    Overview = collection["Overview"].Replace("\n", "<br/>"),
                    Description = collection["Description"].Replace("\n", "<br/>"),
                    Metatags = collection["CategoryMetaKeywords"],
                    MetaDesc = collection["CategoryMetaDescription"],
                    PageTitile = collection["CategoryPageTitle"],
                    IMG_URL = !string.IsNullOrEmpty(Request.Files["Thumb"].FileName) ? string.Concat(Request.Url.GetLeftPart(UriPartial.Authority), FileTransferHelper.UploadImage(Request.Files["Thumb"], Server)) : "#",
                    Banner_IMG = !string.IsNullOrEmpty(Request.Files["Banner"].FileName) ? string.Concat(Request.Url.GetLeftPart(UriPartial.Authority), FileTransferHelper.UploadImage(Request.Files["Banner"], Server)) : "#",
                    BG_IMG = !string.IsNullOrEmpty(Request.Files["BG"].FileName) ? string.Concat(Request.Url.GetLeftPart(UriPartial.Authority), FileTransferHelper.UploadImage(Request.Files["BG"], Server)) : "#"
                };
                //log
                AuditLoggingHelper.LogCreateAction(Session["Username"].ToString(), new_cat, portalClient);
                //add to db
                catListingClient.add_Category(new_cat);
                
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                //return View();
                throw (ex);
            }
        }

       

        //
        // GET: /Categories/Edit/5

        public ActionResult Edit(long id)
        {
            //redirect to login if session expire
            if (Session["Username"] == null)
                return RedirectToAction("login", "Home");
            CatListingServiceReference.Category category = new CatListingServiceReference.Category();
            category = catListingClient.get_Category(id).First();

            return View(category);
        }

        //
        // POST: /Categories/Edit/5

        [HttpPost]
        public ActionResult Edit(long id, FormCollection collection)
        {
            try
            {
                //get the cat to be edited for logging
                CatListingServiceReference.Category catTemp = catListingClient.get_Category(id).First();
                //init the updated cat
                var new_cat = new CatListingServiceReference.Category()
                {
                    CategoryId = id,
                    CategoryName = collection["CategoryTitle"],
                    Overview = collection["Overview"],
                    Description = collection["Description"],
                    Metatags = collection["CategoryMetaKeywords"],
                    MetaDesc = collection["CategoryMetaDescription"],
                    PageTitile = collection["CategoryPageTitle"],
                    IMG_URL = !string.IsNullOrEmpty(Request.Files["Thumb"].FileName) ? string.Concat(Request.Url.GetLeftPart(UriPartial.Authority), FileTransferHelper.UploadImage(Request.Files["Thumb"], Server)) : catTemp.IMG_URL,
                    Banner_IMG = !string.IsNullOrEmpty(Request.Files["Banner"].FileName) ? string.Concat(Request.Url.GetLeftPart(UriPartial.Authority), FileTransferHelper.UploadImage(Request.Files["Banner"], Server)) : catTemp.Banner_IMG,
                    BG_IMG = !string.IsNullOrEmpty(Request.Files["BG"].FileName) ? string.Concat(Request.Url.GetLeftPart(UriPartial.Authority), FileTransferHelper.UploadImage(Request.Files["BG"], Server)) : catTemp.BG_IMG
                };
                //log
                AuditLoggingHelper.LogUpdateAction(Session["Username"].ToString(), catTemp, new_cat, portalClient);
                //update to DB
                catListingClient.update_Category(new_cat);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                return View();
            }
        }

        //
        // GET: /Categories/Delete/5

      
        public ActionResult Delete(long id)
        {
            try
            {
                //get the cat to be delete for logging
                var old_cat = catListingClient.get_Category(id).First();
                //log
                AuditLoggingHelper.LogDeleteAction(Session["Username"].ToString(), old_cat, portalClient);
                //delete
                catListingClient.delete_Category(id);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                return View();
            }
        }

        
    }
}

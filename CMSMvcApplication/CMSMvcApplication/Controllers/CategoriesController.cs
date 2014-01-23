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
            var catList = from cat in catListingClient.get_Categories()
                          where cat.CategoryId != 1
                         
                          select cat;
            
            ViewBag.defaultRegion = portalClient.getRegion().Where(r => r.RegionName.ToLower() == "au").First();
            int pageMaxSize = 15;
            int pageNumber = page ?? 1;
            return View(catList.ToPagedList(pageNumber,pageMaxSize));
        }

        //
        // GET: /Categories/Details/5

        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        //
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
               
                catListingClient.add_Category(new CatListingServiceReference.Category()
                {
                    CategoryName = collection["CategoryTitle"].Replace("\n","<br/>"),
                    Overview = collection["Overview"].Replace("\n","<br/>"),
                    Description = collection["Description"].Replace("\n","<br/>"), 
                    Metatags = collection["CategoryMetaKeywords"],
                    MetaDesc = collection["CategoryMetaDescription"],
                    PageTitile = collection["CategoryPageTitle"],
                    IMG_URL = !string.IsNullOrEmpty(Request.Files["Thumb"].FileName)?string.Concat(Request.Url.GetLeftPart(UriPartial.Authority),FileTransferHelper.UploadImage(Request.Files["Thumb"],Server)):"#",
                    Banner_IMG = !string.IsNullOrEmpty(Request.Files["Banner"].FileName)?string.Concat(Request.Url.GetLeftPart(UriPartial.Authority),FileTransferHelper.UploadImage(Request.Files["Banner"], Server)):"#",
                    BG_IMG =!string.IsNullOrEmpty(Request.Files["BG"].FileName)? string.Concat(Request.Url.GetLeftPart(UriPartial.Authority),FileTransferHelper.UploadImage(Request.Files["BG"],Server)):"#"
                });
                
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
                // TODO: Add update logic here
                CatListingServiceReference.Category catTemp = catListingClient.get_Category(id).First();
                catListingClient.update_Category(new CatListingServiceReference.Category()
                {
                    CategoryId = id,
                    CategoryName = collection["CategoryTitle"],
                    Overview = collection["Overview"],
                    Description = collection["Description"],
                    Metatags = collection["CategoryMetaKeywords"],
                    MetaDesc = collection["CategoryMetaDescription"],
                    PageTitile = collection["CategoryPageTitle"],
                    IMG_URL = !string.IsNullOrEmpty(Request.Files["Thumb"].FileName) ? string.Concat(Request.Url.GetLeftPart(UriPartial.Authority),FileTransferHelper.UploadImage(Request.Files["Thumb"], Server)) : catTemp.IMG_URL,
                    Banner_IMG = !string.IsNullOrEmpty(Request.Files["Banner"].FileName)?string.Concat(Request.Url.GetLeftPart(UriPartial.Authority),FileTransferHelper.UploadImage(Request.Files["Banner"],Server)): catTemp.Banner_IMG,
                    BG_IMG = !string.IsNullOrEmpty(Request.Files["BG"].FileName)?string.Concat(Request.Url.GetLeftPart(UriPartial.Authority),FileTransferHelper.UploadImage(Request.Files["BG"],Server)): catTemp.BG_IMG
                });
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
                // TODO: Add delete logic here
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

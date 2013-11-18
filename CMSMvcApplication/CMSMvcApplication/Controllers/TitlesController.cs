using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMSMvcApplication.Controllers
{
    public class TitlesController : Controller
    {
        private CatListingServiceReference.CatListingServiceClient catClient = new CatListingServiceReference.CatListingServiceClient();
       
        //
        // GET: /Titles/

        public ActionResult Index()
        {
            if (Session["Username"] == null)
                return RedirectToAction("login", "Home");
            var catTitleList = getTitles("", "");
            return View(catTitleList);
        }

        //
        // GET: /Titles/filter?title=test-1&cat=no-cat
        [HttpPost]
        public ActionResult filter(string title, string cat)
        {
            var catList = getTitles(title, cat);

            return View(catList);
        }
        [HttpPost]
        public ActionResult filterTitle(string title)
        {
            var catList = getTitles(title, "");

            return View(catList);
        }
        [HttpPost]
        public ActionResult filterCat(string cat)
        {
            var catList = getTitles("", cat);

            return View(catList);
        }
        private IEnumerable<ViewModels.Title> getTitles(string title, string cat)
        {
            

            var titleList = catClient.get();
            var catList = catClient.get_Categories();

            if (title != "" && cat == "")
            {
                
                var catTitle = from titleRow in titleList
                               join catRow in catList on titleRow.CategoryID equals catRow.CategoryId
                               where titleRow.Title == title
                               select new ViewModels.Title() { Category = catRow, Specific = titleRow };
                return catTitle;
            }
            else if (title == "" && cat != "")
            {
                var catTitle = from titleRow in titleList
                               join catRow in catList on titleRow.CategoryID equals catRow.CategoryId
                               where catRow.CategoryName == cat
                               select new ViewModels.Title() { Category = catRow, Specific = titleRow };

                return catTitle;
            }
            else  
            {
                
                var catTitle = from titleRow in titleList
                               join catRow in catList on titleRow.CategoryID equals catRow.CategoryId
                              
                               select new ViewModels.Title() { Category = catRow, Specific = titleRow };

                return catTitle;
            }
        }

        //
        // GET: /Titles/Create

        public ActionResult Create()
        {
            if (Session["Username"] == null)
                return RedirectToAction("login", "Home");
            ViewBag.catList = catClient.get_Categories();

            return View();
        }

        //
        // POST: /Titles/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                catClient.add_Specific(new CatListingServiceReference.Specific()
                {
                    Title = collection["Title"],
                    TitleCode = collection["Code"],
                    CategoryID = long.Parse(collection["Category"]),
                    Overview = collection["Overview"],
                    Description = collection["Description"],
                    PageTitle = collection["TitlePageTitle"],
                    Metatags = collection["TitleMetaKeywords"],
                    MetaDesc = collection["TitleMetaDescription"],
                    ImageURL = !string.IsNullOrEmpty(Request.Files["Thumb"].FileName)?string.Concat(Request.Url.GetLeftPart(UriPartial.Authority) ,FileTransferHelper.UploadImage(Request.Files["Thumb"],Server)):"#",
                    BG_Img = !string.IsNullOrEmpty(Request.Files["BG"].FileName) ? string.Concat(Request.Url.GetLeftPart(UriPartial.Authority) + "/", FileTransferHelper.UploadImage(Request.Files["BG"], Server)) : "#",
                   
                    VideoURL = collection["VidURL"],
                    Id = 0 
                });
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                

                return RedirectToAction("error");
            }
        }

        //
        // GET: /Titles/Edit/5

        public ActionResult Edit(long id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("login", "Home");
            var title = catClient.getSpecificByID(id).First();
            ViewBag.CatList = catClient.get_Categories();
            return View(title);
        }

        //
        // POST: /Titles/Edit/5

        [HttpPost]
        public ActionResult Edit(long id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                catClient.update_Specific(new CatListingServiceReference.Specific()
                {
                    Id = id,
                    TitleCode = collection["Code"],
                    Title = collection["Title"],
                    CategoryID = long.Parse(collection["Category"]),
                    Overview = collection["Overview"],
                    Description = collection["Description"],
                    PageTitle = collection["TitlePageTitle"],
                    Metatags = collection["TitleMetaKeywords"],
                    MetaDesc = collection["TitleMetaDescription"],
                    ImageURL = !string.IsNullOrEmpty(Request.Files["Thumb"].FileName) ? string.Concat(Request.Url.GetLeftPart(UriPartial.Authority), FileTransferHelper.UploadImage(Request.Files["Thumb"], Server)) : catClient.getSpecificByID(id).First().ImageURL,
                    BG_Img = !string.IsNullOrEmpty(Request.Files["BG_IMG"].FileName) ? string.Concat(Request.Url.GetLeftPart(UriPartial.Authority), FileTransferHelper.UploadImage(Request.Files["BG_IMG"], Server)) : catClient.getSpecificByID(id).First().BG_Img,
                    VideoURL = collection["VidURL"]

                });
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                throw (ex);
            }
        }

        //
        // GET: /Titles/Delete/5

        public ActionResult Delete(long id)
        {
            try
            {
                // TODO: Add delete logic here
                catClient.delete_Specific(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult error()
        {
            
            return View();
        }
        
        
    }
}

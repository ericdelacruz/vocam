using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
namespace SODAMvcApplication.Controllers
{
    public class CategoriesController : Controller
    {
        CategoriesServiceReference.CatListingServiceClient categoriesServiceClient = new CategoriesServiceReference.CatListingServiceClient();
        private string password = "myS0D@P@ssw0rd";
        //
        // GET: /Categories/
        /// <summary>
        /// List all Categories
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
             if(!categoriesServiceClient.Authenticate(password))
             {
                 //error page
             }

             var listCategories = from cat in categoriesServiceClient.get_Categories()
                                  where cat.CategoryId != 1
                                  select cat;
             return View(listCategories);
        }
        //
        //GET: /Categories/Browse?cID=catname
        /// <summary>
        /// List speccifics of selected category
        /// </summary>
        /// <param name="cID"></param>
        /// <returns></returns>
        public ActionResult Browse(string cID)
        {
            long lCatID = 0;
            long.TryParse(HttpUtility.HtmlEncode(cID), out lCatID);
            if (!categoriesServiceClient.Authenticate(password) || lCatID == 0)
            {
                //error page
            }
            //var listSpecByCat = categoriesServiceClient.getSpecificByCatID(lCatID);
            var listSpecByCat = from ca in categoriesServiceClient.getCatAssign()
                                join spec in categoriesServiceClient.get() on ca.SpecID equals spec.Id
                                where spec.CategoryID == lCatID
                                select spec;

            ViewBag.SelCategory = categoriesServiceClient.get_Category(lCatID).First();

            return View(listSpecByCat);
        }
        //
        //GET:/Categories/Details/1234
        /// <summary>
        /// View specfic details. Landing page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(long id)
        {
            if (!categoriesServiceClient.Authenticate(password))
            {
                //error page
            }
            var spec = categoriesServiceClient.getSpecificByID(id).First();
            ViewBag.SelCategory = categoriesServiceClient.get_Category(spec.CategoryID).First();
            try
            {
                ViewBag.Related = categoriesServiceClient.getRelatedByID(spec.Id);
            }
            catch(Exception ex)
            {
                //
            }
            return View(spec);
        }
    }
}

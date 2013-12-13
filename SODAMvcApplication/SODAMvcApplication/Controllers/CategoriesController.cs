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
                                  orderby ConvertGrade(cat.CategoryId)
                                  select cat;
             return View(listCategories);
        }

        private long ConvertGrade(long CatID)
        {
            //can be database but for now hard code
            return CatID == 22 ? 0 : CatID == 3 ? 1 : 4;
        }
        protected override void Dispose(bool disposing)
        {
            categoriesServiceClient.Close();
            base.Dispose(disposing);
        }
        //
        //GET: /Categories/Browse?cID=catname
        /// <summary>
        /// List speccifics of selected category
        /// </summary>
        /// <param name="cID"></param>
        /// <returns></returns>
        public ActionResult Browse(string cat)
        {
            string strCatName = HttpUtility.HtmlEncode(cat);


            long lCatID = getCategoryId(strCatName);
            //long.TryParse(HttpUtility.HtmlEncode(cID), out lCatID);
            if (!categoriesServiceClient.Authenticate(password) || lCatID == 0)
            {
                //error page
            }
            //var listSpecByCat = categoriesServiceClient.getSpecificByCatID(lCatID);
            var listSpecByCat = from ca in categoriesServiceClient.getCatAssign()
                                join spec in categoriesServiceClient.get() on ca.SpecID equals spec.Id
                                where ca.CategoryId == lCatID
                                select spec;

            ViewBag.SelCategory = categoriesServiceClient.get_Category(lCatID).First();
            Session.Add("CatID", lCatID.ToString());
            return View(listSpecByCat);
        }

        private long getCategoryId(string strCatName)
        {
            //long lCatID = 0;

            var category = categoriesServiceClient.get_Categories().Select(c => c).Where(c => c.CategoryName.ToLower().Replace("-"," ") == strCatName.Replace("-", " ").ToLower());

            
            return category.Count() > 0? category.First().CategoryId:0;
        }
        //
        //GET:/Categories/Details/1234
        /// <summary>
        /// View specfic details. Landing page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(string id,string catid)
        {
            

            if (!categoriesServiceClient.Authenticate(password))
            {
                //error page
            }
            //var spec = categoriesServiceClient.getSpecificByID(id).First();
            var spec = categoriesServiceClient.get().Select(s => s).Where(s => s.Title.ToLower().Replace("-"," ") == id.Replace("-", " ").ToLower());
            long lCatId = 0;
            if (catid == null)
                long.TryParse(Session["CatID"].ToString(), out lCatId);
            else
                long.TryParse(catid,out lCatId);
            ViewBag.SelCategory = categoriesServiceClient.get_Category(lCatId).First();
            
            try
            {
                //Random rnd = new Random();
                var listSpecByCat = from ca in categoriesServiceClient.getCatAssign()
                                    join s in categoriesServiceClient.get() on ca.SpecID equals s.Id
                                    where ca.CategoryId == lCatId && s.Id != spec.First().Id
                                    select s;
                ViewBag.Related = listSpecByCat;
                //ViewBag.Related = categoriesServiceClient.getRelatedByID(spec.First().Id);
            }
            catch(Exception ex)
            {
                //
            }
            return View(spec.First());
        }

         
    }
}

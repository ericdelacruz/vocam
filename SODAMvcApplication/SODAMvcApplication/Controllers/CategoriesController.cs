using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Configuration;
namespace SODAMvcApplication.Controllers
{
    public class CategoriesController : Controller
    {
        CategoriesServiceReference.CatListingServiceClient categoriesServiceClient = new CategoriesServiceReference.CatListingServiceClient();
        PortalServiceReference.PortalServiceClient portalClient = new PortalServiceReference.PortalServiceClient();
        //private int RegionId = int.Parse(ConfigurationManager.AppSettings["RegionId"].ToString());
        //private string Region = ConfigurationManager.AppSettings["Region"].ToString();
        private string defaultRegion = "AU";
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
                                  where cat.CategoryId != 1 && cat.CategoryName.Trim() != "My Favorites" && cat.CategoryName.Trim() != "Downloads" 
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
            portalClient.Close();
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

            var listSpecByCat = getTitlesByCategoryID(lCatID);
           
            ViewBag.SelCategory = categoriesServiceClient.get_Category(lCatID).First();
            Session.Add("CatID", lCatID.ToString());
            return View(listSpecByCat);
        }

        private IEnumerable<CategoriesServiceReference.Specific> getTitlesByCategoryID(long lCatID)
        {
            var listSpecByCat = from ca in categoriesServiceClient.getCatAssign()
                                join spec in categoriesServiceClient.get() on ca.SpecID equals spec.Id
                                join r in portalClient.getRegion() on spec.RegionId equals r.Id
                                where ca.CategoryId == lCatID && r.WebsiteUrl.ToLower() == Request.Url.Host.ToLower()
                                orderby spec.Title
                                select spec;
            if(listSpecByCat.Count() > 0)
            return listSpecByCat;
            else
            {
                return from ca in categoriesServiceClient.getCatAssign()
                       join spec in categoriesServiceClient.get() on ca.SpecID equals spec.Id
                       join r in portalClient.getRegion() on spec.RegionId equals r.Id
                       where ca.CategoryId == lCatID && r.RegionName == defaultRegion
                       orderby spec.Title
                       select spec;
            }
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
        ///
        
        public ActionResult Details(string id,string cat)
        {
            
            
            if (!categoriesServiceClient.Authenticate(password))
            {
                //error page
            }
            
            
            //var spec = categoriesServiceClient.get().Select(s => s).Where(s => s.Title.ToLower().Replace("-"," ") == id.Replace("-", " ").ToLower());
            //var spec = categoriesServiceClient.get().Select(s => s).Where(s => s.Title.Trim().Replace(" ", "-").Replace("---", "-").ToLower() == id);
            var spec = from title in categoriesServiceClient.get()
                       join r in portalClient.getRegion() on title.RegionId equals r.Id
                       where r.WebsiteUrl == Request.Url.Host && title.Title.Trim().Replace(": ", "-").Replace(" ", "-").Replace("---", "-").ToLower() == id
                       select title;

            //default if localhost for debugging purposes
            if(Request.Url.Host == "localhost" && spec.Count() ==0)
            {
                spec = from title in categoriesServiceClient.get()
                       join r in portalClient.getRegion() on title.RegionId equals r.Id
                       where r.RegionName == defaultRegion && title.Title.Trim().Replace(": ", "-").Replace(" ", "-").Replace("---", "-").ToLower() == id
                       select title;
            }

            string strCatName = HttpUtility.HtmlEncode(cat);


            long lCatId = 0 ;
            if (cat == null)
            {
                if (Session["CatID"] != null)
                    long.TryParse(Session["CatID"].ToString(), out lCatId);
                else
                {
                    var ca = categoriesServiceClient.getCatAssign().Where(c => c.SpecID == spec.First().Id);
                    lCatId = ca.Count() > 0 ? ca.First().CategoryId : 1;//else no_cat
                }
            }
            else
                lCatId = getCategoryId(strCatName);

            ViewBag.SelCategory = categoriesServiceClient.get_Category(lCatId).First();
            
            try
            {
             

                    var listSpecByCat = from ca in categoriesServiceClient.getCatAssign()
                                        join s in categoriesServiceClient.get() on ca.SpecID equals s.Id
                                        where ca.CategoryId == lCatId && s.Id != spec.First().Id
                                        select s;//this gets all titles under selected cat
                    var related = getRelatedTitles(listSpecByCat); //this filters the title list by region
                    ViewBag.Related = related;
 
            }
            catch(Exception ex)
            {
                //
            }
            return View(spec.First());
        }

        private IEnumerable<CategoriesServiceReference.Specific> getRelatedTitles(IEnumerable<CategoriesServiceReference.Specific> listSpecByCat)
        {
            var related = from titleCat in listSpecByCat
                          join region in portalClient.getRegion() on titleCat.RegionId equals region.Id
                          where region.WebsiteUrl.ToLower() == Request.Url.Host.ToLower()
                          select titleCat;


            return related.Count() > 0 ? related : from titleCat in listSpecByCat
                                                   join region in portalClient.getRegion() on titleCat.RegionId equals region.Id
                                                   where region.RegionName.ToLower() == defaultRegion.ToLower()
                                                   select titleCat; 
        }

         
    }
}

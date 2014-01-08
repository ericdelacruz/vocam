using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SODAMvcApplication
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "learn-more",
                url: "learn-more",
                 defaults: new { controller = "Home", action = "learnmore" }
                );
            routes.MapRoute(
                name: "Contact",
                url: "contact",
                 defaults: new { controller = "Home", action = "contact" }
                );
            routes.MapRoute(
               name: "legals",
               url: "legals",
                defaults: new { controller = "Home", action = "legals" }
               );
            routes.MapRoute(
                name: "Sitemap",
                 url: "sitemap",
                defaults: new { controller = "Home", action = "sitemap" }
                );
          


            //routes.MapRoute(
            //    name: "Browse",
            //    url: "categories/{cat}",
            //    defaults: new { controller = "Categories", action = "Browse" }
            //    );
            
            routes.MapRoute(
                name: "Browse",
                url: "{cat}",
                defaults: new { controller = "Categories", action = "Browse" },
                constraints: new {cat = new CategoryConstraints() }
                );
            //routes.MapRoute(
            //    name: "Titles",
            //    url: "{cat}/title/{id}",
            //    defaults: new { controller = "Categories", action = "Details", id = UrlParameter.Optional }
            //    );
            routes.MapRoute(
                name: "Titles",
                url: "{cat}/{id}",
                defaults: new { controller = "Categories", action = "Details", id = UrlParameter.Optional },
                constraints: new {cat = new CategoryConstraints() }
                );

            routes.MapRoute(
                name: "authorize",
                url: "users/{action}/{id}",
                defaults: new { controller = "xml", action = "validate", id = UrlParameter.Optional });

             routes.MapRoute(
                 name: "Default",
                 url: "{controller}/{action}/{id}",
                 defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
             );
           
            
        }

        public class CategoryConstraints:IRouteConstraint
        {

            public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
            {

                //CategoriesServiceReference.CatListingServiceClient client = new CategoriesServiceReference.CatListingServiceClient();
                //bool result = false;
                //try
                //{
                //    string strCat = values[parameterName].ToString().Trim().Replace(" ", "-").ToLower();
                //    result = client.get_Categories().Where(c => c.CategoryName.Trim().Replace(" ", "-").ToLower() == strCat).Count() > 0;
                    
                //}
                //catch
                //{
                   
                //}
                //finally
                //{
                //    client.Close();
                    
                //}
                //return result;
                List<string> ignoreList = new List<string>() { "learn-more", "contact", "sitemap", "users", "defaultcaptcha", "scripts","home","xml","categories" };
                return ignoreList.Where(url => url == values[parameterName].ToString().Trim().ToLower()).Count() == 0;
            }
        }
    }

   
}
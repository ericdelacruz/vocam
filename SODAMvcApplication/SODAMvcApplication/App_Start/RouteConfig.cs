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
            //routes.MapRoute(
            //    name: "Browse",
            //    url: "categories/browse/{cat}",
            //    defaults: new { controller = "Categories", action = "Browse" }
            //    );
            //routes.MapRoute(
            //    name: "Titles",
            //    url: "titles/{id}",
            //    defaults: new { controller = "Categories", action = "Details", id = UrlParameter.Optional });

            routes.MapRoute(
                name: "Browse",
                url: "categories/{cat}",
                defaults: new { controller = "Categories", action = "Browse" }
                );
            routes.MapRoute(
                name: "Titles",
                url: "{cat}/title/{id}",
                defaults: new { controller = "Categories", action = "Details", id = UrlParameter.Optional }
                );
            routes.MapRoute(
                name: "authorize",
                url: "users/{action}/{id}",
                defaults: new{controller="xml", action="validate", id = UrlParameter.Optional});
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
           
            
        }
    }
}
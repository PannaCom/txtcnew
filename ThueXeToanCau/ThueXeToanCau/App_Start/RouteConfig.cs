using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ThueXeToanCau
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                "chi tiet",
                "{catname}/{name}-{id}",
                new { controller = "News", action = "Details", catname = UrlParameter.Optional, name = UrlParameter.Optional, id = UrlParameter.Optional }
            );
            routes.MapRoute(
                "danh muc ",
                "{catname}-{cat_id}/{pg}",
                new { controller = "News", action = "List", catname = UrlParameter.Optional, cat_id = UrlParameter.Optional, pg = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}

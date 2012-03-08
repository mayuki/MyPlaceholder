using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Placeholder
{
    // メモ: IIS6 または IIS7 のクラシック モードの詳細については、
    // http://go.microsoft.com/?LinkId=9394801 を参照してください

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                null,
                "{width}/{backgroundColor}/{foregroundColor}",
                new { controller = "Home", action = "Render", backgroundColor = UrlParameter.Optional, foregroundColor = UrlParameter.Optional },
                new { width = @"\d+" }
            );
            routes.MapRoute(
                null,
                "{width}x{height}/{backgroundColor}/{foregroundColor}",
                new { controller = "Home", action = "Render", backgroundColor = UrlParameter.Optional, foregroundColor = UrlParameter.Optional },
                new { width = @"\d+", height = @"\d+" }
            );
            routes.MapRoute(
                "Default",
                "",
                new { controller = "Home", action = "Index" }
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
        }
    }
}
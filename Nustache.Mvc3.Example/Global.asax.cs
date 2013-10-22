using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Nustache.Mvc;
using Nustache.Mvc3.Example.ViewEngine;

namespace Nustache.Mvc3.Example
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            RegisterViewEngines(ViewEngines.Engines);
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("Content/{*pathInfo}");

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("NotFound", "NotFound", new { controller = "Home", action = "NotFound" });

            routes.MapRoute("Home-Custom", "some-custom-url", new { controller = "Home", action = "Custom" });

            routes.MapRoute("Default", "{*pathInfo}", new { controller = "Home", action = "Index" });
        }

        public static void RegisterViewEngines(ViewEngineCollection engines)
        {
            engines.RemoveAt(0);
            
            engines.Add(new NustacheMarkdownViewEngine(new[] { "md" })
            {
                RootContext = NustacheViewEngineRootContext.Model
            });
        }
    }
}
using System;
using System.Web.Mvc;

namespace Nustache.Mvc
{
    public class NustacheViewEngine : VirtualPathProviderViewEngine
    {
        public NustacheViewEngine()
        {
            ViewLocationFormats = new []
                                      {
                                          "~/Views/{1}/{0}.nustache",
                                          "~/Views/Shared/{0}.nustache"
                                      };
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            return new NustacheView(controllerContext, viewPath);
        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            throw new NotImplementedException();
        }
    }
}
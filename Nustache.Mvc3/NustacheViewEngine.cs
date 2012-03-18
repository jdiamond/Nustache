using System.Web;
using System.Web.Mvc;
using Nustache.Core;

namespace Nustache.Mvc
{
    public class NustacheViewEngine : VirtualPathProviderViewEngine
    {
        public NustacheViewEngine(string[] fileExtensions = null)
        {
            // If we're using MVC, we probably want to use the same encoder MVC uses.
            Encoders.HtmlEncode = HttpUtility.HtmlEncode;

            FileExtensions = fileExtensions ?? new[] { "mustache" };
            SetLocationFormats();
            RootContext = NustacheViewEngineRootContext.ViewData;
        }

        private void SetLocationFormats()
        {
            var fileExtension = FileExtensions[0];

            MasterLocationFormats = new[]
                                    {
                                        "~/Views/{1}/{0}." + fileExtension,
                                        "~/Views/Shared/{0}." + fileExtension
                                    };
            ViewLocationFormats = new[]
                                  {
                                      "~/Views/{1}/{0}." + fileExtension,
                                      "~/Views/Shared/{0}." + fileExtension
                                  };
            PartialViewLocationFormats = new[]
                                         {
                                             "~/Views/{1}/{0}." + fileExtension,
                                             "~/Views/Shared/{0}." + fileExtension
                                         };
            AreaMasterLocationFormats = new[]
                                        {
                                            "~/Areas/{2}/Views/{1}/{0}." + fileExtension,
                                            "~/Areas/{2}/Views/Shared/{0}." + fileExtension
                                        };
            AreaViewLocationFormats = new[]
                                      {
                                          "~/Areas/{2}/Views/{1}/{0}." + fileExtension,
                                          "~/Areas/{2}/Views/Shared/{0}." + fileExtension
                                      };
            AreaPartialViewLocationFormats = new[]
                                             {
                                                 "~/Areas/{2}/Views/{1}/{0}." + fileExtension,
                                                 "~/Areas/{2}/Views/Shared/{0}." + fileExtension
                                             };
        }

        public NustacheViewEngineRootContext RootContext { get; set; }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            return GetView(controllerContext, viewPath, masterPath);
        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            return GetView(controllerContext, partialPath, null);
        }

        private IView GetView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            return new NustacheView(this, controllerContext, viewPath, masterPath);
        }
    }

    public enum NustacheViewEngineRootContext
    {
        ViewData,
        Model
    }
}
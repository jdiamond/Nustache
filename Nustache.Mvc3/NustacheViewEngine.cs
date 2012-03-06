using System.Web.Mvc;

namespace Nustache.Mvc
{
    public class NustacheViewEngine : VirtualPathProviderViewEngine
    {
        private string _fileExtension;

        public NustacheViewEngine()
        {
            FileExtension = ".mustache";
            RootContext = NustacheViewEngineRootContext.ViewData;
        }

        public string FileExtension
        {
            get { return _fileExtension; }
            set { _fileExtension = value; UpdatePaths(); }
        }

        private void UpdatePaths()
        {
            var fileExtension = FileExtension;

            MasterLocationFormats = new[]
                                        {
                                            "~/Views/Shared/{0}" + fileExtension
                                        };
            ViewLocationFormats = new[]
                                      {
                                          "~/Views/{1}/{0}" + fileExtension,
                                          "~/Views/Shared/{0}" + fileExtension
                                      };
            PartialViewLocationFormats = new[]
                                             {
                                                 "~/Views/{1}/{0}" + fileExtension,
                                                 "~/Views/Shared/{0}" + fileExtension
                                             };
            AreaViewLocationFormats = new[]
                                          {
                                              "~/Areas/{2}/Views/{1}/{0}" + fileExtension,
                                              "~/Areas/{2}/Views/Shared/{0}" + fileExtension
                                          };
            AreaPartialViewLocationFormats = new[]
                                                 {
                                                     "~/Areas/{2}/Views/{1}/{0}" + fileExtension,
                                                     "~/Areas/{2}/Views/Shared/{0}" + fileExtension
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
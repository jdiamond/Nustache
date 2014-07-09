using System.Web.Mvc;
using Nustache.Mvc;

namespace Nustache.Mvc3.Example.ViewEngine
{
    public class NustacheMarkdownViewEngine : NustacheViewEngine
    {
        public NustacheMarkdownViewEngine(string[] fileExtensions = null) : base(fileExtensions)
        {
        }

        protected override IView GetView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            return new NustacheMarkdownView(this, controllerContext, viewPath, masterPath);
        }
    }
}
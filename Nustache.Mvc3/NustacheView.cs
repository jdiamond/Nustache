using System.IO;
using System.Web.Mvc;
using Nustache.Core;

namespace Nustache.Mvc
{
    public class NustacheView : IView
    {
        private readonly ControllerContext _controllerContext;
        private readonly string _viewPath;

        public NustacheView(ControllerContext controllerContext, string viewPath)
        {
            _controllerContext = controllerContext;
            _viewPath = viewPath;
        }

        public void Render(ViewContext viewContext, TextWriter writer)
        {
            var templatePath = _controllerContext.HttpContext.Server.MapPath(_viewPath);
            var templateSource = File.ReadAllText(templatePath);
            var template = new Template();
            template.Load(new StringReader(templateSource));
            template.Render(viewContext.ViewData.Model ?? viewContext.ViewData, writer, null);
        }
    }
}
using System.IO;
using System.Web.Mvc;
using Nustache.Core;

namespace Nustache.Mvc
{
    public class NustacheView : IView
    {
        private readonly ControllerContext _controllerContext;
        private readonly string _viewPath;
        private readonly string _masterPath;
        private readonly NustacheViewEngineRootContext _rootContext;

        public NustacheView(
            ControllerContext controllerContext,
            string viewPath,
            string masterPath,
            NustacheViewEngineRootContext rootContext)
        {
            _controllerContext = controllerContext;
            _viewPath = viewPath;
            _masterPath = masterPath;
            _rootContext = rootContext;
        }

        public void Render(ViewContext viewContext, TextWriter writer)
        {
            var viewTemplate = GetTemplate(_viewPath);

            if (!string.IsNullOrEmpty(_masterPath))
            {
                var masterTemplate = GetTemplate(_masterPath);

                var data = _rootContext == NustacheViewEngineRootContext.ViewData
                               ? viewContext.ViewData
                               : viewContext.ViewData.Model;

                masterTemplate.Render(
                    data,
                    writer,
                    name =>
                        {
                            if (name == "Body")
                            {
                                return viewTemplate;
                            }

                            return viewTemplate.GetTemplateDefinition(name);
                        });
            }
            else
            {
                // TODO: Do we want to allow rendering external templates via a template locator?
                viewTemplate.Render(viewContext.ViewData, writer, null);
            }
        }

        private Template GetTemplate(string path)
        {
            // TODO: Add caching?
            var templatePath = _controllerContext.HttpContext.Server.MapPath(path);
            var templateSource = File.ReadAllText(templatePath);
            var template = new Template();
            template.Load(new StringReader(templateSource));
            return template;
        }
    }
}
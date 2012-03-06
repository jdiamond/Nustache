using System.IO;
using System.Web.Caching;
using System.Web.Mvc;
using Nustache.Core;

namespace Nustache.Mvc
{
    public class NustacheView : IView
    {
        private readonly NustacheViewEngine _engine;
        private readonly ControllerContext _controllerContext;
        private readonly string _viewPath;
        private readonly string _masterPath;

        public NustacheView(
            NustacheViewEngine engine,
            ControllerContext controllerContext,
            string viewPath,
            string masterPath)
        {
            _engine = engine;
            _controllerContext = controllerContext;
            _viewPath = viewPath;
            _masterPath = masterPath;
        }

        public void Render(ViewContext viewContext, TextWriter writer)
        {
            var viewTemplate = GetTemplate();

            if (!string.IsNullOrEmpty(_masterPath))
            {
                var masterTemplate = LoadTemplate(_masterPath);

                var data = _engine.RootContext == NustacheViewEngineRootContext.ViewData
                               ? viewContext.ViewData
                               : viewContext.ViewData.Model;

                masterTemplate.Render(
                    data,
                    writer,
                    name =>
                        {
                            if (name == "Body")
                            {
                                return GetTemplate();
                            }

                            var template = viewTemplate.GetTemplateDefinition(name);

                            if (template != null)
                            {
                                return template;
                            }

                            return FindPartial(name);
                        });
            }
            else
            {
                GetTemplate().Render(viewContext.ViewData, writer, FindPartial);
            }
        }

        private Template GetTemplate()
        {
            return LoadTemplate(_viewPath);
        }

        private Template LoadTemplate(string path)
        {
            var key = "Nustache:" + path;

            if (_controllerContext.HttpContext.Cache[key] != null)
            {
                return (Template)_controllerContext.HttpContext.Cache[key];
            }

            var templatePath = _controllerContext.HttpContext.Server.MapPath(path);
            var templateSource = File.ReadAllText(templatePath);
            var template = new Template();
            template.Load(new StringReader(templateSource));

            _controllerContext.HttpContext.Cache.Insert(key, template, new CacheDependency(templatePath));

            return template;
        }

        private Template FindPartial(string name)
        {
            var viewResult = _engine.FindPartialView(_controllerContext, name, false);

            if (viewResult != null)
            {
                var nustacheView = viewResult.View as NustacheView;

                if (nustacheView != null)
                {
                    return nustacheView.GetTemplate();
                }
            }

            return null;
        }
    }
}
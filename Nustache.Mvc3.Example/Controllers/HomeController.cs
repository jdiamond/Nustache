using System;
using System.Web.Mvc;

namespace Nustache.Mvc3.Example.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var model = new
            {
                DoModelPropertiesWork = "Model properties work!",
                DoesHtmlEncodingWork = "<em>Should this be encoded?</em>",
                DoesInternationalCharacterEncodingWork = "Iñtërnâtiônàlizætiøn",
                DoesRussianCharacterEncodingWork = "Привет, как дела"
            };

            var path = Request.Path.EndsWith("/") ? Request.Path + "index" : Request.Path;

            if (path.StartsWith("/"))
            {
                path = path.Substring(1, path.Length - 1);
            }

            return View(path, "_Layout", model);
        }

        public ActionResult Custom()
        {
            var model = new
            {
                DoModelPropertiesWork = "Model properties work!",
                DoesHtmlEncodingWork = "<em>Should this be encoded?</em>",
                DoesInternationalCharacterEncodingWork = "Iñtërnâtiônàlizætiøn",
                DoesRussianCharacterEncodingWork = "Привет, как дела"
            }; 
            
            return View("custom-view", "_Layout", model);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;
            // You could use a custom layout for the 404 page.
            View("NotFound", "_Layout", filterContext).ExecuteResult(ControllerContext);
            base.OnException(filterContext);
        }

        public new dynamic ViewBag
        {
            get { throw new NotSupportedException("You cannot use view bag"); }
        }

        public new ViewDataDictionary ViewData
        {
            get { throw new NotSupportedException("You cannot use view data"); }
        }
    }
}
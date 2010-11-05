using System.Web.Mvc;

namespace Nustache.Mvc.Example.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["Message"] = "Welcome to ASP.NET MVC!";

            ViewResult viewResult = View();

            viewResult.ViewEngineCollection = new ViewEngineCollection
                                                  {
                                                      new NustacheViewEngine()
                                                  };

            return viewResult;
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
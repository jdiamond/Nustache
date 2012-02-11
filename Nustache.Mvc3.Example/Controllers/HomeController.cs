using System.Web.Mvc;

namespace Nustache.Mvc3.Example.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["ValueInViewData"] = "ViewData works!";
            ViewBag.ValueInViewBag = "ViewBag works!";

            var model = new
                        {
                            ModelProperty = "Model properties work!"
                        };

            return View("Index", "_Layout", model);
        }

        public ActionResult RazorWithPartialNustache()
        {
            return View();
        }
    }
}
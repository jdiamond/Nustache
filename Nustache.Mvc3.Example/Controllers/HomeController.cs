using System.Web.Mvc;

namespace Nustache.Mvc3.Example.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var model = new
                        {
                            Message = "Nustache works!"
                        };

            return View(model);
        }

        public ActionResult RazorWithPartialNustache()
        {
            return View();
        }
    }
}
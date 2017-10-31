using System.Web.Mvc;

namespace Nustache.Mvc3.Example.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["DoesViewDataWork"] = "ViewData works!";
            ViewBag.DoesViewBagWork = "ViewBag works!";

            var model = new
                        {
                            DoModelPropertiesWork = "Model properties work!",
                            DoesHtmlEncodingWork = "<em>Should this be encoded?</em>",
                            DoesInternationalCharacterEncodingWork = "Iñtërnâtiônàlizætiøn",
                            DoesRussianCharacterEncodingWork = "Привет, как дела"
                        };

            // TODO: Find a better way to specify the default master.
            return View("Index", "_Layout", model);
        }

        public ActionResult RazorWithPartialNustache()
        {
            ViewBag.InternationalCharacters = "Iñtërnâtiônàlizætiøn";

            var model = new
                        {
                            DoModelPropertiesWork = "Model properties work!"
                        };

            return View(model);
        }

        public ActionResult MissingView()
        {
            return View();
        }

        public ActionResult MissingPartial()
        {
            return View();
        }
    }
}
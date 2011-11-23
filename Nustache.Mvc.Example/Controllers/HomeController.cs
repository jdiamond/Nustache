using System.Web.Mvc;
using Nustache.Mvc.Example.Models;
using System.Collections.Generic;

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

		  public ActionResult Collection() {
			  var posts = new List<Post>() {
				  new Post { Id = 111, Title = "Post #1", Body = "Blog post #1" },
				  new Post { Id = 222, Title = "Post #2", Body = "Blog post #2" },
				  new Post { Id = 333, Title = "Post #3", Body = "Blog post #3" }
			  };

			  ViewData["Posts"] = posts;

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
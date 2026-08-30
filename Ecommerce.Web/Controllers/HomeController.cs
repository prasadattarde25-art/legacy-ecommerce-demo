using System.Web.Mvc;
using Ecommerce.Core.Interfaces.Services;

namespace Ecommerce.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICatalogService _catalog;

        public HomeController(ICatalogService catalog)
        {
            _catalog = catalog;
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Home";
            var featured = _catalog.GetFeatured();
            return View(featured);
        }
    }
}
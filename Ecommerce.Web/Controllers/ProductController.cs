using System.Linq;
using System.Web.Mvc;
using Ecommerce.Core.Interfaces.Services;

namespace Ecommerce.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly ICatalogService _catalog;

        public ProductController(ICatalogService catalog)
        {
            _catalog = catalog;
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Index(int? categoryId, string q, int page = 1)
        {
            if (page < 1) page = 1;

            ViewBag.Title = "Products";
            var vm = _catalog.GetListing(categoryId, q, page);
            return View(vm);
        }

        public ActionResult Detail(int id)
        {
            var vm = _catalog.GetDetail(id);
            if (vm == null) return HttpNotFound();

            ViewBag.Title = vm.Product.Name;
            return View(vm);
        }

        [HttpGet]
        public ActionResult Filter(int? categoryId, string q, int page = 1)
        {
            var vm = _catalog.GetListing(categoryId, q, page);
            return PartialView("_ProductList", vm);
        }

        [ChildActionOnly]
        public ActionResult Sidebar()
        {
            var roots = _catalog.GetRootCategories();
            return PartialView("_Sidebar", roots);
        }

        [HttpGet]
        public ActionResult Subcategories(int parentId)
        {
            var children = _catalog.GetSubcategories(parentId)
                .Select(c => new { id = c.Id, name = c.Name, slug = c.Slug })
                .ToList();

            return Json(children, JsonRequestBehavior.AllowGet);
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Core.Interfaces.Services;

namespace Ecommerce.WebApi.Controllers
{
    [ApiController]
    [Route("api")]
    public class ProductsController : ControllerBase
    {
        private readonly ICatalogService _catalog;

        public ProductsController(ICatalogService catalog)
        {
            _catalog = catalog;
        }

        /// <summary>Product listing grid with optional category / query / paging.</summary>
        [HttpGet("products")]
        public async Task<IActionResult> Index(int? categoryId, string q, int page = 1, CancellationToken ct = default)
        {
            if (page < 1) page = 1;
            var vm = await _catalog.GetListingAsync(categoryId, q, page, ct);
            return Ok(vm);
        }

        /// <summary>Single product detail (images, active variants, related).</summary>
        [HttpGet("products/{id:int}")]
        public async Task<IActionResult> Detail(int id, CancellationToken ct = default)
        {
            var vm = await _catalog.GetDetailAsync(id, ct);
            if (vm == null) return NotFound();
            return Ok(vm);
        }

        /// <summary>Root categories for the sidebar menu.</summary>
        [HttpGet("categories")]
        public async Task<IActionResult> RootCategories(CancellationToken ct = default)
        {
            return Ok(await _catalog.GetRootCategoriesAsync(ct));
        }

        /// <summary>Child categories for the lazy-loaded category tree.</summary>
        [HttpGet("categories/{parentId:int}/subcategories")]
        public async Task<IActionResult> Subcategories(int parentId, CancellationToken ct = default)
        {
            var children = await _catalog.GetSubcategoriesAsync(parentId, ct);
            return Ok(children.Select(c => new { id = c.Id, name = c.Name, slug = c.Slug }));
        }
    }
}

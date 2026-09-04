using Microsoft.AspNetCore.Mvc;
using Ecommerce.Core.Interfaces.Services;

namespace Ecommerce.WebApi.Controllers
{
    [ApiController]
    [Route("api/home")]
    public class HomeController : ControllerBase
    {
        private readonly ICatalogService _catalog;

        public HomeController(ICatalogService catalog)
        {
            _catalog = catalog;
        }

        /// <summary>Featured products shown on the storefront homepage.</summary>
        [HttpGet("featured")]
        public async Task<IActionResult> Featured(CancellationToken ct)
        {
            var items = await _catalog.GetFeaturedAsync(ct);
            return Ok(items);
        }
    }
}

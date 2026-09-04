using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces.Repositories;
using Ecommerce.Core.Interfaces.Services;
using Ecommerce.Core.ViewModels;
using Ecommerce.WebApi.Helpers;

namespace Ecommerce.WebApi.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ICartRepository _cartRepository;

        public CartController(ICartService cartService, ICartRepository cartRepository)
        {
            _cartService = cartService;
            _cartRepository = cartRepository;
        }

        /// <summary>Current cart with pricing applied.</summary>
        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct = default)
        {
            var cart = await LoadCartAsync(ct);
            return Ok(cart);
        }

        /// <summary>Adds a product (or increments quantity) to the cart.</summary>
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] AddLineRequest request, CancellationToken ct = default)
        {
            if (request == null || request.ProductId <= 0)
                return BadRequest(new { success = false, message = "Invalid product." });

            var cartId = CartSessionHelper.GetOrCreateCartId(HttpContext);
            var cart = await LoadCartAsync(ct, cartId);
            cart = await _cartService.AddToCartAsync(cart, request.ProductId, request.Quantity, ct);
            await SaveCartAsync(cart, cartId, ct);

            return Ok(new
            {
                success = true,
                itemCount = cart.ItemCount,
                subtotal = cart.Subtotal,
                grandTotal = cart.GrandTotal,
                cart
            });
        }

        /// <summary>Updates quantity of a line (quantity &lt;= 0 removes it).</summary>
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] AddLineRequest request, CancellationToken ct = default)
        {
            if (request == null || request.ProductId <= 0)
                return BadRequest(new { success = false, message = "Invalid product." });

            var cartId = CartSessionHelper.GetOrCreateCartId(HttpContext);
            var cart = await LoadCartAsync(ct, cartId);
            cart = _cartService.UpdateLine(cart, request.ProductId, request.Quantity);
            await SaveCartAsync(cart, cartId, ct);

            return Ok(cart);
        }

        /// <summary>Removes a line from the cart.</summary>
        [HttpPost("remove")]
        public async Task<IActionResult> Remove([FromBody] RemoveLineRequest request, CancellationToken ct = default)
        {
            if (request == null || request.ProductId <= 0)
                return BadRequest(new { success = false, message = "Invalid product." });

            var cartId = CartSessionHelper.GetOrCreateCartId(HttpContext);
            var cart = await LoadCartAsync(ct, cartId);
            cart = _cartService.RemoveLine(cart, request.ProductId);
            await SaveCartAsync(cart, cartId, ct);

            return Ok(cart);
        }

        /// <summary>Applies (or clears) a coupon code.</summary>
        [HttpPost("coupon")]
        public async Task<IActionResult> ApplyCoupon([FromBody] CouponRequest request, CancellationToken ct = default)
        {
            string couponOverride = null;
            if (request != null && !string.IsNullOrWhiteSpace(request.CouponCode))
            {
                CartSessionHelper.SaveCoupon(HttpContext, request.CouponCode);
                couponOverride = request.CouponCode.Trim().ToUpperInvariant();
            }

            var cartId = CartSessionHelper.GetOrCreateCartId(HttpContext);
            var cart = await LoadCartAsync(ct, cartId, couponOverride);
            await SaveCartAsync(cart, cartId, ct, couponOverride);

            return Ok(cart);
        }

        /// <summary>Clears the cart and coupon.</summary>
        [HttpDelete]
        public async Task<IActionResult> Clear(CancellationToken ct = default)
        {
            var cartId = CartSessionHelper.GetOrCreateCartId(HttpContext);
            await _cartRepository.ClearAsync(cartId, ct);
            CartSessionHelper.ClearCoupon(HttpContext);
            return Ok(new { success = true });
        }

        private async Task<CartViewModel> LoadCartAsync(CancellationToken ct, Guid? forcedId = null, string couponOverride = null)
        {
            var cartId = forcedId ?? CartSessionHelper.GetOrCreateCartId(HttpContext);
            var lines = await _cartRepository.GetBySessionIdAsync(cartId, ct);

            var cart = new CartViewModel
            {
                Lines = lines.ToList(),
                CouponCode = couponOverride ?? CartSessionHelper.GetCoupon(HttpContext)
            };

            return _cartService.ApplyPricing(cart);
        }

        private async Task SaveCartAsync(CartViewModel cart, Guid cartId, CancellationToken ct, string couponOverride = null)
        {
            cart.CouponCode = couponOverride ?? CartSessionHelper.GetCoupon(HttpContext);
            await _cartRepository.SaveAllAsync(cartId, cart.Lines, ct);
        }

        public class AddLineRequest
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; } = 1;
        }

        public class RemoveLineRequest
        {
            public int ProductId { get; set; }
        }

        public class CouponRequest
        {
            public string CouponCode { get; set; }
        }
    }
}

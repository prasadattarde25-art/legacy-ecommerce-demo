using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ecommerce.Core.Interfaces.Repositories;
using Ecommerce.Core.Interfaces.Services;
using Ecommerce.Core.ViewModels;
using Ecommerce.WebApi.Helpers;

namespace Ecommerce.WebApi.Controllers
{
    [ApiController]
    [Route("api/checkout")]
    [Authorize]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService _checkout;
        private readonly ICartRepository _cartRepository;

        public CheckoutController(ICheckoutService checkout, ICartRepository cartRepository)
        {
            _checkout = checkout;
            _cartRepository = cartRepository;
        }

        /// <summary>Places an order for the current cart (Address → Shipping → Payment in one call).</summary>
        [HttpPost]
        public async Task<IActionResult> CreateOrder(CheckoutRequest request, CancellationToken ct = default)
        {
            var customerId = GetCustomerId();
            if (customerId <= 0) return Unauthorized();

            var cartId = CartSessionHelper.GetOrCreateCartId(HttpContext);
            var lines = await _cartRepository.GetBySessionIdAsync(cartId, ct);
            var cart = new CartViewModel
            {
                Lines = lines.ToList(),
                CouponCode = CartSessionHelper.GetCoupon(HttpContext)
            };

            var result = await _checkout.CreateOrderAsync(
                cart,
                request.Address,
                request.Shipping,
                request.Payment,
                customerId,
                cartId,
                ct);

            if (!result.Success)
                return BadRequest(new { success = false, message = result.Message });

            // Order placed — clear the cart and coupon
            await _cartRepository.ClearAsync(cartId, ct);
            CartSessionHelper.ClearCoupon(HttpContext);

            return Ok(new { success = true, order = result.Value, message = result.Message });
        }

        /// <summary>Confirmation detail for a placed order (must belong to the user).</summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Confirmation(int id, CancellationToken ct = default)
        {
            var customerId = GetCustomerId();
            var order = await _checkout.GetOrderAsync(id, customerId, ct);
            if (order == null) return NotFound();
            return Ok(order);
        }

        /// <summary>Order history for the signed-in customer.</summary>
        [HttpGet("orders")]
        public async Task<IActionResult> Orders(CancellationToken ct = default)
        {
            var customerId = GetCustomerId();
            var vm = await _checkout.GetOrderHistoryAsync(customerId, ct);
            return Ok(vm);
        }

        private int GetCustomerId()
        {
            var principal = User as ClaimsPrincipal;
            var claim = principal?.FindFirst(ClaimTypes.NameIdentifier);
            int id;
            return claim != null && int.TryParse(claim.Value, out id) ? id : 0;
        }

        public class CheckoutRequest
        {
            public CheckoutAddressViewModel Address { get; set; }
            public CheckoutShippingViewModel Shipping { get; set; }
            public CheckoutPaymentViewModel Payment { get; set; }
        }
    }
}

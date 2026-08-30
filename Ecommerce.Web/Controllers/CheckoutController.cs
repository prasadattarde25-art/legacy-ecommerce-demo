using System.Security.Claims;
using System.Web.Mvc;
using Ecommerce.Core.Interfaces.Services;
using Ecommerce.Core.ViewModels;
using Ecommerce.Web.Helpers;

namespace Ecommerce.Web.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ICheckoutService _checkout;
        private readonly ICartService _cartService;

        public CheckoutController(ICheckoutService checkout, ICartService cartService)
        {
            _checkout = checkout;
            _cartService = cartService;
        }

        public ActionResult Address()
        {
            var cart = BuildCart();
            if (!cart.HasItems) return RedirectToAction("Index", "Cart");

            var vm = Session["Checkout_Address"] as CheckoutAddressViewModel;
            if (vm == null)
            {
                vm = new CheckoutAddressViewModel
                {
                    Email = GetClaim(ClaimTypes.Email),
                    FirstName = GetNamePart(0),
                    LastName = GetNamePart(1)
                };
            }

            ViewBag.Cart = cart;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Address(CheckoutAddressViewModel model)
        {
            var cart = BuildCart();
            if (!cart.HasItems) return RedirectToAction("Index", "Cart");

            if (!ModelState.IsValid)
            {
                ViewBag.Cart = cart;
                return View(model);
            }

            Session["Checkout_Address"] = model;
            return RedirectToAction("Shipping");
        }

        public ActionResult Shipping()
        {
            var cart = BuildCart();
            if (!cart.HasItems) return RedirectToAction("Index", "Cart");
            if (Session["Checkout_Address"] == null) return RedirectToAction("Address");

            var vm = Session["Checkout_Shipping"] as CheckoutShippingViewModel
                     ?? new CheckoutShippingViewModel { ShippingMethod = "Standard" };

            ViewBag.Cart = cart;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Shipping(CheckoutShippingViewModel model)
        {
            var cart = BuildCart();
            if (!cart.HasItems) return RedirectToAction("Index", "Cart");
            if (Session["Checkout_Address"] == null) return RedirectToAction("Address");

            if (!ModelState.IsValid)
            {
                ViewBag.Cart = cart;
                return View(model);
            }

            Session["Checkout_Shipping"] = model;
            return RedirectToAction("Payment");
        }

        public ActionResult Payment()
        {
            var cart = BuildCart();
            if (!cart.HasItems) return RedirectToAction("Index", "Cart");
            if (Session["Checkout_Address"] == null) return RedirectToAction("Address");
            if (Session["Checkout_Shipping"] == null) return RedirectToAction("Shipping");

            var vm = Session["Checkout_Payment"] as CheckoutPaymentViewModel
                     ?? new CheckoutPaymentViewModel { PaymentMethod = "CreditCard" };

            ViewBag.Cart = cart;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Payment(CheckoutPaymentViewModel model)
        {
            var cart = BuildCart();
            if (!cart.HasItems) return RedirectToAction("Index", "Cart");
            if (Session["Checkout_Address"] == null) return RedirectToAction("Address");
            if (Session["Checkout_Shipping"] == null) return RedirectToAction("Shipping");

            if (!ModelState.IsValid)
            {
                ViewBag.Cart = cart;
                return View(model);
            }

            var address = (CheckoutAddressViewModel)Session["Checkout_Address"];
            var shipping = (CheckoutShippingViewModel)Session["Checkout_Shipping"];

            var result = _checkout.CreateOrder(
                cart, address, shipping, model,
                GetCustomerId(),
                CartSessionHelper.GetSessionId(Session));

            if (!result.Success)
            {
                ViewBag.Error = result.Message;
                ViewBag.Cart = cart;
                return View(model);
            }

            // Checkout complete — clear cart + wizard session state.
            CartSessionHelper.Clear(Session);
            Session.Remove("Checkout_Address");
            Session.Remove("Checkout_Shipping");
            Session.Remove("Checkout_Payment");

            return RedirectToAction("Confirmation", new { id = result.Value.Id });
        }

        public ActionResult Confirmation(int id)
        {
            var order = _checkout.GetOrder(id, GetCustomerId());
            if (order == null) return HttpNotFound();

            ViewBag.Title = "Order Confirmation";
            return View(order);
        }

        private CartViewModel BuildCart()
        {
            return _cartService.ApplyPricing(new CartViewModel
            {
                Lines = CartSessionHelper.GetLines(Session),
                CouponCode = CartSessionHelper.GetCoupon(Session)
            });
        }

        private int GetCustomerId()
        {
            int id;
            int.TryParse(GetClaim(ClaimTypes.NameIdentifier), out id);
            return id;
        }

        private string GetClaim(string claimType)
        {
            var principal = User as ClaimsPrincipal;
            if (principal == null) return null;
            var claim = principal.FindFirst(claimType);
            return claim == null ? null : claim.Value;
        }

        private string GetNamePart(int index)
        {
            var name = GetClaim(ClaimTypes.Name);
            if (string.IsNullOrWhiteSpace(name)) return null;

            var parts = name.Split(' ');
            return parts.Length > index ? parts[index] : null;
        }
    }
}
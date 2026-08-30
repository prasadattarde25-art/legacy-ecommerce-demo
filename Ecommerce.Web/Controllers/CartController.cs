using System.Web.Mvc;
using Ecommerce.Core.Interfaces.Services;
using Ecommerce.Core.ViewModels;
using Ecommerce.Web.Filters;
using Ecommerce.Web.Helpers;

namespace Ecommerce.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Index()
        {
            ViewBag.Title = "Cart";
            ViewBag.FullWidth = true;

            var cart = BuildCart();
            return View(cart);
        }

        [HttpGet]
        public ActionResult MiniCart()
        {
            var cart = _cartService.ApplyPricing(BuildCart());
            return PartialView("_MiniCart", new MiniCartViewModel
            {
                ItemCount = cart.ItemCount,
                Subtotal = cart.Subtotal
            });
        }

        [HttpPost]
        [AjaxValidateAntiForgeryToken]
        public ActionResult Add(int productId, int quantity)
        {
            var cart = _cartService.AddToCart(BuildCart(), productId, quantity);
            SaveCart(cart);

            return Json(new
            {
                success = true,
                itemCount = cart.ItemCount,
                subtotal = cart.Subtotal.ToString("C"),
                grandTotal = cart.GrandTotal.ToString("C")
            });
        }

        [HttpPost]
        [AjaxValidateAntiForgeryToken]
        public ActionResult Update(int productId, int quantity)
        {
            var cart = _cartService.UpdateLine(BuildCart(), productId, quantity);
            SaveCart(cart);
            return PartialView("_CartLines", cart);
        }

        [HttpPost]
        [AjaxValidateAntiForgeryToken]
        public ActionResult Remove(int productId)
        {
            var cart = _cartService.RemoveLine(BuildCart(), productId);
            SaveCart(cart);
            return PartialView("_CartLines", cart);
        }

        [HttpPost]
        [AjaxValidateAntiForgeryToken]
        public ActionResult ApplyCoupon(string couponCode)
        {
            if (!string.IsNullOrWhiteSpace(couponCode))
            {
                CartSessionHelper.SaveCoupon(Session, couponCode);
            }

            var cart = BuildCart();
            CartSessionHelper.SaveLines(Session, cart.Lines);

            return PartialView("_CartLines", cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Clear()
        {
            CartSessionHelper.Clear(Session);
            return Json(new { success = true });
        }

        private CartViewModel BuildCart()
        {
            return _cartService.ApplyPricing(new CartViewModel
            {
                Lines = CartSessionHelper.GetLines(Session),
                CouponCode = CartSessionHelper.GetCoupon(Session)
            });
        }

        private void SaveCart(CartViewModel cart)
        {
            CartSessionHelper.SaveLines(Session, cart.Lines);
        }
    }
}
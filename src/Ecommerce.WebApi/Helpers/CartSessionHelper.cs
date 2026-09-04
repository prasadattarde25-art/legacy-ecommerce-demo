using System;
using Ecommerce.Core.ViewModels;

namespace Ecommerce.WebApi.Helpers
{
    /// <summary>
    /// API replacement for the legacy MVC CartSessionHelper. The stateless API
    /// identifies a cart by a stable GUID held in a "cart_id" cookie and the
    /// coupon by a "cart_coupon" cookie. Cart lines are persisted in the
    /// CartItems table keyed by that GUID (via ICartRepository), the same
    /// persistence model the legacy app wired up.
    /// </summary>
    public static class CartSessionHelper
    {
        public const string CartIdCookie = "cart_id";
        public const string CouponCookie = "cart_coupon";

        public static Guid GetOrCreateCartId(HttpContext context)
        {
            if (context.Request.Cookies.TryGetValue(CartIdCookie, out var existing) &&
                Guid.TryParse(existing, out var parsed) && parsed != Guid.Empty)
            {
                return parsed;
            }

            var id = Guid.NewGuid();
            context.Response.Cookies.Append(CartIdCookie, id.ToString(), new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(30)
            });
            return id;
        }

        public static string GetCoupon(HttpContext context)
        {
            if (context.Request.Cookies.TryGetValue(CouponCookie, out var coupon))
                return coupon;
            return null;
        }

        public static void SaveCoupon(HttpContext context, string coupon)
        {
            if (string.IsNullOrWhiteSpace(coupon))
            {
                ClearCoupon(context);
                return;
            }

            context.Response.Cookies.Append(CouponCookie, coupon.Trim().ToUpperInvariant(), new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(30)
            });
        }

        public static void ClearCoupon(HttpContext context)
        {
            context.Response.Cookies.Delete(CouponCookie);
        }

        public static CartViewModel BuildEmpty()
        {
            return new CartViewModel();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Web;
using Ecommerce.Core.Entities;

namespace Ecommerce.Web.Helpers
{
    /// <summary>
    /// Server-side session cart (in-proc HttpSessionState). Holds a list of
    /// CartItem rows plus an optional coupon. A stable Guid keys the optional
    /// CartItems table persistence in the DB.
    /// </summary>
    public static class CartSessionHelper
    {
        private const string CartKey = "LegacyCart_v1";
        private const string CartSessionIdKey = "LegacyCartSessionId";
        private const string CouponKey = "LegacyCartCoupon";

        public static List<CartItem> GetLines(HttpSessionStateBase session)
        {
            if (session == null) return new List<CartItem>();
            return session[CartKey] as List<CartItem> ?? new List<CartItem>();
        }

        public static void SaveLines(HttpSessionStateBase session, List<CartItem> lines)
        {
            session[CartKey] = lines;
        }

        public static string GetCoupon(HttpSessionStateBase session)
        {
            return session == null ? null : session[CouponKey] as string;
        }

        public static void SaveCoupon(HttpSessionStateBase session, string coupon)
        {
            if (coupon != null) session[CouponKey] = coupon.Trim().ToUpperInvariant();
        }

        public static Guid GetSessionId(HttpSessionStateBase session)
        {
            if (session[CartSessionIdKey] == null)
            {
                session[CartSessionIdKey] = Guid.NewGuid();
            }
            return (Guid)session[CartSessionIdKey];
        }

        public static void Clear(HttpSessionStateBase session)
        {
            session.Remove(CartKey);
            session.Remove(CouponKey);
        }
    }
}
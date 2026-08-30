using System;

namespace Ecommerce.Services.Pricing
{
    /// <summary>
    /// Pure pricing rules. No database or HTTP state — safe to unit test.
    /// </summary>
    public static class PriceCalculator
    {
        public const decimal FreeShippingThreshold = 75.00m;
        public const decimal FlatRateShipping = 9.95m;
        public const decimal TaxRate = 0.08m;

        public const string Save10Coupon = "SAVE10";
        private const decimal Save10DiscountRate = 0.10m;

        public static decimal CalculateDiscount(string couponCode, decimal subtotal)
        {
            if (!string.IsNullOrWhiteSpace(couponCode) &&
                string.Equals(couponCode.Trim(), Save10Coupon, StringComparison.OrdinalIgnoreCase) &&
                subtotal > 0)
            {
                return Math.Round(subtotal * Save10DiscountRate, 2);
            }

            return 0m;
        }

        public static decimal CalculateShipping(decimal amountAfterDiscount)
        {
            return amountAfterDiscount >= FreeShippingThreshold ? 0m : FlatRateShipping;
        }

        public static decimal CalculateTax(decimal amountAfterDiscount, decimal shipping)
        {
            return Math.Round((amountAfterDiscount + shipping) * TaxRate, 2);
        }
    }
}
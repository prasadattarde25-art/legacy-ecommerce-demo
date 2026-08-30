using System;
using System.Linq;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces.Repositories;
using Ecommerce.Core.Interfaces.Services;
using Ecommerce.Core.ViewModels;
using Ecommerce.Services.Pricing;

namespace Ecommerce.Services
{
    public class CartService : ICartService
    {
        private readonly IProductRepository _products;

        public CartService(IProductRepository products)
        {
            _products = products;
        }

        public CartViewModel AddToCart(CartViewModel cart, int productId, int quantity)
        {
            if (cart == null) cart = new CartViewModel();
            if (quantity <= 0) quantity = 1;

            var line = cart.Lines.FirstOrDefault(l => l.ProductId == productId);
            if (line != null)
            {
                line.Quantity += quantity;
                line.UpdatedAt = DateTime.Now;
            }
            else
            {
                var product = _products.GetById(productId);
                if (product == null) return cart;

                cart.Lines.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Sku = product.Sku,
                    UnitPrice = product.Price,
                    Quantity = quantity,
                    CreatedAt = DateTime.Now
                });
            }

            return ApplyPricing(cart);
        }

        public CartViewModel UpdateLine(CartViewModel cart, int productId, int quantity)
        {
            if (cart == null) return new CartViewModel();

            var line = cart.Lines.FirstOrDefault(l => l.ProductId == productId);
            if (line == null) return ApplyPricing(cart);

            if (quantity <= 0)
            {
                cart.Lines.Remove(line);
            }
            else
            {
                line.Quantity = quantity;
                line.UpdatedAt = DateTime.Now;
            }

            return ApplyPricing(cart);
        }

        public CartViewModel RemoveLine(CartViewModel cart, int productId)
        {
            if (cart == null) return new CartViewModel();

            var line = cart.Lines.FirstOrDefault(l => l.ProductId == productId);
            if (line != null) cart.Lines.Remove(line);

            return ApplyPricing(cart);
        }

        public CartViewModel ApplyPricing(CartViewModel cart)
        {
            if (cart == null) return new CartViewModel();

            var subtotal = cart.Subtotal;
            cart.Discount = PriceCalculator.CalculateDiscount(cart.CouponCode, subtotal);
            var afterDiscount = subtotal - cart.Discount;
            cart.ShippingTotal = PriceCalculator.CalculateShipping(afterDiscount);
            cart.TaxTotal = PriceCalculator.CalculateTax(afterDiscount, cart.ShippingTotal);

            return cart;
        }
    }
}
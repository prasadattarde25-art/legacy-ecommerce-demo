using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Ecommerce.Core.Common;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces.Repositories;
using Ecommerce.Core.Interfaces.Services;
using Ecommerce.Core.ViewModels;

namespace Ecommerce.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IOrderRepository _orders;
        private readonly ICustomerRepository _customers;
        private readonly ICartRepository _carts;
        private readonly ICartService _cartService;

        public CheckoutService(IOrderRepository orders, ICustomerRepository customers,
            ICartRepository carts, ICartService cartService)
        {
            _orders = orders;
            _customers = customers;
            _carts = carts;
            _cartService = cartService;
        }

        public async Task<ServiceResult<Order>> CreateOrderAsync(CartViewModel cart, CheckoutAddressViewModel address,
            CheckoutShippingViewModel shipping, CheckoutPaymentViewModel payment, int customerId, Guid sessionId,
            CancellationToken cancellationToken = default)
        {
            if (customerId <= 0)
                return ServiceResult<Order>.Fail("You must be signed in to check out.");

            if (cart == null || !cart.HasItems)
                return ServiceResult<Order>.Fail("Your cart is empty.");

            var customer = await _customers.GetByIdAsync(customerId, cancellationToken);
            if (customer == null)
                return ServiceResult<Order>.Fail("Account not found.");

            cart = _cartService.ApplyPricing(cart);

            var order = new Order
            {
                CustomerId = customerId,
                OrderNumber = GenerateOrderNumber(customerId),
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                CouponCode = cart.CouponCode,
                Subtotal = cart.Subtotal,
                Discount = cart.Discount,
                ShippingTotal = cart.ShippingTotal,
                TaxTotal = cart.TaxTotal,
                GrandTotal = cart.GrandTotal,
                PaymentMethod = payment.PaymentMethod,
                TransactionId = BuildTransactionId(payment),
                ShippingMethod = shipping.ShippingMethod,
                Email = string.IsNullOrWhiteSpace(address.Email) ? customer.Email : address.Email,
                ShipToName = string.Format("{0} {1}", address.FirstName, address.LastName).Trim(),
                AddressLine1 = address.AddressLine1,
                AddressLine2 = address.AddressLine2,
                City = address.City,
                State = address.State,
                PostalCode = address.PostalCode,
                Country = string.IsNullOrWhiteSpace(address.Country) ? "US" : address.Country,
                Phone = address.Phone
            };

            foreach (var line in cart.Lines)
            {
                order.Lines.Add(new OrderLine
                {
                    ProductId = line.ProductId,
                    ProductName = line.ProductName,
                    Sku = line.Sku,
                    UnitPrice = line.UnitPrice,
                    Quantity = line.Quantity,
                    LineTotal = line.LineTotal
                });
            }

            await _orders.CreateAsync(order, cancellationToken);

            if (sessionId != Guid.Empty)
            {
                await _carts.ClearAsync(sessionId, cancellationToken);
            }

            return ServiceResult<Order>.Ok(order, "Your order was placed.");
        }

        public async Task<Order> GetOrderAsync(int orderId, int customerId, CancellationToken cancellationToken = default)
        {
            var order = await _orders.GetByIdAsync(orderId, cancellationToken);
            if (order == null || order.CustomerId != customerId) return null;
            return order;
        }

        public async Task<OrderHistoryViewModel> GetOrderHistoryAsync(int customerId, CancellationToken cancellationToken = default)
        {
            var customer = await _customers.GetByIdAsync(customerId, cancellationToken);
            var orders = await _orders.GetByCustomerAsync(customerId, cancellationToken);

            var summaries = orders.Select(o => new OrderSummaryViewModel
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                OrderDate = o.OrderDate,
                Status = o.Status,
                GrandTotal = o.GrandTotal,
                LineCount = o.Lines == null ? 0 : o.Lines.Count,
                ItemCount = o.Lines == null ? 0 : o.Lines.Sum(l => l.Quantity)
            }).ToList();

            return new OrderHistoryViewModel
            {
                Customer = customer,
                Orders = summaries
            };
        }

        private string GenerateOrderNumber(int customerId)
        {
            var random = RandomNumberGenerator.GetInt32(100, 1000);
            return string.Format("ORD-{0:yyyyMMddHHmmss}-{1:D4}-{2}",
                DateTime.UtcNow, customerId, random);
        }

        private string BuildTransactionId(CheckoutPaymentViewModel payment)
        {
            if (payment == null) return null;

            var last4 = string.Empty;
            if (!string.IsNullOrWhiteSpace(payment.CardNumber))
            {
                var digits = new string(payment.CardNumber.Where(char.IsDigit).ToArray());
                last4 = digits.Length >= 4 ? digits.Substring(digits.Length - 4) : digits;
            }

            return string.Format("PM-{0}{1}", payment.PaymentMethod, last4);
        }
    }
}

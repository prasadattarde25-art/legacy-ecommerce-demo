using System;
using Ecommerce.Core.Common;
using Ecommerce.Core.Entities;
using Ecommerce.Core.ViewModels;

namespace Ecommerce.Core.Interfaces.Services
{
    public interface ICheckoutService
    {
        ServiceResult<Order> CreateOrder(CartViewModel cart, CheckoutAddressViewModel address,
            CheckoutShippingViewModel shipping, CheckoutPaymentViewModel payment, int customerId, Guid sessionId);

        Order GetOrder(int orderId, int customerId);

        OrderHistoryViewModel GetOrderHistory(int customerId);
    }
}
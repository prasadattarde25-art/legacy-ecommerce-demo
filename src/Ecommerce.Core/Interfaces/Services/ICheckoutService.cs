using System;
using System.Threading.Tasks;
using Ecommerce.Core.Common;
using Ecommerce.Core.Entities;
using Ecommerce.Core.ViewModels;

namespace Ecommerce.Core.Interfaces.Services
{
    public interface ICheckoutService
    {
        Task<ServiceResult<Order>> CreateOrderAsync(CartViewModel cart, CheckoutAddressViewModel address,
            CheckoutShippingViewModel shipping, CheckoutPaymentViewModel payment, int customerId, Guid sessionId,
            CancellationToken cancellationToken = default);

        Task<Order> GetOrderAsync(int orderId, int customerId, CancellationToken cancellationToken = default);

        Task<OrderHistoryViewModel> GetOrderHistoryAsync(int customerId, CancellationToken cancellationToken = default);
    }
}

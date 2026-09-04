using System.Collections.Generic;
using System.Threading.Tasks;
using Ecommerce.Core.Entities;

namespace Ecommerce.Core.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> CreateAsync(Order order, CancellationToken cancellationToken = default);

        Task<Order> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<Order> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);

        Task<IList<Order>> GetByCustomerAsync(int customerId, CancellationToken cancellationToken = default);
    }
}

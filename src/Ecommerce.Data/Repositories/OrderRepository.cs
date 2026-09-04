using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces.Repositories;

namespace Ecommerce.Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly EcommerceDbContext _db;

        public OrderRepository(EcommerceDbContext db)
        {
            _db = db;
        }

        public async Task<Order> CreateAsync(Order order, CancellationToken cancellationToken = default)
        {
            _db.Orders.Add(order);
            await _db.SaveChangesAsync(cancellationToken);
            return order;
        }

        public async Task<Order> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _db.Orders
                .Include(o => o.Lines)
                .Include(o => o.Address)
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }

        public async Task<Order> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
        {
            return await _db.Orders.FirstOrDefaultAsync(o => o.OrderNumber == orderNumber, cancellationToken);
        }

        public async Task<IList<Order>> GetByCustomerAsync(int customerId, CancellationToken cancellationToken = default)
        {
            return await _db.Orders
                .Include(o => o.Lines)
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync(cancellationToken);
        }
    }
}

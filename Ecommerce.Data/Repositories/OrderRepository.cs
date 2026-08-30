using System.Collections.Generic;
using System.Linq;
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

        public Order Create(Order order)
        {
            _db.Orders.Add(order);
            _db.SaveChanges();
            return order;
        }

        public Order GetById(int id)
        {
            return _db.Orders
                .Include("Lines")
                .Include("Address")
                .FirstOrDefault(o => o.Id == id);
        }

        public Order GetByOrderNumber(string orderNumber)
        {
            return _db.Orders.FirstOrDefault(o => o.OrderNumber == orderNumber);
        }

        public IList<Order> GetByCustomer(int customerId)
        {
            return _db.Orders
                .Include("Lines")
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }
    }
}
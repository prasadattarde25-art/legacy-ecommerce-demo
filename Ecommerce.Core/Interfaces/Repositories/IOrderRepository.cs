using System.Collections.Generic;
using Ecommerce.Core.Entities;

namespace Ecommerce.Core.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Order Create(Order order);

        Order GetById(int id);

        Order GetByOrderNumber(string orderNumber);

        IList<Order> GetByCustomer(int customerId);
    }
}
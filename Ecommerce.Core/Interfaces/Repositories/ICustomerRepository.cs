using Ecommerce.Core.Entities;

namespace Ecommerce.Core.Interfaces.Repositories
{
    public interface ICustomerRepository
    {
        Customer GetByEmail(string email);

        Customer GetById(int id);

        Customer Create(Customer customer);

        void Update(Customer customer);
    }
}
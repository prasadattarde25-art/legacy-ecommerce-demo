using System.Threading.Tasks;
using Ecommerce.Core.Entities;

namespace Ecommerce.Core.Interfaces.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

        Task<Customer> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<Customer> CreateAsync(Customer customer, CancellationToken cancellationToken = default);

        Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default);
    }
}

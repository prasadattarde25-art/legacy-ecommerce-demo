using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces.Repositories;

namespace Ecommerce.Data.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly EcommerceDbContext _db;

        public CustomerRepository(EcommerceDbContext db)
        {
            _db = db;
        }

        public async Task<Customer> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _db.Customers.FirstOrDefaultAsync(c => c.Email == email, cancellationToken);
        }

        public async Task<Customer> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _db.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<Customer> CreateAsync(Customer customer, CancellationToken cancellationToken = default)
        {
            _db.Customers.Add(customer);
            await _db.SaveChangesAsync(cancellationToken);
            return customer;
        }

        public async Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}

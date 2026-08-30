using System.Linq;
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

        public Customer GetByEmail(string email)
        {
            return _db.Customers.FirstOrDefault(c => c.Email == email);
        }

        public Customer GetById(int id)
        {
            return _db.Customers.FirstOrDefault(c => c.Id == id);
        }

        public Customer Create(Customer customer)
        {
            _db.Customers.Add(customer);
            _db.SaveChanges();
            return customer;
        }

        public void Update(Customer customer)
        {
            _db.SaveChanges();
        }
    }
}
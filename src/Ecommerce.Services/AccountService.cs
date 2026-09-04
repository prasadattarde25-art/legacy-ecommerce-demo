using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecommerce.Core.Common;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces.Repositories;
using Ecommerce.Core.Interfaces.Services;
using Ecommerce.Core.ViewModels;
using Ecommerce.Services.Security;

namespace Ecommerce.Services
{
    public class AccountService : IAccountService
    {
        private readonly ICustomerRepository _customers;
        private readonly IOrderRepository _orders;

        public AccountService(ICustomerRepository customers, IOrderRepository orders)
        {
            _customers = customers;
            _orders = orders;
        }

        public async Task<ServiceResult<Customer>> LoginAsync(LoginViewModel model, CancellationToken cancellationToken = default)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                return ServiceResult<Customer>.Fail("Enter your email and password.");

            var customer = await _customers.GetByEmailAsync(model.Email, cancellationToken);
            if (customer == null || !customer.IsActive)
                return ServiceResult<Customer>.Fail("Invalid email or password.");

            if (!PasswordHasher.Verify(model.Password, customer.PasswordSalt, customer.PasswordHash))
                return ServiceResult<Customer>.Fail("Invalid email or password.");

            return ServiceResult<Customer>.Ok(customer);
        }

        public async Task<ServiceResult<Customer>> RegisterAsync(RegisterViewModel model, CancellationToken cancellationToken = default)
        {
            if (model == null)
                return ServiceResult<Customer>.Fail("Missing registration data.");

            if (await _customers.GetByEmailAsync(model.Email, cancellationToken) != null)
                return ServiceResult<Customer>.Fail("An account with that email already exists.");

            var salt = PasswordHasher.GenerateSalt();
            var customer = new Customer
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Phone = model.Phone,
                PasswordSalt = salt,
                PasswordHash = PasswordHasher.Hash(model.Password, salt),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Addresses = new List<Address>()
            };

            await _customers.CreateAsync(customer, cancellationToken);

            return ServiceResult<Customer>.Ok(customer, "Your account was created.");
        }

        public async Task<Customer> GetCustomerByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _customers.GetByIdAsync(id, cancellationToken);
        }

        public async Task<OrderHistoryViewModel> GetOrderHistoryAsync(int customerId, CancellationToken cancellationToken = default)
        {
            var orders = await _orders.GetByCustomerAsync(customerId, cancellationToken);
            var summaries = orders.Select(o => new OrderSummaryViewModel
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                OrderDate = o.OrderDate,
                Status = o.Status,
                GrandTotal = o.GrandTotal,
                LineCount = o.Lines == null ? 0 : o.Lines.Count,
                ItemCount = o.Lines == null ? 0 : o.Lines.Sum(l => l.Quantity)
            }).ToList();

            return new OrderHistoryViewModel
            {
                Customer = await _customers.GetByIdAsync(customerId, cancellationToken),
                Orders = summaries
            };
        }
    }
}

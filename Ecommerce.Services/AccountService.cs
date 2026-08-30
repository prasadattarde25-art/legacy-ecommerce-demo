using System;
using System.Collections.Generic;
using System.Linq;
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

        public ServiceResult<Customer> Login(LoginViewModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                return ServiceResult<Customer>.Fail("Enter your email and password.");

            var customer = _customers.GetByEmail(model.Email);
            if (customer == null || !customer.IsActive)
                return ServiceResult<Customer>.Fail("Invalid email or password.");

            if (!PasswordHasher.Verify(model.Password, customer.PasswordSalt, customer.PasswordHash))
                return ServiceResult<Customer>.Fail("Invalid email or password.");

            return ServiceResult<Customer>.Ok(customer);
        }

        public ServiceResult<Customer> Register(RegisterViewModel model)
        {
            if (model == null)
                return ServiceResult<Customer>.Fail("Missing registration data.");

            if (_customers.GetByEmail(model.Email) != null)
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
                CreatedAt = DateTime.Now,
                Addresses = new List<Address>()
            };

            _customers.Create(customer);

            return ServiceResult<Customer>.Ok(customer, "Your account was created.");
        }

        public Customer GetCustomerById(int id)
        {
            return _customers.GetById(id);
        }

        public OrderHistoryViewModel GetOrderHistory(int customerId)
        {
            var orders = _orders.GetByCustomer(customerId);
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
                Customer = _customers.GetById(customerId),
                Orders = summaries
            };
        }
    }
}
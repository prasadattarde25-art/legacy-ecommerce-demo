using Ecommerce.Core.Common;
using Ecommerce.Core.Entities;
using Ecommerce.Core.ViewModels;

namespace Ecommerce.Core.Interfaces.Services
{
    public interface IAccountService
    {
        ServiceResult<Customer> Login(LoginViewModel model);

        ServiceResult<Customer> Register(RegisterViewModel model);

        Customer GetCustomerById(int id);

        OrderHistoryViewModel GetOrderHistory(int customerId);
    }
}
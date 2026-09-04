using System.Threading.Tasks;
using Ecommerce.Core.Common;
using Ecommerce.Core.Entities;
using Ecommerce.Core.ViewModels;

namespace Ecommerce.Core.Interfaces.Services
{
    public interface IAccountService
    {
        Task<ServiceResult<Customer>> LoginAsync(LoginViewModel model, CancellationToken cancellationToken = default);

        Task<ServiceResult<Customer>> RegisterAsync(RegisterViewModel model, CancellationToken cancellationToken = default);

        Task<Customer> GetCustomerByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<OrderHistoryViewModel> GetOrderHistoryAsync(int customerId, CancellationToken cancellationToken = default);
    }
}

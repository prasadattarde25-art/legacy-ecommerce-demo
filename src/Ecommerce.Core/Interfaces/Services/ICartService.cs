using System.Threading.Tasks;
using Ecommerce.Core.ViewModels;

namespace Ecommerce.Core.Interfaces.Services
{
    public interface ICartService
    {
        Task<CartViewModel> AddToCartAsync(CartViewModel cart, int productId, int quantity, CancellationToken cancellationToken = default);

        CartViewModel UpdateLine(CartViewModel cart, int productId, int quantity);

        CartViewModel RemoveLine(CartViewModel cart, int productId);

        CartViewModel ApplyPricing(CartViewModel cart);
    }
}

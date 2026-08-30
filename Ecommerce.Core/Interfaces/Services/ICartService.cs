using Ecommerce.Core.ViewModels;

namespace Ecommerce.Core.Interfaces.Services
{
    public interface ICartService
    {
        CartViewModel AddToCart(CartViewModel cart, int productId, int quantity);

        CartViewModel UpdateLine(CartViewModel cart, int productId, int quantity);

        CartViewModel RemoveLine(CartViewModel cart, int productId);

        CartViewModel ApplyPricing(CartViewModel cart);
    }
}
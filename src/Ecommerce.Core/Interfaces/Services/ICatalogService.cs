using System.Collections.Generic;
using System.Threading.Tasks;
using Ecommerce.Core.Entities;
using Ecommerce.Core.ViewModels;

namespace Ecommerce.Core.Interfaces.Services
{
    public interface ICatalogService
    {
        Task<ProductListViewModel> GetListingAsync(int? categoryId, string q, int page, CancellationToken cancellationToken = default);

        Task<ProductDetailViewModel> GetDetailAsync(int productId, CancellationToken cancellationToken = default);

        Task<IList<ProductCardViewModel>> GetFeaturedAsync(CancellationToken cancellationToken = default);

        Task<IList<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default);

        Task<IList<Category>> GetSubcategoriesAsync(int parentId, CancellationToken cancellationToken = default);
    }
}

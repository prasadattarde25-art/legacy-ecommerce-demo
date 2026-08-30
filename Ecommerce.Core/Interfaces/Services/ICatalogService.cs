using System.Collections.Generic;
using Ecommerce.Core.Entities;
using Ecommerce.Core.ViewModels;

namespace Ecommerce.Core.Interfaces.Services
{
    public interface ICatalogService
    {
        ProductListViewModel GetListing(int? categoryId, string q, int page);

        ProductDetailViewModel GetDetail(int productId);

        IList<ProductCardViewModel> GetFeatured();

        IList<Category> GetRootCategories();

        IList<Category> GetSubcategories(int parentId);
    }
}
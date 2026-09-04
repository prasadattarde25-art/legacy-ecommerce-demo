using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecommerce.Core.Common;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces.Repositories;
using Ecommerce.Core.Interfaces.Services;
using Ecommerce.Core.ViewModels;

namespace Ecommerce.Services
{
    public class CatalogService : ICatalogService
    {
        private const int PageSize = 12;
        private readonly IProductRepository _products;
        private readonly ICategoryRepository _categories;

        public CatalogService(IProductRepository products, ICategoryRepository categories)
        {
            _products = products;
            _categories = categories;
        }

        public async Task<ProductListViewModel> GetListingAsync(int? categoryId, string q, int page, CancellationToken cancellationToken = default)
        {
            if (page < 1) page = 1;

            var result = await _products.SearchAsync(categoryId, q, page, PageSize, cancellationToken);
            var cards = result.Items.Select(ToCard).ToList();

            return new ProductListViewModel
            {
                Products = cards,
                PagedResult = new PagedResult<ProductCardViewModel>
                {
                    Items = cards,
                    Page = result.Page,
                    PageSize = result.PageSize,
                    TotalItems = result.TotalItems
                },
                Categories = await _categories.GetActiveCategoriesAsync(cancellationToken),
                CategoryId = categoryId,
                Query = q,
                Page = page
            };
        }

        public async Task<ProductDetailViewModel> GetDetailAsync(int productId, CancellationToken cancellationToken = default)
        {
            var product = await _products.GetByIdAsync(productId, cancellationToken);
            if (product == null) return null;

            var activeVariants = (product.Variants ?? Enumerable.Empty<ProductVariant>())
                .Where(v => v.IsActive)
                .ToList();

            return new ProductDetailViewModel
            {
                Product = product,
                Images = (product.Images ?? Enumerable.Empty<ProductImage>())
                    .OrderBy(i => i.SortOrder)
                    .ToList(),
                Variants = activeVariants,
                SelectedVariantId = activeVariants.Count > 0 ? activeVariants[0].Id : (int?)null,
                RelatedProducts = product.CategoryId.HasValue
                    ? (await _products.GetRelatedAsync(product.CategoryId.Value, product.Id, 4, cancellationToken)).Select(ToCard).ToList()
                    : new List<ProductCardViewModel>()
            };
        }

        public async Task<IList<ProductCardViewModel>> GetFeaturedAsync(CancellationToken cancellationToken = default)
        {
            return (await _products.GetFeaturedAsync(8, cancellationToken)).Select(ToCard).ToList();
        }

        public async Task<IList<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default)
        {
            return await _categories.GetRootCategoriesAsync(cancellationToken);
        }

        public async Task<IList<Category>> GetSubcategoriesAsync(int parentId, CancellationToken cancellationToken = default)
        {
            return await _categories.GetSubcategoriesAsync(parentId, cancellationToken);
        }

        private static ProductCardViewModel ToCard(Product p)
        {
            return new ProductCardViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                Price = p.Price,
                ListPrice = p.ListPrice,
                ThumbnailUrl = p.ThumbnailUrl,
                ShortDescription = p.ShortDescription
            };
        }
    }
}

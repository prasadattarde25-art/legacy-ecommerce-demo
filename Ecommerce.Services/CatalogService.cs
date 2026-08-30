using System.Collections.Generic;
using System.Linq;
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

        public ProductListViewModel GetListing(int? categoryId, string q, int page)
        {
            if (page < 1) page = 1;

            var result = _products.Search(categoryId, q, page, PageSize);
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
                Categories = _categories.GetActiveCategories(),
                CategoryId = categoryId,
                Query = q,
                Page = page
            };
        }

        public ProductDetailViewModel GetDetail(int productId)
        {
            var product = _products.GetById(productId);
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
                    ? _products.GetRelated(product.CategoryId.Value, product.Id, 4).Select(ToCard).ToList()
                    : new List<ProductCardViewModel>()
            };
        }

        public IList<ProductCardViewModel> GetFeatured()
        {
            return _products.GetFeatured(8).Select(ToCard).ToList();
        }

        public IList<Category> GetRootCategories()
        {
            return _categories.GetRootCategories();
        }

        public IList<Category> GetSubcategories(int parentId)
        {
            return _categories.GetSubcategories(parentId);
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
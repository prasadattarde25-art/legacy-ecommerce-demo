using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Core.Common;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces.Repositories;

namespace Ecommerce.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly EcommerceDbContext _db;

        public ProductRepository(EcommerceDbContext db)
        {
            _db = db;
        }

        public async Task<Product> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _db.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<Product> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            return await _db.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.Slug == slug && p.IsActive, cancellationToken);
        }

        public async Task<PagedResult<Product>> SearchAsync(int? categoryId, string q, int page, int size, CancellationToken cancellationToken = default)
        {
            IQueryable<Product> query = _db.Products.Where(p => p.IsActive);

            if (categoryId.HasValue)
            {
                var ids = await GetCategoryAndDescendantIdsAsync(categoryId.Value, cancellationToken);
                query = query.Where(p => p.CategoryId != null && ids.Contains(p.CategoryId.Value));
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(p =>
                    p.Name.Contains(q) ||
                    (p.ShortDescription != null && p.ShortDescription.Contains(q)) ||
                    p.Sku.Contains(q));
            }

            var total = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync(cancellationToken);

            return new PagedResult<Product>
            {
                Items = items,
                Page = page,
                PageSize = size,
                TotalItems = total
            };
        }

        public async Task<IList<Product>> GetFeaturedAsync(int take, CancellationToken cancellationToken = default)
        {
            return await _db.Products
                .Where(p => p.IsActive && p.IsFeatured)
                .OrderByDescending(p => p.CreatedAt)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        public async Task<IList<Product>> GetRelatedAsync(int categoryId, int excludeProductId, int take, CancellationToken cancellationToken = default)
        {
            return await _db.Products
                .Where(p => p.IsActive && p.CategoryId == categoryId && p.Id != excludeProductId)
                .OrderByDescending(p => p.CreatedAt)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        private async Task<IList<int>> GetCategoryAndDescendantIdsAsync(int categoryId, CancellationToken cancellationToken)
        {
            var result = new List<int>();
            var pending = new Queue<int>();
            pending.Enqueue(categoryId);

            while (pending.Count > 0)
            {
                var current = pending.Dequeue();
                result.Add(current);

                var children = await _db.Categories
                    .Where(c => c.ParentId == current)
                    .Select(c => c.Id)
                    .ToListAsync(cancellationToken);

                foreach (var child in children)
                {
                    pending.Enqueue(child);
                }
            }

            return result;
        }
    }
}

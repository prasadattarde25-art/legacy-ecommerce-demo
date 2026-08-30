using System;
using System.Collections.Generic;
using System.Linq;
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

        public Product GetById(int id)
        {
            return _db.Products
                .Include("Category")
                .Include("Images")
                .Include("Variants")
                .FirstOrDefault(p => p.Id == id);
        }

        public Product GetBySlug(string slug)
        {
            return _db.Products
                .Include("Category")
                .Include("Images")
                .Include("Variants")
                .FirstOrDefault(p => p.Slug == slug && p.IsActive);
        }

        public Core.Common.PagedResult<Product> Search(int? categoryId, string q, int page, int size)
        {
            IQueryable<Product> query = _db.Products.Where(p => p.IsActive);

            if (categoryId.HasValue)
            {
                var ids = GetCategoryAndDescendantIds(categoryId.Value);
                query = query.Where(p => ids.Contains(p.CategoryId.Value));
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(p =>
                    p.Name.Contains(q) ||
                    p.ShortDescription.Contains(q) ||
                    p.Sku.Contains(q));
            }

            var total = query.Count();
            var items = query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();

            return new Core.Common.PagedResult<Product>
            {
                Items = items,
                Page = page,
                PageSize = size,
                TotalItems = total
            };
        }

        public IList<Product> GetFeatured(int take)
        {
            return _db.Products
                .Where(p => p.IsActive && p.IsFeatured)
                .OrderByDescending(p => p.CreatedAt)
                .Take(take)
                .ToList();
        }

        public IList<Product> GetRelated(int categoryId, int excludeProductId, int take)
        {
            return _db.Products
                .Where(p => p.IsActive && p.CategoryId == categoryId && p.Id != excludeProductId)
                .OrderByDescending(p => p.CreatedAt)
                .Take(take)
                .ToList();
        }

        private IList<int> GetCategoryAndDescendantIds(int categoryId)
        {
            var result = new List<int>();
            var pending = new Queue<int>();
            pending.Enqueue(categoryId);

            while (pending.Count > 0)
            {
                var current = pending.Dequeue();
                result.Add(current);

                var children = _db.Categories
                    .Where(c => c.ParentId == current)
                    .Select(c => c.Id)
                    .ToList();

                foreach (var child in children)
                {
                    pending.Enqueue(child);
                }
            }

            return result;
        }
    }
}
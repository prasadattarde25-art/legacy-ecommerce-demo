using System.Collections.Generic;
using System.Linq;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces.Repositories;

namespace Ecommerce.Data.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly EcommerceDbContext _db;

        public CategoryRepository(EcommerceDbContext db)
        {
            _db = db;
        }

        public IList<Category> GetActiveCategories()
        {
            return _db.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .ToList();
        }

        public IList<Category> GetRootCategories()
        {
            return _db.Categories
                .Where(c => c.IsActive && c.ParentId == null)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .ToList();
        }

        public IList<Category> GetSubcategories(int parentId)
        {
            return _db.Categories
                .Where(c => c.IsActive && c.ParentId == parentId)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .ToList();
        }

        public Category GetById(int id)
        {
            return _db.Categories.FirstOrDefault(c => c.Id == id);
        }

        public Category GetBySlug(string slug)
        {
            return _db.Categories.FirstOrDefault(c => c.Slug == slug && c.IsActive);
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IList<Category>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default)
        {
            return await _db.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<IList<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default)
        {
            return await _db.Categories
                .Where(c => c.IsActive && c.ParentId == null)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<IList<Category>> GetSubcategoriesAsync(int parentId, CancellationToken cancellationToken = default)
        {
            return await _db.Categories
                .Where(c => c.IsActive && c.ParentId == parentId)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<Category> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _db.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<Category> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            return await _db.Categories.FirstOrDefaultAsync(c => c.Slug == slug && c.IsActive, cancellationToken);
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Ecommerce.Core.Entities;

namespace Ecommerce.Core.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        Task<IList<Category>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default);

        Task<IList<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default);

        Task<IList<Category>> GetSubcategoriesAsync(int parentId, CancellationToken cancellationToken = default);

        Task<Category> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<Category> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    }
}

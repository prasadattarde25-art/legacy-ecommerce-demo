using System.Collections.Generic;
using System.Threading.Tasks;
using Ecommerce.Core.Common;
using Ecommerce.Core.Entities;

namespace Ecommerce.Core.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<Product> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<Product> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

        Task<PagedResult<Product>> SearchAsync(int? categoryId, string q, int page, int size, CancellationToken cancellationToken = default);

        Task<IList<Product>> GetFeaturedAsync(int take, CancellationToken cancellationToken = default);

        Task<IList<Product>> GetRelatedAsync(int categoryId, int excludeProductId, int take, CancellationToken cancellationToken = default);
    }
}

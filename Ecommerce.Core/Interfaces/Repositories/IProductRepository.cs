using System.Collections.Generic;
using Ecommerce.Core.Common;
using Ecommerce.Core.Entities;

namespace Ecommerce.Core.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Product GetById(int id);

        Product GetBySlug(string slug);

        PagedResult<Product> Search(int? categoryId, string q, int page, int size);

        IList<Product> GetFeatured(int take);

        IList<Product> GetRelated(int categoryId, int excludeProductId, int take);
    }
}
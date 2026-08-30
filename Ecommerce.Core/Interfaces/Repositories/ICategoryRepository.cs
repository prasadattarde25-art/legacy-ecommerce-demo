using System.Collections.Generic;
using Ecommerce.Core.Entities;

namespace Ecommerce.Core.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        IList<Category> GetActiveCategories();

        IList<Category> GetRootCategories();

        IList<Category> GetSubcategories(int parentId);

        Category GetById(int id);

        Category GetBySlug(string slug);
    }
}
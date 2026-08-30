using System.Collections.Generic;
using Ecommerce.Core.Common;
using Ecommerce.Core.Entities;

namespace Ecommerce.Core.ViewModels
{
    public class ProductListViewModel
    {
        public IList<ProductCardViewModel> Products { get; set; }

        public IList<Category> Categories { get; set; }

        public PagedResult<ProductCardViewModel> PagedResult { get; set; }

        public int? CategoryId { get; set; }

        public string Query { get; set; }

        public int Page { get; set; }
    }
}
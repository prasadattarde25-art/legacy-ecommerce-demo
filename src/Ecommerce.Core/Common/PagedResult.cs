using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Common
{
    public class PagedResult<T>
    {
        public IList<T> Items { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalItems { get; set; }

        public int TotalPages
        {
            get { return PageSize <= 0 ? 0 : (int)Math.Ceiling((double)TotalItems / PageSize); }
        }

        public bool HasPrevious
        {
            get { return Page > 1; }
        }

        public bool HasNext
        {
            get { return Page < TotalPages; }
        }
    }
}
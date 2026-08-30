using System.Collections.Generic;
using Ecommerce.Core.Entities;

namespace Ecommerce.Core.ViewModels
{
    public class OrderHistoryViewModel
    {
        public Customer Customer { get; set; }

        public IList<OrderSummaryViewModel> Orders { get; set; }
    }
}
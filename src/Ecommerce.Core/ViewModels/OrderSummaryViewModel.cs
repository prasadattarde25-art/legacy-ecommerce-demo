using System;

namespace Ecommerce.Core.ViewModels
{
    public class OrderSummaryViewModel
    {
        public int Id { get; set; }

        public string OrderNumber { get; set; }

        public DateTime OrderDate { get; set; }

        public string Status { get; set; }

        public decimal GrandTotal { get; set; }

        public int LineCount { get; set; }

        public int ItemCount { get; set; }
    }
}
using System.Collections.Generic;
using System.Linq;
using Ecommerce.Core.Entities;

namespace Ecommerce.Core.ViewModels
{
    public class CartViewModel
    {
        public CartViewModel()
        {
            Lines = new List<CartItem>();
        }

        public List<CartItem> Lines { get; set; }

        public string CouponCode { get; set; }

        public decimal Discount { get; set; }

        public decimal ShippingTotal { get; set; }

        public decimal TaxTotal { get; set; }

        public decimal Subtotal
        {
            get { return Lines.Sum(l => l.LineTotal); }
        }

        public decimal GrandTotal
        {
            get { return Subtotal - Discount + ShippingTotal + TaxTotal; }
        }

        public int ItemCount
        {
            get { return Lines.Sum(l => l.Quantity); }
        }

        public bool HasItems
        {
            get { return Lines.Count > 0; }
        }
    }
}
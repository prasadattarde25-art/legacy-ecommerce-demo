using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Core.Entities
{
    [Table("Orders")]
    public class Order
    {
        public Order()
        {
            Lines = new HashSet<OrderLine>();
        }

        [Key]
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int? AddressId { get; set; }

        [Required, MaxLength(32)]
        public string OrderNumber { get; set; }

        public DateTime OrderDate { get; set; }

        [Required, MaxLength(20)]
        public string Status { get; set; }

        [MaxLength(40)]
        public string CouponCode { get; set; }

        public decimal Subtotal { get; set; }

        public decimal Discount { get; set; }

        public decimal ShippingTotal { get; set; }

        public decimal TaxTotal { get; set; }

        public decimal GrandTotal { get; set; }

        [Required, MaxLength(40)]
        public string PaymentMethod { get; set; }

        [MaxLength(100)]
        public string TransactionId { get; set; }

        [MaxLength(40)]
        public string ShippingMethod { get; set; }

        [Required, MaxLength(200)]
        public string Email { get; set; }

        [MaxLength(200)]
        public string ShipToName { get; set; }

        [MaxLength(200)]
        public string AddressLine1 { get; set; }

        [MaxLength(200)]
        public string AddressLine2 { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        [MaxLength(100)]
        public string State { get; set; }

        [MaxLength(20)]
        public string PostalCode { get; set; }

        [MaxLength(100)]
        public string Country { get; set; }

        [MaxLength(40)]
        public string Phone { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual Address Address { get; set; }

        public virtual ICollection<OrderLine> Lines { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Core.Entities
{
    [Table("CartItems")]
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        public Guid SessionId { get; set; }

        public int ProductId { get; set; }

        [Required, MaxLength(200)]
        public string ProductName { get; set; }

        [Required, MaxLength(64)]
        public string Sku { get; set; }

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [NotMapped]
        public decimal LineTotal
        {
            get { return UnitPrice * Quantity; }
        }

        public virtual Product Product { get; set; }
    }
}
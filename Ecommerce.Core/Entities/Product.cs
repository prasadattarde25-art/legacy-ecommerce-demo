using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Core.Entities
{
    [Table("Products")]
    public class Product
    {
        public Product()
        {
            Images = new HashSet<ProductImage>();
            Variants = new HashSet<ProductVariant>();
        }

        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }

        [Required, MaxLength(200)]
        public string Slug { get; set; }

        [MaxLength(600)]
        public string ShortDescription { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public decimal? ListPrice { get; set; }

        [Required, MaxLength(64)]
        public string Sku { get; set; }

        public int? CategoryId { get; set; }

        public bool IsFeatured { get; set; }

        public bool IsActive { get; set; }

        public int StockQuantity { get; set; }

        [MaxLength(600)]
        public string ThumbnailUrl { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual Category Category { get; set; }

        public virtual ICollection<ProductImage> Images { get; set; }

        public virtual ICollection<ProductVariant> Variants { get; set; }
    }
}
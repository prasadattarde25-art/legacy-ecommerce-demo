using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Core.Entities
{
    [Table("Categories")]
    public class Category
    {
        public Category()
        {
            Children = new HashSet<Category>();
            Products = new HashSet<Product>();
        }

        [Key]
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; }

        [Required, MaxLength(150)]
        public string Slug { get; set; }

        public int? ParentId { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }

        public virtual Category Parent { get; set; }

        public virtual ICollection<Category> Children { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
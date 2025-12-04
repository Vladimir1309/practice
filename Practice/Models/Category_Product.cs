using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Practice.Models
{
    [Table("category_product")]
    public class Category_Product
    {
        [Key]
        [Column("IdCategory")]
        public int IdCategory { get; set; }

        [Column("Name")]
        [StringLength(255)]
        public string Name { get; set; }

        // Навигационное свойство
        public virtual ICollection<Product> Products { get; set; }

        public Category_Product()
        {
            Products = new HashSet<Product>();
        }
    }
}
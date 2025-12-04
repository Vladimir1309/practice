using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Practice.Models
{
    [Table("product")]
    public class Product
    {
        [Key]
        [Column("IdProduct")]
        public int IdProduct { get; set; }

        [Column("IdCategory")]
        public int IdCategory { get; set; }

        [Column("Name")]
        [StringLength(255)]
        public string Name { get; set; }

        [Column("Price")]
        public decimal Price { get; set; }

        // Добавляем свойство для изображения (его нет в БД, но нужно для отображения)
        [NotMapped] // Не маппится на БД
        public string ImagePath { get; set; }

        // Навигационные свойства
        [ForeignKey("IdCategory")]
        public virtual Category_Product Category { get; set; }

        public virtual ICollection<Order_Product> Order_Products { get; set; }
        public virtual ICollection<Accounting> Accountings { get; set; }

        public Product()
        {
            Order_Products = new HashSet<Order_Product>();
            Accountings = new HashSet<Accounting>();
        }
    }
}
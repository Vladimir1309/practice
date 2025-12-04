using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Practice.Models
{
    [Table("order_product")]
    public class Order_Product
    {
        [Key]
        [Column("IdOrderProduct")]
        public int IdOrderProduct { get; set; }

        [Column("IdOrder")]
        public int IdOrder { get; set; }

        [Column("IdProduct")]
        public int IdProduct { get; set; }

        [Column("Amount")]
        public int Amount { get; set; }

        [Column("IsFavourited")]
        public bool IsFavourited { get; set; }

        // Навигационные свойства
        [ForeignKey("IdOrder")]
        public virtual Order Order { get; set; }

        [ForeignKey("IdProduct")]
        public virtual Product Product { get; set; }
    }
}
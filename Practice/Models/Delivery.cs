using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Practice.Models
{
    [Table("delivery")]
    public class Delivery
    {
        [Key]
        [Column("IdDelivery")]
        public int IdDelivery { get; set; }

        [Column("`IdUser(Client)`")]
        public int IdUserClient { get; set; }

        [Column("`IdUser(Employee)`")]
        public int IdUserEmployee { get; set; }

        [Column("IdOrder")]
        public int IdOrder { get; set; }

        [Column("Date")]
        public DateTime Date { get; set; }

        [Column("AddressDelivery")]
        [StringLength(255)]
        public string AddressDelivery { get; set; }

        [Column("IsCompleted")]
        public bool IsCompleted { get; set; }

        // Навигационные свойства
        [ForeignKey("IdUserClient")]
        public virtual User UserClient { get; set; }

        [ForeignKey("IdUserEmployee")]
        public virtual User UserEmployee { get; set; }

        [ForeignKey("IdOrder")]
        public virtual Order Order { get; set; }
    }
}
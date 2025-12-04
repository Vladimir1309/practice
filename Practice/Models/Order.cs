using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Practice.Models
{
    [Table("`order`")] // Обратные кавычки для зарезервированного слова
    public class Order
    {
        [Key]
        [Column("IdOrder")]
        public int IdOrder { get; set; }

        [Column("`IdUser(Client)`")] // Обратные кавычки для столбца с скобками
        public int IdUserClient { get; set; }

        [Column("Check")]
        public decimal Check { get; set; }

        [Column("Delivery")]
        public bool Delivery { get; set; } // В БД это int, а не bool

        [Column("IsCompleted")]
        public bool IsCompleted { get; set; }

        // Добавляем свойство OrderDate если оно есть в БД
        [Column("OrderDate")]
        public DateTime? OrderDate { get; set; }

        // Навигационные свойства
        [ForeignKey("IdUserClient")]
        public virtual User UserClient { get; set; }

        public virtual ICollection<Order_Product> Order_Products { get; set; }
        public virtual ICollection<Order_Employee> Order_Employees { get; set; }
        public virtual ICollection<Delivery> Deliveries { get; set; }

        public Order()
        {
            Order_Products = new HashSet<Order_Product>();
            Order_Employees = new HashSet<Order_Employee>();
            Deliveries = new HashSet<Delivery>();
            OrderDate = DateTime.Now;
        }
    }
}
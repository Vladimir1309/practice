using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Practice.Models
{
    [Table("user")]
    public class User
    {
        [Key]
        [Column("IdUser")]
        public int IdUser { get; set; }

        [Column("IdPost")]
        public int IdPost { get; set; }

        [Column("Login")]
        [StringLength(255)]
        public string Login { get; set; }

        [Column("Password")]
        [StringLength(255)]
        public string Password { get; set; }

        [Column("LastName")]
        [StringLength(255)]
        public string LastName { get; set; }

        [Column("FirstName")]
        [StringLength(255)]
        public string FirstName { get; set; }

        [Column("Patronymic")]
        [StringLength(255)]
        public string Patronymic { get; set; }

        [Column("Phone")]
        [StringLength(255)]
        public string Phone { get; set; }

        [Column("Email")]
        [StringLength(255)]
        public string Email { get; set; }

        [Column("Birthday")]
        public DateTime Birthday { get; set; }

        [Column("Address")]
        [StringLength(255)]
        public string Address { get; set; }

        // Навигационные свойства
        [ForeignKey("IdPost")]
        public virtual Post Post { get; set; }

        // Коллекции для связей
        public virtual ICollection<Order> OrdersAsClient { get; set; }
        public virtual ICollection<Order_Employee> OrderEmployees { get; set; }
        public virtual ICollection<Delivery> DeliveriesAsClient { get; set; }
        public virtual ICollection<Delivery> DeliveriesAsEmployee { get; set; }

        public User()
        {
            OrdersAsClient = new HashSet<Order>();
            OrderEmployees = new HashSet<Order_Employee>();
            DeliveriesAsClient = new HashSet<Delivery>();
            DeliveriesAsEmployee = new HashSet<Delivery>();
        }
    }
}
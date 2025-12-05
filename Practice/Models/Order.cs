namespace Practice.Models
{
    public class Order
    {
        public int IdOrder { get; set; }
        public int IdUserClient { get; set; }
        public decimal Check { get; set; }
        public bool Delivery { get; set; } // Измените с int на bool
        public bool IsCompleted { get; set; }
        public DateTime? OrderDate { get; set; }

        // Навигационные свойства
        public User UserClient { get; set; }
        public List<Order_Product> OrderProducts { get; set; } = new List<Order_Product>();
    }
}
using System;

namespace Practice.Models
{
    public class Order
    {
        public int IdOrder { get; set; }
        public int IdUserClient { get; set; }
        public decimal Check { get; set; }
        public bool Delivery { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
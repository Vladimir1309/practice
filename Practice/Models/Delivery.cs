using System;

namespace Practice.Models
{
    public class Delivery
    {
        public int IdDelivery { get; set; }
        public int IdUserClient { get; set; }
        public int IdUserEmployee { get; set; }
        public int IdOrder { get; set; }
        public DateTime Date { get; set; }
        public string AddressDelivery { get; set; }
        public bool IsCompleted { get; set; }
    }
}
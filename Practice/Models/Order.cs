using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice.Models
{
    public class Order
    {
        public int IdOrder { get; set; }
        public int IdUserClient { get; set; }
        public decimal Check {  get; set; }
        public bool Delivery { get; set; }
        public bool IsCompleted { get; set; }
        //public List<Order_Product> Order_Products { get; set; } = new List<Order_Product>();

    }
}

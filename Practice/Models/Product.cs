using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice.Models
{
    public class Product
    {
        public int IdProduct { get; set; }
        public int IdCategory { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        //public List<Order_Product> Order_Products { get; set; } = new List<Order_Product>();
    }
}

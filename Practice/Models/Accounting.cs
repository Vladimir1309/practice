using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice.Models
{
    public class Accounting
    {
        public int IdAccounting {  get; set; }
        public int IdProduct {  get; set; }
        public int IdStorage {  get; set; }
        public int AmountOfProduct { get; set; }

        public List<Product> Products { get; set; } = new List<Product>();
        public List<Storage> Storages { get; set; } = new List<Storage>();
    }
}

using System.Collections.Generic;

namespace Practice.Models
{
    public class Accounting
    {
        public int IdAccounting { get; set; }
        public int IdProduct { get; set; }
        public int IdStorage { get; set; }
        public int AmountOfProduct { get; set; }

        // Навигационные свойства
        public virtual Product Product { get; set; }
        public virtual Storage Storage { get; set; }
    }
}
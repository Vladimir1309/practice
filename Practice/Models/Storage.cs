using System.Collections.Generic;

namespace Practice.Models
{
    public class Storage
    {
        public int IdStorage { get; set; }
        public int IdSupplier { get; set; }
        public string Address { get; set; }

        // Навигационные свойства
        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<Accounting> Accountings { get; set; } = new List<Accounting>();
    }
}
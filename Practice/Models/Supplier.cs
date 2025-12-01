using System.Collections.Generic;

namespace Practice.Models
{
    public class Supplier
    {
        public int IdSupplier { get; set; }
        public string NameOfDealer { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        // Навигационное свойство
        public virtual ICollection<Storage> Storages { get; set; } = new List<Storage>();
    }
}
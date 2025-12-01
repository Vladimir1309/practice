namespace Practice.Models
{
    public class Order_Employee
    {
        public int IdOrderEmployee { get; set; }
        public int IdOrder { get; set; }
        public int IdUserEmployee { get; set; }

        // Навигационные свойства
        public virtual Order Order { get; set; }
        public virtual User UserEmployee { get; set; }
    }
}
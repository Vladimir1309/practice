namespace Practice.Models
{
    public class Order_Product
    {
        public int IdOrderProduct { get; set; }
        public int IdOrder { get; set; }
        public int IdProduct { get; set; }
        public int Amount { get; set; } // Изначальное количество
        public int ReturnedQuantity { get; set; } // Количество возвращенных
        public bool IsFavourited { get; set; }
        public bool IsReturned { get; set; } // Флаг возврата

        // Навигационные свойства
        public Product Product { get; set; }
        public Order Order { get; set; }

        // Вычисляемое свойство для оставшегося количества
        public int RemainingQuantity => Amount - ReturnedQuantity;
    }
}
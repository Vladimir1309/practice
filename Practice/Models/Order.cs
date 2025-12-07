namespace Practice.Models
{
    public class Order
    {
        public int IdOrder { get; set; }
        public int IdUserClient { get; set; }
        public decimal Check { get; set; } // Оригинальная сумма заказа (без учета возвратов)
        public bool Delivery { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? OrderDate { get; set; }

        // Навигационные свойства
        public User UserClient { get; set; }
        public List<Order_Product> OrderProducts { get; set; } = new List<Order_Product>();

        // Вычисляемое свойство (не сохраняется в БД)
        public decimal ActualAmount
        {
            get
            {
                if (OrderProducts == null || OrderProducts.Count == 0)
                    return Check;

                decimal returnedAmount = OrderProducts
                    .Where(op => op.ReturnedQuantity > 0)
                    .Sum(op => op.ReturnedQuantity * op.Product?.Price ?? 0);

                return Check - returnedAmount;
            }
        }
    }
}
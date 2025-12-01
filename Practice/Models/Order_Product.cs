namespace Practice.Models
{
    public class Order_Product
    {
        public int IdOrderProduct { get; set; }
        public int IdOrder { get; set; }
        public int IdProduct { get; set; }
        public int Amount { get; set; }
        public bool IsFavourited { get; set; }
    }
}
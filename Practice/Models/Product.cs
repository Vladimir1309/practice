namespace Practice.Models
{
    public class Product
    {
        public int IdProduct { get; set; }
        public int IdCategory { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ImagePath { get; set; }
    }
}
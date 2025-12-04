using Practice.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Practice
{
    public static class DataManager
    {
        // Статические коллекции для хранения данных в памяти
        public static ObservableCollection<Order_Product> OrderProducts { get; set; } = new ObservableCollection<Order_Product>();
        public static ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();
        public static ObservableCollection<Order> Orders { get; set; } = new ObservableCollection<Order>();

        static DataManager()
        {
            InitializeTestData();
        }

        private static void InitializeTestData()
        {
            // Очищаем коллекции
            OrderProducts.Clear();
            Products.Clear();
            Orders.Clear();

            // =========== ТЕСТОВЫЕ ДАННЫЕ ===========

            // Вино
            Products.Add(new Product { IdProduct = 1, IdCategory = 1, Name = "Syrah Toscana IGT", Price = 4869, ImagePath = "/materials/Rectangle 12.png" });
            Products.Add(new Product { IdProduct = 2, IdCategory = 1, Name = "Author's Collection", Price = 1101, ImagePath = "/materials/Rectangle 1.png" });
            Products.Add(new Product { IdProduct = 3, IdCategory = 1, Name = "Insight Single Vineyard", Price = 2900, ImagePath = "/materials/Rectangle 1 (1).png" });

            // Шампанское
            Products.Add(new Product { IdProduct = 4, IdCategory = 2, Name = "Cristal Brut Champagne", Price = 81630, ImagePath = "/materials/Rectangle 2.png" });
            Products.Add(new Product { IdProduct = 5, IdCategory = 2, Name = "Morize Brut Tradition", Price = 6480, ImagePath = "/materials/Rectangle 2 (3).png" });
            Products.Add(new Product { IdProduct = 6, IdCategory = 2, Name = "Ultradition Brut", Price = 12747, ImagePath = "/materials/Rectangle 2 (2).png" });

            // Водка
            Products.Add(new Product { IdProduct = 7, IdCategory = 3, Name = "7 Zlakov", Price = 522, ImagePath = "/materials/Vodka_7Zlakov.png" });
            Products.Add(new Product { IdProduct = 8, IdCategory = 3, Name = "Tchaikovsky", Price = 700, ImagePath = "/materials/Rectangle 3.png" });
            Products.Add(new Product { IdProduct = 9, IdCategory = 3, Name = "Tchaikovsky Symphony", Price = 1552, ImagePath = "/materials/Rectangle 3 (2).png" });

            // Пиво
            Products.Add(new Product { IdProduct = 10, IdCategory = 4, Name = "Iryston Export Lager", Price = 888, ImagePath = "/materials/Rectangle 4.png" });
            Products.Add(new Product { IdProduct = 11, IdCategory = 4, Name = "Strakovice Svetle", Price = 840, ImagePath = "/materials/Rectangle 4 (2).png" });
            Products.Add(new Product { IdProduct = 12, IdCategory = 4, Name = "Double Tree Grapefruit", Price = 830, ImagePath = "/materials/Rectangle 4 (3).png" });

            // Тестовые заказы
            Orders.Add(new Order { IdOrder = 1, IdUserClient = 1, Check = 0, Delivery = false, IsCompleted = false, OrderDate = DateTime.Now });

            // Тестовые товары в корзине
            OrderProducts.Add(new Order_Product { IdOrderProduct = 1, IdOrder = 1, IdProduct = 1, Amount = 2, IsFavourited = false });
            OrderProducts.Add(new Order_Product { IdOrderProduct = 2, IdOrder = 1, IdProduct = 4, Amount = 1, IsFavourited = true });
        }

        // Методы для работы с корзиной
        public static void AddToCart(int productId, int amount = 1)
        {
            var existingItem = OrderProducts.FirstOrDefault(op => op.IdProduct == productId);
            if (existingItem != null)
            {
                existingItem.Amount += amount;
            }
            else
            {
                var newId = OrderProducts.Count > 0 ? OrderProducts.Max(op => op.IdOrderProduct) + 1 : 1;
                OrderProducts.Add(new Order_Product
                {
                    IdOrderProduct = newId,
                    IdOrder = 1,
                    IdProduct = productId,
                    Amount = amount,
                    IsFavourited = false
                });
            }
        }

        public static void RemoveFromCart(int orderProductId)
        {
            var item = OrderProducts.FirstOrDefault(op => op.IdOrderProduct == orderProductId);
            if (item != null)
            {
                OrderProducts.Remove(item);
            }
        }

        public static void UpdateAmount(int orderProductId, int newAmount)
        {
            var item = OrderProducts.FirstOrDefault(op => op.IdOrderProduct == orderProductId);
            if (item != null && newAmount > 0)
            {
                item.Amount = newAmount;
            }
        }

        public static void ClearCart()
        {
            OrderProducts.Clear();
        }

        public static decimal CalculateTotal()
        {
            decimal total = 0;
            foreach (var orderProduct in OrderProducts)
            {
                var product = GetProductById(orderProduct.IdProduct);
                if (product != null)
                {
                    total += orderProduct.Amount * product.Price;
                }
            }
            return total;
        }

        public static Product GetProductById(int productId)
        {
            return Products.FirstOrDefault(p => p.IdProduct == productId);
        }

        public static Order_Product GetOrderProductById(int orderProductId)
        {
            return OrderProducts.FirstOrDefault(op => op.IdOrderProduct == orderProductId);
        }
    }
}
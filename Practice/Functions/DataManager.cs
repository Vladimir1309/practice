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

        static DataManager()
        {
            InitializeTestData();
        }

        private static void InitializeTestData()
        {
            // Очищаем коллекции
            OrderProducts.Clear();
            Products.Clear();

            // =========== ВИНО (ID 1-5) ===========
            // Вино с Main1.xaml
            Products.Add(new Product
            {
                IdProduct = 1,
                IdCategory = 1,
                Name = "Syrah Toscana IGT Tenuta Mordini, 0.75 л",
                Price = 4869,
                ImagePath = "/materials/Rectangle 12.png"
            });

            Products.Add(new Product
            {
                IdProduct = 2,
                IdCategory = 1,
                Name = "7 Zlakov, 0.5 л",
                Price = 522,
                ImagePath = "/materials/Vodka_7Zlakov.png"
            });

            Products.Add(new Product
            {
                IdProduct = 3,
                IdCategory = 1,
                Name = "Vodka Polugar, 0.7 л",
                Price = 1793,
                ImagePath = "/materials/Vodka_Polugar.png"
            });

            // Вино с Wine.xaml
            Products.Add(new Product
            {
                IdProduct = 4,
                IdCategory = 1,
                Name = "Syrah Toscana IGT Tenuta Mordini, 0.75 л",
                Price = 4869,
                ImagePath = "/materials/Rectangle 12.png"
            });

            Products.Add(new Product
            {
                IdProduct = 5,
                IdCategory = 1,
                Name = "Author's Collection Kindzmarauli Askaneli, 0.75 л",
                Price = 1101,
                ImagePath = "/materials/Rectangle 1.png"
            });

            Products.Add(new Product
            {
                IdProduct = 6,
                IdCategory = 1,
                Name = "Insight Single Vineyard Sauvignon Blanc, 0.75 л",
                Price = 2900, // Примерная цена
                ImagePath = "/materials/Rectangle 1 (1).png"
            });

            // =========== ШАМПАНСКОЕ (ID 7-9) ===========
            Products.Add(new Product
            {
                IdProduct = 7,
                IdCategory = 2,
                Name = "Cristal Brut Champagne AOC Louis Roederer (gift box) 0.75 л",
                Price = 81630,
                ImagePath = "/materials/Rectangle 2.png"
            });

            Products.Add(new Product
            {
                IdProduct = 8,
                IdCategory = 2,
                Name = "Morize Brut Tradition Champagne AOC 0.75 л",
                Price = 6480,
                ImagePath = "/materials/Rectangle 2 (3).png"
            });

            Products.Add(new Product
            {
                IdProduct = 9,
                IdCategory = 2,
                Name = "Ultradition Brut Champagne AOС Laherte Freres 0.75 л",
                Price = 12747,
                ImagePath = "/materials/Rectangle 2 (2).png"
            });

            // =========== ВОДКА (ID 10-12) ===========
            Products.Add(new Product
            {
                IdProduct = 10,
                IdCategory = 3,
                Name = "7 Zlakov 0.5 л",
                Price = 522,
                ImagePath = "/materials/Vodka_7Zlakov.png"
            });

            Products.Add(new Product
            {
                IdProduct = 11,
                IdCategory = 3,
                Name = "Tchaikovsky - 0.5 л",
                Price = 700,
                ImagePath = "/materials/Rectangle 3.png"
            });

            Products.Add(new Product
            {
                IdProduct = 12,
                IdCategory = 3,
                Name = "Tchaikovsky Symphony 0.7 л",
                Price = 1552,
                ImagePath = "/materials/Rectangle 3 (2).png"
            });

            // =========== ПИВО (ID 13-15) ===========
            Products.Add(new Product
            {
                IdProduct = 13,
                IdCategory = 4,
                Name = "Iryston Export Lager Classic 0.45 л",
                Price = 888,
                ImagePath = "/materials/Rectangle 4.png"
            });

            Products.Add(new Product
            {
                IdProduct = 14,
                IdCategory = 4,
                Name = "Strakovice Svetle 0.45 л",
                Price = 840,
                ImagePath = "/materials/Rectangle 4 (2).png"
            });

            Products.Add(new Product
            {
                IdProduct = 15,
                IdCategory = 4,
                Name = "Double Tree Grapefruit & Mango 0.5 л",
                Price = 830,
                ImagePath = "/materials/Rectangle 4 (3).png"
            });

            // Добавляем начальные тестовые товары в корзину (пример)
            OrderProducts.Add(new Order_Product
            {
                IdOrderProduct = 1,
                IdOrder = 1,
                IdProduct = 1,
                Amount = 2,
                IsFavourited = false
            });

            OrderProducts.Add(new Order_Product
            {
                IdOrderProduct = 2,
                IdOrder = 1,
                IdProduct = 5,
                Amount = 1,
                IsFavourited = true
            });
        }

        // Методы для работы с корзиной
        public static void AddToCart(int productId, int amount = 1)
        {
            // Проверяем, существует ли товар
            var product = GetProductById(productId);
            if (product == null)
            {
                throw new ArgumentException($"Товар с ID {productId} не найден");
            }

            // Проверяем, есть ли уже такой товар в корзине
            var existingItem = OrderProducts.FirstOrDefault(op => op.IdProduct == productId);
            if (existingItem != null)
            {
                // Увеличиваем количество
                existingItem.Amount += amount;
            }
            else
            {
                // Добавляем новый товар
                var newId = OrderProducts.Count > 0 ? OrderProducts.Max(op => op.IdOrderProduct) + 1 : 1;
                OrderProducts.Add(new Order_Product
                {
                    IdOrderProduct = newId,
                    IdOrder = 1, // текущий заказ
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

        public static Product GetProductById(int productId)
        {
            return Products.FirstOrDefault(p => p.IdProduct == productId);
        }
    }
}
// BasketService.cs
using Practice.Models;
using System.Collections.Generic;
using System.Linq;

namespace Practice.Services
{
    public static class BasketService
    {
        public static List<Order_Product> GetCartItems()
        {
            // Получаем товары из корзины через DbService
            var cartItems = DbService.GetUserCart(AuthManager.CurrentUserId);

            // Возвращаем как есть, или можно преобразовать
            return cartItems;
        }

        // Можно добавить методы для работы с локальной корзиной
        public static void AddToLocalCart(int productId, int amount)
        {
            // Логика для неавторизованных пользователей
            // Например, хранение в LocalStorage или в памяти
        }

        public static List<Order_Product> GetLocalCartItems()
        {
            // Получение локальной корзины
            return new List<Order_Product>();
        }
    }
}
using System.Windows;

namespace Practice
{
    public static class DataManager
    {
        public static void AddToCart(int productId, int amount)
        {
            if (AuthManager.IsAuthenticated)
            {
                if (DbService.AddToCart(AuthManager.CurrentUserId, productId, amount))
                {
                    MessageBox.Show("Товар добавлен в корзину!");
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении в корзину!");
                }
            }
            else
            {
                MessageBox.Show("Авторизуйтесь для добавления в корзину!", "Внимание",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
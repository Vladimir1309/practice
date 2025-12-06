using Practice.Models;
using Practice.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq; // ДОБАВЬТЕ ЭТУ СТРОЧКУ
using System.Windows;
using System.Windows.Input;

namespace Practice
{
    public partial class Favourite : Window
    {
        public ObservableCollection<Order_Product> FavouriteItems { get; set; }

        public Favourite()
        {
            InitializeComponent();

            FavouriteItems = new ObservableCollection<Order_Product>();

            LoadFavourites();
            DataContext = this;
        }

        private void LoadFavourites()
        {
            FavouriteItems.Clear();

            if (AuthManager.IsAuthenticated)
            {
                // Используем статический вызов
                var favourites = DbService.GetFavourites(AuthManager.CurrentUserId);
                foreach (var item in favourites)
                {
                    FavouriteItems.Add(item);
                }
            }

            UpdateFavouriteTitle();
        }

        private void UpdateFavouriteTitle()
        {
            int itemCount = FavouriteItems.Count;
            // Исправьте: должно быть favouriteTitleLabel.Content, а не Title
            if (favouriteTitleLabel != null)
            {
                favouriteTitleLabel.Content = $"Избранное ({itemCount} товаров)";
            }

            // Показываем/скрываем сообщение если пусто
            if (emptyMessageLabel != null)
            {
                emptyMessageLabel.Visibility = itemCount == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        // === ДОБАВЛЕНО: Методы для изменения количества ===
        private void DecreaseAmount_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Order_Product item)
            {
                if (item.Amount > 1)
                {
                    item.Amount--;

                    // Обновляем количество в базе данных
                    if (DbService.UpdateCartItemAmount(item.IdOrderProduct, item.Amount))
                    {
                        // Обновляем отображение
                        FavouriteItemsControl.Items.Refresh();
                    }
                }
            }
        }

        private void IncreaseAmount_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Order_Product item)
            {
                item.Amount++;

                // Обновляем количество в базе данных
                if (DbService.UpdateCartItemAmount(item.IdOrderProduct, item.Amount))
                {
                    // Обновляем отображение
                    FavouriteItemsControl.Items.Refresh();
                }
            }
        }
        // === КОНЕЦ ДОБАВЛЕННЫХ МЕТОДОВ ===

        private void MLBD_GWAN(object sender, RoutedEventArgs e)
        {
            Main1 main1 = new Main1();
            main1.Show();
            this.Close();
        }

        private void MLBD_Basket(object sender, RoutedEventArgs e)
        {
            Basket basket = new Basket();
            basket.Show();
            this.Close();
        }

        private void MLBD_Favourite(object sender, RoutedEventArgs e)
        {
            // Уже на странице избранного
        }

        private void MLBD_Account(object sender, RoutedEventArgs e)
        {
            if (AuthManager.IsAuthenticated)
            {
                Account account = new Account();
                account.Show();
            }
            else
            {
                Login login = new Login();
                login.Show();
            }
            this.Close();
        }

        // Метод для удаления из избранного
        private void RemoveFromFavourite_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Order_Product item)
            {
                var result = MessageBox.Show("Удалить товар из избранного?",
                                            "Подтверждение",
                                            MessageBoxButton.YesNo,
                                            MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Проверяем, есть ли товар в корзине (Amount > 0)
                    if (item.Amount > 0)
                    {
                        // Если есть в корзине, спрашиваем что делать
                        var choiceResult = MessageBox.Show(
                            "Этот товар также находится в корзине. Что вы хотите сделать?\n\n" +
                            "Да - удалить только из избранного\n" +
                            "Нет - удалить и из корзины, и из избранного\n" +
                            "Отмена - ничего не делать",
                            "Товар в корзине",
                            MessageBoxButton.YesNoCancel,
                            MessageBoxImage.Question);

                        if (choiceResult == MessageBoxResult.Yes)
                        {
                            // Удаляем только из избранного
                            if (DbService.RemoveFromFavouriteOnly(item.IdOrderProduct))
                            {
                                LoadFavourites();
                                MessageBox.Show("Товар удален из избранного (остался в корзине)");
                            }
                        }
                        else if (choiceResult == MessageBoxResult.No)
                        {
                            // Удаляем и из избранного, и из корзины
                            if (DbService.RemoveFromCartOnly(item.IdOrderProduct))
                            {
                                LoadFavourites();
                                MessageBox.Show("Товар удален из избранного и корзины");
                            }
                        }
                        // Если Cancel - ничего не делаем
                    }
                    else
                    {
                        // Если товара нет в корзине (Amount = 0), просто удаляем из избранного
                        if (DbService.RemoveFromFavouriteOnly(item.IdOrderProduct))
                        {
                            LoadFavourites();
                            MessageBox.Show("Товар удален из избранного");
                        }
                    }
                }
            }
        }

        // Метод для добавления товара из избранного в корзину
        // Метод для добавления товара из избранного в корзину
        private void AddToCartFromFavourite_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Order_Product item)
            {
                if (AuthManager.IsAuthenticated)
                {
                    try
                    {
                        // Используем специальный метод для добавления в корзину из избранного
                        bool success = DbService.AddToCartFromFavourite(AuthManager.CurrentUserId, item.IdOrderProduct);

                        if (success)
                        {
                            MessageBox.Show($"Товар \"{item.Product.Name}\" добавлен в корзину!",
                                          "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                            // Обновляем список избранного
                            LoadFavourites();
                        }
                        else
                        {
                            MessageBox.Show("Ошибка при добавлении в корзину");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("Авторизуйтесь для добавления в корзину!",
                                  "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
    }
}
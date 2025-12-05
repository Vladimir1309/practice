using Practice.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
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
            Title = $"Избранное ({itemCount} товаров)";

            // Если есть Label с названием на странице, обновите его:
            // favouriteTitleLabel.Content = $"Избранное ({itemCount} товаров)";
        }

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
                // Используем статический вызов
                DbService.ToggleFavourite(item.IdOrderProduct);
                LoadFavourites();
                MessageBox.Show("Товар удален из избранного");
            }
        }

        private void AddToCartFromFavourite_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Order_Product item)
            {
                // Используем статический вызов
                if (AuthManager.IsAuthenticated)
                {
                    DbService.AddToCart(AuthManager.CurrentUserId, item.IdProduct, item.Amount);
                    MessageBox.Show("Товар добавлен в корзину");
                }
            }
        }
    }
}
using Practice.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Linq; // ДОБАВЬТЕ ЭТУ СТРОЧКУ!

namespace Practice
{
    public partial class Main1 : Window
    {
        public Main1()
        {
            InitializeComponent();
            InitializeFavouriteIcons();
        }

        // Метод для переключения избранного
        // Обновленный метод для переключения избранного
        // В методе ToggleFavourite_Click в Main1.xaml.cs:
        private void ToggleFavourite_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag != null)
                {
                    int productId = Convert.ToInt32(button.Tag.ToString());

                    if (!AuthManager.IsAuthenticated)
                    {
                        MessageBox.Show("Авторизуйтесь для добавления в избранное", "Внимание",
                                      MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Проверяем, есть ли товар уже в избранном
                    bool isCurrentlyFavourite = CheckIfProductIsFavourite(productId);

                    if (isCurrentlyFavourite)
                    {
                        // Находим запись в избранном
                        var favourites = DbService.GetFavourites(AuthManager.CurrentUserId);
                        var favouriteItem = favourites.FirstOrDefault(f => f.IdProduct == productId);

                        if (favouriteItem != null)
                        {
                            // Удаляем из избранного
                            if (DbService.RemoveFromFavouriteOnly(favouriteItem.IdOrderProduct))
                            {
                                // Меняем иконку на пустое сердечко
                                UpdateFavouriteIcon(productId, false);
                                MessageBox.Show("Товар удален из избранного");
                            }
                        }
                    }
                    else
                    {
                        // Добавляем в избранное (БЕЗ добавления в корзину)
                        if (DbService.AddToFavouriteOnly(AuthManager.CurrentUserId, productId)) // ИСПРАВЛЕНО
                        {
                            // Меняем иконку на заполненное сердечко
                            UpdateFavouriteIcon(productId, true);
                            MessageBox.Show("Товар добавлен в избранное!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        // Проверяем, есть ли товар в избранном
        private bool CheckIfProductIsFavourite(int productId)
        {
            if (!AuthManager.IsAuthenticated) return false;

            try
            {
                var favourites = DbService.GetFavourites(AuthManager.CurrentUserId);
                return favourites.Any(f => f.IdProduct == productId);
            }
            catch
            {
                return false;
            }
        }

        // В конструкторе Main1 или отдельном методе инициализации:
        // В методе InitializeFavouriteIcons:
        private void InitializeFavouriteIcons()
        {
            if (!AuthManager.IsAuthenticated) return;

            try
            {
                // Получаем список избранных товаров
                var favourites = DbService.GetFavourites(AuthManager.CurrentUserId);

                // Проверяем каждый товар и обновляем иконки
                foreach (var favourite in favourites)
                {
                    UpdateFavouriteIcon(favourite.IdProduct, true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки избранного: {ex.Message}");
            }
        }

        // Обновить иконку для конкретного товара
        private void UpdateFavouriteIcon(int productId, bool isFavourite)
        {
            // Находим кнопку по ID товара (Tag)
            Button favouriteButton = null;
            string iconName = "";

            switch (productId)
            {
                case 1: // Syrah Toscana
                    favouriteButton = FindName("favouriteButton1") as Button;
                    iconName = "favouriteIcon1";
                    break;
                case 7: // 7 Zlakov
                    favouriteButton = FindName("favouriteButton2") as Button;
                    iconName = "favouriteIcon2";
                    break;
                case 10: // Vodka Polugar
                    favouriteButton = FindName("favouriteButton3") as Button;
                    iconName = "favouriteIcon3";
                    break;
            }

            if (favouriteButton != null)
            {
                var image = favouriteButton.Content as Image;
                if (image != null)
                {
                    image.Source = new BitmapImage(
                        new Uri(isFavourite ?
                            "/materials/Favorite_filled.png" :
                            "/materials/Favorite_empty.png",
                            UriKind.Relative));
                }
            }
        }

        // Добавить товар в избранное
        // В Main1.xaml.cs, Vodka.xaml.cs и т.д.
        private void AddToFavourite1_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthManager.IsAuthenticated)
            {
                MessageBox.Show("Авторизуйтесь для добавления в избранное", "Внимание",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Используем productId (например, для товара 1 это ID=1)
            if (DbService.AddToFavouriteOnly(AuthManager.CurrentUserId, 1)) // Замените 1 на нужный ID
            {
                MessageBox.Show("Товар добавлен в избранное!");
                // Обновить иконку, если нужно
            }
        }

        // В Main1.xaml.cs, Vodka.xaml.cs и т.д.
        private void AddToFavourite2_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthManager.IsAuthenticated)
            {
                MessageBox.Show("Авторизуйтесь для добавления в избранное", "Внимание",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Используем productId (например, для товара 1 это ID=1)
            if (DbService.AddToFavouriteOnly(AuthManager.CurrentUserId, 7)) // Замените 1 на нужный ID
            {
                MessageBox.Show("Товар добавлен в избранное!");
                // Обновить иконку, если нужно
            }
        }

        // В Main1.xaml.cs, Vodka.xaml.cs и т.д.
        private void AddToFavourite3_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthManager.IsAuthenticated)
            {
                MessageBox.Show("Авторизуйтесь для добавления в избранное", "Внимание",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Используем productId (например, для товара 1 это ID=1)
            if (DbService.AddToFavouriteOnly(AuthManager.CurrentUserId, 10)) // Замените 1 на нужный ID
            {
                MessageBox.Show("Товар добавлен в избранное!");
                // Обновить иконку, если нужно
            }
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
            Favourite favourite = new Favourite();
            favourite.Show();
            this.Close();
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

        int i = 1;
        private void minus1(object sender, RoutedEventArgs e)
        {
            if (i > 1) i--;
            count1.Content = i;
        }

        private void plus1(object sender, RoutedEventArgs e)
        {
            if (i > 0) i++;
            count1.Content = i;
        }

        int j = 1;
        private void minus2(object sender, RoutedEventArgs e)
        {
            if (j > 1) j--;
            count2.Content = j;
        }

        private void plus2(object sender, RoutedEventArgs e)
        {
            if (j > 0) j++;
            count2.Content = j;
        }

        int m = 1;
        private void minus3(object sender, RoutedEventArgs e)
        {
            if (m > 1) m--;
            count3.Content = m;
        }

        private void plus3(object sender, RoutedEventArgs e)
        {
            if (m > 0) m++;
            count3.Content = m;
        }

        private void MLBD_Wine(object sender, RoutedEventArgs e)
        {
            Wine wine = new Wine();
            wine.Show();
            this.Close();
        }

        private void MLBD_Champagne(object sender, RoutedEventArgs e)
        {
            Champagne champagne = new Champagne();
            champagne.Show();
            this.Close();
        }

        private void MLBD_Vodka(object sender, RoutedEventArgs e)
        {
            Vodka vodka = new Vodka();
            vodka.Show();
            this.Close();
        }

        private void MLBD_Beer(object sender, RoutedEventArgs e)
        {
            Beer beer = new Beer();
            beer.Show();
            this.Close();
        }

        private void ME_Wine(Object sender, RoutedEventArgs e)
        {
            BorderWine.Opacity = 0.5;
        }

        private void ML_Wine(Object sender, RoutedEventArgs e)
        {
            BorderWine.Opacity = 1;
        }

        private void ME_Champagne(Object sender, RoutedEventArgs e)
        {
            BorderChampagne.Opacity = 0.5;
        }

        private void ML_Champagne(Object sender, RoutedEventArgs e)
        {
            BorderChampagne.Opacity = 1;
        }

        private void ME_Vodka(Object sender, RoutedEventArgs e)
        {
            BorderVodka.Opacity = 0.5;
        }

        private void ML_Vodka(Object sender, RoutedEventArgs e)
        {
            BorderVodka.Opacity = 1;
        }

        private void ME_Beer(Object sender, RoutedEventArgs e)
        {
            BorderBeer.Opacity = 0.5;
        }

        private void ML_Beer(Object sender, RoutedEventArgs e)
        {
            BorderBeer.Opacity = 1;
        }

        private void ME_WineLabel(Object sender, RoutedEventArgs e)
        {
            BorderWine.Opacity = 0.5;
        }

        private void ML_WineLabel(Object sender, RoutedEventArgs e)
        {
            BorderWine.Opacity = 1;
        }

        private void ME_ChampagneLabel(Object sender, EventArgs e)
        {
            BorderChampagne.Opacity = 0.5;
        }

        private void ML_ChampagneLabel(Object sender, RoutedEventArgs e)
        {
            BorderChampagne.Opacity = 1;
        }

        private void ME_VodkaLabel(Object sender, RoutedEventArgs e)
        {
            BorderVodka.Opacity = 0.5;
        }

        private void ML_VodkaLabel(Object sender, RoutedEventArgs e)
        {
            BorderVodka.Opacity = 1;
        }

        private void ME_BeerLabel(Object sender, RoutedEventArgs e)
        {
            BorderBeer.Opacity = 0.5;
        }

        private void ML_BeerLabel(Object sender, RoutedEventArgs e)
        {
            BorderBeer.Opacity = 1;
        }

        // В Main1.xaml.cs, Vodka.xaml.cs и других:
        private void AddToBasket1_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(count1.Content?.ToString(), out int amount))
            {
                if (AuthManager.IsAuthenticated)
                {
                    // Добавляем в БД корзину
                    if (DbService.AddToCart(AuthManager.CurrentUserId, 1, amount))
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
                    // Добавляем в локальную корзину
                    LocalCartService.AddToLocalCart(1, amount);
                    MessageBox.Show("Товар добавлен в корзину!\nАвторизуйтесь для сохранения корзины.",
                                  "Корзина", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        // В Main1.xaml.cs, Vodka.xaml.cs и других:
        private void AddToBasket2_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(count1.Content?.ToString(), out int amount))
            {
                if (AuthManager.IsAuthenticated)
                {
                    // Добавляем в БД корзину
                    if (DbService.AddToCart(AuthManager.CurrentUserId, 7, amount))
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
                    // Добавляем в локальную корзину
                    LocalCartService.AddToLocalCart(7, amount);
                    MessageBox.Show("Товар добавлен в корзину!\nАвторизуйтесь для сохранения корзины.",
                                  "Корзина", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        // В Main1.xaml.cs, Vodka.xaml.cs и других: 
        private void AddToBasket3_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(count1.Content?.ToString(), out int amount))
            {
                if (AuthManager.IsAuthenticated)
                {
                    // Добавляем в БД корзину
                    if (DbService.AddToCart(AuthManager.CurrentUserId, 10, amount))
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
                    // Добавляем в локальную корзину
                    LocalCartService.AddToLocalCart(10, amount);
                    MessageBox.Show("Товар добавлен в корзину!\nАвторизуйтесь для сохранения корзины.",
                                  "Корзина", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}
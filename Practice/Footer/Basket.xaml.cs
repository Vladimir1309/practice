using Practice.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Practice
{
    public partial class Basket : Window
    {
        public ObservableCollection<Order_ProductViewModel> BasketItems { get; set; }

        public class Order_ProductViewModel
        {
            public int IdOrderProduct { get; set; }
            public int IdOrder { get; set; }
            public int IdProduct { get; set; }
            public int Amount { get; set; }
            public bool IsFavourited { get; set; }
            public string ProductName { get; set; }
            public decimal ProductPrice { get; set; }
            public string ProductImagePath { get; set; }
            public decimal TotalPrice => Amount * ProductPrice;
        }

        public Basket()
        {
            InitializeComponent();

            BasketItems = new ObservableCollection<Order_ProductViewModel>();

            LoadBasketData();
            DataContext = this;
        }

        private void LoadBasketData()
        {
            BasketItems.Clear();

            if (AuthManager.IsAuthenticated)
            {
                // TODO: Реализовать метод GetUserCart через ADO.NET
                // Временно используем тестовые данные
                BasketItems.Add(new Order_ProductViewModel
                {
                    IdOrderProduct = 1,
                    IdOrder = 1,
                    IdProduct = 1,
                    Amount = 2,
                    IsFavourited = false,
                    ProductName = "Вино Syrah Toscana",
                    ProductPrice = 4869,
                    ProductImagePath = "/materials/Rectangle 12.png"
                });

                BasketItems.Add(new Order_ProductViewModel
                {
                    IdOrderProduct = 2,
                    IdOrder = 1,
                    IdProduct = 2,
                    Amount = 1,
                    IsFavourited = true,
                    ProductName = "Водка 7 Zlakov",
                    ProductPrice = 522,
                    ProductImagePath = "/materials/Vodka_7Zlakov.png"
                });
            }

            UpdateTotalPrice();
            UpdateBasketTitle();
        }

        private void UpdateTotalPrice()
        {
            if (totalLabel != null)
            {
                decimal total = BasketItems.Sum(item => item.TotalPrice);
                totalLabel.Content = $"{total} ₽";
            }
        }

        private void UpdateBasketTitle()
        {
            int itemCount = BasketItems.Sum(item => item.Amount);
            basketTitleLabel.Content = $"Корзина ({itemCount} товар(а/ов))";
        }

        private void DeleteItem(object parameter)
        {
            if (parameter is Order_ProductViewModel item)
            {
                // TODO: Реализовать удаление через ADO.NET
                BasketItems.Remove(item);
                UpdateTotalPrice();
                UpdateBasketTitle();
                MessageBox.Show("Товар удален из корзины");
            }
        }

        private void Pay_Click(object sender, RoutedEventArgs e)
        {
            if (!BasketItems.Any())
            {
                MessageBox.Show("Корзина пуста");
                return;
            }

            // TODO: Реализовать оплату через БД
            MessageBox.Show("Заказ успешно оплачен!");

            // Очищаем корзину
            BasketItems.Clear();
            UpdateTotalPrice();
            UpdateBasketTitle();
        }

        // Обработчики изменения количества
        private void DecreaseAmount_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag != null && int.TryParse(button.Tag.ToString(), out int orderProductId))
            {
                var item = BasketItems.FirstOrDefault(op => op.IdOrderProduct == orderProductId);
                if (item != null && item.Amount > 1)
                {
                    item.Amount--;
                    UpdateBasketTitle();
                    BasketItemsControl.Items.Refresh();
                    // Используем статический вызов
                    DbService.UpdateCartItemAmount(orderProductId, item.Amount);
                    UpdateTotalPrice();
                }
            }
        }

        private void IncreaseAmount_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag != null && int.TryParse(button.Tag.ToString(), out int orderProductId))
            {
                var item = BasketItems.FirstOrDefault(op => op.IdOrderProduct == orderProductId);
                if (item != null)
                {
                    item.Amount++;
                    UpdateBasketTitle();
                    BasketItemsControl.Items.Refresh();
                    // Используем статический вызов
                    DbService.UpdateCartItemAmount(orderProductId, item.Amount);
                    UpdateTotalPrice();
                }
            }
        }

        // Остальные методы навигации
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

        private void Delivery_Click(object sender, RoutedEventArgs e)
        {
            Store_Button.Background = Brushes.DarkGray;
            Delivery_Button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB70000"));
            Address.Content = "Адрес доставки";
        }

        private void Store_Click(object sender, RoutedEventArgs e)
        {
            Delivery_Button.Background = Brushes.DarkGray;
            Store_Button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB70000"));
            Address.Content = "Адрес магазина";
        }
    }
}
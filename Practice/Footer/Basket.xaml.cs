using Practice.Models;
using Practice.Services;
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
        public ObservableCollection<Order_Product> BasketItems { get; set; }
        public ICommand DeleteItemCommand { get; private set; }
        private int currentUserId;
        private bool needDelivery = false;
        private string userAddress = "";
        private string storeAddress = "Улица Пушкина, дом Колотушкина, 42";

        public Basket()
        {
            InitializeComponent();

            // Получаем ID текущего пользователя и его адрес
            currentUserId = AuthManager.CurrentUser?.IdUser ?? 0;

            // Получаем адрес пользователя или используем заглушку
            if (AuthManager.IsAuthenticated && AuthManager.CurrentUser != null)
            {
                userAddress = AuthManager.CurrentUser.Address ?? "Адрес не указан";
            }
            else
            {
                userAddress = "Авторизуйтесь, чтобы указать адрес доставки";
            }

            BasketItems = new ObservableCollection<Order_Product>();
            DeleteItemCommand = new RelayCommand(DeleteItem);

            DataContext = this;
            LoadBasketData();

            UpdateAddressDisplay();
        }

        private void LoadBasketData()
        {
            try
            {
                BasketItems.Clear();

                var cartItems = DbService.GetUserCart(currentUserId);

                foreach (var item in cartItems)
                {
                    BasketItems.Add(item);
                }

                UpdateTotalPrice();
                UpdateBasketTitle();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки корзины: {ex.Message}");
            }
        }

        private void UpdateTotalPrice()
        {
            if (totalLabel != null)
            {
                decimal total = BasketItems.Sum(item => item.Amount * item.Product.Price);
                totalLabel.Content = $"{total} ₽";
            }
        }

        private void UpdateBasketTitle()
        {
            int itemCount = BasketItems.Sum(item => item.Amount);
            basketTitleLabel.Content = $"Корзина ({itemCount} товар(а/ов))";
        }

        private void UpdateAddressDisplay()
        {
            if (needDelivery)
            {
                Address.Content = userAddress;
                Store_Button.Background = Brushes.DarkGray;
                Delivery_Button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB70000"));
            }
            else
            {
                Address.Content = storeAddress;
                Delivery_Button.Background = Brushes.DarkGray;
                Store_Button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB70000"));
            }
        }

        private void DeleteItem(object parameter)
        {
            if (parameter is Order_Product item)
            {
                try
                {
                    // Используем новый метод, который не удаляет из избранного
                    if (DbService.RemoveFromCartOnly(item.IdOrderProduct))
                    {
                        BasketItems.Remove(item);

                        if (item.IdOrder > 0)
                        {
                            DbService.UpdateOrderTotal(item.IdOrder);
                        }

                        UpdateTotalPrice();
                        UpdateBasketTitle();
                        MessageBox.Show("Товар удален из корзины");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}");
                }
            }
        }

        private void UpdateDeliveryInOrder()
        {
            if (BasketItems.Any())
            {
                int orderId = BasketItems.First().IdOrder;
                if (orderId > 0)
                {
                    DbService.UpdateOrderDelivery(orderId, needDelivery);
                }
            }
        }

        private void Pay_Click(object sender, RoutedEventArgs e)
        {
            if (!BasketItems.Any())
            {
                MessageBox.Show("Корзина пуста");
                return;
            }

            int orderId = BasketItems.First().IdOrder;

            if (orderId == 0)
            {
                MessageBox.Show("Ошибка: не найден ID заказа");
                return;
            }

            decimal currentTotal = BasketItems.Sum(item => item.Amount * item.Product.Price);
            string deliveryType = needDelivery ? "доставка курьером" : "самовывоз из магазина";
            string address = needDelivery ? userAddress : storeAddress;

            var result = MessageBox.Show(
                $"Оплатить заказ на сумму {currentTotal}₽?\n" +
                $"Способ получения: {deliveryType}\n" +
                $"Адрес: {address}",
                "Подтверждение оплаты",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    DbService.UpdateOrderDelivery(orderId, needDelivery);

                    var paymentResult = DbService.CompleteOrderWithPayment(orderId, needDelivery);

                    if (paymentResult.success)
                    {
                        string message = $"Заказ №{orderId} успешно оплачен!\n" +
                                       $"Сумма: {paymentResult.totalSum}₽\n" +
                                       $"Способ получения: {deliveryType}";

                        if (needDelivery)
                        {
                            message += $"\nАдрес доставки: {userAddress}";
                        }

                        MessageBox.Show(message);

                        BasketItems.Clear();
                        UpdateTotalPrice();
                        UpdateBasketTitle();

                        if (needDelivery)
                        {
                            DbService.CreateDeliveryRecord(orderId, currentUserId, userAddress);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при оформлении заказа");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка оплаты: {ex.Message}");
                }
            }
        }

        // Обработчики изменения количества
        private void DecreaseAmount_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag != null &&
                int.TryParse(button.Tag.ToString(), out int orderProductId))
            {
                var item = BasketItems.FirstOrDefault(op => op.IdOrderProduct == orderProductId);
                if (item != null && item.Amount > 1)
                {
                    item.Amount--;

                    if (DbService.UpdateCartItemAmount(orderProductId, item.Amount))
                    {
                        DbService.UpdateOrderTotal(item.IdOrder);
                        UpdateTotalPrice();
                        UpdateBasketTitle();
                        BasketItemsControl.Items.Refresh();
                    }
                }
            }
        }

        private void IncreaseAmount_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag != null &&
                int.TryParse(button.Tag.ToString(), out int orderProductId))
            {
                var item = BasketItems.FirstOrDefault(op => op.IdOrderProduct == orderProductId);
                if (item != null)
                {
                    item.Amount++;

                    if (DbService.UpdateCartItemAmount(orderProductId, item.Amount))
                    {
                        DbService.UpdateOrderTotal(item.IdOrder);
                        UpdateTotalPrice();
                        UpdateBasketTitle();
                        BasketItemsControl.Items.Refresh();
                    }
                }
            }
        }

        // Навигация
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
            needDelivery = true;
            UpdateAddressDisplay();
            UpdateDeliveryInOrder();
        }

        private void Store_Click(object sender, RoutedEventArgs e)
        {
            needDelivery = false;
            UpdateAddressDisplay();
            UpdateDeliveryInOrder();
        }
    }

    // Класс RelayCommand для команд
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
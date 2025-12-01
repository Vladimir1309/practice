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
        public ICommand DeleteItemCommand { get; private set; }

        public Basket()
        {
            InitializeComponent();

            // Инициализируем команду
            DeleteItemCommand = new RelayCommand(DeleteItem);

            // Инициализируем коллекцию
            BasketItems = new ObservableCollection<Order_ProductViewModel>();

            // Загружаем данные
            LoadBasketData();

            // Обновляем заголовок с количеством товаров
            UpdateBasketTitle();

            // Устанавливаем контекст данных
            DataContext = this;
        }

        // ViewModel для отображения товаров в корзине
        public class Order_ProductViewModel
        {
            public int IdOrderProduct { get; set; }
            public int IdOrder { get; set; }
            public int IdProduct { get; set; }
            public int Amount { get; set; }
            public bool IsFavourited { get; set; }

            // Свойства продукта для отображения
            public string ProductName { get; set; }
            public decimal ProductPrice { get; set; }
            public string ProductImagePath { get; set; }

            // Вычисляемое свойство для общей стоимости
            public decimal TotalPrice => Amount * ProductPrice;
        }

        private void LoadBasketData()
        {
            BasketItems.Clear();

            // Используем DataManager для получения данных
            foreach (var orderProduct in DataManager.OrderProducts)
            {
                var product = DataManager.GetProductById(orderProduct.IdProduct);
                if (product != null)
                {
                    BasketItems.Add(new Order_ProductViewModel
                    {
                        IdOrderProduct = orderProduct.IdOrderProduct,
                        IdOrder = orderProduct.IdOrder,
                        IdProduct = orderProduct.IdProduct,
                        Amount = orderProduct.Amount,
                        IsFavourited = orderProduct.IsFavourited,
                        ProductName = product.Name,
                        ProductPrice = product.Price,
                        ProductImagePath = product.ImagePath
                    });
                }
            }

            UpdateTotalPrice();
        }

        private void UpdateBasketTitle()
        {
            int itemCount = BasketItems.Sum(item => item.Amount);
            basketTitleLabel.Content = $"Корзина ({itemCount} товара)";
        }

        private void UpdateTotalPrice()
        {
            if (totalLabel != null)
            {
                decimal total = BasketItems.Sum(item => item.TotalPrice);
                totalLabel.Content = $"{total} ₽";
            }
            UpdateBasketTitle();
        }

        private void DeleteItem(object parameter)
        {
            if (parameter is Order_ProductViewModel item)
            {
                // Удаляем из DataManager
                DataManager.RemoveFromCart(item.IdOrderProduct);

                // Удаляем из коллекции для отображения
                BasketItems.Remove(item);

                // Обновляем общую сумму и заголовок
                UpdateTotalPrice();

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

            // Очищаем корзину в DataManager
            DataManager.ClearCart();

            // Очищаем отображаемые данные
            BasketItems.Clear();

            // Обновляем общую сумму и заголовок
            UpdateTotalPrice();

            // Показываем окно успешной оплаты
            PopUps.SucceedPay succeedPay = new PopUps.SucceedPay();
            succeedPay.ShowDialog();

            MessageBox.Show("Заказ успешно оплачен!");
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
                    DataManager.UpdateAmount(orderProductId, item.Amount);
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
                    DataManager.UpdateAmount(orderProductId, item.Amount);
                    UpdateTotalPrice();
                }
            }
        }

        // Остальные методы навигации остаются без изменений
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
            Account account = new Account();
            account.Show();
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
}
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Practice.Models;

namespace Practice.Panel.Delivery
{
    public partial class Hit : Window
    {
        private static ObservableCollection<Product> _products = new ObservableCollection<Product>();
        private Product _currentProduct;
        private int _quantity = 1;

        static Hit()
        {
            // Инициализируем товары
            _products.Add(new Product { IdProduct = 1, IdCategory = 1, Name = "Ноутбук HP Pavilion", Price = 54999 });
            _products.Add(new Product { IdProduct = 2, IdCategory = 1, Name = "Смартфон Samsung Galaxy S21", Price = 67999 });
            _products.Add(new Product { IdProduct = 3, IdCategory = 2, Name = "Наушники Sony WH-1000XM4", Price = 22999 });
            _products.Add(new Product { IdProduct = 4, IdCategory = 3, Name = "Кофеварка De'Longhi", Price = 15999 });
            _products.Add(new Product { IdProduct = 5, IdCategory = 4, Name = "Книга 'Война и мир'", Price = 899 });
            _products.Add(new Product { IdProduct = 6, IdCategory = 1, Name = "Планшет Apple iPad Air", Price = 72999 });
            _products.Add(new Product { IdProduct = 7, IdCategory = 2, Name = "Клавиатура Logitech MX Keys", Price = 10999 });
        }

        public Hit()
        {
            InitializeComponent();
            Amount.Text = "1";
        }

        private void FindProductButton_Click(object sender, RoutedEventArgs e)
        {
            LoadProductInfo();
        }

        private void LoadProductInfo()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(IdProductText.Text))
                {
                    MessageBox.Show("Введите ID товара!");
                    return;
                }

                if (!int.TryParse(IdProductText.Text, out int productId))
                {
                    MessageBox.Show("ID должен быть числом!");
                    return;
                }

                _currentProduct = _products.FirstOrDefault(p => p.IdProduct == productId);

                if (_currentProduct != null)
                {
                    ProductNameLabel.Content = _currentProduct.Name;
                    ProductPriceLabel.Content = $"{_currentProduct.Price:N0} ₽";
                    ProductNameLabel.Foreground = System.Windows.Media.Brushes.Black;
                    ProductPriceLabel.Foreground = System.Windows.Media.Brushes.Black;

                    CalculateOrderTotal(); // Рассчитываем сумму
                    Amount.Focus();
                    Amount.SelectAll();
                }
                else
                {
                    MessageBox.Show($"Товар с ID {productId} не найден!");
                    ClearProductInfo();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        // Безопасный метод расчета суммы
        private void CalculateOrderTotal()
        {
            try
            {
                if (_currentProduct != null && int.TryParse(Amount.Text, out int quantity) && quantity > 0)
                {
                    _quantity = quantity;
                    decimal total = _currentProduct.Price * quantity;

                    // Безопасное обновление Label
                    if (OrderTotalLabel != null)
                        OrderTotalLabel.Content = $"{total:N0} ₽";
                }
                else
                {
                    if (OrderTotalLabel != null)
                        OrderTotalLabel.Content = "0 ₽";
                }
            }
            catch
            {
                if (OrderTotalLabel != null)
                    OrderTotalLabel.Content = "0 ₽";
            }
        }

        private void Amount_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CalculateOrderTotal();
        }

        private void ClearProductInfo()
        {
            _currentProduct = null;
            _quantity = 1;

            ProductNameLabel.Content = "-";
            ProductPriceLabel.Content = "0 ₽";
            ProductNameLabel.Foreground = System.Windows.Media.Brushes.Gray;
            ProductPriceLabel.Foreground = System.Windows.Media.Brushes.Gray;

            if (OrderTotalLabel != null)
                OrderTotalLabel.Content = "0 ₽";

            Amount.Text = "1";
        }

        private void GetBack(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_currentProduct == null)
                {
                    MessageBox.Show("Сначала найдите товар!");
                    return;
                }

                if (!int.TryParse(Amount.Text, out int quantity) || quantity <= 0)
                {
                    MessageBox.Show("Введите корректное количество!");
                    return;
                }

                decimal total = _currentProduct.Price * quantity;

                string message = $"Оформить заказ?\n\n" +
                               $"Товар: {_currentProduct.Name}\n" +
                               $"Количество: {quantity}\n" +
                               $"Сумма: {total:N0} ₽";

                if (MessageBox.Show(message, "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    MessageBox.Show($"Заказ на сумму {total:N0} ₽ оформлен!");
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            IdProductText.Text = "";
            ClearProductInfo();
            IdProductText.Focus();
        }

        // Навигационные методы
        private void Hitt(object sender, RoutedEventArgs e) { }
        private void Back(object sender, RoutedEventArgs e)
        {
            new Reload().Show();
            this.Close();
        }
        private void History(object sender, RoutedEventArgs e)
        {
            new Sales.HistoryOrdersSales().Show();
            this.Close();
        }
        private void MLBD_Exit(object sender, EventArgs e)
        {
            new Login().Show();
            this.Close();
        }
    }
}
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Practice.Models;

namespace Practice.Panel.Delivery
{
    public partial class Reload : Window
    {
        // Локальная коллекция товаров
        private static ObservableCollection<Product> _products = new ObservableCollection<Product>();
        private Product _currentProduct;

        static Reload()
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

        public Reload()
        {
            InitializeComponent();
        }

        // Простой метод поиска товара при изменении текста
        private void IdProductText_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Опционально: можно сделать автоматический поиск при вводе
            // Но лучше убрать или оставить только для демонстрации
        }

        // Кнопка "Найти товар" (добавьте в XAML)
        private void FindProductButton_Click(object sender, RoutedEventArgs e)
        {
            LoadProductInfo();
        }

        // Загрузка информации о товаре
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
                    MessageBox.Show("ID товара должен быть числом!");
                    ClearProductInfo();
                    return;
                }

                // Ищем товар
                _currentProduct = _products.FirstOrDefault(p => p.IdProduct == productId);

                if (_currentProduct != null)
                {
                    // Обновляем информацию
                    ProductNameLabel.Content = _currentProduct.Name;
                    ProductPriceLabel.Content = $"{_currentProduct.Price:N0} ₽";

                    // Меняем цвет на черный
                    ProductNameLabel.Foreground = System.Windows.Media.Brushes.Black;
                    ProductPriceLabel.Foreground = System.Windows.Media.Brushes.Black;

                    // Фокус на поле причины
                    Amount.Focus();
                }
                else
                {
                    MessageBox.Show($"Товар с ID {productId} не найден!");
                    ClearProductInfo();
                    IdProductText.Focus();
                    IdProductText.SelectAll();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
                ClearProductInfo();
            }
        }

        // Очистка информации о товаре
        private void ClearProductInfo()
        {
            _currentProduct = null;
            ProductNameLabel.Content = "-";
            ProductPriceLabel.Content = "0 ₽";
            ProductNameLabel.Foreground = System.Windows.Media.Brushes.Gray;
            ProductPriceLabel.Foreground = System.Windows.Media.Brushes.Gray;
        }

        // Кнопка "Оформить возврат"
        private void GetBack(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_currentProduct == null)
                {
                    MessageBox.Show("Сначала найдите товар по ID!");
                    IdProductText.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(Amount.Text))
                {
                    MessageBox.Show("Введите причину возврата!");
                    Amount.Focus();
                    return;
                }

                if (Amount.Text.Length < 10)
                {
                    MessageBox.Show("Причина возврата должна содержать минимум 10 символов!");
                    Amount.Focus();
                    return;
                }

                string message = $"Возврат товара оформлен!\n\n" +
                               $"ID товара: {_currentProduct.IdProduct}\n" +
                               $"Наименование: {_currentProduct.Name}\n" +
                               $"Цена: {_currentProduct.Price:N0} ₽\n" +
                               $"Причина: {Amount.Text}";

                MessageBox.Show(message, "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);

                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        // Кнопка "Отмена"
        private void Cancel(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        // Очистка формы
        private void ClearForm()
        {
            IdProductText.Text = "";
            Amount.Text = "";
            ClearProductInfo();
            IdProductText.Focus();
        }

        // Навигационные методы
        private void Hitt(object sender, RoutedEventArgs e)
        {
            new Hit().Show();
            this.Close();
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            // Уже в этом окне
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
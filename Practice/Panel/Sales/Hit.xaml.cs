using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Practice.Models;
using MySql.Data.MySqlClient;

namespace Practice.Panel.Delivery
{
    public partial class Hit : Window
    {
        private Product _currentProduct;
        private User _selectedClient;
        public ObservableCollection<User> Clients { get; set; }

        public Hit()
        {
            InitializeComponent();
            Amount.Text = "1";

            // Инициализируем коллекцию клиентов
            Clients = new ObservableCollection<User>();
            ClientComboBox.ItemsSource = Clients;

            // Подписываемся на событие загрузки
            this.Loaded += Hit_Loaded;
        }

        private void Hit_Loaded(object sender, RoutedEventArgs e)
        {
            // Все элементы теперь инициализированы
            CalculateOrderTotal();

            // Загружаем список клиентов
            LoadClients();
        }

        private void LoadClients()
        {
            try
            {
                Clients.Clear();

                // Получаем всех пользователей из БД через DbService
                var users = DbService.GetAllUsers();

                // Фильтруем только клиентов (покупателей) или всех пользователей
                foreach (var user in users)
                {
                    // Можно добавить фильтр по роли, если нужно
                    // if (user.Post?.Role?.RoleName == "Пользователь" || user.Post?.Role?.RoleName == "Покупатель")
                    Clients.Add(user);
                }

                // Выбираем охранника (ID=6) по умолчанию
                foreach (User client in Clients)
                {
                    if (client.IdUser == 6) // Охранник
                    {
                        ClientComboBox.SelectedItem = client;
                        _selectedClient = client;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки клиентов: {ex.Message}");
            }
        }

        private void RefreshClientsButton_Click(object sender, RoutedEventArgs e)
        {
            LoadClients();
        }

        private void ClientComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ClientComboBox.SelectedItem is User selectedUser)
            {
                _selectedClient = selectedUser;
            }
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
                    MessageBox.Show("Введите ID товара!", "Внимание");
                    return;
                }

                if (!int.TryParse(IdProductText.Text, out int productId))
                {
                    MessageBox.Show("ID должен быть числом!", "Ошибка");
                    return;
                }

                // Получаем товар из БД через DbService
                _currentProduct = DbService.GetProductById(productId);

                if (_currentProduct != null)
                {
                    ProductNameLabel.Content = _currentProduct.Name;
                    ProductPriceLabel.Content = $"{_currentProduct.Price:N2} ₽";

                    // Показываем остаток на складе через DbService
                    int stockAmount = GetProductStock(productId);

                    // Проверяем, существует ли StockLabel
                    if (StockLabel != null)
                    {
                        StockLabel.Content = $"Остаток на складе: {stockAmount} шт.";
                        StockLabel.Visibility = Visibility.Visible;
                    }

                    CalculateOrderTotal();
                    Amount.Focus();
                }
                else
                {
                    MessageBox.Show($"Товар с ID {productId} не найден!", "Ошибка");
                    ClearProductInfo();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private int GetProductStock(int productId)
        {
            try
            {
                // Используем DbService.GetConnection()
                using (var connection = DbService.GetConnection())
                {
                    connection.Open();

                    string query = "SELECT AmountOfProduct FROM accounting WHERE IdProduct = @productId LIMIT 1";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@productId", productId);
                        var result = command.ExecuteScalar();
                        return result != DBNull.Value && result != null ? Convert.ToInt32(result) : 0;
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        private void CalculateOrderTotal()
        {
            try
            {
                // Безопасная проверка всех элементов
                if (OrderTotalLabel == null || _currentProduct == null || Amount == null)
                    return;

                if (int.TryParse(Amount.Text, out int quantity) && quantity > 0)
                {
                    decimal total = _currentProduct.Price * quantity;
                    OrderTotalLabel.Content = $"{total:N2} ₽";
                }
                else
                {
                    OrderTotalLabel.Content = "0 ₽";
                }
            }
            catch
            {
                if (OrderTotalLabel != null)
                    OrderTotalLabel.Content = "0 ₽";
            }
        }

        private void Amount_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateOrderTotal();
        }

        private void ProcessSaleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 1. Проверяем выбран ли клиент
                if (_selectedClient == null)
                {
                    MessageBox.Show("Выберите клиента из списка!", "Внимание");
                    ClientComboBox.Focus();
                    return;
                }

                // 2. Проверяем товар
                if (_currentProduct == null)
                {
                    MessageBox.Show("Сначала найдите товар!", "Внимание");
                    IdProductText.Focus();
                    return;
                }

                // 3. Проверяем количество
                if (!int.TryParse(Amount.Text, out int quantity) || quantity <= 0)
                {
                    MessageBox.Show("Введите корректное количество!", "Ошибка");
                    Amount.Focus();
                    return;
                }

                // 4. Проверяем наличие на складе
                int stockAmount = GetProductStock(_currentProduct.IdProduct);
                if (stockAmount <= 0)
                {
                    MessageBox.Show("Товара нет на складе!", "Ошибка");
                    return;
                }

                if (quantity > stockAmount)
                {
                    MessageBox.Show($"Недостаточно товара! Доступно: {stockAmount} шт.", "Ошибка");
                    return;
                }

                decimal total = _currentProduct.Price * quantity;

                // 5. Подтверждение с информацией о клиенте
                string clientInfo = $"{_selectedClient.LastName} {_selectedClient.FirstName}";
                if (!string.IsNullOrEmpty(_selectedClient.Patronymic))
                    clientInfo += $" {_selectedClient.Patronymic}";

                string confirmationMessage = $"Оформить продажу?\n\n" +
                                           $"Клиент: {clientInfo}\n" +
                                           $"Телефон: {_selectedClient.Phone}\n" +
                                           $"Товар: {_currentProduct.Name}\n" +
                                           $"Количество: {quantity}\n" +
                                           $"Сумма: {total:N2} ₽";

                if (MessageBox.Show(confirmationMessage, "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    // 6. Используем метод ProcessSale с clientId
                    if (DbService.ProcessSale(_selectedClient.IdUser, _currentProduct.IdProduct, quantity, total))
                    {
                        MessageBox.Show("✅ Продажа оформлена!", "Успех");
                        ClearForm();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка оформления!", "Ошибка");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void ClearForm()
        {
            // Очищаем только товар, клиента не сбрасываем
            IdProductText.Text = "";
            Amount.Text = "1";
            ClearProductInfo();
            IdProductText.Focus();
        }

        private void ClearProductInfo()
        {
            _currentProduct = null;

            if (ProductNameLabel != null)
                ProductNameLabel.Content = "-";

            if (ProductPriceLabel != null)
                ProductPriceLabel.Content = "0 ₽";

            if (ProductNameLabel != null)
                ProductNameLabel.Foreground = System.Windows.Media.Brushes.Gray;

            if (ProductPriceLabel != null)
                ProductPriceLabel.Foreground = System.Windows.Media.Brushes.Gray;

            if (StockLabel != null)
            {
                StockLabel.Content = "";
                StockLabel.Visibility = Visibility.Collapsed;
            }

            if (OrderTotalLabel != null)
                OrderTotalLabel.Content = "0 ₽";
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        // Методы навигации
        private void Hitt(object sender, RoutedEventArgs e)
        {
            // Уже на этой странице
        }

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

        private void MLBD_Exit(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            new Login().Show();
            this.Close();
        }
    }
}
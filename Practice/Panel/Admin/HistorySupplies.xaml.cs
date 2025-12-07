using Practice.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using MySql.Data.MySqlClient;

namespace Practice.Panel.Admin
{
    public partial class HistorySupplies : Window
    {
        // Используем полное имя Practice.Models.Delivery
        public ObservableCollection<Practice.Models.Delivery> Deliveries { get; set; }
        public ObservableCollection<Accounting> Accountings { get; set; }

        public HistorySupplies()
        {
            InitializeComponent();

            // Инициализируем коллекции
            Deliveries = new ObservableCollection<Practice.Models.Delivery>();
            Accountings = new ObservableCollection<Accounting>();

            // Используем Accountings как основную коллекцию для DataGrid
            DataContext = this;

            // Загружаем данные из БД
            LoadDeliveriesFromDatabase();
            LoadAccountingsFromDatabase();
        }

        // Загрузка истории доставок из БД
        private void LoadDeliveriesFromDatabase()
        {
            try
            {
                Deliveries.Clear();

                using (var connection = DbService.GetConnection())
                {
                    connection.Open();

                    string query = @"
                        SELECT d.*, 
                               uc.LastName as ClientLastName, 
                               uc.FirstName as ClientFirstName,
                               ue.LastName as EmployeeLastName,
                               ue.FirstName as EmployeeFirstName
                        FROM delivery d
                        LEFT JOIN user uc ON d.IdUserClient = uc.IdUser
                        LEFT JOIN user ue ON d.IdUserEmployee = ue.IdUser
                        ORDER BY d.IdDelivery DESC";

                    using (var command = new MySqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var delivery = new Practice.Models.Delivery
                            {
                                IdDelivery = reader.GetInt32("IdDelivery"),
                                IdUserClient = reader.GetInt32("IdUserClient"),
                                IdUserEmployee = reader.GetInt32("IdUserEmployee"),
                                IdOrder = reader.GetInt32("IdOrder"),
                                Date = reader.GetDateTime("Date"),
                                AddressDelivery = reader.GetString("AddressDelivery"),
                                IsCompleted = reader.GetBoolean("IsCompleted")
                            };

                            Deliveries.Add(delivery);
                        }
                    }
                }

                Console.WriteLine($"Загружено {Deliveries.Count} записей о доставке из БД");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки истории доставок: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Загрузка истории поставок на склад из БД
        private void LoadAccountingsFromDatabase()
        {
            try
            {
                Accountings.Clear();

                using (var connection = DbService.GetConnection())
                {
                    connection.Open();

                    // Получаем все движения товаров
                    string query = @"
                        SELECT a.*, p.Name as ProductName, s.Address as StorageAddress
                        FROM accounting a
                        JOIN product p ON a.IdProduct = p.IdProduct
                        JOIN storage s ON a.IdStorage = s.IdStorage
                        ORDER BY a.IdAccounting DESC";

                    using (var command = new MySqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var accounting = new Accounting
                            {
                                IdAccounting = reader.GetInt32("IdAccounting"),
                                IdProduct = reader.GetInt32("IdProduct"),
                                IdStorage = reader.GetInt32("IdStorage"),
                                AmountOfProduct = reader.GetInt32("AmountOfProduct"),
                                Product = new Product
                                {
                                    IdProduct = reader.GetInt32("IdProduct"),
                                    Name = reader.GetString("ProductName")
                                },
                                Storage = new Storage
                                {
                                    IdStorage = reader.GetInt32("IdStorage"),
                                    Address = reader.GetString("StorageAddress")
                                }
                            };

                            Accountings.Add(accounting);
                        }
                    }
                }

                Console.WriteLine($"Загружено {Accountings.Count} записей о поставках из БД");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки истории поставок: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Навигационные методы
        private void Registration(object sender, RoutedEventArgs e)
        {
            new Register().Show();
            this.Close();
        }

        private void Edit(object sender, RoutedEventArgs e)
        {
            new Edit().Show();
            this.Close();
        }

        private void Remove(object sender, RoutedEventArgs e)
        {
            new Remove().Show();
            this.Close();
        }

        private void Clients(object sender, RoutedEventArgs e)
        {
            new Clients().Show();
            this.Close();
        }

        private void Employees(object sender, RoutedEventArgs e)
        {
            new Employees().Show();
            this.Close();
        }

        private void Orders(object sender, RoutedEventArgs e)
        {
            new HistoryOrders().Show();
            this.Close();
        }

        private void Supplies(object sender, RoutedEventArgs e)
        {
            // Обновляем данные при клике
            LoadDeliveriesFromDatabase();
            LoadAccountingsFromDatabase();
        }

        private void MLBD_Exit(object sender, EventArgs e)
        {
            new Login().Show();
            this.Close();
        }
    }
}
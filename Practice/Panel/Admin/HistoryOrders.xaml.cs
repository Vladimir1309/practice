using Practice.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using MySql.Data.MySqlClient;

namespace Practice.Panel.Admin
{
    public partial class HistoryOrders : Window
    {
        public ObservableCollection<Order> Orderss { get; set; }

        public HistoryOrders()
        {
            InitializeComponent();

            Orderss = new ObservableCollection<Order>();
            DataContext = this;

            // Загружаем заказы из БД
            LoadOrdersFromDatabase();
        }

        // Загрузка заказов из БД
        private void LoadOrdersFromDatabase()
        {
            try
            {
                Orderss.Clear();

                using (var connection = DbService.GetConnection())
                {
                    connection.Open();

                    // Получаем все заказы с информацией о клиентах
                    string query = @"
                        SELECT o.*, u.LastName, u.FirstName, u.Patronymic 
                        FROM orders o
                        JOIN user u ON o.IdUserClient = u.IdUser
                        WHERE o.IsCompleted = 1
                        ORDER BY o.IdOrder DESC";

                    using (var command = new MySqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var order = new Order
                            {
                                IdOrder = reader.GetInt32("IdOrder"),
                                IdUserClient = reader.GetInt32("IdUserClient"),
                                Check = reader.GetDecimal("TotalSum"),
                                Delivery = reader.GetBoolean("NeedDelivery"),
                                IsCompleted = true,
                                OrderDate = reader.GetDateTime("OrderDate")
                            };

                            Orderss.Add(order);
                        }
                    }
                }

                Console.WriteLine($"Загружено {Orderss.Count} заказов из БД");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заказов из БД: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обновленный DataGrid в XAML для отображения заказов
        // Добавьте в HistoryOrders.xaml:

        private void Registration(object sender, RoutedEventArgs e)
        {
            Register register = new Register();
            register.Show();
            this.Close();
        }

        private void Edit(object sender, RoutedEventArgs e)
        {
            Edit edit = new Edit();
            edit.Show();
            this.Close();
        }

        private void Remove(object sender, RoutedEventArgs e)
        {
            Remove remove = new Remove();
            remove.Show();
            this.Close();
        }

        private void Clients(object sender, RoutedEventArgs e)
        {
            Clients clients = new Clients();
            clients.Show();
            this.Close();
        }

        private void Employees(object sender, RoutedEventArgs e)
        {
            Employees employees = new Employees();
            employees.Show();
            this.Close();
        }

        private void Orders(object sender, RoutedEventArgs e)
        {
            // Обновляем список при клике
            LoadOrdersFromDatabase();
        }

        private void Supplies(object sender, RoutedEventArgs e)
        {
            HistorySupplies supplies = new HistorySupplies();
            supplies.Show();
            this.Close();
        }

        private void MLBD_Exit(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }
    }
}
using MySql.Data.MySqlClient;
using Practice.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Practice.Panel.Sales
{
    public partial class HistoryOrdersSales : Window
    {
        public ObservableCollection<OrderViewModel> Orderss { get; set; }

        public HistoryOrdersSales()
        {
            InitializeComponent();
            LoadOrdersFromDb();
            DataContext = this;
        }

        private void LoadOrdersFromDb()
        {
            try
            {
                // Получаем заказы из БД
                var orders = DbService.GetAllOrders();

                Orderss = new ObservableCollection<OrderViewModel>();

                foreach (var order in orders)
                {
                    // Получаем имя клиента
                    string clientName = GetClientName(order.IdUserClient);

                    Orderss.Add(new OrderViewModel
                    {
                        IdOrder = order.IdOrder,
                        Client = clientName,
                        Sum = order.Check,
                        Delivery = order.Delivery ? "Да" : "Нет",
                        Status = order.IsCompleted ? "Завершен" : "В процессе",
                        Date = order.OrderDate?.ToString("dd.MM.yyyy HH:mm") ?? "Не указана"
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заказов: {ex.Message}");
            }
        }

        private string GetClientName(int userId)
        {
            try
            {
                // Простой запрос для получения имени клиента
                using (var connection = DbService.GetConnection())
                {
                    connection.Open();

                    string query = "SELECT LastName, FirstName FROM user WHERE IdUser = @userId";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string lastName = reader.GetString("LastName");
                                string firstName = reader.GetString("FirstName");
                                return $"{lastName} {firstName}";
                            }
                        }
                    }
                }
            }
            catch
            {
                // Если ошибка - возвращаем ID
                return $"Клиент ID: {userId}";
            }

            return $"Клиент ID: {userId}";
        }

        private void Hitt(object sender, RoutedEventArgs e)
        {
            Panel.Delivery.Hit hit = new Panel.Delivery.Hit();
            hit.Show();
            this.Close();
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            Panel.Delivery.Reload reload = new Panel.Delivery.Reload();
            reload.Show();
            this.Close();
        }

        private void History(object sender, RoutedEventArgs e)
        {
            // Просто обновляем данные при нажатии
            LoadOrdersFromDb();
        }

        private void MLBD_Exit(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }
    }

    // Простой класс для отображения в таблице
    public class OrderViewModel
    {
        public int IdOrder { get; set; }
        public string Client { get; set; }
        public decimal Sum { get; set; }
        public string Delivery { get; set; }
        public string Status { get; set; }
        public string Date { get; set; }
    }
}
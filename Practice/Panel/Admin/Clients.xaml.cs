using Practice.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Practice.Panel.Admin
{
    /// <summary>
    /// Логика взаимодействия для Clients.xaml
    /// </summary>
    public partial class Clients : Window
    {
        public ObservableCollection<User> Users { get; set; }

        public Clients()
        {
            InitializeComponent();

            Users = new ObservableCollection<User>();
            DataContext = this;

            // Загружаем пользователей из БД
            LoadUsersFromDatabase();
        }

        // Загрузка пользователей из БД
        private void LoadUsersFromDatabase()
        {
            try
            {
                Users.Clear();

                // Получаем всех пользователей из БД
                var usersFromDb = DbService.GetAllUsers();

                // Фильтруем только клиентов (IdPost = 5 - Покупатель)
                foreach (var user in usersFromDb)
                {
                    if (user.IdPost == 5) // Покупатель
                    {
                        Users.Add(user);
                    }
                }

                Console.WriteLine($"Загружено {Users.Count} клиентов из БД");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки клиентов из БД: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Registration(object sender, RoutedEventArgs e)
        {
            Panel.Admin.Register register = new Panel.Admin.Register();
            register.Show();
            this.Close();
        }

        private void Edit(object sender, RoutedEventArgs e)
        {
            Panel.Admin.Edit edit = new Panel.Admin.Edit();
            edit.Show();
            this.Close();
        }

        private void Remove(object sender, RoutedEventArgs e)
        {
            Panel.Admin.Remove remove = new Panel.Admin.Remove();
            remove.Show();
            this.Close();
        }

        private void Client(object sender, RoutedEventArgs e)
        {
            // Уже на этой странице
            LoadUsersFromDatabase(); // Обновляем список
        }

        private void Employees(object sender, RoutedEventArgs e)
        {
            Panel.Admin.Employees employees = new Panel.Admin.Employees();
            employees.Show();
            this.Close();
        }

        private void Orders(object sender, RoutedEventArgs e)
        {
            Panel.Admin.HistoryOrders orders = new Panel.Admin.HistoryOrders();
            orders.Show();
            this.Close();
        }

        private void Supplies(object sender, RoutedEventArgs e)
        {
            Panel.Admin.HistorySupplies supplies = new Panel.Admin.HistorySupplies();
            supplies.Show();
            this.Close();
        }

        private void MLBD_Exit(object sender, EventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }
    }
}

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
    /// Логика взаимодействия для Employees.xaml
    /// </summary>
    public partial class Employees : Window
    {
        public ObservableCollection<User> Users { get; set; }

        public Employees()
        {
            InitializeComponent();

            Users = new ObservableCollection<User>();
            DataContext = this;

            // Загружаем сотрудников из БД
            LoadEmployeesFromDatabase();
        }

        // Загрузка сотрудников из БД
        private void LoadEmployeesFromDatabase()
        {
            try
            {
                Users.Clear();

                // Получаем всех пользователей из БД
                var usersFromDb = DbService.GetAllUsers();

                // Фильтруем только сотрудников (все кроме покупателей)
                foreach (var user in usersFromDb)
                {
                    if (user.IdPost != 5) // Не покупатель
                    {
                        Users.Add(user);
                    }
                }

                Console.WriteLine($"Загружено {Users.Count} сотрудников из БД");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки сотрудников из БД: {ex.Message}", "Ошибка",
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

        private void Clients(object sender, RoutedEventArgs e)
        {
            Panel.Admin.Clients clients = new Panel.Admin.Clients();
            clients.Show();
            this.Close();
        }

        private void Employee(object sender, RoutedEventArgs e)
        {
            // Уже на этой странице
            LoadEmployeesFromDatabase(); // Обновляем список
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

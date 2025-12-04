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

        // Используем статическую коллекцию из Register
        public ObservableCollection<User> UsersTemp => Register.TempUsers;
        public Clients()
        {
            InitializeComponent();

            Users = new ObservableCollection<User>()
            {
                new User
                {
                    IdUser = 1,
                    IdPost = 4,
                    Login = "nope",
                    Password = "nope1",
                    LastName = "Сусович",
                    FirstName = "Сус",
                    Patronymic = "Суснов",
                    Phone = "89119998473",
                    Email = "sus@gmail.com",
                    Birthday = new DateTime(2004, 9, 14),
                    Address = "Улица Бобова, д. 7, кв. 66"
                },
                new User
                {
                    IdUser = 2,
                    IdPost = 5,
                    Login = "bob",
                    Password = "bob1",
                    LastName = "Великий",
                    FirstName = "Владимир",
                    Patronymic = "Павлович",
                    Phone = "89112410026",
                    Email = "megabob@gmail.com",
                    Birthday = new DateTime(2006, 9, 14),
                    Address = "Шоссе Гвардейцев, д. 7, кв. 66"
                }
            };
            DataContext = this;
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
            Panel.Admin.Clients clients = new Panel.Admin.Clients();
            clients.Show();
            this.Close();
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

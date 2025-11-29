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
    /// Логика взаимодействия для HistoryOrders.xaml
    /// </summary>
    public partial class HistoryOrders : Window
    {
        public ObservableCollection<Order> Orderss { get; set; }
        public HistoryOrders()
        {
            InitializeComponent();

            Orderss = new ObservableCollection<Order>()
            {
                new Order
                {
                    IdOrder = 1,
                    IdUserClient = 4,
                    Check = 1600,
                    Delivery = true,
                    IsCompleted = true
                },
                new Order
                {
                    IdOrder = 2,
                    IdUserClient = 2,
                    Check = 2000,
                    Delivery = false,
                    IsCompleted = true
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

        private void Clients(object sender, RoutedEventArgs e)
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

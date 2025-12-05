using Practice.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Practice.Panel.Admin
{
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
            // Уже на этой странице
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
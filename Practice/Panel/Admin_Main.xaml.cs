using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Practice
{
    /// <summary>
    /// Логика взаимодействия для Admin_Main.xaml
    /// </summary>
    public partial class Admin_Main : Window
    {
        public Admin_Main()
        {
            InitializeComponent();
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

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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Practice
{
    /// <summary>
    /// Логика взаимодействия для Delivery_Main.xaml
    /// </summary>
    public partial class Delivery_Main : Window
    {
        public Delivery_Main()
        {
            InitializeComponent();
        }

        private void Info(object sender, RoutedEventArgs e)
        {
            Panel.Delivery.OrderInfo info = new Panel.Delivery.OrderInfo();
            info.Show();
            this.Close();
        }

        private void History(object sender, RoutedEventArgs e)
        {
            Panel.Delivery.HistoryOrdersDelivery history = new Panel.Delivery.HistoryOrdersDelivery();
            history.Show();
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

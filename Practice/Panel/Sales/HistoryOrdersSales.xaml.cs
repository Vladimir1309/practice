using Practice.Panel.Delivery;
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

namespace Practice.Panel.Sales
{
    /// <summary>
    /// Логика взаимодействия для HistoryOrdersSales.xaml
    /// </summary>
    public partial class HistoryOrdersSales : Window
    {
        public HistoryOrdersSales()
        {
            InitializeComponent();
        }
        private void Hitt(object sender, RoutedEventArgs e)
        {
            Hit hit = new Hit();
            hit.Show();
            this.Close();
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            Reload reload = new Reload();
            reload.Show();
            this.Close();
        }

        private void History(object sender, RoutedEventArgs e)
        {
            HistoryOrdersDelivery history = new HistoryOrdersDelivery();
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

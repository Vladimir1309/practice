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
    /// Логика взаимодействия для Basket.xaml
    /// </summary>
    public partial class Basket : Window
    {
        public Basket()
        {
            InitializeComponent();
        }
        private void MLBD_GWAN(object sender, EventArgs e)
        {
            Main1 main1 = new Main1();
            main1.Show();
            this.Close();
        }
        private void MLBD_Basket(object sender, EventArgs e)
        {
            Basket basket = new Basket();
            basket.Show();
            this.Close();
        }

        private void MLBD_Favourite(object sender, EventArgs e)
        {
            Favourite favourite = new Favourite();
            favourite.Show();
            this.Close();
        }

        private void MLBD_Account(object sender, EventArgs e)
        {
            Account account = new Account();
            account.Show();
            this.Close();
        }

        private void Pay_Click(object sender, RoutedEventArgs e)
        {
            total.Content = $" ₽";
            PopUps.SucceedPay succeedPay = new PopUps.SucceedPay();
            succeedPay.Show();
        }

        private void Delivery_Click(object sender, RoutedEventArgs e)
        {
            var converter = new BrushConverter();
            Brush myBrush = (Brush)converter.ConvertFrom("#B70000");
            Store_Button.Background = Brushes.DarkGray;
            Delivery_Button.Background = myBrush;
            Address.Content = "Адрес доставки";
        }
        private void Store_Click(object sender, RoutedEventArgs e)
        {
            var converter = new BrushConverter();
            Brush myBrush = (Brush)converter.ConvertFrom("#B70000");
            Delivery_Button.Background = Brushes.DarkGray;
            Store_Button.Background = myBrush;
            Address.Content = "Адрес магазина";
        }
    }
}

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

namespace Practice
{
    /// <summary>
    /// Логика взаимодействия для Basket.xaml
    /// </summary>
    public partial class Basket : Window
    {
        public ObservableCollection<Order_Product> Order_Products { get; set; }
        public ObservableCollection<Order> Orders { get; set; }
        public ObservableCollection<Product> Products { get; set; }
        public Basket()
        {
            InitializeComponent();
            //DataContext = new MainViewModel();

            Order_Products = new ObservableCollection<Order_Product>()
            {
                new Order_Product
                {
                    IdOrderProduct = 1,
                    IdOrder = 1,
                    IdProduct = 1,
                    Amount = 1,
                    IsFavourited = false
                }
            };

            Orders = new ObservableCollection<Order>()
            {
                new Order
                {
                    IdOrder = 1,
                    IdUserClient = 4,
                    Check = 1600,
                    Delivery = true,
                    IsCompleted = true
                }
            };

            Products = new ObservableCollection<Product>()
            {
                 new Product
                 {
                     IdProduct = 1,
                     IdCategory = 1,
                     Name = "Красное вино",
                     Price = 1000
                 }
            };

            DataContext = this;
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

using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
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
    /// Логика взаимодействия для Wine.xaml
    /// </summary>
    public partial class Beer : Window
    {
        public Beer()
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
        private void MLBD_Account(object sender, RoutedEventArgs e)
        {
            if (AuthManager.IsAuthenticated)
            {
                // Пользователь авторизован - открываем профиль
                Account account = new Account();
                account.Show();
            }
            else
            {
                // Пользователь не авторизован - открываем страницу входа
                Login login = new Login();
                login.Show();
            }

            this.Close();
        }

        //private void MLBD_Account(object sender, EventArgs e)
        //{
        //    Account account = new Account();
        //    account.Show();
        //    this.Close();
        //}
        int i = 1;
        private void minus1(object sender, EventArgs e)
        {
            if (i > 1) i--;
            count1.Content = i;
        }
        private void plus1(object sender, EventArgs e)
        {
            if (i > 0) i++;
            count1.Content = i;
        }
        int j = 1;
        private void minus2(object sender, EventArgs e)
        {
            if (j > 1) j--;
            count2.Content = j;
        }
        private void plus2(object sender, EventArgs e)
        {
            if (j > 0) j++;
            count2.Content = j;
        }
        int m = 1;
        private void minus3(object sender, EventArgs e)
        {
            if (m > 1) m--;
            count3.Content = m;
        }
        private void plus3(object sender, EventArgs e)
        {
            if (m > 0) m++;
            count3.Content = m;
        }
        private void AddToBasket1_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(count1.Content?.ToString(), out int amount))
            {
                DataManager.AddToCart(13, amount);
            }
        }

        private void AddToBasket2_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(count2.Content?.ToString(), out int amount))
            {
                DataManager.AddToCart(14, amount);
            }
        }

        private void AddToBasket3_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(count3.Content?.ToString(), out int amount))
            {
                DataManager.AddToCart(15, amount);
            }
        }
    }
}

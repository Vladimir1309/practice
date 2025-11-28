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
    public partial class Vodka : Window
    {
        public Vodka()
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
        int i = 1;
        private void minus1(object sender, EventArgs e)
        {
            if (i > 0) i--;
            count1.Content = i;
        }
        private void plus1(object sender, EventArgs e)
        {
            if (i >= 0) i++;
            count1.Content = i;
        }
        int j = 1;
        private void minus2(object sender, EventArgs e)
        {
            if (j > 0) j--;
            count2.Content = j;
        }
        private void plus2(object sender, EventArgs e)
        {
            if (j >= 0) j++;
            count2.Content = j;
        }
        int m = 1;
        private void minus3(object sender, EventArgs e)
        {
            if (m > 0) m--;
            count3.Content = m;
        }
        private void plus3(object sender, EventArgs e)
        {
            if (m >= 0) m++;
            count3.Content = m;
        }
    }
}

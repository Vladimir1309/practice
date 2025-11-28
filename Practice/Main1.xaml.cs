using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
    /// Логика взаимодействия для Main1.xaml
    /// </summary>
    public partial class Main1 : Window
    {
        public Main1()
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

        private void MLBD_Wine(object sender, EventArgs e)
        {
            Wine wine = new Wine();
            wine.Show();
            this.Close();
        }

        private void MLBD_Champagne(object sender, EventArgs e)
        {
            Champagne champagne = new Champagne();
            champagne.Show();
            this.Close();
        }

        private void MLBD_Vodka(object sender, EventArgs e)
        {
            Vodka vodka = new Vodka();
            vodka.Show();
            this.Close();
        }

        private void MLBD_Beer(object sender, EventArgs e)
        {
            Beer beer = new Beer();
            beer.Show();
            this.Close();
        }

        private void ME_Wine(Object sender, EventArgs e)
        {
            BorderWine.Opacity = 0.5;
        }
        private void ML_Wine(Object sender, EventArgs e)
        {
            BorderWine.Opacity = 1;
        }
        private void ME_Champagne(Object sender, EventArgs e)
        {
            BorderChampagne.Opacity = 0.5;
        }
        private void ML_Champagne(Object sender, EventArgs e)
        {
            BorderChampagne.Opacity = 1;
        }
        private void ME_Vodka(Object sender, EventArgs e)
        {
            BorderVodka.Opacity = 0.5;
        }
        private void ML_Vodka(Object sender, EventArgs e)
        {
            BorderVodka.Opacity = 1;
        }
        private void ME_Beer(Object sender, EventArgs e)
        {
            BorderBeer.Opacity = 0.5;
        }
        private void ML_Beer(Object sender, EventArgs e)
        {
            BorderBeer.Opacity = 1;
        }
        private void ME_WineLabel(Object sender, EventArgs e)
        {
            BorderWine.Opacity = 0.5;
        }
        private void ML_WineLabel(Object sender, EventArgs e)
        {
            BorderWine.Opacity = 1;
        }
        private void ME_ChampagneLabel(Object sender, EventArgs e)
        {
            BorderChampagne.Opacity = 0.5;
        }
        private void ML_ChampagneLabel(Object sender, EventArgs e)
        {
            BorderChampagne.Opacity = 1;
        }
        private void ME_VodkaLabel(Object sender, EventArgs e)
        {
            BorderVodka.Opacity = 0.5;
        }
        private void ML_VodkaLabel(Object sender, EventArgs e)
        {
            BorderVodka.Opacity = 1;
        }
        private void ME_BeerLabel(Object sender, EventArgs e)
        {
            BorderBeer.Opacity = 0.5;
        }
        private void ML_BeerLabel(Object sender, EventArgs e)
        {
            BorderBeer.Opacity = 1;
        }
    }
}

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Practice
{
    public partial class Main1 : Window
    {
        public Main1()
        {
            InitializeComponent();
        }

        private void MLBD_GWAN(object sender, RoutedEventArgs e)
        {
            Main1 main1 = new Main1();
            main1.Show();
            this.Close();
        }

        private void MLBD_Basket(object sender, RoutedEventArgs e)
        {
            Basket basket = new Basket();
            basket.Show();
            this.Close();
        }

        private void MLBD_Favourite(object sender, RoutedEventArgs e)
        {
            Favourite favourite = new Favourite();
            favourite.Show();
            this.Close();
        }

        private void MLBD_Account(object sender, RoutedEventArgs e)
        {
            Account account = new Account();
            account.Show();
            this.Close();
        }

        int i = 1;
        private void minus1(object sender, RoutedEventArgs e)
        {
            if (i > 1) i--;
            count1.Content = i;
        }

        private void plus1(object sender, RoutedEventArgs e)
        {
            if (i > 0) i++;
            count1.Content = i;
        }

        int j = 1;
        private void minus2(object sender, RoutedEventArgs e)
        {
            if (j > 1) j--;
            count2.Content = j;
        }

        private void plus2(object sender, RoutedEventArgs e)
        {
            if (j > 0) j++;
            count2.Content = j;
        }

        int m = 1;
        private void minus3(object sender, RoutedEventArgs e)
        {
            if (m > 1) m--;
            count3.Content = m;
        }

        private void plus3(object sender, RoutedEventArgs e)
        {
            if (m > 0) m++;
            count3.Content = m;
        }

        private void MLBD_Wine(object sender, RoutedEventArgs e)
        {
            Wine wine = new Wine();
            wine.Show();
            this.Close();
        }

        private void MLBD_Champagne(object sender, RoutedEventArgs e)
        {
            Champagne champagne = new Champagne();
            champagne.Show();
            this.Close();
        }

        private void MLBD_Vodka(object sender, RoutedEventArgs e)
        {
            Vodka vodka = new Vodka();
            vodka.Show();
            this.Close();
        }

        private void MLBD_Beer(object sender, RoutedEventArgs e)
        {
            Beer beer = new Beer();
            beer.Show();
            this.Close();
        }

        private void ME_Wine(Object sender, RoutedEventArgs e)
        {
            BorderWine.Opacity = 0.5;
        }

        private void ML_Wine(Object sender, RoutedEventArgs e)
        {
            BorderWine.Opacity = 1;
        }

        private void ME_Champagne(Object sender, RoutedEventArgs e)
        {
            BorderChampagne.Opacity = 0.5;
        }

        private void ML_Champagne(Object sender, RoutedEventArgs e)
        {
            BorderChampagne.Opacity = 1;
        }

        private void ME_Vodka(Object sender, RoutedEventArgs e)
        {
            BorderVodka.Opacity = 0.5;
        }

        private void ML_Vodka(Object sender, RoutedEventArgs e)
        {
            BorderVodka.Opacity = 1;
        }

        private void ME_Beer(Object sender, RoutedEventArgs e)
        {
            BorderBeer.Opacity = 0.5;
        }

        private void ML_Beer(Object sender, RoutedEventArgs e)
        {
            BorderBeer.Opacity = 1;
        }

        private void ME_WineLabel(Object sender, RoutedEventArgs e)
        {
            BorderWine.Opacity = 0.5;
        }

        private void ML_WineLabel(Object sender, RoutedEventArgs e)
        {
            BorderWine.Opacity = 1;
        }

        private void ME_ChampagneLabel(Object sender, EventArgs e)
        {
            BorderChampagne.Opacity = 0.5;
        }

        private void ML_ChampagneLabel(Object sender, RoutedEventArgs e)
        {
            BorderChampagne.Opacity = 1;
        }

        private void ME_VodkaLabel(Object sender, RoutedEventArgs e)
        {
            BorderVodka.Opacity = 0.5;
        }

        private void ML_VodkaLabel(Object sender, RoutedEventArgs e)
        {
            BorderVodka.Opacity = 1;
        }

        private void ME_BeerLabel(Object sender, RoutedEventArgs e)
        {
            BorderBeer.Opacity = 0.5;
        }

        private void ML_BeerLabel(Object sender, RoutedEventArgs e)
        {
            BorderBeer.Opacity = 1;
        }

        // Обработчики кнопок "В корзину" (без Entity Framework)
        private void AddToBasket1_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(count1.Content?.ToString(), out int amount))
            {
                // Товар 1: Syrah Toscana (ID 1)
                DataManager.AddToCart(1, amount);
                MessageBox.Show("Товар добавлен в корзину!");
            }
        }

        private void AddToBasket2_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(count2.Content?.ToString(), out int amount))
            {
                // Товар 2: 7 Zlakov (ID 2)
                DataManager.AddToCart(2, amount);
                MessageBox.Show("Товар добавлен в корзину!");
            }
        }

        private void AddToBasket3_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(count3.Content?.ToString(), out int amount))
            {
                // Товар 3: Vodka Polugar (ID 3)
                DataManager.AddToCart(3, amount);
                MessageBox.Show("Товар добавлен в корзину!");
            }
        }
    }
}
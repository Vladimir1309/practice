using Practice.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Practice.Panel.Sales
{
    public partial class HistoryOrdersSales : Window
    {
        public ObservableCollection<Practice.Models.Order> Orderss { get; set; }

        public HistoryOrdersSales()
        {
            InitializeComponent();

            Orderss = new ObservableCollection<Practice.Models.Order>()
            {
                new Practice.Models.Order
                {
                    IdOrder = 1,
                    IdUserClient = 4,
                    Check = 1600,
                    Delivery = true,
                    IsCompleted = true
                },
                new Practice.Models.Order
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

        private void Hitt(object sender, RoutedEventArgs e)
        {
            Panel.Delivery.Hit hit = new Panel.Delivery.Hit();
            hit.Show();
            this.Close();
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            Panel.Delivery.Reload reload = new Panel.Delivery.Reload();
            reload.Show();
            this.Close();
        }

        private void History(object sender, RoutedEventArgs e)
        {
            // Уже на этой странице
        }

        private void MLBD_Exit(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }
    }
}
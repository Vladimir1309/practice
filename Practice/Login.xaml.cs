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
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }
        private void MLBD_Email(object sender, MouseButtonEventArgs e)
        {
            var converter = new BrushConverter();
            Brush myBrush = (Brush)converter.ConvertFrom("#B70000");
            E_mail.Foreground = myBrush;
            Phone.Foreground = Brushes.Black;
            E_P_Label.Content = "E-mail";
        }

        private void MLBD_Phone(object sender, MouseButtonEventArgs e)
        {
            var converter = new BrushConverter();
            Brush myBrush = (Brush)converter.ConvertFrom("#B70000");
            Phone.Foreground = myBrush;
            E_mail.Foreground = Brushes.Black;
            E_P_Label.Content = "Номер телефона";
        }

        private void AlreadyAccount_MouseEnter(object sender, MouseEventArgs e)
        {
            var converter = new BrushConverter();
            Brush myBrush = (Brush)converter.ConvertFrom("#B70000");
            AlreadyAccount.Foreground = myBrush;
            AlreadyAccount.Opacity = 0.5;
        }

        private void AlreadyAccount_MouseLeave(object sender, MouseEventArgs e)
        {
            AlreadyAccount.Foreground = Brushes.Black;
            AlreadyAccount.Opacity = 0.13;
        }

        //private void AdminPanel_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    var converter = new BrushConverter();
        //    Brush myBrush = (Brush)converter.ConvertFrom("#B70000");
        //    Admin.Foreground = myBrush;
        //    Admin.Opacity = 0.5;
        //}

        //private void AdminPanel_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    Admin.Foreground = Brushes.Black;
        //    Admin.Opacity = 0.13;
        //}

        //private void SalesPanel_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    var converter = new BrushConverter();
        //    Brush myBrush = (Brush)converter.ConvertFrom("#B70000");
        //    Sales.Foreground = myBrush;
        //    Sales.Opacity = 0.5;
        //}

        //private void SalesPanel_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    Sales.Foreground = Brushes.Black;
        //    Sales.Opacity = 0.13;
        //}

        //private void DeliveryPanel_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    var converter = new BrushConverter();
        //    Brush myBrush = (Brush)converter.ConvertFrom("#B70000");
        //    Delivery.Foreground = myBrush;
        //    Delivery.Opacity = 0.5;
        //}

        //private void DeliveryPanel_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    Delivery.Foreground = Brushes.Black;
        //    Delivery.Opacity = 0.13;
        //}

        private void MLBD_Already(object sender, EventArgs e)
        {
            MainWindow mainwindow = new MainWindow();
            mainwindow.Show();
            this.Close();
        }

        //private void MLBD_AdminPanel(object sender, EventArgs e)
        //{
        //    Admin_Main adminMain = new Admin_Main();
        //    adminMain.Show();
        //    this.Close();
        //}

        //private void MLBD_SalesPanel(object sender, EventArgs e)
        //{
        //    Sales_Main salesMain = new Sales_Main();
        //    salesMain.Show();
        //    this.Close();
        //}

        //private void MLBD_DeliveryPanel(object sender, EventArgs e)
        //{
        //    Delivery_Main deliveryMain = new Delivery_Main();
        //    deliveryMain.Show();
        //    this.Close();
        //}

        private void MLBD_VK_TG(object sender, EventArgs e)
        {
            InWork inwork = new InWork();
            inwork.Show();
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

        private void LoginButton(object sender, RoutedEventArgs e)
        {
            if (LoginText.Text == "admin" && PasswordText.Text == "admin")
            {
                Admin_Main adminMain = new Admin_Main();
                adminMain.Show();
                this.Close();
            }
            if (LoginText.Text == "sales" && PasswordText.Text == "sales")
            {
                Sales_Main salesMain = new Sales_Main();
                salesMain.Show();
                this.Close();
            }
            if (LoginText.Text == "delivery" && PasswordText.Text == "delivery")
            {
                Delivery_Main deliveryMain = new Delivery_Main();
                deliveryMain.Show();
                this.Close();
            }
            if (LoginText.Text == "user" && PasswordText.Text == "user")
            {
                Account account = new Account();
                account.Show();
                this.Close();
            }
        }
        private void E_mail_MouseEnter(object sender, MouseEventArgs e)
        {
            //var converter = new BrushConverter();
            //Brush myBrush = (Brush)converter.ConvertFrom("#B70000");
            //if (this.Foreground == Brushes.Black)
            //{
            //    E_mail.Opacity = 0.5;
            //}
            E_mail.Opacity = 0.5;
        }

        private void E_mail_MouseLeave(object sender, MouseEventArgs e)
        {
            //var converter = new BrushConverter();
            //Brush myBrush = (Brush)converter.ConvertFrom("#B70000");
            //if (this.Foreground == Brushes.Black) 
            //{
            //    E_mail.Opacity = 1;
            //}
            E_mail.Opacity = 1;
        }

        private void Phone_MouseEnter(object sender, MouseEventArgs e)
        {
            //var converter = new BrushConverter();
            //Brush myBrush = (Brush)converter.ConvertFrom("#B70000");
            //if (E_mail.Foreground == myBrush) 
            //{
            //    Phone.Foreground = myBrush;
            //    Phone.Opacity = 0.5;
            //}
            Phone.Opacity = 0.5;
        }

        private void Phone_MouseLeave(object sender, MouseEventArgs e)
        {
            //var converter = new BrushConverter();
            //Brush myBrush = (Brush)converter.ConvertFrom("#B70000");
            //if (E_mail.Foreground == myBrush) 
            //{
            //    Phone.Foreground = Brushes.Black;
            //    Phone.Opacity = 1;
            //}
            Phone.Opacity = 1;
        }
    }
}

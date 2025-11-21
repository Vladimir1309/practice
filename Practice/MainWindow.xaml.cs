using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Practice
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void E_mail_MouseEnter(object sender, MouseEventArgs e)
        {
            var converter = new BrushConverter();
            Brush myBrush = (Brush)converter.ConvertFrom("#B70000");
            E_mail.Foreground = myBrush;
            E_mail.Opacity = 0.5;
        }

        private void E_mail_MouseLeave(object sender, MouseEventArgs e)
        {
            E_mail.Foreground = Brushes.Black;
            E_mail.Opacity = 0.13;
        }

        private void Phone_MouseEnter(object sender, MouseEventArgs e)
        {
            var converter = new BrushConverter();
            Brush myBrush = (Brush)converter.ConvertFrom("#B70000");

            Phone.Foreground = myBrush;
            Phone.Opacity = 0.5;

        }

        private void Phone_MouseLeave(object sender, MouseEventArgs e)
        {
            Phone.Foreground = Brushes.Black;
            Phone.Opacity = 0.13;
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
    }
}
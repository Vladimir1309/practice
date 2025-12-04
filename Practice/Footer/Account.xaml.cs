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
    /// Логика взаимодействия для Account.xaml
    /// </summary>
    public partial class Account : Window
    {
        public Account()
        {
            InitializeComponent();

            // Проверяем авторизацию при загрузке страницы
            if (!AuthManager.IsAuthenticated)
            {
                MessageBox.Show("Вы не авторизованы!", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                Login login = new Login();
                login.Show();
                this.Close();
                return;
            }

            // Если пользователь авторизован, загружаем данные профиля
            LoadUserData();
        
        // Остальные методы...
        }


        private void LoadUserData()
        {
            LoginTextBox.Text = AuthManager.CurrentUserLogin;
            //roleTextBlock.Text = AuthManager.CurrentUserRole;
            //UserTextBox.Text = AuthManager.CurrentUserId.ToString();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            AuthManager.Logout();
            MessageBox.Show("Вы вышли из системы", "Выход",
                          MessageBoxButton.OK, MessageBoxImage.Information);

            Main1 main1 = new Main1();
            main1.Show();
            this.Close();
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
    }
}

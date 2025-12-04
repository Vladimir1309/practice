using MySql.Data.MySqlClient;
using Practice.Models;
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

        private void AdvancedDiagnostics_Click(object sender, RoutedEventArgs e)
        {
            TestConnectionWindow testWindow = new TestConnectionWindow();
            testWindow.ShowDialog();
        }

        private void Diagnostics_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string info = "=== ДИАГНОСТИКА ПОДКЛЮЧЕНИЯ ===\n\n";

                // Тестируем все варианты строк подключения
                string[] connections = {
            DatabaseManager.ConnectionString,
            DatabaseManager.ConnectionString2,
            DatabaseManager.ConnectionString3,
            DatabaseManager.ConnectionString4
        };

                string[] connectionNames = {
            "Основная (SslMode=none)",
            "Альтернативная (SslMode=Preferred)",
            "Без SSL (SslMode=Disabled)",
            "Без указания SSL"
        };

                bool anySuccess = false;

                for (int i = 0; i < connections.Length; i++)
                {
                    info += $"\n--- Тест {i + 1}: {connectionNames[i]} ---\n";

                    try
                    {
                        using (var connection = new MySqlConnection(connections[i]))
                        {
                            connection.Open();
                            info += "✓ Подключение успешно\n";

                            // Проверяем таблицы
                            using (var command = new MySqlCommand("SHOW TABLES", connection))
                            using (var reader = command.ExecuteReader())
                            {
                                List<string> tables = new List<string>();
                                while (reader.Read())
                                {
                                    tables.Add(reader[0].ToString());
                                }
                                info += $"Таблиц найдено: {tables.Count}\n";

                                // Проверяем конкретно таблицу user
                                if (tables.Contains("user"))
                                {
                                    info += "✓ Таблица 'user' существует\n";

                                    // Проверяем пользователей
                                    connection.Close();
                                    connection.Open();
                                    using (var cmd = new MySqlCommand("SELECT Login, Password FROM `user` LIMIT 5", connection))
                                    using (var userReader = cmd.ExecuteReader())
                                    {
                                        info += "Первые 5 пользователей:\n";
                                        while (userReader.Read())
                                        {
                                            info += $"  Логин: {userReader["Login"]}, Пароль: {userReader["Password"]}\n";
                                        }
                                    }
                                }
                                else
                                {
                                    info += "✗ Таблица 'user' не найдена\n";
                                    info += "Доступные таблицы:\n";
                                    foreach (var table in tables.Take(5))
                                    {
                                        info += $"  - {table}\n";
                                    }
                                    if (tables.Count > 5) info += $"  ... и еще {tables.Count - 5}\n";
                                }
                            }

                            connection.Close();
                            anySuccess = true;
                        }
                    }
                    catch (MySqlException ex)
                    {
                        info += $"✗ Ошибка MySQL: {ex.Message}\n";
                        info += $"Код: {ex.Number}\n";
                    }
                    catch (Exception ex)
                    {
                        info += $"✗ Ошибка: {ex.Message}\n";
                    }
                }

                if (!anySuccess)
                {
                    info += "\n✗ Ни одно подключение не сработало!\n";
                    info += "Проверьте:\n";
                    info += "1. Интернет соединение\n";
                    info += "2. Доступность сервера tompsons.beget.tech\n";
                    info += "3. Правильность логина/пароля\n";
                    info += "4. Разрешены ли удаленные подключения на beget\n";
                }
                else
                {
                    info += "\n✓ Хотя бы одно подключение работает!\n";
                    info += "Попробуйте войти с:\n";
                    info += "Логин: nope, Пароль: nope1\n";
                    info += "Логин: bob, Пароль: bob1\n";
                }

                MessageBox.Show(info, "Диагностика", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка диагностики: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            E_P_Label.Content = "Логин";
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

        // В Login.xaml.cs в методе LoginButton
        private void LoginButton(object sender, RoutedEventArgs e)
        {
            string login = LoginText.Text;
            string password = PasswordText.Text;

            // Логируем ввод
            string debugInfo = $"Попытка входа:\nЛогин: {login}\nПароль: {password}\n\n";

            // Для отладки - временно показываем что вводим
            Console.WriteLine($"Login attempt: {login}/{password}");

            // Сначала проверяем простые тестовые логины
            if (login == "admin" && password == "admin")
            {
                AuthManager.SetTestUser(TestUserFactory.CreateAdmin());
                Admin_Main adminMain = new Admin_Main();
                adminMain.Show();
                this.Close();
                return;
            }
            else if (login == "user" && password == "user")
            {
                AuthManager.SetTestUser(TestUserFactory.CreateClient());
                Account account = new Account();
                account.Show();
                this.Close();
                return;
            }

            // Пробуем реальную БД
            try
            {
                debugInfo += "Пробуем подключиться к БД...\n";

                if (AuthManager.Login(login, password))
                {
                    debugInfo += "Аутентификация успешна!\n";
                    debugInfo += $"Роль: {AuthManager.CurrentUserRole}\n";

                    // В зависимости от роли открываем соответствующую страницу
                    if (AuthManager.IsAdmin())
                    {
                        Admin_Main adminMain = new Admin_Main();
                        adminMain.Show();
                    }
                    else if (AuthManager.IsSales())
                    {
                        Sales_Main salesMain = new Sales_Main();
                        salesMain.Show();
                    }
                    else if (AuthManager.IsDelivery())
                    {
                        Delivery_Main deliveryMain = new Delivery_Main();
                        deliveryMain.Show();
                    }
                    else if (AuthManager.IsClient())
                    {
                        Account account = new Account();
                        account.Show();
                    }

                    this.Close();
                }
                else
                {
                    debugInfo += "Аутентификация не удалась\n";

                    // Показываем для отладки
                    MessageBox.Show($"{debugInfo}\nВозможно, пользователя нет в БД или неверный пароль.",
                                  "Отладка", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                debugInfo += $"Ошибка: {ex.Message}\n";
                MessageBox.Show($"{debugInfo}\n\nИспользуйте тестовые логины:\nadmin/admin\nuser/user",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        

        private void OpenTestAdminPanel()
        {
            var testUser = new User
            {
                IdUser = 1,
                Login = "admin",
                Post = new Post
                {
                    IdPost = 4,
                    PostName = "Администратор",
                    Role = new Role
                    {
                        IdRole = 2,
                        RoleName = "Администратор"
                    }
                }
            };

            AuthManager.SetTestUser(testUser);

            Admin_Main adminMain = new Admin_Main();
            adminMain.Show();
            this.Close();
        }

        private void OpenTestUserPanel()
        {
            var testUser = new User
            {
                IdUser = 2,
                Login = "user",
                Post = new Post
                {
                    IdPost = 5,
                    PostName = "Покупатель",
                    Role = new Role
                    {
                        IdRole = 1,
                        RoleName = "Пользователь"
                    }
                }
            };

            AuthManager.SetTestUser(testUser);

            Account account = new Account();
            account.Show();
            this.Close();
        }

        //private void LoginButton(object sender, RoutedEventArgs e)
        //{
        //    if (LoginText.Text == "admin" && PasswordText.Text == "admin")
        //    {
        //        Admin_Main adminMain = new Admin_Main();
        //        adminMain.Show();
        //        this.Close();
        //    }
        //    if (LoginText.Text == "sales" && PasswordText.Text == "sales")
        //    {
        //        Sales_Main salesMain = new Sales_Main();
        //        salesMain.Show();
        //        this.Close();
        //    }
        //    if (LoginText.Text == "delivery" && PasswordText.Text == "delivery")
        //    {
        //        Delivery_Main deliveryMain = new Delivery_Main();
        //        deliveryMain.Show();
        //        this.Close();
        //    }
        //    if (LoginText.Text == "user" && PasswordText.Text == "user")
        //    {
        //        Account account = new Account();
        //        account.Show();
        //        this.Close();
        //    }
        //}
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

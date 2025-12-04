using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Practice.Models;

namespace Practice.Panel.Admin
{
    public partial class Register : Window
    {
        // Временное хранилище пользователей
        private static ObservableCollection<User> _tempUsers = new ObservableCollection<User>();

        public static ObservableCollection<User> TempUsers => _tempUsers;

        public Register()
        {
            InitializeComponent();

            // Инициализация тестовыми данными
            if (_tempUsers.Count == 0)
            {
                _tempUsers.Add(new User
                {
                    IdUser = 1,
                    IdPost = 4,
                    Login = "nope",
                    Password = "nope1",
                    LastName = "Сусович",
                    FirstName = "Сус",
                    Patronymic = "Суснов",
                    Phone = "89119998473",
                    Email = "sus@gmail.com",
                    Birthday = new DateTime(2004, 9, 14),
                    Address = "Улица Бобова, д. 7, кв. 66"
                });
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка обязательных полей
                if (string.IsNullOrWhiteSpace(LastName.Text) ||
                    string.IsNullOrWhiteSpace(FirstName.Text) ||
                    string.IsNullOrWhiteSpace(Patronymic.Text) ||
                    string.IsNullOrWhiteSpace(Birthday.Text) ||
                    string.IsNullOrWhiteSpace(Phone.Text) ||
                    string.IsNullOrWhiteSpace(Post.Text))
                {
                    MessageBox.Show("Заполните все поля!");
                    return;
                }

                // Проверка даты
                if (!DateTime.TryParse(Birthday.Text, out var birthday))
                {
                    MessageBox.Show("Некорректная дата!");
                    return;
                }

                // Проверка уникальности телефона
                if (_tempUsers.Any(u => u.Phone == Phone.Text))
                {
                    MessageBox.Show("Телефон уже используется!");
                    return;
                }

                // Получение логина и пароля
                string login = string.IsNullOrWhiteSpace(Login.Text) || Login.Text == "авто"
                    ? $"{LastName.Text.ToLower()}{FirstName.Text[0]}"
                    : Login.Text;

                string password = string.IsNullOrWhiteSpace(Password.Text) || Password.Text == "авто"
                    ? $"{LastName.Text.ToLower().Substring(0, Math.Min(3, LastName.Text.Length))}{Phone.Text.Substring(0, Math.Min(2, Phone.Text.Length))}"
                    : Password.Text;

                // Проверка уникальности логина
                if (_tempUsers.Any(u => u.Login == login))
                {
                    MessageBox.Show("Логин уже существует!");
                    return;
                }

                // Создание нового пользователя
                var newUser = new User
                {
                    IdUser = _tempUsers.Count > 0 ? _tempUsers.Max(u => u.IdUser) + 1 : 1,
                    IdPost = 1, // Временное значение
                    Login = login,
                    Password = password,
                    LastName = LastName.Text,
                    FirstName = FirstName.Text,
                    Patronymic = Patronymic.Text,
                    Phone = Phone.Text,
                    Email = "",
                    Birthday = birthday,
                    Address = ""
                };

                // Сохранение
                _tempUsers.Add(newUser);

                MessageBox.Show($"Пользователь зарегистрирован!\nЛогин: {login}\nПароль: {password}");
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void ClearFields()
        {
            LastName.Text = "";
            FirstName.Text = "";
            Patronymic.Text = "";
            Birthday.Text = "";
            Phone.Text = "";
            Salary.Text = "";
            Post.Text = "";
            Login.Text = "";
            Password.Text = "";
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }

        // Навигационные методы
        private void Registration(object sender, RoutedEventArgs e) { }

        private void Edit(object sender, RoutedEventArgs e)
        {
            new Panel.Admin.Edit().Show();
            this.Close();
        }

        private void Remove(object sender, RoutedEventArgs e)
        {
            new Panel.Admin.Remove().Show();
            this.Close();
        }

        private void Clients(object sender, RoutedEventArgs e)
        {
            new Panel.Admin.Clients().Show();
            this.Close();
        }

        private void Employees(object sender, RoutedEventArgs e)
        {
            new Panel.Admin.Employees().Show();
            this.Close();
        }

        private void Orders(object sender, RoutedEventArgs e)
        {
            new Panel.Admin.HistoryOrders().Show();
            this.Close();
        }

        private void Supplies(object sender, RoutedEventArgs e)
        {
            new Panel.Admin.HistorySupplies().Show();
            this.Close();
        }

        private void MLBD_Exit(object sender, EventArgs e)
        {
            new Login().Show();
            this.Close();
        }
    }
}
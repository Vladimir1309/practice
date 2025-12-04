using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Practice.Models;

namespace Practice.Panel.Admin
{
    public partial class Edit : Window
    {
        private User _currentUser;

        // Локальная коллекция пользователей (дублируем из Register)
        private static ObservableCollection<User> _localUsers = new ObservableCollection<User>();

        static Edit()
        {
            // Инициализируем теми же данными, что и в Register
            _localUsers.Add(new User
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

            _localUsers.Add(new User
            {
                IdUser = 2,
                IdPost = 5,
                Login = "bob",
                Password = "bob1",
                LastName = "Великий",
                FirstName = "Владимир",
                Patronymic = "Павлович",
                Phone = "89112410026",
                Email = "megabob@gmail.com",
                Birthday = new DateTime(2006, 9, 14),
                Address = "Шоссе Гвардейцев, д. 7, кв. 66"
            });
        }

        public Edit()
        {
            InitializeComponent();

            // Изначально показываем только поле UserID и кнопку поиска
            ShowUserIdSection();
            HideEditFields();

            // Для отладки: покажем всех пользователей в консоль
            PrintAllUsers();
        }

        // Показываем секцию ввода UserID
        private void ShowUserIdSection()
        {
            UserID.Visibility = Visibility.Visible;
            FindUserButton.Visibility = Visibility.Visible;
            UserIDLabel.Visibility = Visibility.Visible;

            EnterID.Visibility = Visibility.Visible;
        }

        // Скрываем секцию ввода UserID
        private void HideUserIdSection()
        {
            UserID.Visibility = Visibility.Hidden;
            FindUserButton.Visibility = Visibility.Hidden;
            UserIDLabel.Visibility = Visibility.Hidden;

            EnterID.Visibility = Visibility.Hidden;
        }

        // Показываем поля для редактирования
        private void ShowEditFields()
        {
            LastName.Visibility = Visibility.Visible;
            FirstName.Visibility = Visibility.Visible;
            Patronymic.Visibility = Visibility.Visible;
            Birthday.Visibility = Visibility.Visible;
            Phone.Visibility = Visibility.Visible;
            Salary.Visibility = Visibility.Visible;
            Post.Visibility = Visibility.Visible;

            LastNameLabel.Visibility = Visibility.Visible;
            FirstNameLabel.Visibility = Visibility.Visible;
            PatronymicLabel.Visibility = Visibility.Visible;
            BirthdayLabel.Visibility = Visibility.Visible;
            PhoneLabel.Visibility = Visibility.Visible;
            SalaryLabel.Visibility = Visibility.Visible;
            PostLabel.Visibility = Visibility.Visible;

            EditButton.Visibility = Visibility.Visible;
            CancelEditButton.Visibility = Visibility.Visible;
        }

        // Скрываем поля для редактирования
        private void HideEditFields()
        {
            LastName.Visibility = Visibility.Hidden;
            FirstName.Visibility = Visibility.Hidden;
            Patronymic.Visibility = Visibility.Hidden;
            Birthday.Visibility = Visibility.Hidden;
            Phone.Visibility = Visibility.Hidden;
            Salary.Visibility = Visibility.Hidden;
            Post.Visibility = Visibility.Hidden;

            LastNameLabel.Visibility = Visibility.Hidden;
            FirstNameLabel.Visibility = Visibility.Hidden;
            PatronymicLabel.Visibility = Visibility.Hidden;
            BirthdayLabel.Visibility = Visibility.Hidden;
            PhoneLabel.Visibility = Visibility.Hidden;
            SalaryLabel.Visibility = Visibility.Hidden;
            PostLabel.Visibility = Visibility.Hidden;

            EditButton.Visibility = Visibility.Hidden;
            CancelEditButton.Visibility = Visibility.Hidden;
        }

        // Кнопка "Найти пользователя"
        private void FindUserButton_Click(object sender, RoutedEventArgs e)
        {
            FindUser();
        }

        // Поиск пользователя по ID
        private void FindUser()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(UserID.Text))
                {
                    MessageBox.Show("Введите ID пользователя!");
                    return;
                }

                if (int.TryParse(UserID.Text, out int userId))
                {
                    // Ищем пользователя в локальной коллекции
                    _currentUser = _localUsers.FirstOrDefault(u => u.IdUser == userId);

                    if (_currentUser != null)
                    {
                        // Заполняем поля данными
                        LastName.Text = _currentUser.LastName;
                        FirstName.Text = _currentUser.FirstName;
                        Patronymic.Text = _currentUser.Patronymic;
                        Birthday.Text = _currentUser.Birthday.ToString("yyyy-MM-dd");
                        Phone.Text = _currentUser.Phone;
                        Post.Text = _currentUser.IdPost.ToString();

                        // Меняем видимость элементов
                        HideUserIdSection();
                        ShowEditFields();

                        // Фокус на первое поле для редактирования
                        LastName.Focus();
                    }
                    else
                    {
                        MessageBox.Show($"Пользователь с ID {userId} не найден!\nДоступные ID: {string.Join(", ", _localUsers.Select(u => u.IdUser))}");
                        UserID.Focus();
                        UserID.SelectAll();
                    }
                }
                else
                {
                    MessageBox.Show("ID должен быть числом!");
                    UserID.Focus();
                    UserID.SelectAll();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        // Кнопка "Изменить"
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_currentUser == null)
                {
                    MessageBox.Show("Ошибка: пользователь не выбран!");
                    ResetForm();
                    return;
                }

                // Проверка обязательных полей
                if (string.IsNullOrWhiteSpace(LastName.Text) ||
                    string.IsNullOrWhiteSpace(FirstName.Text) ||
                    string.IsNullOrWhiteSpace(Patronymic.Text) ||
                    string.IsNullOrWhiteSpace(Birthday.Text) ||
                    string.IsNullOrWhiteSpace(Phone.Text))
                {
                    MessageBox.Show("Заполните все обязательные поля!");
                    return;
                }

                // Проверка даты
                if (!DateTime.TryParse(Birthday.Text, out var birthday))
                {
                    MessageBox.Show("Некорректная дата! Используйте формат ГГГГ-ММ-ДД");
                    Birthday.Focus();
                    return;
                }

                // Проверка уникальности телефона (кроме текущего пользователя)
                if (_localUsers.Any(u => u.Phone == Phone.Text && u.IdUser != _currentUser.IdUser))
                {
                    MessageBox.Show("Телефон уже используется другим пользователем!");
                    Phone.Focus();
                    return;
                }

                // Обновляем данные пользователя
                _currentUser.LastName = LastName.Text;
                _currentUser.FirstName = FirstName.Text;
                _currentUser.Patronymic = Patronymic.Text;
                _currentUser.Birthday = birthday;
                _currentUser.Phone = Phone.Text;

                if (int.TryParse(Post.Text, out int postId))
                {
                    _currentUser.IdPost = postId;
                }

                // Также обновляем в общей коллекции Register если пользователь там есть
                var userInRegister = Register.TempUsers.FirstOrDefault(u => u.IdUser == _currentUser.IdUser);
                if (userInRegister != null)
                {
                    userInRegister.LastName = _currentUser.LastName;
                    userInRegister.FirstName = _currentUser.FirstName;
                    userInRegister.Patronymic = _currentUser.Patronymic;
                    userInRegister.Birthday = _currentUser.Birthday;
                    userInRegister.Phone = _currentUser.Phone;
                    userInRegister.IdPost = _currentUser.IdPost;
                }

                MessageBox.Show($"Данные пользователя {_currentUser.LastName} {_currentUser.FirstName} успешно обновлены!\nID: {_currentUser.IdUser}");

                // Сбрасываем форму
                ResetForm();

                // Для отладки
                PrintAllUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

        // Кнопка "Отмена" (при редактировании)
        private void CancelEditButton_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }

        // Сброс формы
        private void ResetForm()
        {
            _currentUser = null;
            UserID.Text = "";
            LastName.Text = "";
            FirstName.Text = "";
            Patronymic.Text = "";
            Birthday.Text = "";
            Phone.Text = "";
            Salary.Text = "";
            Post.Text = "";

            HideEditFields();
            ShowUserIdSection();
            UserID.Focus();
        }

        // Метод для отладки
        private void PrintAllUsers()
        {
            Console.WriteLine("\n=== ЛОКАЛЬНЫЕ ПОЛЬЗОВАТЕЛИ В EDIT ===");
            foreach (var user in _localUsers)
            {
                Console.WriteLine($"ID: {user.IdUser}, ФИО: {user.LastName} {user.FirstName} {user.Patronymic}, Телефон: {user.Phone}");
            }
            Console.WriteLine("=====================================\n");
        }

        // Навигационные методы
        private void Registration(object sender, RoutedEventArgs e)
        {
            new Register().Show();
            this.Close();
        }

        private void Editt(object sender, RoutedEventArgs e) { }

        private void Remove(object sender, RoutedEventArgs e)
        {
            new Remove().Show();
            this.Close();
        }

        private void Clients(object sender, RoutedEventArgs e)
        {
            new Clients().Show();
            this.Close();
        }

        private void Employees(object sender, RoutedEventArgs e)
        {
            new Employees().Show();
            this.Close();
        }

        private void Orders(object sender, RoutedEventArgs e)
        {
            new HistoryOrders().Show();
            this.Close();
        }

        private void Supplies(object sender, RoutedEventArgs e)
        {
            new HistorySupplies().Show();
            this.Close();
        }

        private void MLBD_Exit(object sender, EventArgs e)
        {
            new Login().Show();
            this.Close();
        }
    }
}
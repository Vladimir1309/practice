using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using Practice.Models;
using MySql.Data.MySqlClient;

namespace Practice.Panel.Admin
{
    public partial class Register : Window
    {
        // Временное хранилище пользователей
        private static System.Collections.ObjectModel.ObservableCollection<User> _tempUsers =
            new System.Collections.ObjectModel.ObservableCollection<User>();

        public static System.Collections.ObjectModel.ObservableCollection<User> TempUsers => _tempUsers;

        public Register()
        {
            InitializeComponent();

            // Загружаем пользователей из БД при инициализации
            LoadUsersFromDatabase();
        }

        // Загрузка пользователей из БД
        private void LoadUsersFromDatabase()
        {
            try
            {
                // Очищаем временную коллекцию
                _tempUsers.Clear();

                // Получаем всех пользователей из БД через DbService
                var usersFromDb = DbService.GetAllUsers();

                // Добавляем в локальную коллекцию
                foreach (var user in usersFromDb)
                {
                    _tempUsers.Add(user);
                }

                Console.WriteLine($"Загружено {usersFromDb.Count} пользователей из БД");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки пользователей из БД: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // ВАЛИДАЦИЯ ВСЕХ ПОЛЕЙ

                // 1. Проверка обязательных полей
                if (string.IsNullOrWhiteSpace(LastName.Text))
                {
                    MessageBox.Show("Введите фамилию!", "Ошибка");
                    LastName.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(FirstName.Text))
                {
                    MessageBox.Show("Введите имя!", "Ошибка");
                    FirstName.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(Patronymic.Text))
                {
                    // Патронимик может быть пустым, но предупреждаем
                    if (MessageBox.Show("Отчество не указано. Продолжить?", "Внимание",
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    {
                        Patronymic.Focus();
                        return;
                    }
                }

                // 2. Валидация даты рождения
                if (string.IsNullOrWhiteSpace(Birthday.Text))
                {
                    MessageBox.Show("Введите дату рождения!", "Ошибка");
                    Birthday.Focus();
                    return;
                }

                if (!DateTime.TryParse(Birthday.Text, out var birthday))
                {
                    MessageBox.Show("Некорректная дата рождения! Используйте формат ГГГГ-ММ-ДД", "Ошибка");
                    Birthday.Focus();
                    return;
                }

                // Проверка возраста (старше 18 лет)
                int age = DateTime.Now.Year - birthday.Year;
                if (DateTime.Now.Month < birthday.Month ||
                   (DateTime.Now.Month == birthday.Month && DateTime.Now.Day < birthday.Day))
                {
                    age--;
                }

                if (age < 18)
                {
                    MessageBox.Show("Пользователь должен быть старше 18 лет!", "Ошибка");
                    Birthday.Focus();
                    return;
                }

                // 3. Валидация телефона
                if (string.IsNullOrWhiteSpace(Phone.Text))
                {
                    MessageBox.Show("Введите телефон!", "Ошибка");
                    Phone.Focus();
                    return;
                }

                if (!IsValidPhone(Phone.Text))
                {
                    MessageBox.Show("Некорректный формат телефона! Используйте формат: +7XXXXXXXXXX или 8XXXXXXXXXX", "Ошибка");
                    Phone.Focus();
                    Phone.SelectAll();
                    return;
                }

                // Нормализация телефона
                string normalizedPhone = NormalizePhone(Phone.Text);

                // Проверка уникальности телефона в БД
                if (IsPhoneExists(normalizedPhone))
                {
                    MessageBox.Show($"Телефон '{normalizedPhone}' уже используется! Введите другой телефон.", "Ошибка");
                    Phone.Focus();
                    Phone.SelectAll();
                    return;
                }

                // 4. Валидация email (необязательное поле)
                string email = Email.Text?.Trim();
                if (!string.IsNullOrEmpty(email))
                {
                    if (!IsValidEmail(email))
                    {
                        MessageBox.Show("Некорректный формат email! Пример: user@example.com", "Ошибка");
                        Email.Focus();
                        Email.SelectAll();
                        return;
                    }

                    // Проверка уникальности email в БД
                    if (IsEmailExists(email))
                    {
                        MessageBox.Show($"Email '{email}' уже используется!", "Ошибка");
                        Email.Focus();
                        Email.SelectAll();
                        return;
                    }
                }

                // 5. Валидация должности
                if (string.IsNullOrWhiteSpace(Post.Text))
                {
                    // По умолчанию - покупатель (ID: 5)
                    Post.Text = "5";
                }

                if (!int.TryParse(Post.Text, out int postId) || postId < 1 || postId > 5)
                {
                    MessageBox.Show("ID должности должен быть числом от 1 до 5!\n" +
                                  "1 - Продавец, 2 - Охранник, 3 - Курьер, 4 - Администратор, 5 - Покупатель",
                                  "Ошибка");
                    Post.Focus();
                    Post.SelectAll();
                    return;
                }

                // 6. Получение логина и пароля
                string login = string.IsNullOrWhiteSpace(Login.Text) || Login.Text == "авто"
                    ? GenerateLogin(LastName.Text, FirstName.Text)
                    : Login.Text.Trim();

                string password = string.IsNullOrWhiteSpace(Password.Text) || Password.Text == "авто"
                    ? GeneratePassword(LastName.Text, normalizedPhone)
                    : Password.Text;

                // Проверка уникальности логина в БД
                if (IsLoginExists(login))
                {
                    MessageBox.Show($"Логин '{login}' уже существует! Введите другой логин.", "Ошибка");
                    Login.Focus();
                    Login.SelectAll();
                    return;
                }

                // 7. Адрес (необязательное поле)
                string address = Address.Text?.Trim();

                // 8. Регистрация пользователя в БД
                if (RegisterUserInDatabase(login, password, LastName.Text, FirstName.Text,
                    Patronymic.Text, normalizedPhone, email, birthday, address, postId))
                {
                    // Обновляем локальную коллекцию
                    LoadUsersFromDatabase();

                    // Показываем успешное сообщение с деталями
                    string successMessage = $"✅ Пользователь успешно зарегистрирован!\n\n" +
                                          $"Логин: {login}\n" +
                                          $"Пароль: {password}\n" +
                                          $"ФИО: {LastName.Text} {FirstName.Text} {Patronymic.Text}\n" +
                                          $"Телефон: {normalizedPhone}\n" +
                                          $"Должность: {GetPostName(postId)}\n" +
                                          $"Email: {(string.IsNullOrEmpty(email) ? "не указан" : email)}";

                    MessageBox.Show(successMessage, "Успешная регистрация",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    ClearFields();
                }
                else
                {
                    MessageBox.Show("Ошибка при регистрации пользователя в БД!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка регистрации: {ex.Message}\n\n{ex.StackTrace}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ===== ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ВАЛИДАЦИИ =====

        // Валидация телефона
        private bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            // Удаляем все нецифровые символы
            string digitsOnly = new string(phone.Where(char.IsDigit).ToArray());

            // Российские форматы: 7XXXXXXXXXX или 8XXXXXXXXXX (11 цифр)
            if (digitsOnly.Length == 11 && (digitsOnly.StartsWith("7") || digitsOnly.StartsWith("8")))
                return true;

            // Международный формат: +7XXXXXXXXXX
            if (phone.StartsWith("+7") && phone.Length == 12 && phone.Substring(1).All(char.IsDigit))
                return true;

            return false;
        }

        // Нормализация телефона (приводим к формату +7XXXXXXXXXX)
        private string NormalizePhone(string phone)
        {
            string digitsOnly = new string(phone.Where(char.IsDigit).ToArray());

            if (digitsOnly.Length == 11)
            {
                if (digitsOnly.StartsWith("8"))
                    digitsOnly = "7" + digitsOnly.Substring(1);

                return "+" + digitsOnly;
            }

            return phone; // Возвращаем как есть, если не удалось нормализовать
        }

        // Валидация email
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Простая проверка формата
                string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        // Получение названия должности по ID
        private string GetPostName(int postId)
        {
            return postId switch
            {
                1 => "Продавец-консультант",
                2 => "Охранник",
                3 => "Курьер",
                4 => "Администратор",
                5 => "Покупатель",
                _ => "Неизвестная должность"
            };
        }

        // Регистрация пользователя в БД
        private bool RegisterUserInDatabase(string login, string password, string lastName,
            string firstName, string patronymic, string phone, string email,
            DateTime birthday, string address, int postId)
        {
            try
            {
                using (var connection = DbService.GetConnection())
                {
                    connection.Open();

                    // SQL запрос для вставки пользователя
                    string query = @"
                        INSERT INTO user 
                        (IdPost, Login, Password, LastName, FirstName, Patronymic, Phone, Email, Birthday, Address) 
                        VALUES (@postId, @login, @password, @lastName, @firstName, @patronymic, @phone, @email, @birthday, @address)";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@postId", postId);
                        command.Parameters.AddWithValue("@login", login);
                        command.Parameters.AddWithValue("@password", password);
                        command.Parameters.AddWithValue("@lastName", lastName);
                        command.Parameters.AddWithValue("@firstName", firstName);
                        command.Parameters.AddWithValue("@patronymic", patronymic ?? "");
                        command.Parameters.AddWithValue("@phone", phone);
                        command.Parameters.AddWithValue("@email", email ?? "");
                        command.Parameters.AddWithValue("@birthday", birthday);
                        command.Parameters.AddWithValue("@address", address ?? "");

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Ошибка MySQL: {ex.Message}\nКод ошибки: {ex.Number}",
                    "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Общая ошибка: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        // Проверка существования логина в БД
        private bool IsLoginExists(string login)
        {
            try
            {
                using (var connection = DbService.GetConnection())
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM user WHERE Login = @login";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@login", login);
                        long count = (long)command.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        // Проверка существования телефона в БД
        private bool IsPhoneExists(string phone)
        {
            try
            {
                using (var connection = DbService.GetConnection())
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM user WHERE Phone = @phone";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@phone", phone);
                        long count = (long)command.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        // Проверка существования email в БД
        private bool IsEmailExists(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                using (var connection = DbService.GetConnection())
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM user WHERE Email = @email";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@email", email);
                        long count = (long)command.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        // Генерация логина (Фамилия + первая буква имени + случайное число)
        private string GenerateLogin(string lastName, string firstName)
        {
            string baseLogin = $"{lastName.ToLower()}{firstName.ToLower().First()}";

            // Добавляем случайное число от 1 до 999
            Random random = new Random();
            int randomNum = random.Next(1, 1000);

            string login = $"{baseLogin}{randomNum}";

            // Проверяем уникальность
            int attempts = 0;
            while (IsLoginExists(login) && attempts < 10)
            {
                randomNum = random.Next(1, 1000);
                login = $"{baseLogin}{randomNum}";
                attempts++;
            }

            return login;
        }

        // Генерация пароля (первые 3 буквы фамилии + последние 4 цифры телефона + !)
        private string GeneratePassword(string lastName, string phone)
        {
            string lastNamePart = lastName.Length >= 3
                ? lastName.Substring(0, 3).ToUpper()
                : lastName.ToUpper();

            // Получаем последние 4 цифры телефона
            string digitsOnly = new string(phone.Where(char.IsDigit).ToArray());
            string phonePart = digitsOnly.Length >= 4
                ? digitsOnly.Substring(digitsOnly.Length - 4)
                : digitsOnly.PadLeft(4, '0');

            return $"{lastNamePart}{phonePart}!";
        }

        private void ClearFields()
        {
            LastName.Text = "";
            FirstName.Text = "";
            Patronymic.Text = "";
            Birthday.Text = "";
            Phone.Text = "";
            Email.Text = "";
            Post.Text = "";
            Address.Text = "";
            Login.Text = "";
            Password.Text = "";

            LastName.Focus();
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
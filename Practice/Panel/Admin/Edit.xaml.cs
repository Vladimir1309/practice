using System;
using System.Linq;
using System.Windows;
using Practice.Models;
using MySql.Data.MySqlClient;

namespace Practice.Panel.Admin
{
    public partial class Edit : Window
    {
        private User _currentUser;

        public Edit()
        {
            InitializeComponent();

            // Изначально показываем только поле UserID и кнопку поиска
            ShowUserIdSection();
            HideEditFields();
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

            // Поле Salary будем использовать для Email
            SalaryLabel.Content = "Email:";
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

        // Поиск пользователя по ID в БД
        private void FindUser()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(UserID.Text))
                {
                    MessageBox.Show("Введите ID пользователя!");
                    return;
                }

                if (!int.TryParse(UserID.Text, out int userId))
                {
                    MessageBox.Show("ID должен быть числом!");
                    return;
                }

                // Ищем пользователя в БД
                var allUsers = DbService.GetAllUsers();
                _currentUser = allUsers.FirstOrDefault(u => u.IdUser == userId);

                if (_currentUser != null)
                {
                    // Заполняем поля данными
                    LastName.Text = _currentUser.LastName;
                    FirstName.Text = _currentUser.FirstName;
                    Patronymic.Text = _currentUser.Patronymic;
                    Birthday.Text = _currentUser.Birthday.ToString("yyyy-MM-dd");
                    Phone.Text = _currentUser.Phone;
                    Salary.Text = _currentUser.Email; // Используем Salary поле для Email
                    Post.Text = _currentUser.IdPost.ToString();

                    // Меняем видимость элементов
                    HideUserIdSection();
                    ShowEditFields();

                    // Фокус на первое поле для редактирования
                    LastName.Focus();
                    LastName.SelectAll();
                }
                else
                {
                    MessageBox.Show($"Пользователь с ID {userId} не найден в базе данных!");
                    UserID.Focus();
                    UserID.SelectAll();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска пользователя: {ex.Message}");
            }
        }

        // Кнопка "Изменить" - сохранение изменений в БД
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
                if (IsPhoneExists(Phone.Text, _currentUser.IdUser))
                {
                    MessageBox.Show("Телефон уже используется другим пользователем!");
                    Phone.Focus();
                    return;
                }

                // Проверка email если указан
                string email = Salary.Text?.Trim();
                if (!string.IsNullOrEmpty(email) && IsEmailExists(email, _currentUser.IdUser))
                {
                    MessageBox.Show("Email уже используется другим пользователем!");
                    Salary.Focus();
                    return;
                }

                // Проверка IdPost
                if (!int.TryParse(Post.Text, out int postId) || postId < 1 || postId > 5)
                {
                    MessageBox.Show("ID должности должен быть числом от 1 до 5!");
                    Post.Focus();
                    return;
                }

                // Обновляем данные пользователя в БД
                if (UpdateUserInDatabase(_currentUser.IdUser, LastName.Text, FirstName.Text,
                    Patronymic.Text, Phone.Text, email, birthday, Post.Text))
                {
                    MessageBox.Show($"Данные пользователя {LastName.Text} {FirstName.Text} успешно обновлены!\nID: {_currentUser.IdUser}");

                    // Сбрасываем форму
                    ResetForm();
                }
                else
                {
                    MessageBox.Show("Ошибка при обновлении данных пользователя!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

        // Обновление пользователя в БД
        private bool UpdateUserInDatabase(int userId, string lastName, string firstName,
            string patronymic, string phone, string email, DateTime birthday, string post)
        {
            try
            {
                using (var connection = DbService.GetConnection())
                {
                    connection.Open();

                    string query = @"
                        UPDATE user 
                        SET LastName = @lastName,
                            FirstName = @firstName,
                            Patronymic = @patronymic,
                            Phone = @phone,
                            Email = @email,
                            Birthday = @birthday,
                            IdPost = @postId
                        WHERE IdUser = @userId";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        command.Parameters.AddWithValue("@lastName", lastName);
                        command.Parameters.AddWithValue("@firstName", firstName);
                        command.Parameters.AddWithValue("@patronymic", patronymic ?? "");
                        command.Parameters.AddWithValue("@phone", phone);
                        command.Parameters.AddWithValue("@email", email ?? "");
                        command.Parameters.AddWithValue("@birthday", birthday);
                        command.Parameters.AddWithValue("@postId", int.Parse(post));

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Ошибка MySQL: {ex.Message}\nКод: {ex.Number}", "Ошибка БД");
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления: {ex.Message}", "Ошибка");
                return false;
            }
        }

        // Проверка существования телефона у других пользователей
        private bool IsPhoneExists(string phone, int excludeUserId)
        {
            try
            {
                using (var connection = DbService.GetConnection())
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM user WHERE Phone = @phone AND IdUser != @excludeUserId";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@phone", phone);
                        command.Parameters.AddWithValue("@excludeUserId", excludeUserId);
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

        // Проверка существования email у других пользователей
        private bool IsEmailExists(string email, int excludeUserId)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                using (var connection = DbService.GetConnection())
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM user WHERE Email = @email AND IdUser != @excludeUserId";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@excludeUserId", excludeUserId);
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
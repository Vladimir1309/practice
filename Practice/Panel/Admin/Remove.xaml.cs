using System;
using System.Linq;
using System.Windows;
using Practice.Models;
using MySql.Data.MySqlClient;

namespace Practice.Panel.Admin
{
    public partial class Remove : Window
    {
        private User _currentUser;

        public Remove()
        {
            InitializeComponent();
        }

        // Кнопка "Найти" (заполняет фамилию по ID из БД)
        private void FindUserButton_Click(object sender, RoutedEventArgs e)
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
                    // Заполняем поле фамилии
                    LastName.Text = _currentUser.LastName;

                    // Показываем информацию о пользователе
                    string postName = GetPostName(_currentUser.IdPost);

                    string info = $"Найден пользователь:\n" +
                                 $"ID: {_currentUser.IdUser}\n" +
                                 $"ФИО: {_currentUser.LastName} {_currentUser.FirstName} {_currentUser.Patronymic}\n" +
                                 $"Телефон: {_currentUser.Phone}\n" +
                                 $"Должность: {postName} (ID: {_currentUser.IdPost})";

                    MessageBox.Show(info, "Пользователь найден",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"Пользователь с ID {userId} не найден в базе данных!");
                    LastName.Text = "";
                    _currentUser = null;
                    UserID.Focus();
                    UserID.SelectAll();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
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

        // Кнопка "Удалить" - удаление из БД
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_currentUser == null)
                {
                    MessageBox.Show("Сначала найдите пользователя по ID!");
                    return;
                }

                // Проверяем, что ID в поле совпадает с найденным пользователем
                if (!int.TryParse(UserID.Text, out int userId) || userId != _currentUser.IdUser)
                {
                    MessageBox.Show("ID не совпадает с найденным пользователем!");
                    return;
                }

                // Нельзя удалить текущего администратора
                if (AuthManager.CurrentUserId == _currentUser.IdUser)
                {
                    MessageBox.Show("Вы не можете удалить себя! Попросите другого администратора.");
                    return;
                }

                // Подтверждение удаления
                string postName = GetPostName(_currentUser.IdPost);

                string message = $"Вы уверены, что хотите удалить пользователя?\n\n" +
                               $"ID: {_currentUser.IdUser}\n" +
                               $"ФИО: {_currentUser.LastName} {_currentUser.FirstName} {_currentUser.Patronymic}\n" +
                               $"Телефон: {_currentUser.Phone}\n" +
                               $"Должность: {postName}";

                var result = MessageBox.Show(message, "Подтверждение удаления",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    // Удаляем пользователя из БД
                    if (DeleteUserFromDatabase(_currentUser.IdUser))
                    {
                        MessageBox.Show($"Пользователь {_currentUser.LastName} {_currentUser.FirstName} успешно удален из базы данных!",
                            "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Очищаем поля
                        ClearFields();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении пользователя из базы данных!",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        // Удаление пользователя из БД
        private bool DeleteUserFromDatabase(int userId)
        {
            try
            {
                using (var connection = DbService.GetConnection())
                {
                    connection.Open();

                    // Проверяем, есть ли у пользователя активные заказы
                    string checkOrdersQuery = "SELECT COUNT(*) FROM orders WHERE IdUserClient = @userId AND IsCompleted = 0";

                    using (var checkCommand = new MySqlCommand(checkOrdersQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@userId", userId);
                        long activeOrders = (long)checkCommand.ExecuteScalar();

                        if (activeOrders > 0)
                        {
                            MessageBox.Show("Нельзя удалить пользователя с активными заказами!", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                            return false;
                        }
                    }

                    // Удаляем пользователя
                    string deleteQuery = "DELETE FROM user WHERE IdUser = @userId";

                    using (var deleteCommand = new MySqlCommand(deleteQuery, connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@userId", userId);
                        int rowsAffected = deleteCommand.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                // Ошибка внешнего ключа - у пользователя есть связанные данные
                if (ex.Number == 1451)
                {
                    MessageBox.Show("Нельзя удалить пользователя, у которого есть история заказов!\n" +
                                  "Сначала удалите все связанные записи.", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                MessageBox.Show($"Ошибка MySQL: {ex.Message}\nКод: {ex.Number}", "Ошибка БД");
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Общая ошибка: {ex.Message}", "Ошибка");
                return false;
            }
        }

        // Кнопка "Отмена"
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }

        // Очистка полей
        private void ClearFields()
        {
            UserID.Text = "";
            LastName.Text = "";
            _currentUser = null;
            UserID.Focus();
        }

        // Навигационные методы
        private void Registration(object sender, RoutedEventArgs e)
        {
            new Register().Show();
            this.Close();
        }

        private void Edit(object sender, RoutedEventArgs e)
        {
            new Edit().Show();
            this.Close();
        }

        private void Removee(object sender, RoutedEventArgs e)
        {
            // Уже в этом окне
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
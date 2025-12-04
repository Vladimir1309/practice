using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Practice.Models;

namespace Practice.Panel.Admin
{
    public partial class Remove : Window
    {
        // Локальная коллекция пользователей
        private static ObservableCollection<User> _localUsers = new ObservableCollection<User>();
        private User _currentUser; // Текущий пользователь для удаления

        static Remove()
        {
            // Инициализируем теми же данными
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

        public Remove()
        {
            InitializeComponent();
        }

        // Кнопка "Найти" (заполняет фамилию по ID)
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

                // Ищем пользователя
                _currentUser = _localUsers.FirstOrDefault(u => u.IdUser == userId);

                if (_currentUser != null)
                {
                    // Заполняем поле фамилии (оно неактивное, но показывает данные)
                    LastName.Text = _currentUser.LastName;

                    // Показываем информацию о пользователе
                    string info = $"Найден пользователь:\n" +
                                 $"ID: {_currentUser.IdUser}\n" +
                                 $"ФИО: {_currentUser.LastName} {_currentUser.FirstName} {_currentUser.Patronymic}\n" +
                                 $"Телефон: {_currentUser.Phone}\n" +
                                 $"Должность: {_currentUser.IdPost}";

                    MessageBox.Show(info, "Пользователь найден",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"Пользователь с ID {userId} не найден!\n" +
                                  $"Доступные ID: {string.Join(", ", _localUsers.Select(u => u.IdUser))}");
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

        // Кнопка "Удалить"
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

                // Подтверждение удаления
                string message = $"Вы уверены, что хотите удалить пользователя?\n\n" +
                               $"ID: {_currentUser.IdUser}\n" +
                               $"ФИО: {_currentUser.LastName} {_currentUser.FirstName} {_currentUser.Patronymic}\n" +
                               $"Телефон: {_currentUser.Phone}\n" +
                               $"Должность: {_currentUser.IdPost}";

                var result = MessageBox.Show(message, "Подтверждение удаления",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    // Удаляем из локальной коллекции
                    _localUsers.Remove(_currentUser);

                    // Также удаляем из коллекции Register если там есть
                    var userInRegister = Register.TempUsers.FirstOrDefault(u => u.IdUser == _currentUser.IdUser);
                    if (userInRegister != null)
                    {
                        Register.TempUsers.Remove(userInRegister);
                    }

                    MessageBox.Show($"Пользователь {_currentUser.LastName} {_currentUser.FirstName} успешно удален!");

                    // Очищаем поля
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
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
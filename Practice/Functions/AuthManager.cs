using Practice.Models;
using System;
using System.Windows;

namespace Practice
{
    public static class AuthManager
    {
        private static DatabaseManager _dbManager = null;
        private static readonly object _lock = new object();

        private static bool _isAuthenticated = false;
        private static User _currentUser = null;

        public static bool IsAuthenticated => _isAuthenticated;
        public static User CurrentUser => _currentUser;

        private static DatabaseManager GetDbManager()
        {
            lock (_lock)
            {
                if (_dbManager == null)
                {
                    Console.WriteLine("Создаем новый DatabaseManager...");
                    _dbManager = new DatabaseManager();
                }
                return _dbManager;
            }
        }

        public static bool Login(string login, string password)
        {
            Console.WriteLine($"\n=== Попытка входа ===");
            Console.WriteLine($"Логин: {login}");
            Console.WriteLine($"Пароль: {new string('*', password.Length)}");

            try
            {
                if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Введите логин и пароль", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                using (var dbManager = GetDbManager())
                {
                    Console.WriteLine("DatabaseManager создан, пробуем аутентификацию...");

                    var user = dbManager.AuthenticateUser(login, password);

                    if (user != null)
                    {
                        _isAuthenticated = true;
                        _currentUser = user;

                        string role = user.Post?.Role?.RoleName ?? "Не определена";
                        Console.WriteLine($"=== УСПЕШНЫЙ ВХОД ===");
                        Console.WriteLine($"Пользователь: {user.Login}");
                        Console.WriteLine($"Роль: {role}");
                        Console.WriteLine($"ID: {user.IdUser}");
                        Console.WriteLine("===================\n");

                        return true;
                    }
                    else
                    {
                        Console.WriteLine("=== НЕУДАЧНАЯ АУТЕНТИФИКАЦИЯ ===");
                        MessageBox.Show("Неверный логин или пароль", "Ошибка авторизации",
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== ОШИБКА ПРИ ВХОДЕ ===");
                Console.WriteLine($"Сообщение: {ex.Message}");
                Console.WriteLine($"Тип: {ex.GetType().Name}");
                Console.WriteLine($"====================\n");

                MessageBox.Show($"Ошибка при авторизации: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        // Метод для установки тестового пользователя
        public static void SetTestUser(User user)
        {
            if (user == null)
            {
                _isAuthenticated = false;
                _currentUser = null;
            }
            else
            {
                _isAuthenticated = true;
                _currentUser = user;
            }
        }

        public static void Logout()
        {
            _isAuthenticated = false;
            _currentUser = null;

            if (_dbManager != null)
            {
                _dbManager.Dispose();
                _dbManager = null;
            }
        }

        public static int CurrentUserId => _currentUser?.IdUser ?? 0;
        public static string CurrentUserLogin => _currentUser?.Login ?? "Гость";
        public static string CurrentUserRole => _currentUser?.Post?.Role?.RoleName ?? "Пользователь";

        public static bool IsAdmin() => CurrentUserRole == "Администратор";
        public static bool IsSales() => CurrentUserRole == "Продавец-консультант";
        public static bool IsDelivery() => CurrentUserRole == "Курьер";
        public static bool IsClient() => CurrentUserRole == "Пользователь" || string.IsNullOrEmpty(CurrentUserRole);
    }
}
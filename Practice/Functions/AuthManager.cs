using Practice.Models;
using Practice.Services;
using System;

namespace Practice
{
    public static class AuthManager
    {
        private static User _currentUser;

        public static User CurrentUser
        {
            get => _currentUser;
            private set => _currentUser = value;
        }

        public static bool IsAuthenticated => _currentUser != null;

        public static int CurrentUserId => _currentUser?.IdUser ?? 0;

        public static string CurrentUserLogin => _currentUser?.Login ?? "";

        public static string CurrentUserAddress => _currentUser?.Address ?? "";

        // Добавляем недостающие свойства
        public static string CurrentUserRole => _currentUser?.Post?.Role?.RoleName ?? "";

        public static bool IsAdmin => CurrentUserRole == "Администратор";

        public static bool IsSales => CurrentUserRole == "Продавец-консультант";

        public static bool IsDelivery => CurrentUserRole == "Курьер";

        public static bool IsClient => CurrentUserRole == "Пользователь" || CurrentUserRole == "Покупатель";

        // Метод для входа
        public static bool Login(string login, string password)
        {
            var user = DbService.AuthenticateUser(login, password);
            if (user != null)
            {
                _currentUser = user;
                return true;
            }
            return false;
        }

        // Метод для выхода
        public static void Logout()
        {
            _currentUser = null;
        }

        // Метод для обновления данных пользователя
        public static void UpdateUserData(string login, string lastName, string firstName,
                                         string patronymic, string phone, string email, string address)
        {
            if (_currentUser != null)
            {
                _currentUser.Login = login;
                _currentUser.LastName = lastName;
                _currentUser.FirstName = firstName;
                _currentUser.Patronymic = patronymic;
                _currentUser.Phone = phone;
                _currentUser.Email = email;
                _currentUser.Address = address;
            }
        }

        // Метод для установки пользователя (например, после регистрации)
        public static void SetUser(User user)
        {
            _currentUser = user;
        }

        // Метод для тестирования (можно удалить после отладки)
        public static void SetTestUser()
        {
            // Тестовый пользователь для отладки
            _currentUser = new User
            {
                IdUser = 2,
                Login = "testuser",
                Password = "testpass",
                FirstName = "Тест",
                LastName = "Тестов",
                Patronymic = "Тестович",
                Phone = "89112223344",
                Email = "test@test.ru",
                Birthday = new DateTime(1990, 1, 1),
                Address = "ул. Тестовая, д. 1",
                Post = new Post
                {
                    IdPost = 5,
                    PostName = "Покупатель",
                    Role = new Role
                    {
                        RoleName = "Пользователь"
                    }
                }
            };
        }
    }
}
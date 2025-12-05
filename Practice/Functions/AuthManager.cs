using Practice;
using Practice.Models;
using System.Windows;

public static class AuthManager
{
    private static bool _isAuthenticated = false;
    private static User _currentUser = null;

    public static bool IsAuthenticated => _isAuthenticated;
    public static User CurrentUser => _currentUser;

    public static bool Login(string login, string password)
    {
        Console.WriteLine($"\n=== ПОПЫТКА ВХОДА ===");
        Console.WriteLine($"Логин: {login}");

        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
        {
            MessageBox.Show("Введите логин и пароль", "Ошибка",
                          MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        // Сначала тестовые логины
        if (login == "admin" && password == "admin")
        {
            SetTestUser(TestUserFactory.CreateAdmin());
            return true;
        }
        else if (login == "user" && password == "user")
        {
            SetTestUser(TestUserFactory.CreateClient());
            return true;
        }

        try
        {
            // Используем статический вызов
            var user = DbService.AuthenticateUser(login, password);

            if (user != null)
            {
                _isAuthenticated = true;
                _currentUser = user;

                Console.WriteLine($"=== УСПЕШНЫЙ ВХОД ===");
                Console.WriteLine($"Пользователь: {user.Login}");
                Console.WriteLine($"Роль: {user.Role?.RoleName ?? "Не определена"}");
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
            Console.WriteLine($"Установлен тестовый пользователь: {user.Login}, Роль: {user.Role?.RoleName}");
        }
    }

    public static void Logout()
    {
        _isAuthenticated = false;
        _currentUser = null;
    }

    public static int CurrentUserId => _currentUser?.IdUser ?? 0;
    public static string CurrentUserLogin => _currentUser?.Login ?? "Гость";
    public static string CurrentUserRole => _currentUser?.Role?.RoleName ?? "Пользователь";

    public static bool IsAdmin() => CurrentUserRole == "Администратор";
    public static bool IsSales() => CurrentUserRole == "Продавец-консультант";
    public static bool IsDelivery() => CurrentUserRole == "Курьер";
    public static bool IsClient() => CurrentUserRole == "Пользователь" || string.IsNullOrEmpty(CurrentUserRole);
}
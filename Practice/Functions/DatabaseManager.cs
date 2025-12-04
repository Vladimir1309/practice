using Practice.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace Practice
{
    public class DatabaseManager : IDisposable
    {
        private ApplicationContext _context;
        private bool _isConnected = false;

        public static string ConnectionString { get; } =
            "Server=tompsons.beget.tech;Port=3306;Database=tompsons_stud04;Uid=tompsons_stud04;Pwd=1234567Vv;CharSet=utf8;";

        // Альтернативные варианты на случай ошибок
        public static string ConnectionString2 { get; } =
            "Server=tompsons.beget.tech;Port=3306;Database=tompsons_stud04;Uid=tompsons_stud04;Pwd=1234567Vv;CharSet=utf8;SslMode=Preferred";

        public static string ConnectionString3 { get; } =
            "Server=tompsons.beget.tech;Port=3306;Database=tompsons_stud04;Uid=tompsons_stud04;Pwd=1234567Vv;CharSet=utf8;SslMode=Disabled";

        public static string ConnectionString4 { get; } =
            "Server=tompsons.beget.tech;Port=3306;Database=tompsons_stud04;Uid=tompsons_stud04;Pwd=1234567Vv;CharSet=utf8";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public DatabaseManager()
        {
            Console.WriteLine("Инициализация DatabaseManager...");

            try
            {
                // Сначала проверяем подключение простым способом
                if (!TestSimpleConnection())
                {
                    MessageBox.Show("Не удалось подключиться к базе данных.\nПроверьте:\n1. Интернет-соединение\n2. Доступность сервера\n3. Кредиты на аккаунте\n\nИспользуется тестовый режим.",
                                  "Ошибка подключения",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Warning);
                    _isConnected = false;
                    _context = null;
                    return;
                }

                // Если простое подключение работает, создаем контекст
                Console.WriteLine("Создаем контекст БД...");
                _context = new ApplicationContext();
                _isConnected = TestEntityFrameworkConnection();

                if (_isConnected)
                {
                    Console.WriteLine("EF подключение успешно!");
                }
                else
                {
                    Console.WriteLine("EF подключение не удалось");
                    _context = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в конструкторе DatabaseManager: {ex.Message}");
                _context = null;
                _isConnected = false;
            }
        }

        private bool TestSimpleConnection()
        {
            try
            {
                Console.WriteLine("Тестируем простое подключение...");
                using (var connection = GetConnection())
                {
                    connection.Open();

                    // Проверяем доступность таблицы user
                    string query = "SELECT 1 FROM `user` LIMIT 1";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.ExecuteScalar();
                    }

                    connection.Close();
                    Console.WriteLine("Простое подключение успешно!");
                    return true;
                }
            }
            catch (MySqlException ex)
            {
                string error = GetMySqlError(ex);
                Console.WriteLine($"Ошибка MySQL: {error}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Общая ошибка: {ex.Message}");
                return false;
            }
        }

        private bool TestEntityFrameworkConnection()
        {
            try
            {
                if (_context == null) return false;

                // Простой тест через EF
                var exists = _context.Database.Exists();
                Console.WriteLine($"EF Database.Exists() вернул: {exists}");

                if (exists)
                {
                    // Пробуем получить количество пользователей
                    var userCount = _context.Users.Count();
                    Console.WriteLine($"В БД найдено пользователей: {userCount}");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка тестирования EF: {ex.Message}");
                return false;
            }
        }

        private string GetMySqlError(MySqlException ex)
        {
            switch (ex.Number)
            {
                case 0: return "Не удалось подключиться к серверу";
                case 1042: return "Не удается подключиться к хосту MySQL";
                case 1045: return "Неверное имя пользователя или пароль";
                case 1049: return "Неизвестная база данных";
                case 2003: return "Can't connect to MySQL server (сервер не доступен)";
                case 2005: return $"Unknown MySQL server host 'tompsons.beget.tech'";
                default: return $"Код ошибки: {ex.Number}, Сообщение: {ex.Message}";
            }
        }

        public User AuthenticateUser(string login, string password)
        {
            try
            {
                if (_context == null || !_isConnected)
                {
                    Console.WriteLine("Контекст БД не инициализирован, используем тестовую авторизацию");
                    return AuthenticateUserOffline(login, password);
                }

                Console.WriteLine($"Попытка авторизации: {login}");

                var user = _context.Users
                    .Include(u => u.Post)
                    .Include(u => u.Post.Role)
                    .FirstOrDefault(u => u.Login == login && u.Password == password);

                if (user != null)
                {
                    Console.WriteLine($"Пользователь найден: {user.Login}, Роль: {user.Post?.Role?.RoleName}");
                }
                else
                {
                    Console.WriteLine($"Пользователь не найден: {login}");
                }

                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в AuthenticateUser: {ex.Message}");
                return AuthenticateUserOffline(login, password);
            }
        }

        private User AuthenticateUserOffline(string login, string password)
        {
            // Тестовые пользователи
            var testUsers = new Dictionary<string, (string pwd, Func<User> create)>
            {
                { "admin", ("admin", () => new User { IdUser = 1, Login = "admin", Post = new Post { Role = new Role { RoleName = "Администратор" } } }) },
                { "user", ("user", () => new User { IdUser = 2, Login = "user", Post = new Post { Role = new Role { RoleName = "Пользователь" } } }) },
                { "sales", ("sales", () => new User { IdUser = 3, Login = "sales", Post = new Post { Role = new Role { RoleName = "Продавец-консультант" } } }) },
                { "delivery", ("delivery", () => new User { IdUser = 4, Login = "delivery", Post = new Post { Role = new Role { RoleName = "Курьер" } } }) }
            };

            if (testUsers.TryGetValue(login, out var userInfo) && userInfo.pwd == password)
            {
                Console.WriteLine($"Тестовый вход: {login}");
                return userInfo.create();
            }

            return null;
        }

        public List<User> GetAllUsers()
        {
            try
            {
                return _context.Users
                    .Include(u => u.Post)
                    .Include(u => u.Post.Role)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки пользователей: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<User>();
            }
        }

        public bool AddUser(User user)
        {
            try
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления пользователя: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool UpdateUser(User user)
        {
            try
            {
                _context.Entry(user).State = EntityState.Modified;
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления пользователя: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool DeleteUser(int userId)
        {
            try
            {
                var user = _context.Users.Find(userId);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления пользователя: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        // =========== ТОВАРЫ ===========
        public List<Product> GetAllProducts()
        {
            try
            {
                return _context.Products
                    .Include(p => p.Category)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки товаров: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<Product>();
            }
        }

        public Product GetProductById(int productId)
        {
            try
            {
                return _context.Products
                    .Include(p => p.Category)
                    .FirstOrDefault(p => p.IdProduct == productId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки товара: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        // =========== КОРЗИНА ===========
        public List<Order_Product> GetUserCart(int userId)
        {
            try
            {
                // Находим текущий незавершенный заказ пользователя
                var currentOrder = _context.Orders
                    .FirstOrDefault(o => o.IdUserClient == userId && !o.IsCompleted);

                if (currentOrder != null)
                {
                    return _context.Order_Products
                        .Include(op => op.Product)
                        .Include(op => op.Product.Category)
                        .Where(op => op.IdOrder == currentOrder.IdOrder)
                        .ToList();
                }

                return new List<Order_Product>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки корзины: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<Order_Product>();
            }
        }

        public List<Order_Product> GetFavourites(int userId)
        {
            try
            {
                return _context.Order_Products
                    .Include(op => op.Product)
                    .Include(op => op.Product.Category)
                    .Where(op => op.IsFavourited && op.Order.IdUserClient == userId)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки избранного: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<Order_Product>();
            }
        }

        public void AddToCart(int userId, int productId, int amount = 1)
        {
            try
            {
                // Находим или создаем текущий заказ
                var currentOrder = _context.Orders
                    .FirstOrDefault(o => o.IdUserClient == userId && !o.IsCompleted);

                if (currentOrder == null)
                {
                    currentOrder = new Order
                    {
                        IdUserClient = userId,
                        Check = 0,
                        Delivery = false, // 0 - самовывоз, 1 - доставка
                        IsCompleted = false,
                        OrderDate = DateTime.Now
                    };
                    _context.Orders.Add(currentOrder);
                    _context.SaveChanges();
                }

                // Проверяем, есть ли уже товар в корзине
                var existingItem = _context.Order_Products
                    .FirstOrDefault(op => op.IdOrder == currentOrder.IdOrder && op.IdProduct == productId);

                if (existingItem != null)
                {
                    existingItem.Amount += amount;
                }
                else
                {
                    var orderProduct = new Order_Product
                    {
                        IdOrder = currentOrder.IdOrder,
                        IdProduct = productId,
                        Amount = amount,
                        IsFavourited = false
                    };
                    _context.Order_Products.Add(orderProduct);
                }

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления в корзину: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void RemoveFromCart(int orderProductId)
        {
            try
            {
                var item = _context.Order_Products.Find(orderProductId);
                if (item != null)
                {
                    _context.Order_Products.Remove(item);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления из корзины: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateCartItemAmount(int orderProductId, int newAmount)
        {
            try
            {
                var item = _context.Order_Products.Find(orderProductId);
                if (item != null && newAmount > 0)
                {
                    item.Amount = newAmount;
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления количества: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ToggleFavourite(int orderProductId)
        {
            try
            {
                var item = _context.Order_Products.Find(orderProductId);
                if (item != null)
                {
                    item.IsFavourited = !item.IsFavourited;
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка изменения избранного: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // =========== ЗАКАЗЫ ===========
        public List<Order> GetUserOrders(int userId)
        {
            try
            {
                return _context.Orders
                    .Include(o => o.Order_Products)
                    .Include(o => o.Order_Products.Select(op => op.Product))
                    .Where(o => o.IdUserClient == userId)
                    .OrderByDescending(o => o.OrderDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заказов: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<Order>();
            }
        }

        public bool CompleteOrder(int orderId)
        {
            try
            {
                var order = _context.Orders.Find(orderId);
                if (order != null)
                {
                    // Рассчитываем сумму заказа
                    decimal total = _context.Order_Products
                        .Where(op => op.IdOrder == orderId)
                        .Sum(op => op.Amount * op.Product.Price);

                    order.Check = total;
                    order.IsCompleted = true;
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка завершения заказа: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        // =========== КАТЕГОРИИ ===========
        public List<Category_Product> GetAllCategories()
        {
            try
            {
                return _context.Category_Products.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки категорий: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<Category_Product>();
            }
        }

        // =========== ПОСТЫ И РОЛИ ===========
        public List<Post> GetAllPosts()
        {
            try
            {
                return _context.Posts
                    .Include(p => p.Role)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки должностей: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<Post>();
            }
        }

        public List<Role> GetAllRoles()
        {
            try
            {
                return _context.Roles.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки ролей: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<Role>();
            }
        }

        // Сохранение изменений
        public void SaveChanges()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Освобождение ресурсов
        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
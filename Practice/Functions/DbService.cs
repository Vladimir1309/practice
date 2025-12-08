using MySql.Data.MySqlClient;
using Practice;
using Practice.Models;
using System.Windows;

public static class DbService
{
    // ОДНА основная строка подключения
    private static string _connectionString = "server=tompsons.beget.tech;database=tompsons_stud04;user=tompsons_stud04;password=1234567Vv;SslMode=Preferred;";

    // Свойства для доступа к строкам подключения
    public static string ConnectionString => _connectionString;
    public static string ConnectionString2 => _connectionString + ";SslMode=Preferred";
    public static string ConnectionString3 => _connectionString + ";SslMode=Disabled";
    public static string ConnectionString4 => "server=tompsons.beget.tech;database=tompsons_stud04;user=tompsons_stud04;password=1234567Vv";

    // Метод для создания и открытия подключения к БД
    public static MySqlConnection GetConnection()
    {

        try
        {
            return new MySqlConnection(_connectionString);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка создания подключения: {ex.Message}");
            throw;
        }
    }

    // Метод для добавления в корзину из избранного
    public static bool AddToCartFromFavourite(int userId, int orderProductId)
    {
        try
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                // 1. Получаем информацию о товаре
                string getQuery = @"SELECT op.IdProduct, op.Amount 
                             FROM order_product op
                             JOIN `orders` o ON op.IdOrder = o.IdOrder
                             WHERE op.IdOrderProduct = @id 
                               AND o.IdUserClient = @userId
                               AND o.IsCompleted = 0";

                int productId = 0;
                int currentAmount = 0;

                using (var getCmd = new MySqlCommand(getQuery, conn))
                {
                    getCmd.Parameters.AddWithValue("@id", orderProductId);
                    getCmd.Parameters.AddWithValue("@userId", userId);

                    using (var reader = getCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            productId = reader.GetInt32("IdProduct");
                            currentAmount = reader.GetInt32("Amount");
                        }
                        else
                        {
                            return false; // Товар не найден
                        }
                    }
                }

                // 2. Устанавливаем количество = 1 (или +1, если уже есть)
                int newAmount = currentAmount == 0 ? 1 : currentAmount + 1;

                string updateQuery = @"UPDATE order_product 
                                SET Amount = @amount, IsFavourited = 1 
                                WHERE IdOrderProduct = @id";

                using (var updateCmd = new MySqlCommand(updateQuery, conn))
                {
                    updateCmd.Parameters.AddWithValue("@amount", newAmount);
                    updateCmd.Parameters.AddWithValue("@id", orderProductId);
                    return updateCmd.ExecuteNonQuery() > 0;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка добавления в корзину: {ex.Message}");
            return false;
        }
    }

    // Метод для очистки записей с Amount = 0
    public static bool CleanEmptyOrderProducts()
    {
        try
        {
            string query = "DELETE FROM order_product WHERE Amount = 0 AND IsFavourited = 0";

            using (var conn = GetConnection())
            {
                conn.Open();

                using (var cmd = new MySqlCommand(query, conn))
                {
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка очистки пустых записей: {ex.Message}");
            return false;
        }
    }

    // Универсальный метод для выполнения SQL-запросов
    public static int ExecuteNonQueryWithParameters(string query, Dictionary<string, object> parameters = null)
    {
        int rowsAffected = 0;
        using (MySqlConnection connection = GetConnection())
        {
            connection.Open(); // ← ДОБАВИТЬ ЭТУ СТРОКУ
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                    }
                }
                rowsAffected = command.ExecuteNonQuery();
            }
        }
        return rowsAffected;
    }

    // Универсальный метод для получения данных
    public static List<T> GetData<T>(string query, Func<MySqlDataReader, T> mapFunction, Dictionary<string, object> parameters = null)
    {
        List<T> data = new List<T>();
        using (MySqlConnection connection = GetConnection())
        {
            connection.Open(); // ← ДОБАВИТЬ ЭТУ СТРОКУ

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                    }
                }

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data.Add(mapFunction(reader));
                    }
                }
            }
        }
        return data;
    }

    // Метод для аутентификации пользователя
    public static User AuthenticateUser(string login, string password)
    {
        try
        {
            Console.WriteLine($"Попытка аутентификации: {login}");

            string query = @"
            SELECT u.*, p.Post as PostName, r.Role as RoleName 
            FROM user u
            LEFT JOIN post p ON u.IdPost = p.IdPost
            LEFT JOIN role r ON p.IdRole = r.IdRole
            WHERE u.Login = @login AND u.Password = @password";

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@login", login);
                    command.Parameters.AddWithValue("@password", password);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                IdUser = reader.GetInt32("IdUser"),
                                Login = reader.GetString("Login"),
                                Password = reader.GetString("Password"),
                                FirstName = reader.GetString("FirstName"),
                                LastName = reader.GetString("LastName"),
                                Patronymic = reader.IsDBNull(reader.GetOrdinal("Patronymic")) ? "" : reader.GetString("Patronymic"),
                                Phone = reader.GetString("Phone"),
                                Email = reader.GetString("Email"),
                                Birthday = reader.GetDateTime("Birthday"),
                                Address = reader.GetString("Address"),
                                IdPost = reader.GetInt32("IdPost"),
                                Post = new Post
                                {
                                    IdPost = reader.GetInt32("IdPost"),
                                    PostName = reader.GetString("PostName"),
                                    Role = new Role
                                    {
                                        RoleName = reader.GetString("RoleName")
                                    }
                                }
                            };
                        }
                        else
                        {
                            Console.WriteLine("Пользователь не найден в БД");
                            return null;
                        }
                    }
                }
            }
        }
        catch (MySqlException ex)
        {
            Console.WriteLine($"Ошибка MySQL при аутентификации: {ex.Message}");
            Console.WriteLine($"Код ошибки: {ex.Number}");
            MessageBox.Show($"Ошибка MySQL: {ex.Message}\nКод: {ex.Number}", "Ошибка БД");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Общая ошибка при аутентификации: {ex.Message}");
            //MessageBox.Show($"Ошибка аутентификации: {ex.Message}", "Ошибка");
            MessageBox.Show(ex.ToString());

            return null;
        }
    }

    // Метод для получения всех доставок
    public static List<Delivery> GetAllDeliveries()
    {
        try
        {
            string query = @"
            SELECT d.*, 
                   uc.LastName as ClientLastName, 
                   uc.FirstName as ClientFirstName,
                   ue.LastName as EmployeeLastName,
                   ue.FirstName as EmployeeFirstName
            FROM delivery d
            LEFT JOIN user uc ON d.IdUserClient = uc.IdUser
            LEFT JOIN user ue ON d.IdUserEmployee = ue.IdUser
            ORDER BY d.IdDelivery DESC";

            return GetData(query, reader => new Delivery
            {
                IdDelivery = reader.GetInt32("IdDelivery"),
                IdUserClient = reader.GetInt32("IdUserClient"),
                IdUserEmployee = reader.GetInt32("IdUserEmployee"),
                IdOrder = reader.GetInt32("IdOrder"),
                Date = reader.GetDateTime("Date"),
                AddressDelivery = reader.GetString("AddressDelivery"),
                IsCompleted = reader.GetBoolean("IsCompleted")
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка получения доставок: {ex.Message}");
            return new List<Delivery>();
        }
    }

    // Метод для получения всех записей учета
    public static List<Accounting> GetAllAccountings()
    {
        try
        {
            string query = @"
            SELECT a.*, p.Name as ProductName, s.Address as StorageAddress
            FROM accounting a
            JOIN product p ON a.IdProduct = p.IdProduct
            JOIN storage s ON a.IdStorage = s.IdStorage
            ORDER BY a.IdAccounting DESC";

            return GetData(query, reader => new Accounting
            {
                IdAccounting = reader.GetInt32("IdAccounting"),
                IdProduct = reader.GetInt32("IdProduct"),
                IdStorage = reader.GetInt32("IdStorage"),
                AmountOfProduct = reader.GetInt32("AmountOfProduct"),
                Product = new Product
                {
                    IdProduct = reader.GetInt32("IdProduct"),
                    Name = reader.GetString("ProductName")
                },
                Storage = new Storage
                {
                    IdStorage = reader.GetInt32("IdStorage"),
                    Address = reader.GetString("StorageAddress")
                }
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка получения записей учета: {ex.Message}");
            return new List<Accounting>();
        }
    }

    // Получение информации о товаре по ID
    public static Product GetProductById(int productId)
    {
        try
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                string query = "SELECT * FROM product WHERE IdProduct = @id";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", productId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Product
                            {
                                IdProduct = reader.GetInt32("IdProduct"),
                                IdCategory = reader.GetInt32("IdCategory"),
                                Name = reader.GetString("Name"),
                                Price = reader.GetDecimal("Price"),
                                ImagePath = reader.IsDBNull(reader.GetOrdinal("ImagePath")) ?
                                    "/materials/2/3/1.png" : reader.GetString("ImagePath")
                            };
                        }
                        return null;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка получения товара: {ex.Message}");
            return null;
        }
    }

    // Получение деталей заказа
    public static List<Order_Product> GetOrderDetails(int orderId)
    {
        try
        {
            string query = @"
            SELECT op.*, p.Name, p.Price, p.ImagePath 
            FROM order_product op
            JOIN product p ON op.IdProduct = p.IdProduct
            WHERE op.IdOrder = @orderId AND op.Amount > 0";

            var parameters = new Dictionary<string, object>
            {
                ["@orderId"] = orderId
            };

            return GetData(query, reader => new Order_Product
            {
                IdOrderProduct = reader.GetInt32("IdOrderProduct"),
                IdOrder = reader.GetInt32("IdOrder"),
                IdProduct = reader.GetInt32("IdProduct"),
                Amount = reader.GetInt32("Amount"),
                IsFavourited = reader.GetBoolean("IsFavourited"),
                Product = new Product
                {
                    IdProduct = reader.GetInt32("IdProduct"),
                    Name = reader.GetString("Name"),
                    Price = reader.GetDecimal("Price"),
                    ImagePath = reader.IsDBNull(reader.GetOrdinal("ImagePath")) ?
                        "/materials/2/3/1.png" : reader.GetString("ImagePath")
                }
            }, parameters);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка получения деталей заказа: {ex.Message}");
            return new List<Order_Product>();
        }
    }

    // Добавить в DbService.cs

    // Метод для получения заказов для доставщика
    public static List<Delivery> GetDeliveriesForEmployee(int employeeId)
    {
        try
        {
            string query = @"
        SELECT d.*, 
               uc.LastName as ClientLastName, 
               uc.FirstName as ClientFirstName,
               uc.Phone as ClientPhone,
               ue.LastName as EmployeeLastName,
               ue.FirstName as EmployeeFirstName,
               o.TotalSum as OrderTotal,
               o.OrderDate as OrderDateOriginal,
               u.Address as ClientAddress
        FROM delivery d
        LEFT JOIN user uc ON d.IdUserClient = uc.IdUser
        LEFT JOIN user ue ON d.IdUserEmployee = ue.IdUser
        LEFT JOIN orders o ON d.IdOrder = o.IdOrder
        LEFT JOIN user u ON d.IdUserClient = u.IdUser
        WHERE d.IdUserEmployee = @employeeId
        ORDER BY d.IsCompleted, d.Date DESC";

            var parameters = new Dictionary<string, object>
            {
                ["@employeeId"] = employeeId
            };

            return GetData(query, reader => new Delivery
            {
                IdDelivery = reader.GetInt32("IdDelivery"),
                IdUserClient = reader.GetInt32("IdUserClient"),
                IdUserEmployee = reader.GetInt32("IdUserEmployee"),
                IdOrder = reader.GetInt32("IdOrder"),
                Date = reader.GetDateTime("Date"),
                AddressDelivery = reader.GetString("AddressDelivery"),
                IsCompleted = reader.GetBoolean("IsCompleted"),
                UserClient = new User
                {
                    IdUser = reader.GetInt32("IdUserClient"),
                    FirstName = reader.GetString("ClientFirstName"),
                    LastName = reader.GetString("ClientLastName"),
                    Phone = reader.GetString("ClientPhone"),
                    Address = reader.IsDBNull(reader.GetOrdinal("ClientAddress")) ?
                        reader.GetString("AddressDelivery") : reader.GetString("ClientAddress")
                },
                Order = new Order
                {
                    IdOrder = reader.GetInt32("IdOrder"),
                    Check = reader.GetDecimal("OrderTotal"),
                    OrderDate = reader.IsDBNull(reader.GetOrdinal("OrderDateOriginal")) ?
                        null : (DateTime?)reader.GetDateTime("OrderDateOriginal")
                }
            }, parameters);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка получения заказов доставки: {ex.Message}");
            return new List<Delivery>();
        }
    }

    // Метод для подтверждения доставки
    public static bool CompleteDelivery(int deliveryId)
    {
        try
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                string query = "UPDATE delivery SET IsCompleted = 1 WHERE IdDelivery = @deliveryId";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@deliveryId", deliveryId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка подтверждения доставки: {ex.Message}");
            return false;
        }
    }

    // Метод для получения информации о конкретной доставке
    public static Delivery GetDeliveryById(int deliveryId)
    {
        try
        {
            string query = @"
        SELECT d.*, 
               uc.LastName as ClientLastName, 
               uc.FirstName as ClientFirstName,
               uc.Phone as ClientPhone,
               uc.Email as ClientEmail,
               ue.LastName as EmployeeLastName,
               ue.FirstName as EmployeeFirstName,
               o.TotalSum as OrderTotal,
               o.OrderDate as OrderDateOriginal,
               u.Address as ClientAddress
        FROM delivery d
        LEFT JOIN user uc ON d.IdUserClient = uc.IdUser
        LEFT JOIN user ue ON d.IdUserEmployee = ue.IdUser
        LEFT JOIN orders o ON d.IdOrder = o.IdOrder
        LEFT JOIN user u ON d.IdUserClient = u.IdUser
        WHERE d.IdDelivery = @deliveryId";

            var parameters = new Dictionary<string, object>
            {
                ["@deliveryId"] = deliveryId
            };

            var deliveries = GetData(query, reader => new Delivery
            {
                IdDelivery = reader.GetInt32("IdDelivery"),
                IdUserClient = reader.GetInt32("IdUserClient"),
                IdUserEmployee = reader.GetInt32("IdUserEmployee"),
                IdOrder = reader.GetInt32("IdOrder"),
                Date = reader.GetDateTime("Date"),
                AddressDelivery = reader.GetString("AddressDelivery"),
                IsCompleted = reader.GetBoolean("IsCompleted"),
                UserClient = new User
                {
                    IdUser = reader.GetInt32("IdUserClient"),
                    FirstName = reader.GetString("ClientFirstName"),
                    LastName = reader.GetString("ClientLastName"),
                    Phone = reader.GetString("ClientPhone"),
                    Email = reader.GetString("ClientEmail"),
                    Address = reader.IsDBNull(reader.GetOrdinal("ClientAddress")) ?
                        reader.GetString("AddressDelivery") : reader.GetString("ClientAddress")
                },
                Order = new Order
                {
                    IdOrder = reader.GetInt32("IdOrder"),
                    Check = reader.GetDecimal("OrderTotal"),
                    OrderDate = reader.IsDBNull(reader.GetOrdinal("OrderDateOriginal")) ?
                        null : (DateTime?)reader.GetDateTime("OrderDateOriginal")
                }
            }, parameters);

            return deliveries.FirstOrDefault();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка получения доставки: {ex.Message}");
            return null;
        }
    }

    // В DbService.cs
    public static User GetUserById(int userId)
    {
        try
        {
            string query = @"
            SELECT u.*, p.Post as PostName, r.Role as RoleName 
            FROM user u
            LEFT JOIN post p ON u.IdPost = p.IdPost
            LEFT JOIN role r ON p.IdRole = r.IdRole
            WHERE u.IdUser = @userId";

            var parameters = new Dictionary<string, object>
            {
                ["@userId"] = userId
            };

            var users = GetData(query, reader => new User
            {
                IdUser = reader.GetInt32("IdUser"),
                Login = reader.GetString("Login"),
                Password = reader.GetString("Password"),
                FirstName = reader.GetString("FirstName"),
                LastName = reader.GetString("LastName"),
                Patronymic = reader.IsDBNull(reader.GetOrdinal("Patronymic")) ? "" : reader.GetString("Patronymic"),
                Phone = reader.GetString("Phone"),
                Email = reader.GetString("Email"),
                Birthday = reader.GetDateTime("Birthday"),
                Address = reader.GetString("Address"),
                IdPost = reader.GetInt32("IdPost"),
                Post = new Post
                {
                    IdPost = reader.GetInt32("IdPost"),
                    PostName = reader.GetString("PostName"),
                    Role = new Role
                    {
                        RoleName = reader.GetString("RoleName")
                    }
                }
            }, parameters);

            return users.FirstOrDefault();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка получения пользователя: {ex.Message}");
            return null;
        }
    }

    // Метод для оформления продажи
    public static bool ProcessSale(int clientId, int productId, int quantity, decimal totalPrice)
    {
        try
        {
            // Сначала проверяем, существует ли клиент
            if (!CheckUserExists(clientId))
            {
                MessageBox.Show($"Клиент с ID {clientId} не найден!", "Ошибка");
                return false;
            }

            using (var conn = GetConnection())
            {
                conn.Open();

                // 1. Создаем заказ для указанного клиента
                string orderQuery = @"
                INSERT INTO orders (IdUserClient, TotalSum, NeedDelivery, IsCompleted, OrderDate) 
                VALUES (@clientId, @totalPrice, 0, 1, CURDATE());
                SELECT LAST_INSERT_ID();";

                int orderId = 0;

                using (var cmd = new MySqlCommand(orderQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@clientId", clientId);
                    cmd.Parameters.AddWithValue("@totalPrice", totalPrice);
                    var result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        orderId = Convert.ToInt32(result);
                    }
                }

                if (orderId == 0)
                {
                    MessageBox.Show("Не удалось создать заказ");
                    return false;
                }

                // 2. Добавляем товар в заказ
                string productQuery = @"
                INSERT INTO order_product (IdOrder, IdProduct, Amount, IsFavourited) 
                VALUES (@orderId, @productId, @quantity, 0)";

                using (var cmd = new MySqlCommand(productQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@orderId", orderId);
                    cmd.Parameters.AddWithValue("@productId", productId);
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        MessageBox.Show("Не удалось добавить товар в заказ");
                        return false;
                    }
                }

                // 3. Обновляем склад (уменьшаем количество)
                string stockQuery = @"
                UPDATE accounting 
                SET AmountOfProduct = AmountOfProduct - @quantity 
                WHERE IdProduct = @productId";

                using (var cmd = new MySqlCommand(stockQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@productId", productId);
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.ExecuteNonQuery();
                }

                // 4. Получаем имя клиента для сообщения
                string clientName = GetUserName(clientId);

                return true;
            }
        }
        catch (MySqlException ex)
        {
            MessageBox.Show($"Ошибка MySQL: {ex.Message}\nКод ошибки: {ex.Number}");
            return false;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка продажи: {ex.Message}");
            return false;
        }
    }

    // Метод для проверки существования пользователя
    private static bool CheckUserExists(int userId)
    {
        try
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                string query = "SELECT COUNT(*) FROM user WHERE IdUser = @userId";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    long count = (long)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        catch
        {
            return false;
        }
    }

    // Метод для получения имени пользователя
    private static string GetUserName(int userId)
    {
        try
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                string query = "SELECT LastName, FirstName FROM user WHERE IdUser = @userId";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return $"{reader.GetString("LastName")} {reader.GetString("FirstName")}";
                        }
                    }
                }
            }
        }
        catch
        {
            // Игнорируем ошибки
        }

        return $"Клиент ID: {userId}";
    }

    // Метод для получения названия должности по ID
    public static string GetPostName(int postId)
    {
        try
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                string query = "SELECT Post FROM post WHERE IdPost = @postId";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@postId", postId);
                    var result = cmd.ExecuteScalar();
                    return result?.ToString() ?? "Неизвестная должность";
                }
            }
        }
        catch
        {
            return "Неизвестная должность";
        }
    }

    // Метод для получения всех должностей
    public static Dictionary<int, string> GetAllPosts()
    {
        var posts = new Dictionary<int, string>();

        try
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                string query = "SELECT IdPost, Post FROM post ORDER BY IdPost";

                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        posts.Add(reader.GetInt32("IdPost"), reader.GetString("Post"));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка получения должностей: {ex.Message}");
        }

        return posts;
    }

    // Метод для получения всех пользователей
    public static List<User> GetAllUsers()
    {
        try
        {
            string query = @"
            SELECT u.*, p.Post as PostName, r.Role as RoleName 
            FROM user u
            LEFT JOIN post p ON u.IdPost = p.IdPost
            LEFT JOIN role r ON p.IdRole = r.IdRole
            ORDER BY u.LastName, u.FirstName";

            return GetData(query, reader => new User
            {
                IdUser = reader.GetInt32("IdUser"),
                Login = reader.GetString("Login"),
                Password = reader.GetString("Password"),
                FirstName = reader.GetString("FirstName"),
                LastName = reader.GetString("LastName"),
                Patronymic = reader.IsDBNull(reader.GetOrdinal("Patronymic")) ? "" : reader.GetString("Patronymic"),
                Phone = reader.GetString("Phone"),
                Email = reader.GetString("Email"),
                Birthday = reader.GetDateTime("Birthday"),
                Address = reader.GetString("Address"),
                IdPost = reader.GetInt32("IdPost"),
                Post = new Post
                {
                    IdPost = reader.GetInt32("IdPost"),
                    PostName = reader.GetString("PostName"),
                    Role = new Role
                    {
                        RoleName = reader.GetString("RoleName")
                    }
                }
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка получения пользователей: {ex.Message}");
            return new List<User>();
        }
    }

    // Метод для добавления товара в корзину


    public static bool AddToCart(int userId, int productId, int amount = 1)
    {
        try
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                // 1. Получаем или создаем активный заказ
                int orderId = GetActiveOrderId(userId);

                if (orderId == 0)
                {
                    orderId = CreateNewOrder(userId);
                    if (orderId == 0)
                    {
                        MessageBox.Show("Не удалось создать заказ", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                }

                // 2. Проверяем, есть ли уже этот товар в order_product для этого заказа
                string checkQuery = @"
                SELECT IdOrderProduct, Amount, IsFavourited 
                FROM order_product 
                WHERE IdOrder = @orderId AND IdProduct = @productId";

                int orderProductId = 0;
                int existingAmount = 0;
                bool isFavourite = false;

                using (var checkCmd = new MySqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@orderId", orderId);
                    checkCmd.Parameters.AddWithValue("@productId", productId);

                    using (var reader = checkCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            orderProductId = reader.GetInt32("IdOrderProduct");
                            existingAmount = reader.GetInt32("Amount");
                            isFavourite = reader.GetBoolean("IsFavourited");
                        }
                    }
                }

                // 3. Обновляем или добавляем запись
                if (orderProductId > 0)
                {
                    // Если товар уже есть, увеличиваем количество
                    int newAmount = existingAmount + amount;

                    // Если товар был в избранном (Amount = 0), теперь он будет в корзине
                    string updateQuery = @"
                    UPDATE order_product 
                    SET Amount = @amount 
                    WHERE IdOrderProduct = @id";

                    using (var updateCmd = new MySqlCommand(updateQuery, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@amount", newAmount);
                        updateCmd.Parameters.AddWithValue("@id", orderProductId);

                        bool result = updateCmd.ExecuteNonQuery() > 0;

                        // Обновляем общую сумму заказа
                        if (result)
                        {
                            UpdateOrderTotal(orderId);
                        }

                        return result;
                    }
                }
                else
                {
                    // Создаем новую запись для корзины
                    string insertQuery = @"
                    INSERT INTO order_product 
                    (IdOrder, IdProduct, Amount, IsFavourited, IsReturned, ReturnedQuantity) 
                    VALUES (@orderId, @productId, @amount, 0, 0, 0)";

                    using (var insertCmd = new MySqlCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@orderId", orderId);
                        insertCmd.Parameters.AddWithValue("@productId", productId);
                        insertCmd.Parameters.AddWithValue("@amount", amount);

                        bool result = insertCmd.ExecuteNonQuery() > 0;

                        // Обновляем общую сумму заказа
                        if (result)
                        {
                            UpdateOrderTotal(orderId);
                        }

                        return result;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка добавления в корзину: {ex.Message}\n\n{ex.StackTrace}",
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }


    //public static bool AddToCart(int userId, int productId, int amount = 1)
    //{
    //    try
    //    {
    //        // Проверяем, существует ли пользователь
    //        using (var conn = GetConnection())
    //        {
    //            conn.Open();

    //            string checkUserQuery = "SELECT COUNT(*) FROM user WHERE IdUser = @userId";

    //            using (var checkCmd = new MySqlCommand(checkUserQuery, conn))
    //            {
    //                checkCmd.Parameters.AddWithValue("@userId", userId);
    //                long userCount = (long)checkCmd.ExecuteScalar();

    //                if (userCount == 0)
    //                {
    //                    MessageBox.Show($"Ошибка: пользователь с ID {userId} не найден",
    //                                  "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    //                    return false;
    //                }
    //            }
    //        }

    //        // 1. Проверяем есть ли активный заказ
    //        string findOrderQuery = "SELECT IdOrder FROM `orders` WHERE IdUserClient = @userId AND IsCompleted = 0";
    //        int orderId = 0;

    //        using (var connection = GetConnection())
    //        {
    //            connection.Open();

    //            using (var command = new MySqlCommand(findOrderQuery, connection))
    //            {
    //                command.Parameters.AddWithValue("@userId", userId);
    //                var result = command.ExecuteScalar();
    //                if (result != null)
    //                {
    //                    orderId = Convert.ToInt32(result);
    //                }
    //            }

    //            // 2. Если нет активного заказа - создаем новый
    //            if (orderId == 0)
    //            {
    //                // Используем метод CreateNewOrder, который проверяет пользователя
    //                orderId = CreateNewOrder(userId);

    //                if (orderId == 0)
    //                {
    //                    return false; // Не удалось создать заказ
    //                }
    //            }

    //            // 3. Проверяем есть ли товар уже в корзине
    //            string checkItemQuery = "SELECT IdOrderProduct, Amount FROM order_product WHERE IdOrder = @orderId AND IdProduct = @productId";
    //            int existingId = 0;
    //            int existingAmount = 0;

    //            using (var command = new MySqlCommand(checkItemQuery, connection))
    //            {
    //                command.Parameters.AddWithValue("@orderId", orderId);
    //                command.Parameters.AddWithValue("@productId", productId);

    //                using (var reader = command.ExecuteReader())
    //                {
    //                    if (reader.Read())
    //                    {
    //                        existingId = reader.GetInt32("IdOrderProduct");
    //                        existingAmount = reader.GetInt32("Amount");
    //                    }
    //                }
    //            }

    //            // 4. Обновляем или добавляем товар
    //            if (existingId > 0)
    //            {
    //                string updateQuery = "UPDATE order_product SET Amount = @amount WHERE IdOrderProduct = @id";
    //                using (var command = new MySqlCommand(updateQuery, connection))
    //                {
    //                    command.Parameters.AddWithValue("@amount", existingAmount + amount);
    //                    command.Parameters.AddWithValue("@id", existingId);
    //                    command.ExecuteNonQuery();
    //                }
    //            }
    //            else
    //            {
    //                string insertQuery = "INSERT INTO order_product (IdOrder, IdProduct, Amount, IsFavourited) VALUES (@orderId, @productId, @amount, 0)";
    //                using (var command = new MySqlCommand(insertQuery, connection))
    //                {
    //                    command.Parameters.AddWithValue("@orderId", orderId);
    //                    command.Parameters.AddWithValue("@productId", productId);
    //                    command.Parameters.AddWithValue("@amount", amount);
    //                    command.ExecuteNonQuery();
    //                }
    //            }
    //        }
    //        return true;
    //    }
    //    catch (Exception ex)
    //    {
    //        MessageBox.Show($"Ошибка добавления в корзину: {ex.Message}");
    //        return false;
    //    }
    //}

    // Метод для получения только избранных товаров
    public static List<Order_Product> GetFavourites(int userId)
    {
        string query = @"
    SELECT op.*, p.Name, p.Price, p.ImagePath 
    FROM order_product op
    JOIN `orders` o ON op.IdOrder = o.IdOrder
    JOIN product p ON op.IdProduct = p.IdProduct
    WHERE op.IsFavourited = 1 
      AND o.IdUserClient = @userId
      AND o.IsCompleted = 0
    ORDER BY op.IdOrderProduct DESC";  // Убрали условие Amount > 0

        var parameters = new Dictionary<string, object>
        {
            ["@userId"] = userId
        };

        return GetData(query, reader => new Order_Product
        {
            IdOrderProduct = reader.GetInt32("IdOrderProduct"),
            IdOrder = reader.GetInt32("IdOrder"),
            IdProduct = reader.GetInt32("IdProduct"),
            Amount = reader.GetInt32("Amount"),
            IsFavourited = reader.GetBoolean("IsFavourited"),
            Product = new Product
            {
                IdProduct = reader.GetInt32("IdProduct"),
                Name = reader.GetString("Name"),
                Price = reader.GetDecimal("Price"),
                ImagePath = reader.IsDBNull(reader.GetOrdinal("ImagePath")) ? "" : reader.GetString("ImagePath")
            }
        }, parameters);
    }

    // Метод для добавления ТОЛЬКО в избранное (БЕЗ корзины)
    public static bool AddToFavouriteOnly(int userId, int productId)
    {
        try
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                // 1. Ищем активный заказ для избранного (специальный флаг)
                string findOrderQuery = @"SELECT IdOrder FROM `orders` 
                                   WHERE IdUserClient = @userId AND IsCompleted = 0
                                   ORDER BY IdOrder DESC LIMIT 1";

                int orderId = 0;

                using (var findCmd = new MySqlCommand(findOrderQuery, conn))
                {
                    findCmd.Parameters.AddWithValue("@userId", userId);
                    var result = findCmd.ExecuteScalar();
                    if (result != null)
                    {
                        orderId = Convert.ToInt32(result);
                    }
                }

                // 2. Если нет активного заказа - создаем новый
                if (orderId == 0)
                {
                    string createOrderQuery = @"INSERT INTO `orders` 
                                         (IdUserClient, TotalSum, NeedDelivery, IsCompleted, OrderDate) 
                                         VALUES (@userId, 0, 0, 0, NOW());
                                         SELECT LAST_INSERT_ID();";

                    using (var createCmd = new MySqlCommand(createOrderQuery, conn))
                    {
                        createCmd.Parameters.AddWithValue("@userId", userId);
                        orderId = Convert.ToInt32(createCmd.ExecuteScalar());
                    }
                }

                // 3. Проверяем, есть ли уже этот товар в order_product
                string checkQuery = @"SELECT IdOrderProduct, IsFavourited FROM order_product 
                               WHERE IdOrder = @orderId AND IdProduct = @productId";

                int orderProductId = 0;
                bool alreadyFavourite = false;

                using (var checkCmd = new MySqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@orderId", orderId);
                    checkCmd.Parameters.AddWithValue("@productId", productId);

                    using (var reader = checkCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            orderProductId = reader.GetInt32("IdOrderProduct");
                            alreadyFavourite = reader.GetBoolean("IsFavourited");
                        }
                    }
                }

                // 4. Если уже в избранном - ничего не делаем
                if (alreadyFavourite)
                {
                    return true; // Уже в избранном
                }

                // 5. Добавляем или обновляем запись
                if (orderProductId > 0)
                {
                    // Обновляем существующую запись (ставим IsFavourited = 1)
                    string updateQuery = @"UPDATE order_product 
                                    SET IsFavourited = 1 
                                    WHERE IdOrderProduct = @id";

                    using (var updateCmd = new MySqlCommand(updateQuery, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@id", orderProductId);
                        return updateCmd.ExecuteNonQuery() > 0;
                    }
                }
                else
                {
                    // Создаем новую запись ТОЛЬКО для избранного (Amount = 0, IsFavourited = 1)
                    string insertQuery = @"INSERT INTO order_product 
                                    (IdOrder, IdProduct, Amount, IsFavourited) 
                                    VALUES (@orderId, @productId, 0, 1)";

                    using (var insertCmd = new MySqlCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@orderId", orderId);
                        insertCmd.Parameters.AddWithValue("@productId", productId);
                        return insertCmd.ExecuteNonQuery() > 0;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка добавления в избранное: {ex.Message}");
            return false;
        }
    }

    // Метод для переключения состояния "избранное" (должен уже быть в DbService.cs)
    public static bool ToggleFavourite(int orderProductId)
    {
        try
        {
            // Сначала получаем текущее состояние
            string getQuery = "SELECT IsFavourited FROM order_product WHERE IdOrderProduct = @id";
            bool currentState = false;

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var getCommand = new MySqlCommand(getQuery, connection))
                {
                    getCommand.Parameters.AddWithValue("@id", orderProductId);
                    currentState = Convert.ToBoolean(getCommand.ExecuteScalar());
                }

                // Инвертируем состояние
                string updateQuery = "UPDATE order_product SET IsFavourited = @state WHERE IdOrderProduct = @id";
                using (var updateCommand = new MySqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@state", !currentState);
                    updateCommand.Parameters.AddWithValue("@id", orderProductId);
                    return updateCommand.ExecuteNonQuery() > 0;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка изменения избранного: {ex.Message}");
            return false;
        }
    }

    // Метод для обновления количества товара в корзине
    public static bool UpdateCartItemAmount(int orderProductId, int newAmount)
        {
            try
            {
                string query = "UPDATE order_product SET Amount = @amount WHERE IdOrderProduct = @id";
                var parameters = new Dictionary<string, object>
                {
                    ["@amount"] = newAmount,
                    ["@id"] = orderProductId
                };

                return ExecuteNonQueryWithParameters(query, parameters) > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления количества: {ex.Message}");
                return false;
            }
        }

    public static List<Order_Product> GetOrderDetailsWithReturns(int orderId)
    {
        try
        {
            string query = @"
            SELECT op.*, 
                   p.Name, 
                   p.Price, 
                   p.ImagePath,
                   (op.Amount - op.ReturnedQuantity) as RemainingQuantity
            FROM order_product op
            JOIN product p ON op.IdProduct = p.IdProduct
            WHERE op.IdOrder = @orderId";

            var parameters = new Dictionary<string, object>
            {
                ["@orderId"] = orderId
            };

            return GetData(query, reader => new Order_Product
            {
                IdOrderProduct = reader.GetInt32("IdOrderProduct"),
                IdOrder = reader.GetInt32("IdOrder"),
                IdProduct = reader.GetInt32("IdProduct"),
                Amount = reader.GetInt32("Amount"),
                ReturnedQuantity = reader.IsDBNull(reader.GetOrdinal("ReturnedQuantity")) ?
                                 0 : reader.GetInt32("ReturnedQuantity"),
                IsFavourited = reader.GetBoolean("IsFavourited"),
                IsReturned = reader.IsDBNull(reader.GetOrdinal("IsReturned")) ?
                            false : reader.GetBoolean("IsReturned"),
                Product = new Product
                {
                    IdProduct = reader.GetInt32("IdProduct"),
                    Name = reader.GetString("Name"),
                    Price = reader.GetDecimal("Price"),
                    ImagePath = reader.IsDBNull(reader.GetOrdinal("ImagePath")) ?
                        "/materials/2/3/1.png" : reader.GetString("ImagePath")
                }
            }, parameters);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка получения деталей заказа: {ex.Message}");
            return new List<Order_Product>();
        }
    }

    // Метод для получения всех завершенных заказов
    public static List<Order> GetAllOrders()
    {
        try
        {
            string query = @"
            SELECT o.*, 
                   u.LastName, 
                   u.FirstName, 
                   u.Patronymic
            FROM orders o
            JOIN user u ON o.IdUserClient = u.IdUser
            WHERE o.IsCompleted = 1
            ORDER BY o.IdOrder DESC";

            return GetData(query, reader => new Order
            {
                IdOrder = reader.GetInt32("IdOrder"),
                IdUserClient = reader.GetInt32("IdUserClient"),
                Check = reader.GetDecimal("TotalSum"), // Оригинальная сумма заказа
                Delivery = reader.GetBoolean("NeedDelivery"),
                IsCompleted = true,
                OrderDate = reader.GetDateTime("OrderDate"),
                UserClient = new User
                {
                    IdUser = reader.GetInt32("IdUserClient"),
                    LastName = reader.GetString("LastName"),
                    FirstName = reader.GetString("FirstName"),
                    Patronymic = reader.GetString("Patronymic")
                }
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка получения заказов: {ex.Message}");
            return new List<Order>();
        }
    }

    // Метод для получения всех товаров
    public static List<Product> GetAllProducts()
        {
            string query = "SELECT * FROM product ORDER BY IdProduct";

            return GetData(query, reader => new Product
            {
                IdProduct = reader.GetInt32("IdProduct"),
                IdCategory = reader.GetInt32("IdCategory"),
                Name = reader.GetString("Name"),
                Price = reader.GetDecimal("Price"),
                ImagePath = reader.IsDBNull(reader.GetOrdinal("ImagePath")) ? "" : reader.GetString("ImagePath")
            });
        }

    // Метод для удаления из корзины (НЕ удаляет из избранного)
    public static bool RemoveFromCartOnly(int orderProductId)
    {
        try
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                // Проверяем, есть ли товар в избранном
                string checkQuery = "SELECT IsFavourited FROM order_product WHERE IdOrderProduct = @id";
                bool isFavourite = false;

                using (var checkCmd = new MySqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@id", orderProductId);
                    var result = checkCmd.ExecuteScalar();
                    if (result != null)
                    {
                        isFavourite = Convert.ToBoolean(result);
                    }
                }

                if (isFavourite)
                {
                    // Если товар в избранном, просто обнуляем количество
                    string updateQuery = "UPDATE order_product SET Amount = 0 WHERE IdOrderProduct = @id";

                    using (var updateCmd = new MySqlCommand(updateQuery, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@id", orderProductId);
                        return updateCmd.ExecuteNonQuery() > 0;
                    }
                }
                else
                {
                    // Если товар не в избранном, удаляем полностью
                    string deleteQuery = "DELETE FROM order_product WHERE IdOrderProduct = @id";

                    using (var deleteCmd = new MySqlCommand(deleteQuery, conn))
                    {
                        deleteCmd.Parameters.AddWithValue("@id", orderProductId);
                        return deleteCmd.ExecuteNonQuery() > 0;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка удаления из корзины: {ex.Message}");
            return false;
        }
    }

    // Метод для удаления только из избранного
    public static bool RemoveFromFavouriteOnly(int orderProductId)
    {
        try
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                string query = "UPDATE order_product SET IsFavourited = 0 WHERE IdOrderProduct = @id";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", orderProductId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка удаления из избранного: {ex.Message}");
            return false;
        }
    }


    // Метод для получения товаров из корзины пользователя
    // Метод для получения только товаров корзины (для покупки)

    public static List<Order_Product> GetUserCart(int userId)
    {
        try
        {
            Console.WriteLine($"Получение корзины для пользователя {userId}");

            string query = @"
            SELECT op.*, p.Name, p.Price, p.ImagePath 
            FROM order_product op
            JOIN `orders` o ON op.IdOrder = o.IdOrder
            JOIN product p ON op.IdProduct = p.IdProduct
            WHERE o.IdUserClient = @userId
              AND o.IsCompleted = 0
              AND op.Amount > 0
            ORDER BY op.IdOrderProduct DESC";

            var parameters = new Dictionary<string, object>
            {
                ["@userId"] = userId
            };

            var result = GetData(query, reader => new Order_Product
            {
                IdOrderProduct = reader.GetInt32("IdOrderProduct"),
                IdOrder = reader.GetInt32("IdOrder"),
                IdProduct = reader.GetInt32("IdProduct"),
                Amount = reader.GetInt32("Amount"),
                IsFavourited = reader.GetBoolean("IsFavourited"),
                Product = new Product
                {
                    IdProduct = reader.GetInt32("IdProduct"),
                    Name = reader.GetString("Name"),
                    Price = reader.GetDecimal("Price"),
                    ImagePath = reader.IsDBNull(reader.GetOrdinal("ImagePath")) ?
                        "/materials/2/3/1.png" : reader.GetString("ImagePath")
                }
            }, parameters);

            Console.WriteLine($"Найдено {result.Count} товаров в корзине");

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка получения корзины: {ex.Message}");
            return new List<Order_Product>();
        }
    }

    //public static List<Order_Product> GetUserCart(int userId)
    //{
    //    string query = @"
    //SELECT op.*, p.Name, p.Price, p.ImagePath 
    //FROM order_product op
    //JOIN `orders` o ON op.IdOrder = o.IdOrder
    //JOIN product p ON op.IdProduct = p.IdProduct
    //WHERE o.IdUserClient = @userId
    //  AND o.IsCompleted = 0
    //  AND op.Amount > 0  -- Только товары с количеством > 0
    //ORDER BY op.IdOrderProduct DESC";

    //    var parameters = new Dictionary<string, object>
    //    {
    //        ["@userId"] = userId
    //    };

    //    return GetData(query, reader => new Order_Product
    //    {
    //        IdOrderProduct = reader.GetInt32("IdOrderProduct"),
    //        IdOrder = reader.GetInt32("IdOrder"),
    //        IdProduct = reader.GetInt32("IdProduct"),
    //        Amount = reader.GetInt32("Amount"),
    //        IsFavourited = reader.GetBoolean("IsFavourited"),
    //        Product = new Product
    //        {
    //            IdProduct = reader.GetInt32("IdProduct"),
    //            Name = reader.GetString("Name"),
    //            Price = reader.GetDecimal("Price"),
    //            ImagePath = reader.IsDBNull(reader.GetOrdinal("ImagePath")) ? "" : reader.GetString("ImagePath")
    //        }
    //    }, parameters);
    //}

    public static bool DeleteCartItem(int orderProductId)
    {
        try
        {
            string query = "DELETE FROM order_product WHERE IdOrderProduct = @id";
            var parameters = new Dictionary<string, object>
            {
                ["@id"] = orderProductId
            };

            return ExecuteNonQueryWithParameters(query, parameters) > 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Ошибка удаления товара: " + ex.Message);
            return false;
        }
    }


    public static bool CompleteOrder(int orderId)
    {
        try
        {
            string query = "UPDATE `orders` SET IsCompleted = 1 WHERE IdOrder = @id";
            var parameters = new Dictionary<string, object>
            {
                ["@id"] = orderId
            };

            return ExecuteNonQueryWithParameters(query, parameters) > 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Ошибка завершения заказа: " + ex.Message);
            return false;
        }
    }

    // Получить активный заказ пользователя
    // GetActiveOrderId
    public static int GetActiveOrderId(int userId)
    {
        try
        {
            using (var conn = GetConnection())
            {
                conn.Open(); // ← ДОБАВИТЬ ЭТУ СТРОКУ

                string query = @"SELECT IdOrder FROM `orders` 
                           WHERE IdUserClient = @userId 
                           AND IsCompleted = 0 
                           ORDER BY IdOrder DESC LIMIT 1";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    var result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка получения заказа: {ex.Message}");
            return 0;
        }
    }

    // UpdateOrderTotal
    // Обновить общую сумму заказа
    public static void UpdateOrderTotal(int orderId)
    {
        try
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                string sumQuery = @"
                SELECT COALESCE(SUM(p.Price * op.Amount), 0) 
                FROM order_product op
                JOIN product p ON op.IdProduct = p.IdProduct
                WHERE op.IdOrder = @orderId";

                decimal total = 0;

                using (var sumCmd = new MySqlCommand(sumQuery, conn))
                {
                    sumCmd.Parameters.AddWithValue("@orderId", orderId);
                    var result = sumCmd.ExecuteScalar();
                    total = result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                }

                string updateQuery = "UPDATE `orders` SET TotalSum = @total WHERE IdOrder = @orderId";

                using (var updateCmd = new MySqlCommand(updateQuery, conn))
                {
                    updateCmd.Parameters.AddWithValue("@total", total);
                    updateCmd.Parameters.AddWithValue("@orderId", orderId);
                    updateCmd.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка обновления суммы заказа: {ex.Message}");
        }
    }

    // Обновить способ доставки
    public static bool UpdateOrderDelivery(int orderId, bool needDelivery)
    {
        try
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                string query = "UPDATE `orders` SET NeedDelivery = @needDelivery WHERE IdOrder = @orderId";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@needDelivery", needDelivery ? 1 : 0);
                    cmd.Parameters.AddWithValue("@orderId", orderId);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка обновления способа доставки: {ex.Message}");
            return false;
        }
    }

    // Создать запись о доставке
    public static bool CreateDeliveryRecord(int orderId, int userId, string address)
    {
        try
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                // Находим свободного курьера (сотрудника с ролью курьера)
                string findCourierQuery = @"
                SELECT u.IdUser 
                FROM user u
                JOIN post p ON u.IdPost = p.IdPost
                JOIN role r ON p.IdRole = r.IdRole
                WHERE r.Role = 'Курьер' 
                LIMIT 1";

                int courierId = 0;

                using (var courierCmd = new MySqlCommand(findCourierQuery, conn))
                {
                    var result = courierCmd.ExecuteScalar();
                    courierId = result != null ? Convert.ToInt32(result) : 0;
                }

                // Если нет курьеров, используем администратора
                if (courierId == 0)
                {
                    courierId = 1; // ID администратора или другого сотрудника
                }

                // Создаем запись о доставке
                string insertQuery = @"
                INSERT INTO delivery (`IdUserClient`, `IdUserEmployee`, IdOrder, Date, AddressDelivery, IsCompleted)
                VALUES (@clientId, @employeeId, @orderId, CURDATE(), @address, 0)";

                using (var cmd = new MySqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@clientId", userId);
                    cmd.Parameters.AddWithValue("@employeeId", courierId);
                    cmd.Parameters.AddWithValue("@orderId", orderId);
                    cmd.Parameters.AddWithValue("@address", address);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка создания записи о доставке: {ex.Message}");
            return false;
        }
    }

    // Получить адрес пользователя
    public static string GetUserAddress(int userId)
    {
        try
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                string query = "SELECT Address FROM user WHERE IdUser = @userId";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    var result = cmd.ExecuteScalar();

                    return result != null ? result.ToString() : "Адрес не указан";
                }
            }
        }
        catch (Exception ex)
        {
            return "Ошибка получения адреса";
        }
    }


    // CompleteOrderWithPayment
    public static (bool success, decimal totalSum) CompleteOrderWithPayment(int orderId, bool needDelivery = false)
    {
        try
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        decimal totalSum = 0;

                        // 1. Рассчитываем сумму
                        string sumQuery = @"
                        SELECT COALESCE(SUM(p.Price * op.Amount), 0) 
                        FROM order_product op
                        JOIN product p ON op.IdProduct = p.IdProduct
                        WHERE op.IdOrder = @orderId";

                        using (var sumCmd = new MySqlCommand(sumQuery, conn))
                        {
                            sumCmd.Transaction = transaction;
                            sumCmd.Parameters.AddWithValue("@orderId", orderId);
                            var result = sumCmd.ExecuteScalar();
                            totalSum = result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                        }

                        // 2. Если сумма = 0, значит корзина пуста
                        if (totalSum == 0)
                        {
                            transaction.Rollback();
                            return (false, 0);
                        }

                        // 3. Обновляем заказ (добавляем NeedDelivery)
                        string updateOrderQuery = @"
                        UPDATE `orders` 
                        SET IsCompleted = 1, TotalSum = @totalSum, NeedDelivery = @needDelivery 
                        WHERE IdOrder = @orderId";

                        using (var updateCmd = new MySqlCommand(updateOrderQuery, conn))
                        {
                            updateCmd.Transaction = transaction;
                            updateCmd.Parameters.AddWithValue("@orderId", orderId);
                            updateCmd.Parameters.AddWithValue("@totalSum", totalSum);
                            updateCmd.Parameters.AddWithValue("@needDelivery", needDelivery ? 1 : 0);
                            updateCmd.ExecuteNonQuery();
                        }

                        // 4. Обновляем склад
                        string updateStockQuery = @"
                        UPDATE accounting a
                        JOIN order_product op ON a.IdProduct = op.IdProduct
                        SET a.AmountOfProduct = a.AmountOfProduct - op.Amount
                        WHERE op.IdOrder = @orderId";

                        using (var stockCmd = new MySqlCommand(updateStockQuery, conn))
                        {
                            stockCmd.Transaction = transaction;
                            stockCmd.Parameters.AddWithValue("@orderId", orderId);
                            stockCmd.ExecuteNonQuery();
                        }

                        // 5. Фиксируем транзакцию
                        transaction.Commit();
                        return (true, totalSum);
                    }
                    catch (Exception ex)
                    {
                        try { transaction.Rollback(); } catch { }
                        MessageBox.Show($"Ошибка оформления заказа: {ex.Message}");
                        return (false, 0);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка подключения: {ex.Message}");
            return (false, 0);
        }
    }

    // CreateNewOrder
    public static int CreateNewOrder(int userId)
    {
        try
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                // Проверяем, существует ли пользователь
                string checkUserQuery = "SELECT COUNT(*) FROM user WHERE IdUser = @userId";

                using (var checkCmd = new MySqlCommand(checkUserQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@userId", userId);
                    long userCount = (long)checkCmd.ExecuteScalar();

                    if (userCount == 0)
                    {
                        throw new Exception($"Пользователь с ID {userId} не найден в базе данных");
                    }
                }

                // Создаем заказ
                string query = @"
            INSERT INTO `orders` (IdUserClient, TotalSum, NeedDelivery, IsCompleted, OrderDate) 
            VALUES (@userId, 0, 0, 0, NOW());
            SELECT LAST_INSERT_ID();";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
        catch (MySqlException ex) when (ex.Number == 1452) // Ошибка внешнего ключа
        {
            throw new Exception($"Не удалось создать заказ. Пользователь с ID {userId} не существует.");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка создания заказа: {ex.Message}");
            return 0;
        }
    }

    // Метод для обновления данных пользователя
    public static bool UpdateUser(int userId, string login, string password,
                                 string lastName, string firstName, string patronymic,
                                 string phone, string email, DateTime birthday, string address)
    {
        try
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                string query = @"
                UPDATE user 
                SET Login = @login,
                    Password = @password,
                    LastName = @lastName,
                    FirstName = @firstName,
                    Patronymic = @patronymic,
                    Phone = @phone,
                    Email = @email,
                    Birthday = @birthday,
                    Address = @address
                WHERE IdUser = @userId";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@login", login);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.Parameters.AddWithValue("@lastName", lastName);
                    cmd.Parameters.AddWithValue("@firstName", firstName);
                    cmd.Parameters.AddWithValue("@patronymic", patronymic ?? "");
                    cmd.Parameters.AddWithValue("@phone", phone ?? "");
                    cmd.Parameters.AddWithValue("@email", email ?? "");
                    cmd.Parameters.AddWithValue("@birthday", birthday);
                    cmd.Parameters.AddWithValue("@address", address ?? "");

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка обновления пользователя: {ex.Message}");
            return false;
        }
    }

    // В DbService.cs
    public static void CleanOrphanedFavourites()
    {
        try
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                // Удаляем записи, которые в избранном (IsFavourited = 1) но Amount = 0
                string query = @"DELETE FROM order_product 
                           WHERE IsFavourited = 1 AND Amount = 0";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка очистки избранного: {ex.Message}");
        }
    }
}
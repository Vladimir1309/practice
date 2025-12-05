using MySql.Data.MySqlClient;
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
            MySqlConnection connection = new MySqlConnection(_connectionString);
            connection.Open();
            return connection;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка подключения: {ex.Message}");
            throw;
        }
    }

    // Универсальный метод для выполнения SQL-запросов
    public static int ExecuteNonQueryWithParameters(string query, Dictionary<string, object> parameters = null)
        {
            int rowsAffected = 0;
            using (MySqlConnection connection = GetConnection())
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
            return rowsAffected;
        }

        // Универсальный метод для получения данных
        public static List<T> GetData<T>(string query, Func<MySqlDataReader, T> mapFunction, Dictionary<string, object> parameters = null)
        {
            List<T> data = new List<T>();
            using (MySqlConnection connection = GetConnection())
            {
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

    // Метод для получения всех пользователей
    public static List<User> GetAllUsers()
        {
            string query = @"
                SELECT u.*, p.Post as PostName, r.Role as RoleName 
                FROM user u
                LEFT JOIN post p ON u.IdPost = p.IdPost
                LEFT JOIN role r ON p.IdRole = r.IdRole
                ORDER BY u.IdUser";

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

        // Метод для добавления товара в корзину
        public static bool AddToCart(int userId, int productId, int amount = 1)
        {
            try
            {
                // 1. Проверяем есть ли активный заказ
                string findOrderQuery = "SELECT IdOrder FROM `order` WHERE IdUserClient = @userId AND IsCompleted = 0";
                int orderId = 0;

                using (var connection = GetConnection())
                {
                    using (var command = new MySqlCommand(findOrderQuery, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        var result = command.ExecuteScalar();
                        if (result != null)
                        {
                            orderId = Convert.ToInt32(result);
                        }
                    }

                    // 2. Если нет активного заказа - создаем новый
                    if (orderId == 0)
                    {
                        string createOrderQuery = @"
                            INSERT INTO `order` (IdUserClient, Check, Delivery, IsCompleted, OrderDate) 
                            VALUES (@userId, 0, 0, 0, NOW());
                            SELECT LAST_INSERT_ID();";

                        using (var command = new MySqlCommand(createOrderQuery, connection))
                        {
                            command.Parameters.AddWithValue("@userId", userId);
                            orderId = Convert.ToInt32(command.ExecuteScalar());
                        }
                    }

                    // 3. Проверяем есть ли товар уже в корзине
                    string checkItemQuery = "SELECT IdOrderProduct, Amount FROM order_product WHERE IdOrder = @orderId AND IdProduct = @productId";
                    int existingId = 0;
                    int existingAmount = 0;

                    using (var command = new MySqlCommand(checkItemQuery, connection))
                    {
                        command.Parameters.AddWithValue("@orderId", orderId);
                        command.Parameters.AddWithValue("@productId", productId);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                existingId = reader.GetInt32("IdOrderProduct");
                                existingAmount = reader.GetInt32("Amount");
                            }
                        }
                    }

                    // 4. Обновляем или добавляем товар
                    if (existingId > 0)
                    {
                        string updateQuery = "UPDATE order_product SET Amount = @amount WHERE IdOrderProduct = @id";
                        using (var command = new MySqlCommand(updateQuery, connection))
                        {
                            command.Parameters.AddWithValue("@amount", existingAmount + amount);
                            command.Parameters.AddWithValue("@id", existingId);
                            command.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        string insertQuery = "INSERT INTO order_product (IdOrder, IdProduct, Amount, IsFavourited) VALUES (@orderId, @productId, @amount, 0)";
                        using (var command = new MySqlCommand(insertQuery, connection))
                        {
                            command.Parameters.AddWithValue("@orderId", orderId);
                            command.Parameters.AddWithValue("@productId", productId);
                            command.Parameters.AddWithValue("@amount", amount);
                            command.ExecuteNonQuery();
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления в корзину: {ex.Message}");
                return false;
            }
        }

        // Метод для получения избранных товаров пользователя
        public static List<Order_Product> GetFavourites(int userId)
        {
            string query = @"
                SELECT op.*, p.Name, p.Price, p.ImagePath 
                FROM order_product op
                JOIN `order` o ON op.IdOrder = o.IdOrder
                JOIN product p ON op.IdProduct = p.IdProduct
                WHERE op.IsFavourited = 1 AND o.IdUserClient = @userId";

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

        // Метод для переключения состояния "избранное"
        public static bool ToggleFavourite(int orderProductId)
        {
            try
            {
                // Сначала получаем текущее состояние
                string getQuery = "SELECT IsFavourited FROM order_product WHERE IdOrderProduct = @id";
                bool currentState = false;

                using (var connection = GetConnection())
                {
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

        // Метод для получения товара по ID
        public static Product GetProductById(int productId)
        {
            string query = "SELECT * FROM product WHERE IdProduct = @id";
            var parameters = new Dictionary<string, object>
            {
                ["@id"] = productId
            };

            var products = GetData(query, reader => new Product
            {
                IdProduct = reader.GetInt32("IdProduct"),
                IdCategory = reader.GetInt32("IdCategory"),
                Name = reader.GetString("Name"),
                Price = reader.GetDecimal("Price"),
                ImagePath = reader.IsDBNull(reader.GetOrdinal("ImagePath")) ? "" : reader.GetString("ImagePath")
            }, parameters);

            return products.FirstOrDefault();
        }
    }
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace Practice
{
    public partial class TestConnectionWindow : Window
    {
        public TestConnectionWindow()
        {
            InitializeComponent();
            TestAllConnections();
        }

        private void TestAllConnections()
        {
            string result = "=== ТЕСТ ПОДКЛЮЧЕНИЯ К MYSQL ===\n\n";

            // Ваша строка подключения
            string connectionString = "Server=tompsons.beget.tech;Port=3306;Database=tompsons_stud04;Uid=tompsons_stud04;Pwd=1234567Vv;CharSet=utf8";

            result += $"Строка подключения:\n{connectionString}\n\n";

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    result += "Попытка подключения...\n";
                    connection.Open();
                    result += "✓ Подключение установлено!\n\n";

                    // Проверяем версию MySQL
                    using (var cmd = new MySqlCommand("SELECT VERSION()", connection))
                    {
                        result += $"Версия MySQL: {cmd.ExecuteScalar()}\n";
                    }

                    // Проверяем таблицы
                    result += "\nТаблицы в базе данных:\n";
                    using (var cmd = new MySqlCommand("SHOW TABLES", connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result += $"  • {reader[0]}\n";
                        }
                    }

                    // Проверяем конкретно таблицу user
                    connection.Close();
                    connection.Open();
                    result += "\nПользователи в таблице user:\n";
                    using (var cmd = new MySqlCommand("SELECT IdUser, Login, Password, FirstName, LastName FROM `user`", connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result += $"  ID: {reader["IdUser"]}, Логин: {reader["Login"]}, Пароль: {reader["Password"]}, Имя: {reader["FirstName"]} {reader["LastName"]}\n";
                        }
                    }

                    connection.Close();
                }
            }
            catch (MySqlException ex)
            {
                result += $"✗ Ошибка MySQL: {ex.Message}\n";
                result += $"Код ошибки: {ex.Number}\n";
                result += $"Подробности: {ex}\n";
            }
            catch (Exception ex)
            {
                result += $"✗ Общая ошибка: {ex.Message}\n";
            }

            ResultTextBox.Text = result;
        }

        private void RetryButton_Click(object sender, RoutedEventArgs e)
        {
            TestAllConnections();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace Practice
{
    public static class DbHelper
    {
        public static string ConnectionString => DbService.ConnectionString;

        public static bool TestConnection()
        {
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    return connection.State == ConnectionState.Open;
                }
            }
            catch
            {
                return false;
            }
        }

        public static DataTable GetDataTable(string query, params MySqlParameter[] parameters)
        {
            var dataTable = new DataTable();

            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                using (var command = new MySqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    connection.Open();

                    using (var adapter = new MySqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка выполнения запроса: {ex.Message}");
            }

            return dataTable;
        }

        public static int ExecuteNonQuery(string query, params MySqlParameter[] parameters)
        {
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                using (var command = new MySqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка выполнения запроса: {ex.Message}");
                return -1;
            }
        }
    }
}
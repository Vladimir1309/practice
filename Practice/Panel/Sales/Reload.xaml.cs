using System;
using System.Windows;
using Practice.Models;
using MySql.Data.MySqlClient;
using Microsoft.VisualBasic;

namespace Practice.Panel.Delivery
{
    public partial class Reload : Window
    {
        private Product _currentProduct;

        public Reload()
        {
            InitializeComponent();
        }

        private void FindProductButton_Click(object sender, RoutedEventArgs e)
        {
            LoadProductInfo();
        }

        private void LoadProductInfo()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(IdProductText.Text))
                {
                    MessageBox.Show("Введите ID товара!", "Внимание");
                    return;
                }

                if (!int.TryParse(IdProductText.Text, out int productId))
                {
                    MessageBox.Show("ID должен быть числом!", "Ошибка");
                    ClearProductInfo();
                    return;
                }

                // Получаем товар из БД
                _currentProduct = DbService.GetProductById(productId);

                if (_currentProduct != null)
                {
                    ProductNameLabel.Content = _currentProduct.Name;
                    ProductPriceLabel.Content = $"{_currentProduct.Price:N2} ₽";
                    ProductNameLabel.Foreground = System.Windows.Media.Brushes.Black;
                    ProductPriceLabel.Foreground = System.Windows.Media.Brushes.Black;

                    ReturnReasonText.Focus();
                }
                else
                {
                    MessageBox.Show($"Товар с ID {productId} не найден!", "Ошибка");
                    ClearProductInfo();
                    IdProductText.Focus();
                    IdProductText.SelectAll();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
                ClearProductInfo();
            }
        }

        private void ProcessReturnButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_currentProduct == null)
                {
                    MessageBox.Show("Сначала найдите товар по ID!", "Внимание");
                    IdProductText.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(ReturnReasonText.Text))
                {
                    MessageBox.Show("Введите причину возврата!", "Внимание");
                    ReturnReasonText.Focus();
                    return;
                }

                if (ReturnReasonText.Text.Length < 10)
                {
                    MessageBox.Show("Причина возврата должна содержать минимум 10 символов!", "Ошибка");
                    ReturnReasonText.Focus();
                    return;
                }

                // Получаем ID заказа
                string orderIdInput = Interaction.InputBox(
                    "Введите ID заказа для возврата:",
                    "ID заказа", "", -1, -1);

                if (!int.TryParse(orderIdInput, out int orderId) || orderId <= 0)
                {
                    MessageBox.Show("Введите корректный ID заказа!", "Ошибка");
                    return;
                }

                // Проверяем заказ
                if (!CheckOrderExists(orderId))
                {
                    MessageBox.Show($"Заказ с ID {orderId} не найден!", "Ошибка");
                    return;
                }

                // Проверяем товар в заказе
                var orderProduct = GetOrderProduct(orderId, _currentProduct.IdProduct);
                if (orderProduct == null)
                {
                    MessageBox.Show($"Товар {_currentProduct.Name} не найден в заказе {orderId}!", "Ошибка");
                    return;
                }

                // Проверяем, сколько уже было возвращено
                int purchasedQuantity = orderProduct.Amount;
                int alreadyReturned = orderProduct.ReturnedQuantity;
                int availableForReturn = purchasedQuantity - alreadyReturned;

                if (availableForReturn <= 0)
                {
                    MessageBox.Show($"Весь товар {_currentProduct.Name} уже возвращен по заказу {orderId}!", "Ошибка");
                    return;
                }

                // Получаем количество для возврата
                string quantityInput = Interaction.InputBox(
                    $"Введите количество для возврата (можно вернуть до {availableForReturn} шт.):",
                    "Количество", "1", -1, -1);

                if (!int.TryParse(quantityInput, out int quantity) || quantity <= 0)
                {
                    MessageBox.Show("Введите корректное количество!", "Ошибка");
                    return;
                }

                if (quantity > availableForReturn)
                {
                    MessageBox.Show($"Нельзя вернуть больше {availableForReturn} шт.!", "Ошибка");
                    return;
                }

                decimal returnAmount = _currentProduct.Price * quantity;

                // Подтверждение возврата
                string message = $"Оформить возврат?\n\n" +
                               $"Товар: {_currentProduct.Name}\n" +
                               $"Количество: {quantity} шт. (из {purchasedQuantity} купленных)\n" +
                               $"Сумма возврата: {returnAmount:N2} ₽\n" +
                               $"Заказ: #{orderId}\n" +
                               $"Причина: {ReturnReasonText.Text}";

                if (MessageBox.Show(message, "Подтверждение возврата",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    // Регистрируем возврат
                    if (RegisterReturn(orderId, _currentProduct.IdProduct, quantity, ReturnReasonText.Text))
                    {
                        MessageBox.Show($"✅ Возврат успешно оформлен!\n" +
                                      $"Товар: {_currentProduct.Name}\n" +
                                      $"Количество: {quantity} шт.\n" +
                                      $"Сумма: {returnAmount:N2} ₽\n" +
                                      $"Заказ: #{orderId}",
                                      "Возврат оформлен",
                                      MessageBoxButton.OK,
                                      MessageBoxImage.Information);

                        ClearForm();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при оформлении возврата!", "Ошибка");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private Order_Product GetOrderProduct(int orderId, int productId)
        {
            try
            {
                using (var connection = DbService.GetConnection())
                {
                    connection.Open();

                    // Сначала проверяем, какие колонки существуют
                    string checkColumnsQuery = @"
                SELECT 
                    COLUMN_NAME 
                FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_SCHEMA = DATABASE() 
                AND TABLE_NAME = 'order_product' 
                AND COLUMN_NAME IN ('ReturnedQuantity', 'IsReturned')";

                    List<string> existingColumns = new List<string>();

                    using (var checkCmd = new MySqlCommand(checkColumnsQuery, connection))
                    {
                        using (var reader = checkCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                existingColumns.Add(reader.GetString("COLUMN_NAME"));
                            }
                        }
                    }

                    // Формируем запрос в зависимости от существующих колонок
                    string columns = "op.*, p.Name, p.Price";
                    if (existingColumns.Contains("ReturnedQuantity"))
                        columns += ", op.ReturnedQuantity";
                    if (existingColumns.Contains("IsReturned"))
                        columns += ", op.IsReturned";

                    string query = $@"
                SELECT {columns}
                FROM order_product op
                JOIN product p ON op.IdProduct = p.IdProduct
                WHERE op.IdOrder = @orderId AND op.IdProduct = @productId";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@orderId", orderId);
                        command.Parameters.AddWithValue("@productId", productId);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var orderProduct = new Order_Product
                                {
                                    IdOrderProduct = reader.GetInt32("IdOrderProduct"),
                                    IdOrder = reader.GetInt32("IdOrder"),
                                    IdProduct = reader.GetInt32("IdProduct"),
                                    Amount = reader.GetInt32("Amount"),
                                    IsFavourited = reader.GetBoolean("IsFavourited"),
                                    ReturnedQuantity = 0, // значение по умолчанию
                                    IsReturned = false    // значение по умолчанию
                                };

                                // Если колонки существуют, читаем их
                                int returnedQuantityIndex = reader.GetOrdinal("ReturnedQuantity");
                                if (!reader.IsDBNull(returnedQuantityIndex))
                                    orderProduct.ReturnedQuantity = reader.GetInt32(returnedQuantityIndex);

                                int isReturnedIndex = reader.GetOrdinal("IsReturned");
                                if (!reader.IsDBNull(isReturnedIndex))
                                    orderProduct.IsReturned = reader.GetBoolean(isReturnedIndex);

                                return orderProduct;
                            }
                        }
                    }
                }
            }
            catch
            {
                // Игнорируем ошибки
            }
            return null;
        }

        private int GetPurchasedQuantity(int orderId, int productId)
        {
            try
            {
                using (var connection = DbService.GetConnection())
                {
                    connection.Open();

                    string query = @"
                SELECT Amount 
                FROM order_product 
                WHERE IdOrder = @orderId AND IdProduct = @productId";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@orderId", orderId);
                        command.Parameters.AddWithValue("@productId", productId);

                        var result = command.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : 0;
                    }
                }
            }
            catch
            {
                return 0;
            }
        }


        private bool RegisterReturn(int orderId, int productId, int quantity, string reason)
        {
            try
            {
                using (var connection = DbService.GetConnection())
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // 1. Обновляем order_product (добавляем информацию о возврате)
                            string updateQuery = @"
                        UPDATE order_product 
                        SET ReturnedQuantity = ReturnedQuantity + @quantity,
                            IsReturned = CASE 
                                WHEN (ReturnedQuantity + @quantity) >= Amount THEN 1 
                                ELSE 0 
                            END
                        WHERE IdOrder = @orderId AND IdProduct = @productId";

                            using (var cmd = new MySqlCommand(updateQuery, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@orderId", orderId);
                                cmd.Parameters.AddWithValue("@productId", productId);
                                cmd.Parameters.AddWithValue("@quantity", quantity);
                                cmd.ExecuteNonQuery();
                            }

                            // 2. Обновляем склад (возвращаем товар на склад)
                            string stockQuery = @"
                        UPDATE accounting 
                        SET AmountOfProduct = AmountOfProduct + @quantity 
                        WHERE IdProduct = @productId";

                            using (var cmd = new MySqlCommand(stockQuery, connection))
                            {
                                cmd.Transaction = transaction;
                                cmd.Parameters.AddWithValue("@productId", productId);
                                cmd.Parameters.AddWithValue("@quantity", quantity);
                                cmd.ExecuteNonQuery();
                            }

                            // 3. НЕ обновляем сумму заказа в orders!
                            // Сумма заказа остается неизменной, так как это исходная сумма покупки
                            // Возвраты учитываются только в order_product

                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show($"Ошибка: {ex.Message}");
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка БД: {ex.Message}");
                return false;
            }
        }

        //private void RegisterReturnHistory(int orderId, int productId, int quantity, string reason,
        //                                  MySqlConnection connection, MySqlTransaction transaction)
        //{
        //    try
        //    {
        //        // Проверяем, есть ли таблица return_history
        //        string checkTableQuery = @"SELECT 1 FROM information_schema.tables 
        //                               WHERE table_schema = DATABASE() 
        //                               AND table_name = 'return_history' LIMIT 1";

        //        using (var checkCmd = new MySqlCommand(checkTableQuery, connection))
        //        {
        //            checkCmd.Transaction = transaction;
        //            var tableExists = checkCmd.ExecuteScalar();

        //            if (tableExists != null)
        //            {
        //                string insertQuery = @"
        //                    INSERT INTO return_history 
        //                    (IdOrder, IdProduct, Quantity, Reason, ReturnDate) 
        //                    VALUES (@orderId, @productId, @quantity, @reason, NOW())";

        //                using (var command = new MySqlCommand(insertQuery, connection))
        //                {
        //                    command.Transaction = transaction;
        //                    command.Parameters.AddWithValue("@orderId", orderId);
        //                    command.Parameters.AddWithValue("@productId", productId);
        //                    command.Parameters.AddWithValue("@quantity", quantity);
        //                    command.Parameters.AddWithValue("@reason", reason);
        //                    command.ExecuteNonQuery();
        //                }
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        // Если таблицы нет, просто игнорируем
        //    }
        //}

        private decimal GetProductPrice(int productId)
        {
            try
            {
                using (var connection = DbService.GetConnection())
                {
                    connection.Open();

                    string query = "SELECT Price FROM product WHERE IdProduct = @productId";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@productId", productId);
                        var result = command.ExecuteScalar();
                        return result != null ? Convert.ToDecimal(result) : 0;
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        private bool CheckOrderExists(int orderId)
        {
            try
            {
                using (var connection = DbService.GetConnection())
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM orders WHERE IdOrder = @orderId AND IsCompleted = 1";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@orderId", orderId);
                        long count = (long)command.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            IdProductText.Text = "";
            ReturnReasonText.Text = "";
            ClearProductInfo();
            IdProductText.Focus();
        }

        private void ClearProductInfo()
        {
            _currentProduct = null;
            ProductNameLabel.Content = "-";
            ProductPriceLabel.Content = "0 ₽";
            ProductNameLabel.Foreground = System.Windows.Media.Brushes.Gray;
            ProductPriceLabel.Foreground = System.Windows.Media.Brushes.Gray;
        }

        // Методы навигации
        private void Hitt(object sender, RoutedEventArgs e)
        {
            new Hit().Show();
            this.Close();
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            // Уже на этой странице
        }

        private void History(object sender, RoutedEventArgs e)
        {
            new Sales.HistoryOrdersSales().Show();
            this.Close();
        }

        private void MLBD_Exit(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            new Login().Show();
            this.Close();
        }
    }
}
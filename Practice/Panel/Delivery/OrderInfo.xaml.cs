using System;
using System.Windows;
using System.Windows.Media;
using Practice.Models;

namespace Practice.Panel.Delivery
{
    public partial class OrderInfo : Window
    {
        private int _deliveryId;
        private Models.Delivery _delivery;

        public OrderInfo()
        {
            InitializeComponent();
            _deliveryId = 0;
        }

        public OrderInfo(int deliveryId)
        {
            InitializeComponent();
            _deliveryId = deliveryId;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_deliveryId > 0)
            {
                LoadDeliveryInfo();
            }
            else
            {
                // Показываем сообщение о необходимости выбора заказа
                NoOrderMessage.Visibility = Visibility.Visible;
                OrderContent.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadDeliveryInfo()
        {
            try
            {
                // Скрываем сообщение, показываем контент
                NoOrderMessage.Visibility = Visibility.Collapsed;
                OrderContent.Visibility = Visibility.Visible;

                // Получаем информацию о доставке
                _delivery = DbService.GetDeliveryById(_deliveryId);

                if (_delivery == null)
                {
                    MessageBox.Show("Заказ не найден!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    GoToOrders();
                    return;
                }

                // Заполняем информацию
                OrderIdLabel.Content = $"#{_delivery.IdOrder}";
                ClientNameLabel.Content = $"{_delivery.UserClient?.LastName} {_delivery.UserClient?.FirstName}";

                PhoneText.Text = _delivery.UserClient?.Phone ?? "Не указан";
                EmailText.Text = _delivery.UserClient?.Email ?? "Не указан";
                AddressText.Text = _delivery.AddressDelivery;

                // Настраиваем кнопку подтверждения
                if (!_delivery.IsCompleted)
                {
                    CompleteButton.Visibility = Visibility.Visible;
                    CompleteButton.Content = "Подтвердить выполнение";
                    CompleteButton.Background = new SolidColorBrush(Color.FromRgb(183, 0, 0)); // Красный
                }
                else
                {
                    CompleteButton.Visibility = Visibility.Visible;
                    CompleteButton.Content = "Доставка выполнена";
                    CompleteButton.Background = new SolidColorBrush(Color.FromRgb(0, 150, 0)); // Зеленый
                    CompleteButton.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки информации: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CompleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_delivery == null) return;

            var result = MessageBox.Show("Подтвердить выполнение доставки?\nЭто действие нельзя отменить.",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    bool success = DbService.CompleteDelivery(_deliveryId);

                    if (success)
                    {
                        MessageBox.Show("Доставка успешно подтверждена!", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        // Обновляем отображение
                        CompleteButton.Content = "Доставка выполнена";
                        CompleteButton.Background = new SolidColorBrush(Color.FromRgb(0, 150, 0));
                        CompleteButton.IsEnabled = false;
                    }
                    else
                    {
                        MessageBox.Show("Не удалось подтвердить доставку", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void GoToOrders_Click(object sender, RoutedEventArgs e)
        {
            GoToOrders();
        }

        private void GoToOrders()
        {
            var historyWindow = new HistoryOrdersDelivery();
            historyWindow.Show();
            this.Close(); // Закрываем текущее окно
        }

        private void CurrentOrders_Click(object sender, RoutedEventArgs e)
        {
            GoToOrders();
        }

        private void HistoryOrders_Click(object sender, RoutedEventArgs e)
        {
            GoToOrders();
        }

        private void MLBD_Exit(object sender, EventArgs e)
        {
            var login = new Login();
            login.Show();
            this.Close();
        }
    }
}
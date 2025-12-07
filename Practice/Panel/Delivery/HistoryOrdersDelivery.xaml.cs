using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Practice.Models;

namespace Practice.Panel.Delivery
{
    public partial class HistoryOrdersDelivery : Window
    {
        private List<Models.Delivery> _allDeliveries = new List<Models.Delivery>();
        private List<Models.Delivery> _filteredDeliveries = new List<Models.Delivery>();

        public HistoryOrdersDelivery()
        {
            InitializeComponent();
            LoadDeliveries();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDeliveries();
        }

        private void LoadDeliveries()
        {
            try
            {
                // Получаем ID текущего сотрудника
                int currentEmployeeId = AuthManager.CurrentUserId;

                // Загружаем все доставки (можно добавить фильтр по сотруднику)
                _allDeliveries = DbService.GetAllDeliveries();

                // Фильтруем по текущему сотруднику
                _allDeliveries = _allDeliveries.Where(d => d.IdUserEmployee == currentEmployeeId).ToList();

                // Показываем все заказы по умолчанию
                ShowAllOrders();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заказов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowAllOrders()
        {
            _filteredDeliveries = _allDeliveries.OrderByDescending(d => d.Date).ToList();
            UpdateGrid();
        }

        private void ShowCurrentOrders()
        {
            _filteredDeliveries = _allDeliveries
                .Where(d => !d.IsCompleted)
                .OrderByDescending(d => d.Date)
                .ToList();
            UpdateGrid();
        }

        private void ShowCompletedOrders()
        {
            _filteredDeliveries = _allDeliveries
                .Where(d => d.IsCompleted)
                .OrderByDescending(d => d.Date)
                .ToList();
            UpdateGrid();
        }

        private void UpdateGrid()
        {
            // Добавляем вычисляемые свойства для отображения
            var deliveriesForDisplay = _filteredDeliveries.Select(d => new
            {
                d.IdDelivery,
                d.UserClient,
                d.AddressDelivery,
                d.Date,
                d.Order,
                d.IsCompleted,
                StatusText = d.IsCompleted ? "Выполнен" : "В пути",
                StatusColor = d.IsCompleted ? Brushes.LightGreen : Brushes.LightCoral
            }).ToList();

            DeliveriesGrid.ItemsSource = deliveriesForDisplay;
        }

        private void FilterAll_Click(object sender, RoutedEventArgs e)
        {
            ShowAllOrders();
        }

        private void FilterCurrent_Click(object sender, RoutedEventArgs e)
        {
            ShowCurrentOrders();
        }

        private void FilterCompleted_Click(object sender, RoutedEventArgs e)
        {
            ShowCompletedOrders();
        }

        private void ViewDetails_Click(object sender, RoutedEventArgs e)
        {
            if (DeliveriesGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите заказ из списка", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Получаем ID выбранной доставки
            dynamic selectedItem = DeliveriesGrid.SelectedItem;
            int deliveryId = selectedItem.IdDelivery;

            OpenOrderDetails(deliveryId);
        }

        private void DeliveriesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Можно добавить логику при выборе строки
        }

        private void OpenOrderDetails(int deliveryId)
        {
            var orderInfoWindow = new OrderInfo(deliveryId);
            orderInfoWindow.Show();
            this.Close(); // Закрываем текущее окно
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadDeliveries();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Возврат в главное меню доставщика
            var mainWindow = new Delivery_Main();
            mainWindow.Show();
            this.Close();
        }

        private void CurrentOrders_Click(object sender, RoutedEventArgs e)
        {
            var orderinfo = new OrderInfo();
            orderinfo.Show();
            this.Close();
        }

        private void HistoryOrders_Click(object sender, RoutedEventArgs e)
        {
            // Уже на этой странице
        }

        private void MLBD_Exit(object sender, EventArgs e)
        {
            var login = new Login();
            login.Show();
            this.Close();
        }
    }
}
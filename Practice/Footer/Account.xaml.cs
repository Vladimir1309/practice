using Practice.Models;
using Practice.Services;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Practice
{
    public partial class Account : Window
    {
        private User currentUser;
        private User originalUser;

        public Account()
        {
            InitializeComponent();

            if (!AuthManager.IsAuthenticated)
            {
                MessageBox.Show("Вы не авторизованы!", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                Login login = new Login();
                login.Show();
                this.Close();
                return;
            }

            LoadUserData();
        }

        private void LoadUserData()
        {
            try
            {
                // Получаем пользователя через AuthManager
                currentUser = AuthManager.CurrentUser;

                if (currentUser != null)
                {
                    // Сохраняем оригинальные данные
                    originalUser = new User
                    {
                        Login = currentUser.Login,
                        Password = currentUser.Password,
                        LastName = currentUser.LastName,
                        FirstName = currentUser.FirstName,
                        Patronymic = currentUser.Patronymic,
                        Phone = currentUser.Phone,
                        Email = currentUser.Email,
                        Birthday = currentUser.Birthday,
                        Address = currentUser.Address
                    };

                    // Заполняем текстовые поля
                    LoginTextBox.Text = currentUser.Login;
                    PasswordTextBox.Text = currentUser.Password;

                    // Фамилия, Имя, Отчество
                    var lastNameTextBox = FindName("LastNameTextBox") as TextBox;
                    var firstNameTextBox = FindName("FirstNameTextBox") as TextBox;
                    var patronymicTextBox = FindName("PatronymicTextBox") as TextBox;

                    if (lastNameTextBox != null) lastNameTextBox.Text = currentUser.LastName;
                    if (firstNameTextBox != null) firstNameTextBox.Text = currentUser.FirstName;
                    if (patronymicTextBox != null) patronymicTextBox.Text = currentUser.Patronymic;

                    // Телефон, Email, Адрес
                    var phoneTextBox = FindName("PhoneTextBox") as TextBox;
                    var emailTextBox = FindName("EmailTextBox") as TextBox;
                    var addressTextBox = FindName("AddressTextBox") as TextBox;
                    var birthdayTextBox = FindName("BirthdayTextBox") as TextBox;

                    if (phoneTextBox != null) phoneTextBox.Text = currentUser.Phone;
                    if (emailTextBox != null) emailTextBox.Text = currentUser.Email;
                    if (addressTextBox != null) addressTextBox.Text = currentUser.Address;
                    if (birthdayTextBox != null) birthdayTextBox.Text = currentUser.Birthday.ToString("yyyy-MM-dd");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Метод для проверки изменений
        private bool HasChanges()
        {
            if (currentUser == null || originalUser == null)
                return false;

            string currentLastName = (FindName("LastNameTextBox") as TextBox)?.Text ?? "";
            string currentFirstName = (FindName("FirstNameTextBox") as TextBox)?.Text ?? "";
            string currentPatronymic = (FindName("PatronymicTextBox") as TextBox)?.Text ?? "";
            string currentPhone = (FindName("PhoneTextBox") as TextBox)?.Text ?? "";
            string currentEmail = (FindName("EmailTextBox") as TextBox)?.Text ?? "";
            string currentAddress = (FindName("AddressTextBox") as TextBox)?.Text ?? "";

            return currentLastName != originalUser.LastName ||
                   currentFirstName != originalUser.FirstName ||
                   currentPatronymic != originalUser.Patronymic ||
                   currentPhone != originalUser.Phone ||
                   currentEmail != originalUser.Email ||
                   currentAddress != originalUser.Address ||
                   LoginTextBox.Text != originalUser.Login ||
                   PasswordTextBox.Text != originalUser.Password;
        }

        // Кнопка "Редактировать"
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!HasChanges())
                {
                    MessageBox.Show("Нет изменений для сохранения.", "Информация",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Собираем данные из TextBox
                string lastName = (FindName("LastNameTextBox") as TextBox)?.Text ?? "";
                string firstName = (FindName("FirstNameTextBox") as TextBox)?.Text ?? "";
                string patronymic = (FindName("PatronymicTextBox") as TextBox)?.Text ?? "";
                string phone = (FindName("PhoneTextBox") as TextBox)?.Text ?? "";
                string email = (FindName("EmailTextBox") as TextBox)?.Text ?? "";
                string address = (FindName("AddressTextBox") as TextBox)?.Text ?? "";
                string login = LoginTextBox.Text;
                string password = PasswordTextBox.Text;

                // Валидация
                if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Логин и пароль не могут быть пустыми.", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                {
                    MessageBox.Show("Имя и фамилия не могут быть пустыми.", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Обновляем данные в БД
                bool success = DbService.UpdateUser(
                    currentUser.IdUser,
                    login,
                    password,
                    lastName,
                    firstName,
                    patronymic,
                    phone,
                    email,
                    currentUser.Birthday,
                    address
                );

                if (success)
                {
                    // Обновляем данные в AuthManager
                    AuthManager.UpdateUserData(login, lastName, firstName, patronymic, phone, email, address);

                    // Обновляем оригинальные данные
                    originalUser.Login = login;
                    originalUser.Password = password;
                    originalUser.LastName = lastName;
                    originalUser.FirstName = firstName;
                    originalUser.Patronymic = patronymic;
                    originalUser.Phone = phone;
                    originalUser.Email = email;
                    originalUser.Address = address;

                    // Обновляем currentUser
                    currentUser.Login = login;
                    currentUser.Password = password;
                    currentUser.LastName = lastName;
                    currentUser.FirstName = firstName;
                    currentUser.Patronymic = patronymic;
                    currentUser.Phone = phone;
                    currentUser.Email = email;
                    currentUser.Address = address;

                    MessageBox.Show("Данные успешно обновлены!", "Успех",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Ошибка при обновлении данных.", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            AuthManager.Logout();
            MessageBox.Show("Вы вышли из системы", "Выход",
                          MessageBoxButton.OK, MessageBoxImage.Information);

            Main1 main1 = new Main1();
            main1.Show();
            this.Close();
        }

        private void MLBD_GWAN(object sender, EventArgs e)
        {
            Main1 main1 = new Main1();
            main1.Show();
            this.Close();
        }

        private void MLBD_Basket(object sender, EventArgs e)
        {
            Basket basket = new Basket();
            basket.Show();
            this.Close();
        }

        private void MLBD_Favourite(object sender, EventArgs e)
        {
            Favourite favourite = new Favourite();
            favourite.Show();
            this.Close();
        }

        private void MLBD_Account(object sender, EventArgs e)
        {
            if (AuthManager.IsAuthenticated)
            {
                Account account = new Account();
                account.Show();
            }
            else
            {
                Login login = new Login();
                login.Show();
            }
            this.Close();
        }
    }
}
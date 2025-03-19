using System.Linq;
using System.Windows.Controls;
using System.Windows;
using System;

namespace Kursach.Autorizaehtion
{
    public partial class Registration : Page
    {
        private KursovayaEntities context = new KursovayaEntities();

        public Registration()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;
            string email = EmailTextBox.Text;

            // Проверка, что все поля заполнены
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword) || string.IsNullOrWhiteSpace(email))
            {
                RegistrationErrorMessageTextBlock.Text = "Пожалуйста, заполните все поля.";
                return;
            }

            // Проверка, что пароли совпадают
            if (password != confirmPassword)
            {
                RegistrationErrorMessageTextBlock.Text = "Пароли не совпадают.";
                return;
            }

            // Проверка корректности email
            if (!IsValidEmail(email))
            {
                RegistrationErrorMessageTextBlock.Text = "Некорректный адрес электронной почты.";
                return;
            }

            // Проверка, что пользователь с таким логином или email не существует
            var existingUser = context.users.FirstOrDefault(u => u.login == username || u.email == email);
            if (existingUser != null)
            {
                RegistrationErrorMessageTextBlock.Text = "Пользователь с таким логином или электронной почтой уже существует.";
                return;
            }

            try
            {
                // Создание нового пользователя
                var newUser = new users
                {
                    login = username,
                    password_hash = password, // Сохраняем пароль в открытом виде (не рекомендуется)
                    email = email,
                    is_admin = false // Или true, если это администратор
                };

                context.users.Add(newUser);
                context.SaveChanges();

                // Успешная регистрация, переход на страницу входа
                MessageBox.Show("Регистрация прошла успешно! Теперь вы можете войти в систему.");
                NavigationService.Navigate(new Login());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при регистрации: " + ex.Message);
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Переход на страницу входа
            NavigationService.Navigate(new Login());
        }

        private void TextBlock_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Открытие пользовательского соглашения в браузере
            System.Diagnostics.Process.Start("http://your-terms-url.com");
        }

        // Метод для проверки корректности email
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

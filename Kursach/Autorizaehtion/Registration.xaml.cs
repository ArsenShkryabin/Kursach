using System.Linq;
using System.Windows.Controls;
using System.Windows;
using System;
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

            // Проверка, что все поля заполнены
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
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

            // Проверка, что пользователь с таким логином не существует
            var existingUser = context.users.FirstOrDefault(u => u.login == username);
            if (existingUser != null)
            {
                RegistrationErrorMessageTextBlock.Text = "Пользователь с таким логином уже существует.";
                return;
            }

            try
            {
                // Создание нового пользователя
                var newUser = new users
                {
                    login = username,
                    password_hash = password, // Сохраняем пароль в открытом виде (не рекомендуется)
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
            // Реализация перехода к пользовательскому соглашению
            System.Diagnostics.Process.Start("http://your-terms-url.com");
        }
    }
}


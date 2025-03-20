using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Kursach.Class;
using Kursach.User; // Для доступа к строке подключения

namespace Kursach.Autorizaehtion
{
    public class APPUser // Переименован с User на AppUser
    {
        public int UserId { get; set; }
        public string Login { get; set; }
        public bool IsAdmin { get; set; }
    }

    public partial class Login : Page
    {
        // Используем контекст базы данных Entity Framework
        private KursovayaEntities context = new KursovayaEntities();

        public Login()
        {
            InitializeComponent();
        }

        // Метод для обработки нажатия кнопки "Вход"
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            //Убираем проверку на пустые поля
             if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ErrorMessageTextBlock.Text = "Пожалуйста, введите логин и пароль.";
                return;
            }

            try
            {
                // Ищем пользователя по логину
                var user = context.users.FirstOrDefault(u => u.login == username);

                // Если пользователь найден, продолжим
                if (user != null)
                {
                    // Если пользователь заблокирован, информируем об этом
                    if (user.is_blocked == true)
                    {
                        ErrorMessageTextBlock.Text = "Ваш аккаунт заблокирован.";
                        return;
                    }

                    // Проверяем пароль, даже если пользователь может не иметь всех полей заполненными
                    if (VerifyPassword(password, user.password_hash))
                    {
                        // Успешная авторизация
                        App.CurrentUser = new APPUser
                        {
                            UserId = user.user_id,
                            Login = user.login,
                            IsAdmin = user.is_admin ?? false // Присваиваем false, если is_admin равно null
                        };

                        // Проверка, является ли пользователь администратором
                        if (App.CurrentUser.IsAdmin)
                        {
                            ShowAdminChoiceDialog();
                        }
                        else
                        {
                            NavigationService.Navigate(new Profile(App.CurrentUser.UserId));
                        }
                    }
                    else
                    {
                        ErrorMessageTextBlock.Text = "Неверный логин или пароль.";
                    }
                }
                else
                {
                    // Информируем, если пользователь не найден
                    ErrorMessageTextBlock.Text = "Пользователь не найден.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при авторизации: " + ex.Message);
            }
        }

        // Метод для проверки пароля (замените на реальную проверку хэша)
        private bool VerifyPassword(string inputPassword, string storedPasswordHash)
        {
            // Используйте реальную логику проверки хэша паролей
            return inputPassword == storedPasswordHash; // Заглушка для примера
        }

        // Метод для обработки нажатия кнопки "Регистрация"
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Переход на страницу регистрации
            NavigationService.Navigate(new Registration());
        }

        // Метод для отображения диалогового окна с выбором для администратора
        private void ShowAdminChoiceDialog()
        {
            var result = MessageBox.Show("Вы вошли как администратор. Перейти в админ-панель?", "Выбор", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Переход на страницу админ-панели
                NavigationService.Navigate(new AdminPages.AdminProfile());
            }
            else
            {
                // Переход на страницу профиля
                NavigationService.Navigate(new Profile(App.CurrentUser.UserId));
            }
        }
    }

    // Класс для хранения данных текущего пользователя
    public static class App
    {
        public static APPUser CurrentUser { get; set; }
    }
}
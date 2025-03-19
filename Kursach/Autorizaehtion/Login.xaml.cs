﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Kursach.Class;
using Kursach.User; // Для доступа к строке подключения

namespace Kursach.Autorizaehtion
{
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



            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ErrorMessageTextBlock.Text = "Пожалуйста, введите логин и пароль.";
                return;
            }

            try
            {
                var user = context.users.FirstOrDefault(u => u.login == username);

                if (user != null)
                {
                    // Проверяем, заблокирован ли пользователь
                    if (user.is_blocked == true)
                    {
                        ErrorMessageTextBlock.Text = "Ваш аккаунт заблокирован.";
                        return;
                    }

                  

                    // Проверяем пароль (предполагается, что пароль хранится в виде хэша)
                    if (VerifyPassword(password, user.password_hash))
                    {
                        // Успешная авторизация
                        App.CurrentUser = new User
                        {
                            UserId = user.user_id,
                            Login = user.login,
                            IsAdmin = user.is_admin ?? false // Присваиваем false, если is_admin равно null
                        };

                        // Проверка, является ли пользователь администратором
                        if (App.CurrentUser.IsAdmin)
                        {
                            // Открываем диалоговое окно с выбором
                            ShowAdminChoiceDialog();
                        }
                        else
                        {
                            // Переход на страницу профиля для обычного пользователя
                            NavigationService.Navigate(new Profile(user.user_id));
                        }
                    }
                    else
                    {
                        ErrorMessageTextBlock.Text = "Неверный логин или пароль.";
                    }
                }
                else
                {
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
            // В реальном приложении используйте библиотеку для проверки хэша, например, BCrypt
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
        public static User CurrentUser { get; set; }
    }

    // Класс для представления пользователя
    public class User
    {
        public int UserId { get; set; }
        public string Login { get; set; }
        public bool IsAdmin { get; set; }
    }
}

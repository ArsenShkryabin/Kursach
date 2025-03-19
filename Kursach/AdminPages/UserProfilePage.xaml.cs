using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Kursach.AdminPages
{
    public partial class UserProfilePage : Page
    {
        private KursovayaEntities context = new KursovayaEntities();
        private int userId;

        public UserProfilePage(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            LoadUserProfile();
        }

        // Загрузка данных пользователя
        private void LoadUserProfile()
        {
            try
            {
                var user = context.users.FirstOrDefault(u => u.user_id == userId);
                if (user != null)
                {
                    UsernameTextBlock.Text = $"Имя пользователя: {user.login}";
                    EmailTextBlock.Text = $"Email: {user.email}";
                    StatusTextBlock.Text = $"Статус: {(user.is_admin == true ? "Администратор" : "Пользователь")}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке профиля пользователя: " + ex.Message);
            }
        }

        // Обработчик кнопки "Назад"
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}

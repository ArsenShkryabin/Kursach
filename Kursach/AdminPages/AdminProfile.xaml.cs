using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Kursach.AdminPages
{
    public partial class AdminProfile : Page
    {
        private KursovayaEntities context = new KursovayaEntities();
        public ObservableCollection<UserViewModel> Users { get; set; }

        public AdminProfile()
        {
            InitializeComponent();
            Users = new ObservableCollection<UserViewModel>();
            DataContext = this; // Устанавливаем DataContext для привязки данных
            LoadUsers();
        }

        // Загрузка списка пользователей
        private void LoadUsers()
        {
            try
            {
                Users.Clear(); // Очищаем текущие данные
                var usersFromDb = context.users
                    .Select(u => new UserViewModel
                    {
                        UserId = u.user_id,
                        Username = u.login,
                        Email = u.email,
                        Status = u.is_admin == true ? "Администратор" : "Пользователь",
                        IsBlocked = u.is_blocked == true ? "Заблокирован" : "Разблокирован"
                    })
                    .ToList();

                foreach (var user in usersFromDb)
                {
                    Users.Add(user); // Добавляем пользователей в ObservableCollection
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке пользователей: " + ex.Message);
            }
        }

        // Обработчик кнопки "Заблокировать"
        private void BanButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = UsersDataGrid.SelectedItem as UserViewModel;
            if (selectedUser == null)
            {
                ActionMessageTextBlock.Text = "Пожалуйста, выберите пользователя.";
                return;
            }

            try
            {
                var user = context.users.FirstOrDefault(u => u.user_id == selectedUser.UserId);
                if (user != null)
                {
                    user.is_blocked = true; // Блокируем пользователя
                    context.SaveChanges();

                    // Обновляем статус в ObservableCollection
                    selectedUser.IsBlocked = "Заблокирован";
                    ActionMessageTextBlock.Text = $"Пользователь {user.login} заблокирован.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при блокировке пользователя: " + ex.Message);
            }
        }

        // Обработчик кнопки "Разблокировать"
        private void UnbanButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = UsersDataGrid.SelectedItem as UserViewModel;
            if (selectedUser == null)
            {
                ActionMessageTextBlock.Text = "Пожалуйста, выберите пользователя.";
                return;
            }

            try
            {
                var user = context.users.FirstOrDefault(u => u.user_id == selectedUser.UserId);
                if (user != null)
                {
                    user.is_blocked = false; // Разблокируем пользователя
                    context.SaveChanges();

                    // Обновляем статус в ObservableCollection
                    selectedUser.IsBlocked = "Разблокирован";
                    ActionMessageTextBlock.Text = $"Пользователь {user.login} разблокирован.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при разблокировке пользователя: " + ex.Message);
            }
        }

        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    // Модель для отображения данных о пользователе
    public class UserViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string IsBlocked { get; set; }
    }
}

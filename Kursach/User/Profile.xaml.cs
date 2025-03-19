using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Kursach.Autorizaehtion;

namespace Kursach.User
{
    public partial class Profile : Page
    {
        private KursovayaEntities context;
        private int currentUserId; // Предполагаем, что вы храните ID текущего пользователя

        public Profile(int userId)
        {
            InitializeComponent();
            context = new KursovayaEntities();
            currentUserId = userId;
            LoadUserProfile();
        }

        private void LoadUserProfile()
        {
            var user = context.users.FirstOrDefault(u => u.user_id == currentUserId);
            if (user != null)
            {
                UsernameTextBox.Text = user.login;
                EmailTextBox.Text = user.email;
                HeightTextBox.Text = user.height?.ToString() ?? "Не указано";
                WeightTextBox.Text = user.weight?.ToString() ?? "Не указано";
            }
            else
            {
                MessageBox.Show("Пользователь не найден.");
            }
        }

        private void CalculateBMIButton_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(WeightTextBox.Text, out double weight) && double.TryParse(HeightTextBox.Text, out double height))
            {
                double heightInMeters = height / 100; // Преобразуем в метры
                double bmi = weight / (heightInMeters * heightInMeters);
                BMIResultTextBlock.Text = $"Ваш ИМТ: {bmi:F2}";
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите корректные значения веса и роста.");
            }
        }

        private void EditProfile()
        {
            var user = context.users.FirstOrDefault(u => u.user_id == currentUserId);
            if (user != null)
            {
                // Обновляем данные пользователя
                user.email = EmailTextBox.Text;

                if (double.TryParse(HeightTextBox.Text, out double height))
                {
                    
                }

                if (double.TryParse(WeightTextBox.Text, out double weight))
                {
                    
                }

                try
                {
                    context.SaveChanges(); // Сохраняем изменения в базе данных
                    MessageBox.Show("Профиль успешно обновлен.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при обновлении профиля: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Пользователь не найден.");
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            EditProfile();
        }

        private void Logout()
        {
            // Очищаем данные текущего пользователя
            App.CurrentUser = null;

            // Переходим на страницу авторизации
            NavigationService.Navigate(new Login());
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Logout();
        }

        private void CalorieTrackerButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика перехода к подсчету калорий
            NavigationService.Navigate(new UsersCalories());
        }

        private void UserStateButton_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CondtitionUser());
        }

        private void MyHabigsButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика перехода к привычкам
            NavigationService.Navigate(new HabbitPage());
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика перехода к истории
            NavigationService.Navigate(new HistoryUsers());
        }

        private void MyHabitsButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика перехода к привычкам
            NavigationService.Navigate(new HabbitPage());
        }

        private void SleepTrackerButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SleepTrackerPage());
        }

        private void NutritionTrackerButton_Click_1(object sender, RoutedEventArgs e)
        {
            int userId = App.CurrentUser.user_id; // Получаем ID текущего пользователя
            NutrionTracker nutritionTracker = new NutrionTracker(userId);
            NavigationService.Navigate(nutritionTracker);
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Адаптация интерфейса при изменении размеров окна
            if (e.NewSize.Width < 800)
            {
                // Уменьшаем ширину боковой панели
                SidePanel.Width = 150;
            }
            else
            {
                // Восстанавливаем ширину боковой панели
                SidePanel.Width = 200;
            }
        }

        private void EditButton_Click_1(object sender, RoutedEventArgs e)
        {
            EditProfile();
        }

        private void UserStateButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

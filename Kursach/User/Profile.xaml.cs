using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Kursach.Class;
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
                ProfileImage.Source = new BitmapImage(new Uri(user.profile_image)); // Загрузка изображения профиля
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
                user.first_name = FirstNameTextBox.Text;
                user.last_name = LastNameTextBox.Text;
                user.email = EmailTextBox.Text;

                // Явное преобразование типа double в decimal
                if (double.TryParse(HeightTextBox.Text, out double height))
                {
                    user.height = Convert.ToDecimal(height); // Прямое преобразование double в decimal
                }

                if (double.TryParse(WeightTextBox.Text, out double weight))
                {
                    user.weight = Convert.ToDecimal(weight); // Прямое преобразование double в decimal
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
            EditProfile(); // Вызов метода для редактирования профиля
        }

        private void Logout()
        {
            App.CurrentUser = null; // Очищаем данные текущего пользователя
            NavigationService.Navigate(new Login()); // Переходим на страницу авторизации
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Logout(); // Вызов метода выхода из системы
        }

        // Остальная логика для кнопок перехода...
        // Например, для перехода к подсчету калорий, привычкам и так далее

        private void UploadImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Images|*.png;*.jpg;*.jpeg;*.bmp|All Files|*.*",
                Title = "Выберите изображение профиля"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                ProfileImage.Source = new BitmapImage(new Uri(filePath));

                // Сохранение пути к изображению в базе данных
                var user = context.users.FirstOrDefault(u => u.user_id == currentUserId);
                if (user != null)
                {
                    user.profile_image = filePath;
                    context.SaveChanges();
                }
            }
        }

        private void CalorieTrackerButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FitnessRecommendationsButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GoalsButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MentalHealthRecommendationsButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NutritionTrackerButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void Page_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {

        }

        private void SleepTrackerButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UserStateButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MyHabitsButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NutritionRecommendationsButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SleepRecommendationsButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
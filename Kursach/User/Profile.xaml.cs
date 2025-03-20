using System;
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
        private int currentUserId;

        public Profile(int userId)
        {
            InitializeComponent();
            context = new KursovayaEntities();

            // Получаем пользователя по userId
            var user = context.users.FirstOrDefault(u => u.user_id == userId);

            // Проверяем, существует ли пользователь
            if (user == null)
            {
                MessageBox.Show("Пользователь не найден.");
                NavigationService?.Navigate(new Login()); // Переходим на страницу авторизации
                return;
            }

            // Загружаем профиль
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

                // Устанавливаем изображение профиля, если путь к изображению задан
                if (!string.IsNullOrEmpty(user.profile_image))
                {
                    ProfileImage.Source = new BitmapImage(new Uri(user.profile_image));
                }
                else
                {
                    ProfileImage.Source = null; // Или установить изображение по умолчанию
                }
            }
            else
            {
                MessageBox.Show("Пользователь не найден.");
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditButton.Content.ToString() == "Редактировать")
            {
                // Включаем редактирование текстовых полей
                EnableEditing(true);
                EditButton.Content = "Сохранить"; // Изменяем текст кнопки
            }
            else
            {
                // При повторном нажатии сохраняем изменения
                EditProfile(); // Вызов метода для редактирования профиля
                EnableEditing(false); // Отключаем редактирование текстовых полей
                EditButton.Content = "Редактировать"; // Возвращаем текст кнопки
            }
        }

        private void EnableEditing(bool enable)
        {
            UsernameTextBox.IsEnabled = enable;
            EmailTextBox.IsEnabled = enable;
            HeightTextBox.IsEnabled = enable;
            WeightTextBox.IsEnabled = enable;
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
                    user.height = Convert.ToDecimal(height);
                }

                if (double.TryParse(WeightTextBox.Text, out double weight))
                {
                    user.weight = Convert.ToDecimal(weight);
                }

                try
                {
                    context.SaveChanges(); // Сохраняем изменения
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

        // Остальные методы...

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

        private void Logout()
        {
            App.CurrentUser = null; // Очищаем данные текущего пользователя
            NavigationService?.Navigate(new Login()); // Переходим на страницу авторизации
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Logout(); // Вызов метода выхода из системы
        }

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
            NavigationService?.Navigate(new UsersCalories(currentUserId)); // Замените на ваш класс страницы
        }

        private void GoalsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new GoalsPage(currentUserId));
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new HistoryUsers(currentUserId));
        }

        private void NutritionTrackerButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new NutrionTracker(currentUserId));
        }

        private void UserStateButton_Click(object sender, RoutedEventArgs e)
        {
            int userId = App.CurrentUser.user_id; // Получение идентификатора текущего пользователя
            CondtitionUser condtitionUser = new CondtitionUser(userId); // Передаем userId в конструктор
            NavigationService?.Navigate(new CondtitionUser(currentUserId));
        }

        // Дополнительные методы для других кнопок могут быть добавлены аналогичным образом...

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Логика обработки изменения размера страницы (если нужно)
        }

        private void FitnessRecommendationsButton_Click(object sender, RoutedEventArgs e)
        {
            // Реализация для рекомендаций по фитнесу
        }

        private void MentalHealthRecommendationsButton_Click(object sender, RoutedEventArgs e)
        {
            // Реализация для рекомендаций по психическому здоровью
        }

        private void MyHabitsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new HabbitPage(currentUserId));
        }

        private void NutritionRecommendationsButton_Click(object sender, RoutedEventArgs e)
        {
            // Реализация для рекомендаций по питанию
        }

        private void SleepRecommendationsButton_Click(object sender, RoutedEventArgs e)
        {
            // Реализация для рекомендаций по сну
        }

        private void SleepTrackerButton_Click(object sender, RoutedEventArgs e)
        {
            int userId = App.CurrentUser.user_id; // Получение идентификатора текущего пользователя
            SleepTrackerPage sleepTrackerPage = new SleepTrackerPage(userId); // Передаем userId в конструктор
            NavigationService?.Navigate(new SleepTrackerPage(currentUserId));
        }
    }
}
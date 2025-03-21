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
            currentUserId = userId;

            // Проверяем, существует ли пользователь
            if (!IsUserValid(currentUserId))
            {
                MessageBox.Show("Пользователь не найден.");
                NavigationService?.Navigate(new Login()); // Переходим на страницу авторизации
                return;
            }

            // Загружаем профиль
            LoadUserProfile();
        }

        private bool IsUserValid(int userId)
        {
            return context.users.Any(u => u.user_id == userId);
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

                // Устанавливаем имя и фамилию
                FirstNameTextBox.Text = user.first_name; // Добавьте это
                LastNameTextBox.Text = user.last_name; // Добавьте это

                // Устанавливаем изображение профиля
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

        // Остальные методы остаются без изменений...

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            FirstNameTextBox.IsReadOnly = false;
            LastNameTextBox.IsReadOnly = false;
            HeightTextBox.IsReadOnly = false;
            WeightTextBox.IsReadOnly = false;

            // Изменим название кнопки на "Сохранить"
            EditButton.Content = "Сохранить";
            EditButton.Click -= EditButton_Click; // Удаляем текущий обработчик
            EditButton.Click += SaveProfileButton_Click; // Добавляем новый обработчик
        }

        private void SaveProfileButton_Click(object sender, RoutedEventArgs e)
        {
            EditProfile();
            // Сделаем текстовые поля снова не редактируемыми
            FirstNameTextBox.IsReadOnly = true;
            LastNameTextBox.IsReadOnly = true;
            HeightTextBox.IsReadOnly = true;
            WeightTextBox.IsReadOnly = true;

            // Вернем название кнопки на "Редактировать"
            EditButton.Content = "Редактировать";
            EditButton.Click -= SaveProfileButton_Click; // Удаляем текущий обработчик
            EditButton.Click += EditButton_Click; // Возвращаем оригинальный обработчик
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
            CondtitionUser condtitionUser = new CondtitionUser(currentUserId);
            NavigationService?.Navigate(condtitionUser);
        }

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
            SleepTrackerPage sleepTrackerPage = new SleepTrackerPage(currentUserId);
            NavigationService?.Navigate(sleepTrackerPage);
        }
    }
}
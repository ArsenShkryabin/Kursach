using System;
using System.Windows;
using System.Windows.Controls;

namespace Kursach.User
{
    public partial class Profile : Page
    {
        public Profile()
        {
            InitializeComponent();
        }

        // Ваши остальные методы, например, для перехода к другим страницам

        private void CalculateBMIButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем значения роста и веса
            if (double.TryParse(HeightTextBox.Text, out double height) && double.TryParse(WeightTextBox.Text, out double weight))
            {
                if (height > 0)
                {
                    // Преобразуем рост в метры
                    height /= 100;
                    // Вычисляем ИМТ
                    double bmi = weight / (height * height);
                    // Выводим результат на экран
                    BMIResultTextBlock.Text = $"Ваш ИМТ: {bmi:F2}";
                }
                else
                {
                    BMIResultTextBlock.Text = "Рост должен быть больше нуля.";
                }
            }
            else
            {
                BMIResultTextBlock.Text = "Пожалуйста, введите корректные значения для роста и веса.";
            }
        }

        // Обработчик для кнопки "Мои привычки"
        private void MyHabitsButton_Click(object sender, RoutedEventArgs e)
        {
            HabbitPage habitsPage = new HabbitPage();
            this.NavigationService.Navigate(habitsPage);
        }

        private void CalorieTrackerButton_Click(object sender, RoutedEventArgs e)
        {
            PageClass0.frmObj.Navigate(new UsersCalories());
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UserStateButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UserStateButton_Click_1(object sender, RoutedEventArgs e)
        {
            PageClass0.frmObj.Navigate(new CondtitionUser());
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            PageClass0.frmObj.Navigate(new HistoryUsers());
        }
    }
}

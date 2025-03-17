using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Kursach.User
{
    public partial class UsersCalories : Page
    {
        private List<CalorieRecord> calorieRecords; // Список для хранения истории калорий

        public UsersCalories()
        {
            InitializeComponent();
            calorieRecords = new List<CalorieRecord>(); // Инициализация списка
            CaloriesHistoryDataGrid.ItemsSource = calorieRecords; // Задаем источник данных для DataGrid
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(CaloriesConsumedTextBox.Text, out int consumed) &&
                int.TryParse(CaloriesBurnedTextBox.Text, out int burned) &&
                int.TryParse(DailyCalorieGoalTextBox.Text, out int dailyGoal))
            {
                // Сравнение с дневной нормой и формирование рекомендации
                string recommendation = GetCalorieRecommendation(consumed, burned, dailyGoal);

                // Создаем новую запись
                var record = new CalorieRecord
                {
                    Date = DateTime.Now,
                    CaloriesConsumed = consumed,
                    CaloriesBurned = burned,
                    DailyGoal = dailyGoal,
                    Recommendation = recommendation
                };

                // Добавляем запись в историю
                calorieRecords.Add(record);

                // Обновляем таблицу
                CaloriesHistoryDataGrid.ItemsSource = null; // Обновляем источник
                CaloriesHistoryDataGrid.ItemsSource = calorieRecords; // Устанавливаем новый источник

                // Отображаем сообщение
                MessageTextBlock.Text = recommendation;
                ClearInputs(); // Очищаем поля ввода
            }
            else
            {
                MessageTextBlock.Text = "Пожалуйста, введите корректные значения.";
            }
        }

        private string GetCalorieRecommendation(int consumed, int burned, int dailyGoal)
        {
            int netCalories = consumed - burned;
            if (netCalories > dailyGoal)
            {
                return "Вы превысили дневную норму калорий. Рекомендуется уменьшить потребление.";
            }
            else if (netCalories < dailyGoal)
            {
                return "Вы не достигли своей дневной нормы калорий. Подумайте о добавлении дополнительных приемов пищи.";
            }
            else
            {
                return "Вы точно попали в свою дневную норму калорий!";
            }
        }

        private void ClearInputs()
        {
            CaloriesConsumedTextBox.Clear();
            CaloriesBurnedTextBox.Clear();
            DailyCalorieGoalTextBox.Clear();
        }
    }

    public class CalorieRecord
    {
        public DateTime Date { get; set; }
        public int CaloriesConsumed { get; set; }
        public int CaloriesBurned { get; set; }
        public int DailyGoal { get; set; }
        public string Recommendation { get; set; }
    }
}

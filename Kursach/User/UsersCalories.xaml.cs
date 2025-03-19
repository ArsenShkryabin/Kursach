using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Kursach.Class;

namespace Kursach.User
{
    public partial class UsersCalories : Page
    {
        private KursovayaEntities context = new KursovayaEntities();
        private int _userId; // ID текущего пользователя

        public UsersCalories(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadDailyCalorieGoal();
            LoadDataForToday(); // Загрузка данных за сегодняшний день
        }

        // Загрузка дневной нормы калорий
        private void LoadDailyCalorieGoal()
        {
            var user = context.users.FirstOrDefault(u => u.user_id == _userId);
            if (user != null)
            {
                // Проверяем, указан ли вес
                if (user.weight.HasValue)
                {
                    // Пример расчета нормы калорий на основе веса
                    double dailyCalories = Convert.ToDouble(user.weight.Value) * 30;
                    DailyCalorieGoalTextBox.Text = dailyCalories.ToString("F2");
                }
                else
                {
                    // Если вес не указан, устанавливаем значение по умолчанию
                    DailyCalorieGoalTextBox.Text = "0";
                }
            }
        }

        // Загрузка данных за выбранную дату
        private void LoadDataForDate(DateTime selectedDate)
        {
            var nutritionEntries = context.nutrition
       .Where(n => n.user_id == _userId && n.date.HasValue && EntityFunctions.TruncateTime(n.date) == selectedDate.Date)
       .ToList();

            // Сумма всех калорий за день
            double totalCaloriesConsumed = nutritionEntries.Sum(n => n.calories ?? 0);
            TotalCaloriesConsumedTextBox.Text = totalCaloriesConsumed.ToString("F2");

            // Отображение истории калорий
            CaloriesHistoryDataGrid.ItemsSource = nutritionEntries;
        }

        // Загрузка данных за сегодняшний день
        private void LoadDataForToday()
        {
            LoadDataForDate(DateTime.Today);
        }

        // Обработчик выбора даты
        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedDate = DatePicker.SelectedDate;
            if (selectedDate.HasValue)
            {
                LoadDataForDate(selectedDate.Value);
            }
        }

        // Обработчик сохранения шагов
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(StepsTextBox.Text, out int steps))
            {
                double caloriesBurned = CalculateCaloriesBurned(steps);
                CaloriesBurnedTextBox.Text = caloriesBurned.ToString("F2");

                // Рассчет избытка/недостатка калорий
                double totalCaloriesConsumed = double.Parse(TotalCaloriesConsumedTextBox.Text);
                double dailyCalorieGoal = double.Parse(DailyCalorieGoalTextBox.Text);
                double calorieDifference = totalCaloriesConsumed - dailyCalorieGoal - caloriesBurned;

                CalorieDifferenceTextBox.Text = calorieDifference.ToString("F2");

                // Рекомендации
                if (calorieDifference > 0)
                {
                    RecommendationsTextBox.Text = "Вы употребили больше калорий, чем нужно. Уменьшите потребление.";
                }
                else if (calorieDifference < 0)
                {
                    RecommendationsTextBox.Text = "Вы употребили меньше калорий, чем нужно. Увеличьте потребление.";
                }
                else
                {
                    RecommendationsTextBox.Text = "Ваше питание сбалансировано. Продолжайте в том же духе!";
                }
            }
            else
            {
                MessageBox.Show("Введите корректное количество шагов.");
            }
        }

        // Рассчет сожженных калорий на основе шагов
        private double CalculateCaloriesBurned(int steps)
        {
            // Пример: 0.05 калорий на шаг
            return steps * 0.05;
        }
    }
}

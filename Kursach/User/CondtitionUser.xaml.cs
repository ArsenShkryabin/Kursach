using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Kursach.User
{
    public partial class CondtitionUser : Page
    {
        private KursovayaEntities context;

        public CondtitionUser(int userId)
        {
            InitializeComponent();
            context = new KursovayaEntities();
            LoadStateHistory(); // Загружаем историю состояний при инициализации страницы
        }

        // Метод для оценки состояния и формирования рекомендации
        private void EvaluateStateButton_Click(object sender, RoutedEventArgs e)
        {
            string physicalState = ((ComboBoxItem)PhysicalStateComboBox.SelectedItem)?.Content.ToString();
            string mentalState = ((ComboBoxItem)MentalStateComboBox.SelectedItem)?.Content.ToString();

            // Проверка, выбраны ли состояния
            if (string.IsNullOrEmpty(physicalState) || string.IsNullOrEmpty(mentalState))
            {
                MessageTextBlock.Text = "Пожалуйста, выберите физическое и моральное состояние.";
                return;
            }

            // Формируем рекомендацию на основе состояний
            string recommendation = GetRecommendation(physicalState, mentalState);

            // Показываем уведомление с рекомендацией
            MessageBox.Show(recommendation, "Рекомендация", MessageBoxButton.OK, MessageBoxImage.Information);

            // Обновляем текстовое поле с рекомендацией
            MessageTextBlock.Text = recommendation;
        }

        // Метод для сохранения состояния в базу данных
        private void SaveStateButton_Click(object sender, RoutedEventArgs e)
        {
            string physicalState = ((ComboBoxItem)PhysicalStateComboBox.SelectedItem)?.Content.ToString();
            string mentalState = ((ComboBoxItem)MentalStateComboBox.SelectedItem)?.Content.ToString();

            // Проверка, выбраны ли состояния
            if (string.IsNullOrEmpty(physicalState) || string.IsNullOrEmpty(mentalState))
            {
                MessageTextBlock.Text = "Пожалуйста, выберите физическое и моральное состояние.";
                return;
            }

            // Формируем рекомендацию
            string recommendation = GetRecommendation(physicalState, mentalState);

            try
            {
                // Создаем новую запись о состоянии пользователя
                var userCondition = new user_conditions
                {
                    user_id = App.CurrentUser.user_id, // Связываем с текущим пользователем
                    physical_condition = physicalState,
                    mental_condition = mentalState,
                    date = DateTime.Today,
                    recommendations = recommendation
                };

                // Добавляем запись в контекст и сохраняем изменения
                context.user_conditions.Add(userCondition);
                context.SaveChanges();

                MessageBox.Show("Состояние успешно сохранено!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadStateHistory(); // Обновляем таблицу после сохранения
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении состояния: " + ex.Message);
            }
        }

        // Метод для загрузки истории состояний из базы данных
        private void LoadStateHistory()
        {
            try
            {
                // Получаем историю состояний для текущего пользователя
                var stateHistory = context.user_conditions
                    .Where(uc => uc.user_id == App.CurrentUser.user_id) // Фильтруем по текущему пользователю
                    .OrderByDescending(uc => uc.date)
                    .Select(uc => new UserState
                    {
                        Date = uc.date,
                        PhysicalState = uc.physical_condition,
                        MentalState = uc.mental_condition,
                        Recommendations = uc.recommendations
                    })
                    .ToList();

                // Устанавливаем источник данных для DataGrid
                UserStateDataGrid.ItemsSource = stateHistory;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке истории состояний: " + ex.Message);
            }
        }

        // Метод для обновления таблицы
        private void RefreshStateHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            LoadStateHistory();
        }

        // Метод для формирования рекомендации
        private string GetRecommendation(string physicalState, string mentalState)
        {
            string recommendation = "Рекомендация: ";

            if (physicalState == "Отлично" && mentalState == "Счастлив")
            {
                recommendation += "Поддерживайте свой образ жизни!";
            }
            else if (physicalState == "Хорошо" && mentalState == "Спокойно")
            {
                recommendation += "С продолжением этого образа жизни, вы на правильном пути!";
            }
            else if (physicalState == "Удовлетворительно")
            {
                recommendation += "Рекомендуется улучшить физическую активность.";
            }
            else if (physicalState == "Плохо" || mentalState == "Депрессия")
            {
                recommendation += "Проработайте свои переживания, возможно стоит обратиться к специалисту.";
            }
            else
            {
                recommendation += "Старайтесь поддерживать баланс между физическим и моральным состоянием.";
            }

            return recommendation;
        }
    }

    // Класс для представления состояния пользователя
    public class UserState
    {
        public DateTime Date { get; set; }
        public string PhysicalState { get; set; }
        public string MentalState { get; set; }
        public string Recommendations { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Kursach.User
{
    public partial class CondtitionUser : Page
    {
        // Список для хранения состояния пользователей
        private List<UserState> userStates;

        public CondtitionUser()
        {
            InitializeComponent();
            userStates = new List<UserState>(); // Инициализация списока состояний
        }

        private void EvaluateStateButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выбранные значения из ComboBox
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

            // Добавляем новое состояние в список
            userStates.Add(new UserState { PhysicalState = physicalState, MentalState = mentalState, Recommendation = recommendation });

            // Обновляем DataGrid
            UserStateDataGrid.ItemsSource = null;
            UserStateDataGrid.ItemsSource = userStates;
        }

        private void SaveStateButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика сохранения состояния, например, в базу данных
            // Для демонстрации мы просто покажем сообщение
            MessageBox.Show("Состояние сохранено!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
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
        public string PhysicalState { get; set; }
        public string MentalState { get; set; }
        public string Recommendation { get; set; }
    }
}

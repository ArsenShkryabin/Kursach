using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Kursach.Class;

namespace Kursach.User
{
    public partial class CondtitionUser : Page
    {
        private int currentUserId;
        private KursovayaEntities context;

        public CondtitionUser(int userId)
        {
            InitializeComponent();
            currentUserId = userId;
            context = new KursovayaEntities(); // Инициализация контекста

            LoadCurrentUser(); // Проверка и загрузка текущего пользователя
            LoadStateHistory(); // Загружаем историю состояний при инициализации страницы
        }

        private void LoadCurrentUser()
        {
            int userId = App.CurrentUser.user_id;
            var user = context.users.SingleOrDefault(u => u.user_id == userId);

            if (user == null)
            {
                MessageBox.Show("Пользователь не найден. Пожалуйста, войдите в систему.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Используем оператор ?? для обработки значения IsAdmin
            CurrentUser.Instance.SetCurrentUser(new APPUser(user.user_id, user.first_name, user.is_admin ?? false));
        }

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
                var userCondition = new user_conditions
                {
                    user_id = CurrentUser.Instance.User.UserId,
                    physical_condition = physicalState,
                    mental_condition = mentalState,
                    date = DateTime.Today,
                    recommendations = recommendation
                };

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

        private void LoadStateHistory()
        {
            try
            {
                if (!CurrentUser.Instance.IsAuthenticated())
                {
                    MessageBox.Show("Пользователь не найден. Пожалуйста, войдите в систему.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int userId = CurrentUser.Instance.User.UserId;

                var stateHistory = context.user_conditions
                    .Where(uc => uc.user_id == userId)
                    .OrderByDescending(uc => uc.date)
                    .Select(uc => new UserState
                    {
                        Date = uc.date,
                        PhysicalState = uc.physical_condition,
                        MentalState = uc.mental_condition,
                        Recommendations = uc.recommendations
                    })
                    .ToList();

                UserStateDataGrid.ItemsSource = stateHistory;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке истории состояний: " + ex.Message);
            }
        }

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

        private void EvaluateStateButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для оценки состояния
        }

        private void RefreshStateHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            LoadStateHistory(); // Перезагрузить историю состояний
        }

        // Другие методы остаются без изменений
    }

    public class UserState
    {
        public DateTime Date { get; set; }
        public string PhysicalState { get; set; }
        public string MentalState { get; set; }
        public string Recommendations { get; set; }
    }
}
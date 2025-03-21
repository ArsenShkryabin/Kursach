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
                    user_id = currentUserId, // Используем currentUserId вместо CurrentUser.Instance.User.UserId
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

                // Используем currentUserId, чтобы загрузить только данные этого пользователя
                var stateHistory = context.user_conditions
                    .Where(uc => uc.user_id == currentUserId) // Фильтруем по текущему пользователю
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
            try
            {
                // Получаем последние 5 записей о состоянии
                var lastConditions = context.user_conditions
                    .Where(uc => uc.user_id == currentUserId)
                    .OrderByDescending(uc => uc.date)
                    .Take(5)
                    .ToList();

                if (lastConditions.Count == 0)
                {
                    MessageBox.Show("История состояния пуста. Пожалуйста, сохраните свое состояние, чтобы оценить его.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Подсчитываем общее состояние и оценки
                int physicalScore = 0;
                int mentalScore = 0;

                foreach (var condition in lastConditions)
                {
                    physicalScore += MapPhysicalStateToScore(condition.physical_condition);
                    mentalScore += MapMentalStateToScore(condition.mental_condition);
                }

                int averagePhysical = physicalScore / lastConditions.Count;
                int averageMental = mentalScore / lastConditions.Count;

                string physicalRecommendation = GetPhysicalRecommendations(averagePhysical);
                string mentalRecommendation = GetMentalRecommendations(averageMental);

                MessageBox.Show($"Ваши последние состояния:\n" +
                                $"Физическое состояние (средний балл): {averagePhysical}\n" +
                                $"Психологическое состояние (средний балл): {averageMental}\n\n" +
                                $"Рекомендации:\n{physicalRecommendation}\n{mentalRecommendation}",
                                "Оценка состояния",
                                MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при оценке состояния: " + ex.Message);
            }
        }

        // Метод для отображения физического состояния в баллы
        private int MapPhysicalStateToScore(string physicalState)
        {
            switch (physicalState)
            {
                case "Отлично":
                    return 4;
                case "Хорошо":
                    return 3;
                case "Удовлетворительно":
                    return 2;
                case "Плохо":
                    return 1;
                default:
                    return 0;
            }
        }

        // Метод для отображения психологического состояния в баллы
        private int MapMentalStateToScore(string mentalState)
        {
            switch (mentalState)
            {
                case "Счастлив":
                    return 4;
                case "Спокойно":
                    return 3;
                case "Нормально":
                    return 2;
                case "Депрессия":
                    return 1;
                default:
                    return 0;
            }
        }

        // Метод для получения рекомендаций по физическому состоянию
        private string GetPhysicalRecommendations(int averageScore)
        {
            switch (averageScore)
            {
                case 4:
                    return "Отлично! Продолжайте заботиться о своем физическом состоянии. Поддерживайте активный образ жизни.";
                case 3:
                    return "Хорошо! Продолжайте заниматься физической активностью и следите за своим здоровьем.";
                case 2:
                    return "Удовлетворительно. Рассмотрите возможность увеличения физической активности и улучшения режима питания.";
                case 1:
                    return "Плохо. Это тревожный сигнал. Вам следует обратиться к врачу и начать заниматься своим физическим состоянием.";
                default:
                    return "Неизвестное состояние. Пожалуйста, обратитесь к специалисту.";
            }
        }

        // Метод для получения рекомендаций по психологическому состоянию
        private string GetMentalRecommendations(int averageScore)
        {
            switch (averageScore)
            {
                case 4:
                    return "Вы в прекрасном эмоциональном состоянии! Сохраните этот позитивный настрой.";
                case 3:
                    return "Хорошо! Продолжайте заниматься тем, что приносит вам радость, и старайтесь избегать стресса.";
                case 2:
                    return "Удовлетворительно. Возможно, стоит поговорить с близкими или специалистом о ваших переживаниях.";
                case 1:
                    return "Выделите время для самопомощи и подумайте о том, чтобы обратиться к психологу.";
                default:
                    return "Неизвестное состояние. Настоятельно рекомендуем обратиться за помощью к специалисту.";
            }
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
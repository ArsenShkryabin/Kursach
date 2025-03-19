using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Kursach.User
{
    public partial class HistoryUsers : Page
    {
        private KursovayaEntities _context = new KursovayaEntities();
        private int _userId; // ID текущего пользователя

        public HistoryUsers(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadRecommendation();
        }

        // Загрузка рекомендаций
        private void LoadRecommendation()
        {
            // Пример рекомендации
            string recommendation = "Рекомендуем следить за балансом калорий, спать не менее 7 часов и заниматься физической активностью 3 раза в неделю.";
            RecommendationTextBlock.Text = recommendation;
        }

        // Обработчик для выбора таблицы калорий
        private void CaloriesRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            CaloriesDataGrid.Visibility = Visibility.Visible;
            StateDataGrid.Visibility = Visibility.Collapsed;
            HabitsDataGrid.Visibility = Visibility.Collapsed;
        }

        // Обработчик для выбора таблицы состояния
        private void StateRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            StateDataGrid.Visibility = Visibility.Visible;
            CaloriesDataGrid.Visibility = Visibility.Collapsed;
            HabitsDataGrid.Visibility = Visibility.Collapsed;
        }

        // Обработчик для выбора таблицы привычек
        private void HabitsRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            HabitsDataGrid.Visibility = Visibility.Visible;
            CaloriesDataGrid.Visibility = Visibility.Collapsed;
            StateDataGrid.Visibility = Visibility.Collapsed;
        }

        // Обработчик для кнопки "Просмотреть"
        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            if (CaloriesRadioButton.IsChecked == true)
            {
                LoadCaloriesHistory();
            }
            else if (StateRadioButton.IsChecked == true)
            {
                LoadStateHistory();
            }
            else if (HabitsRadioButton.IsChecked == true)
            {
                LoadHabitsHistory();
            }
        }

        // Загрузка истории калорий
        private void LoadCaloriesHistory()
        {
            var entries = _context.nutrition
                .Where(n => n.user_id == _userId)
                .ToList();

            CaloriesDataGrid.ItemsSource = entries;
        }

        // Загрузка истории состояния
        private void LoadStateHistory()
        {
            var entries = _context.users
                .Where(u => u.user_id == _userId)
                .ToList();

            StateDataGrid.ItemsSource = entries;
        }

        // Загрузка истории привычек
        private void LoadHabitsHistory()
        {
            var entries = _context.habits
                .Where(h => h.user_id == _userId)
                .ToList();

            HabitsDataGrid.ItemsSource = entries;
        }
    }
}

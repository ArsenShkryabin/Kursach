using System.Windows;
using System.Windows.Controls;

namespace Kursach.User
{
    public partial class HistoryUsers : Page
    {
        public HistoryUsers()
        {
            InitializeComponent();
        }

        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            if (CaloriesRadioButton.IsChecked == true)
            {
                // Перейти на историю калорий
               UsersCalories caloriesHistoryPage = new UsersCalories(); // Передайте данные о калориях, если нужно
                this.NavigationService.Navigate(caloriesHistoryPage);
            }
            else if (StateRadioButton.IsChecked == true)
            {
                // Перейти на историю состояния
                Condition userStatePage = new Condition(); // Передайте данные о состоянии, если нужно
                this.NavigationService.Navigate(userStatePage);
            }
        }

        private void CaloriesRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void StateRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void HabitsRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
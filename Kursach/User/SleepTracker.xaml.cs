using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Kursach.User
{
    public partial class SleepTrackerPage : Page
    {
        private KursovayaEntities context = new KursovayaEntities();

        public SleepTrackerPage()
        {
            InitializeComponent();
            LoadSleepData();
        }

        private void LoadSleepData()
        {
            try
            {
                SleepDataGrid.ItemsSource = context.sleep
                    .Where(s => s.user_id == App.CurrentUser.user_id)
                    .OrderByDescending(s => s.date)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных о сне: " + ex.Message);
            }
        }

        private void SaveSleepButton_Click(object sender, RoutedEventArgs e)
        {
            if (DatePicker.SelectedDate == null || string.IsNullOrEmpty(DurationTextBox.Text) || QualityComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            try
            {
                var sleepEntry = new sleep
                {
                    user_id = App.CurrentUser.user_id,
                    date = DatePicker.SelectedDate.Value,
                    duration = int.Parse(DurationTextBox.Text),
                    quality = ((ComboBoxItem)QualityComboBox.SelectedItem).Content.ToString()
                };

                context.sleep.Add(sleepEntry);
                context.SaveChanges();

                MessageBox.Show("Данные о сне успешно сохранены!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadSleepData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении данных о сне: " + ex.Message);
            }
        }
    }
}

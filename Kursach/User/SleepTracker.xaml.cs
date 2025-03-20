using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Kursach.User
{
    public partial class SleepTrackerPage : Page
    {
        private KursovayaEntities context = new KursovayaEntities();
        private int currentUserId;

        public SleepTrackerPage(int userId)
        {
            InitializeComponent();
            currentUserId = userId; // Предполагается, что currentUserId получает значение корректно
            LoadSleepData();
           
        }

        private void LoadSleepData()
        {
            try
            {
                var sleepData = context.sleep
                    .Where(s => s.user_id == currentUserId)
                    .OrderByDescending(s => s.date)
                    .Select(s => new SleepEntryViewModel
                    {
                        Date = s.date,
                        
                        Quality = s.quality
                    })
                    .ToList();

                SleepHistoryDataGrid.ItemsSource = sleepData;
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
                    user_id = currentUserId,
                    date = DatePicker.SelectedDate.Value,
                    duration = int.Parse(DurationTextBox.Text),
                    quality = ((ComboBoxItem)QualityComboBox.SelectedItem).Content.ToString()
                };

                context.sleep.Add(sleepEntry);
                context.SaveChanges();

                MessageBox.Show("Данные о сне успешно сохранены!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadSleepData(); // Обновляем таблицу после сохранения
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении данных о сне: " + ex.Message);
            }
        }

        private void RemoveSleepButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика удаления записи о сне
        }

        private void RemoveSleepButton_Click_1(object sender, RoutedEventArgs e)
        {
            // Логика удаления записи о сне
        }
    }

    public class SleepEntryViewModel
    {
        public DateTime Date { get; set; }
        public int Duration { get; set; }
        public string Quality { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Kursach.User
{
    public partial class HabbitPage : Page
    {
        private KursovayaEntities _context = new KursovayaEntities(); // Контекст базы данных
        private int _userId; // ID текущего пользователя

        public HabbitPage(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadHabits(); // Загружаем привычки из базы данных
        }

        // Метод для загрузки привычек из базы данных
        private void LoadHabits()
        {
            try
            {
                var habits = _context.habits
                    .Where(h => h.user_id == _userId)
                    .ToList();

                HabitsDataGrid.ItemsSource = habits; // Устанавливаем источник данных для DataGrid
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
        }

        // Метод для добавления привычки
        private void AddHabitButton_Click(object sender, RoutedEventArgs e)
        {
            string habitName = HabitTextBox.Text;
            string description = DescriptionTextBox.Text;

            if (!string.IsNullOrWhiteSpace(habitName) && !string.IsNullOrWhiteSpace(description))
            {
                try
                {
                    var newHabit = new habits
                    {
                        user_id = _userId,
                        name = habitName,
                        description = description
                    };

                    _context.habits.Add(newHabit); // Добавляем привычку в контекст
                    _context.SaveChanges(); // Сохраняем изменения в базе данных
                    LoadHabits(); // Перезагружаем список привычек

                    HabitTextBox.Clear();
                    DescriptionTextBox.Clear();
                    ActionMessageTextBlock.Text = "Привычка добавлена.";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении привычки: " + ex.Message);
                }
            }
            else
            {
                ActionMessageTextBlock.Text = "Пожалуйста, заполните все поля.";
            }
        }

        // Метод для удаления привычки
        private void RemoveHabitButton_Click(object sender, RoutedEventArgs e)
        {
            if (HabitsDataGrid.SelectedItem is habits selectedHabbit)
            {
                try
                {
                    _context.habits.Remove(selectedHabbit); // Удаляем привычку из контекста
                    _context.SaveChanges(); // Сохраняем изменения в базе данных
                    LoadHabits(); // Перезагружаем список привычек

                    ActionMessageTextBlock.Text = $"Привычка '{selectedHabbit.name}' удалена.";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении привычки: " + ex.Message);
                }
            }
            else
            {
                ActionMessageTextBlock.Text = "Пожалуйста, выберите привычку для удаления.";
            }
        }
    }
}

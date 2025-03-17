using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Kursach.User
{
    // Класс для представления привычки
    public class Habit
    {
        public string Name { get; set; } // Название привычки
        public string Description { get; set; } // Описание привычки
    }

    public partial class HabbitPage : Page
    {
        private List<Habit> habits; // Поле для хранения списка привычек

        public HabbitPage()
        {
            InitializeComponent();
            habits = new List<Habit>(); // Инициализация списка привычек
            HabitsDataGrid.ItemsSource = habits; // Установка источника данных для DataGrid
        }

        // Метод для обработки нажатия кнопки "Добавить привычку"
        private void AddHabitButton_Click(object sender, RoutedEventArgs e)
        {
            string habitName = HabitTextBox.Text;
            string description = DescriptionTextBox.Text;

            // Проверка, не пустые ли поля
            if (!string.IsNullOrWhiteSpace(habitName) && !string.IsNullOrWhiteSpace(description))
            {
                habits.Add(new Habit { Name = habitName, Description = description }); // Добавляем новую привычку
                HabitsDataGrid.Items.Refresh(); // Обновляем DataGrid для отображения новой привычки
                HabitTextBox.Clear(); // Очищаем текстовое поле для привычки
                DescriptionTextBox.Clear(); // Очищаем текстовое поле для описания
                ActionMessageTextBlock.Text = "Привычка добавлена."; // Отображаем сообщение о добавлении
            }
            else
            {
                ActionMessageTextBlock.Text = "Пожалуйста, заполните все поля."; // Сообщение об ошибке
            }
        }

        // Метод для обработки нажатия кнопки "Удалить привычку"
        private void RemoveHabitButton_Click(object sender, RoutedEventArgs e)
        {
            if (HabitsDataGrid.SelectedItem is Habit selectedHabit) // Проверяем, выбрана ли привычка
            {
                habits.Remove(selectedHabit); // Удаляем выбранную привычку
                HabitsDataGrid.Items.Refresh(); // Обновляем DataGrid для отображения изменений
                ActionMessageTextBlock.Text = $"Привычка '{selectedHabit.Name}' удалена."; // Отображаем сообщение о удалении
            }
            else
            {
                ActionMessageTextBlock.Text = "Пожалуйста, выберите привычку для удаления."; // Сообщение об ошибке
            }
        }
    }
}
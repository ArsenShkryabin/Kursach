﻿using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Kursach.User
{
    public partial class NutrionTracker : Page
    {
        private KursovayaEntities _context = new KursovayaEntities();
        private int _userId; // ID текущего пользователя

        public NutrionTracker(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadNutritionHistory();
        }

        // Загрузка истории питания
        private void LoadNutritionHistory()
        {
            try
            {
                var entries = _context.nutrition
                    .Where(n => n.user_id == _userId) // Фильтр по user_id
                    .OrderByDescending(n => n.nutrition_id)
                    .ToList();

                NutritionHistoryDataGrid.ItemsSource = entries;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
        }

        // Добавление записи о питании
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ProductTextBox.Text) || string.IsNullOrEmpty(CaloriesTextBox.Text))
            {
                MessageBox.Show("Заполните все поля.");
                return;
            }

            if (!int.TryParse(CaloriesTextBox.Text, out int calories))
            {
                MessageBox.Show("Введите корректное количество калорий.");
                return;
            }

            if (DatePicker.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату.");
                return;
            }

            try
            {
                var newEntry = new nutrition
                {
                    user_id = _userId,
                    product = ProductTextBox.Text,
                    calories = calories,
                    date = DatePicker.SelectedDate.Value, // Сохраняем выбранную дату
                    calorie_counting = true // Указываем, что калории учитываются
                };

                _context.nutrition.Add(newEntry);
                _context.SaveChanges();

                MessageBox.Show("Данные сохранены.");
                LoadNutritionHistory(); // Обновляем список
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении данных: " + ex.Message);
            }
        }

        // Удаление записи о питании
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = NutritionHistoryDataGrid.SelectedItem as nutrition;

            if (selectedItem == null)
            {
                MessageBox.Show("Выберите запись для удаления.");
                return;
            }

            try
            {
                _context.nutrition.Remove(selectedItem);
                _context.SaveChanges();

                MessageBox.Show("Запись удалена.");
                LoadNutritionHistory(); // Обновляем список
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при удалении записи: " + ex.Message);
            }
        }
    }
}

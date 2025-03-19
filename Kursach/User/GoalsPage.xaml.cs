using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Kursach.User;

namespace Kursach.User
{
    public partial class GoalsPage : Page
    {
        private KursovayaEntities context = new KursovayaEntities();
        private int currentUserId; // ID текущего пользователя

        public GoalsPage(int userId)
        {
            InitializeComponent();
            currentUserId = userId;
            LoadGoals();
        }

        public class GoalViewModel
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string Status { get; set; }
        }

        // Загрузка целей пользователя
        private void LoadGoals()
        {
            var goals = context.Goals
                .Where(g => g.UserId == currentUserId)
                .Select(g => new
                {
                    Title = g.Title,
                    Description = g.Description,
                    Status = g.IsCompleted ? "Выполнено" : "В процессе"
                })
                .ToList();

            GoalsListView.ItemsSource = goals;

            // Обновление статуса выполнения
            int completedGoals = context.Goals.Count(g => g.UserId == currentUserId && g.IsCompleted);
            int totalGoals = context.Goals.Count(g => g.UserId == currentUserId);
            if (totalGoals > 0)
            {
                ProgressTextBlock.Text = $"Выполнено {completedGoals} из {totalGoals} целей.";
            }
            else
            {
                ProgressTextBlock.Text = "У вас пока нет целей.";
            }
        }

        // Добавление новой цели
        private void AddGoalButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(GoalTextBox.Text))
            {
                MessageBox.Show("Введите название цели.");
                return;
            }

            var newGoal = new Goals
            {
                UserId = currentUserId,
                Title = GoalTextBox.Text,
                Description = DescriptionTextBox.Text,
                IsCompleted = false
            };

            context.Goals.Add(newGoal);
            context.SaveChanges();

            // Очистка полей и обновление списка
            GoalTextBox.Clear();
            DescriptionTextBox.Clear();
            LoadGoals();
        }

        // Отметить цель как выполненную
        private void MarkAsCompletedButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedGoal = GoalsListView.SelectedItem as GoalViewModel;
            if (selectedGoal == null)
            {
                MessageBox.Show("Выберите цель для отметки.");
                return;
            }

            var goal = context.Goals.FirstOrDefault(g => g.UserId == currentUserId && g.Title == selectedGoal.Title);
            if (goal != null)
            {
                goal.IsCompleted = true;
                context.SaveChanges();
                LoadGoals();
            }
        }

        // Удаление цели
        private void DeleteGoalButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedGoal = GoalsListView.SelectedItem as GoalViewModel;
            if (selectedGoal == null)
            {
                MessageBox.Show("Выберите цель для удаления.");
                return;
            }

            var goal = context.Goals.FirstOrDefault(g => g.UserId == currentUserId && g.Title == selectedGoal.Title);
            if (goal != null)
            {
                context.Goals.Remove(goal);
                context.SaveChanges();
                LoadGoals();
            }
        }
    }
}

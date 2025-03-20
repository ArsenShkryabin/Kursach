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
        private int currentUserId;

        public class GoalViewModel
        {
            public int GoalId { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Status { get; set; }
        }

        public GoalsPage(int userId)
        {
            InitializeComponent();
            currentUserId = userId;
            LoadGoals();
        }

        private void LoadGoals()
        {
            var goals = context.Goals
                .Where(g => g.UserId == currentUserId)
                .Select(g => new GoalViewModel
                {
                    GoalId = g.GoalId,
                    Title = g.Title,
                    Description = g.Description,
                    Status = g.IsCompleted ? "Выполнено" : "В процессе"
                })
                .ToList();

            GoalsDataGrid.ItemsSource = goals;
            UpdateProgress();
        }

        private void UpdateProgress()
        {
            int completed = context.Goals.Count(g => g.UserId == currentUserId && g.IsCompleted);
            int total = context.Goals.Count(g => g.UserId == currentUserId);
            ProgressTextBlock.Text = total > 0
                ? $"Выполнено {completed} из {total} целей."
                : "У вас пока нет целей.";
        }

        private void AddGoalButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(GoalTextBox.Text))
            {
                MessageBox.Show("Введите название цели.");
                return;
            }

            context.Goals.Add(new Goals
            {
                UserId = currentUserId,
                Title = GoalTextBox.Text,
                Description = DescriptionTextBox.Text,
                IsCompleted = false
            });

            context.SaveChanges();
            ClearForm();
            LoadGoals();
        }

        private void MarkAsCompletedButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = GoalsDataGrid.SelectedItem as GoalViewModel;
            if (selected == null) return;

            var goal = context.Goals.Find(selected.GoalId);
            if (goal != null)
            {
                goal.IsCompleted = true;
                context.SaveChanges();
                LoadGoals();
            }
        }

        private void DeleteGoalButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = GoalsDataGrid.SelectedItem as GoalViewModel;
            if (selected == null) return;

            var goal = context.Goals.Find(selected.GoalId);
            if (goal != null)
            {
                context.Goals.Remove(goal);
                context.SaveChanges();
                LoadGoals();
            }
        }

        private void ClearForm()
        {
            GoalTextBox.Clear();
            DescriptionTextBox.Clear();
        }
    }
}

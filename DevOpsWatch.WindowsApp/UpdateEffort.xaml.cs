
using DevOpsWatch.BL.Core;
using DevOpsWatch.BL.Core.Interfaces;
using DevOpsWatch.BL.Core.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Windows.UI.Core;
using Windows.UI.Text.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DevOpsWatch.WindowsApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UpdateEffort : Page
    {
        public UpdateEffort()
        {
            this.InitializeComponent();
            Task.Run(async () =>
            {
                try
                {
                    devOpsSource = Services.DevOpsSource;
                    await UpdateActiveTasksSection();
                }
                catch (Exception e)
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        TodaysUpdateText.Text = e.ToString();
                        TodaysUpdateText.Visibility = Visibility.Visible;
                    });
                }
                
            });
        }
        private IDevOpsSource devOpsSource;

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            List<TaskDto> activeTasks = new List<TaskDto>();
            foreach (var item in ActiveTasksStackPanel.Children)
            {
                if (item is TextBox tb && int.TryParse(tb.Name, out int taskId) && double.TryParse(tb.Text, out double completedWork))
                {
                    activeTasks.Add(new TaskDto { TaskId = taskId, CompletedWork = completedWork });
                }
            }
            devOpsSource.UpdateEfforts(activeTasks);
            Frame.Navigate(typeof(MainPage));
        }

        private async Task UpdateActiveTasksSection()
        {
            string todaysUpdate = "";
            List<TaskDto> activeTasks = null;
            try
            {
                List<TaskDto> activeTasks2 = devOpsSource.GetEffortsUpdatedToday().ToList();
                todaysUpdate = "You haven't updated any efforts today";
                if (activeTasks2.Any(t => t.CompletedWork > 0 || t.RemainingWork > 0))
                {
                    var combined = string.Join(",", activeTasks2.Select(a => $"{a.CompletedWork} hrs in {a.TaskId}"));
                    todaysUpdate = $"Today, you've already updated {combined}";
                }
                activeTasks = devOpsSource.GetActiveTasksAssignedTo().ToList();
            }
            catch (Exception e)
            {
                TodaysUpdateText.Text = e.ToString();
                TodaysUpdateText.Visibility = Visibility.Visible;
            }
            
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                TodaysUpdateText.Text = todaysUpdate;
                TodaysUpdateText.Visibility = Visibility.Visible;
            });
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ActiveTasksStackPanel.Children.Clear();
                if (activeTasks.Count == 0)
                {
                    ActiveTasksStackPanel.Children.Add(new TextBlock { Text = "You don't have any active tasks assigned to you today." });
                }
                else
                {
                    foreach (var item in activeTasks)
                    {
                        TextBlock textBox = new TextBlock
                        {
                            Name = $"Efforts_{item.TaskId}_t",
                            Text = $"{item.TaskId}: {item.TaskName}",
                            MinWidth = 300
                        };
                        ActiveTasksStackPanel.Children.Add(textBox);
                        TextBox inputTextBox = new TextBox
                        {
                            Name = $"{item.TaskId}",
                            PlaceholderText = $"efforts for today ({item.RemainingWork} remaining)",
                            Margin = new Thickness(0, 0, 0, 10),
                            MinWidth = 300
                        };
                        ActiveTasksStackPanel.Children.Add(inputTextBox);
                    }

                    ActiveTasksName.Visibility = Visibility.Visible;
                    UpdateButton.Visibility = Visibility.Visible;
                }
                BackToConfig.Visibility = Visibility.Visible;
                ButtonPanel.Visibility = Visibility.Visible;
            });

        }
    }
}

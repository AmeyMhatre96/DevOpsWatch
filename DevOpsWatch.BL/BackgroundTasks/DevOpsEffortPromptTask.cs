using DevOpsWatch.BL.Core;
using DevOpsWatch.BL.Core.Models;

using Microsoft.Toolkit.Uwp.Notifications;

using System;
using System.Collections.Generic;
using System.Linq;

using Windows.ApplicationModel.Background;

using Windows.UI.Notifications;

namespace DevOpsWatch.BL.BackgroundTasks
{
    public sealed class DevOpsReminderTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            ReminderNotification.Show();
        }
    }

    internal static class ReminderNotification
    {
        public static void Show()
        {
            var devOpsSource = Services.DevOpsSource;

            List<TaskDto> activeTasks = devOpsSource.GetEffortsUpdatedToday().ToList();
            string todaysUpdate = "You haven't updated any efforts today";
            if (activeTasks.Any(t => t.CompletedWork > 0 || t.RemainingWork > 0))
            {
                var combined = string.Join(",", activeTasks.Select(a => $"{a.CompletedWork} hrs in {a.TaskId}"));
                todaysUpdate = $"Today, you've already updated {combined}";
            }

            activeTasks = devOpsSource.GetActiveTasksAssignedTo().ToList();

            // Call Notification popup
            var builder = new ToastContentBuilder()
                .AddText(GetPrompt())
                .AddText(todaysUpdate);

            if (activeTasks == null || activeTasks.Count == 0)
            {
                builder.AddText("You don't have any active tasks assigned to you today.");
            }
            else
            {
                // Due to limitation in the notification size, we can only show 5 tasks.
                foreach (var item in activeTasks.Take(5))
                {
                    builder.AddInputTextBox(item.TaskId.ToString(), placeHolderContent: $"efforts for today ({item.RemainingWork} remaining)", title: $"{item.TaskId}: {item.TaskName}");
                }
                builder.AddButton("Update", ToastActivationType.Background, "Update");
            }

            ToastContent content = builder.GetToastContent();
            content.ActivationType = ToastActivationType.Foreground;
            var notification = new ToastNotification(content.GetXml());

            ToastNotificationManager.CreateToastNotifier().Show(notification);
        }

        internal static string GetPrompt()
        {
            string[] prompts = new string[]
            {
                "Knock, knock! It's update time",
                "Hey, hey! It's update o'clock! Don't keep your efforts waiting any longer",
                "Oi, time to update your efforts, innit?",
                "What a bloomin' mess, update your efforts now",
                "Time for an update! Enhance your efforts and make them even better.",
                "Hey, hey! It's update o'clock! Don't keep your efforts waiting any longer.",
                "Stay ahead of the game! It's time to update your efforts and stay on top.",
                "Efforts due for an upgrade! Polish them up and make them shine.",
                "Don't settle for less! Give your efforts a refresh and exceed expectations.",
                "Update alert! Refine your efforts and achieve new milestones.",
                "Step up your game! It's time to update your efforts and show your true potential.",
                "Upgrade your efforts for success! Update, refine, and make your mark.",
                "Time for a makeover! Update your efforts and unleash your full potential.",
                "Efforts in need of a boost? Update them now and reach new heights."
            };

            int index = new Random().Next(prompts.Length);
            return prompts[0];
        }
    }
}

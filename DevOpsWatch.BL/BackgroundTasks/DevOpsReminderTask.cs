using DevOpsWatch.BL.Core;

using System;

using Windows.ApplicationModel.Background;

namespace DevOpsWatch.BL.BackgroundTasks
{

    /// <summary>
    /// Task scehduled on every 
    /// </summary>
    /// <seealso cref="IBackgroundTask" />
    public sealed class NotificationSchedulerTask : IBackgroundTask
    {
        BackgroundTaskDeferral _deferral; // Note: defined at class scope so that we can mark it complete inside the OnCancel() callback if we choose to support cancellation
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            Run(taskInstance, unregister: false);
        }

        public void Run(IBackgroundTaskInstance taskInstance, bool unregister)
        {
            var taskRegistered = false;
            var taskName = nameof(DevOpsReminderTask);
            var notificationTask = nameof(NotificationActionBackgroundTask);

            bool inputHandlingTaskFound = false;

            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                Console.WriteLine(task.Value.Name);
                if (task.Value.Name == taskName)
                {
                    if (unregister)
                    {
                        task.Value.Unregister(cancelTask: true);
                    }
                    else
                    {
                        taskRegistered = true;
                        break;
                    }
                }
                if (task.Value.Name == notificationTask)
                {
                    if (inputHandlingTaskFound)
                    {
                        task.Value.Unregister(cancelTask: true);
                    }
                    else
                    {
                        inputHandlingTaskFound = true;
                    }
                }
            }
            if (!taskRegistered)
            {
                var settings = Settings.GetSettings();

                var newstartTime = settings.ReminderTime;
                var startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, newstartTime.Hours, newstartTime.Minutes, newstartTime.Seconds);
                var now = DateTime.Now;
                if(startTime > now)
                {
                    var startInMinutes = (uint)(startTime - DateTime.Now).TotalMinutes;
                    // Minimum we can schedule is 15 mins from now.
                    startInMinutes += 15;

                    var taskNamespace = typeof(DevOpsReminderTask).FullName;
                    RegisterTask(taskName, taskNamespace, startInMinutes);
                }
            }

            if (!inputHandlingTaskFound)
            {
                BackgroundTaskBuilder builder = new BackgroundTaskBuilder()
                {
                    Name = nameof(NotificationActionBackgroundTask),
                    TaskEntryPoint = typeof(NotificationActionBackgroundTask).FullName
                };
                builder.SetTrigger(new ToastNotificationActionTrigger());
                BackgroundTaskRegistration registration = builder.Register();
            }
        }

        private static void RegisterTask(string exampleTaskName, string entryPoint, uint startInMinutes)
        {
            BackgroundTaskBuilder builder = new BackgroundTaskBuilder
            {
                Name = exampleTaskName,
                TaskEntryPoint = entryPoint
            };
            IBackgroundTrigger trigger = new TimeTrigger(startInMinutes, oneShot: true);
            builder.SetTrigger(trigger);
            SystemCondition internetCondition = new SystemCondition(SystemConditionType.InternetAvailable);
            SystemCondition userPresent = new SystemCondition(SystemConditionType.UserPresent);
            builder.AddCondition(internetCondition);
            builder.AddCondition(userPresent);
            BackgroundTaskRegistration task2 = builder.Register();
        }
    }
}
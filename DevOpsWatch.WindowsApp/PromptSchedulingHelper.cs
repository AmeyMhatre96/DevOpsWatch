
using DevOpsWatch.BL.BackgroundTasks;
using DevOpsWatch.BL.Core;

using Windows.ApplicationModel.Background;

namespace DevOpsWatch.WindowsApp
{
    internal static class PromptSchedulingHelper
    {
        /// <summary>
        /// This task runs every user becomes active on the system.
        /// It is used to schedule the <see cref="NotificationSchedulerTask"/> to run every day at the specified time
        /// </summary>
        internal static void RegisterTimeBasedSchedulingOnUserActiveTask()
        {
            bool taskRegistered = false;
            string taskName = nameof(NotificationSchedulerTask);

            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskName)
                {
                    taskRegistered = true;
                    break;
                }
            }
            if (!taskRegistered)
            {
                var taskNamespace = typeof(NotificationSchedulerTask).FullName;
                BackgroundTaskBuilder builder = new BackgroundTaskBuilder
                {
                    Name = taskName,
                    TaskEntryPoint = taskNamespace
                };
                IBackgroundTrigger trigger = new SystemTrigger(SystemTriggerType.UserPresent, oneShot: false);
                builder.SetTrigger(trigger);
                SystemCondition userPresent = new SystemCondition(SystemConditionType.UserPresent);
                builder.AddCondition(userPresent);
                _ = builder.Register();
                var settings = Settings.GetSettings();
                if (!string.IsNullOrEmpty(settings.PAT))
                {
                    new NotificationSchedulerTask().Run(null, unregister: false);
                }
            }
        }
    }
}

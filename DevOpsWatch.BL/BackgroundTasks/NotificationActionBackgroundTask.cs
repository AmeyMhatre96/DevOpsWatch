using DevOpsWatch.BL.Core;
using DevOpsWatch.BL.Core.Models;

using System.Collections.Generic;

using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;

namespace DevOpsWatch.BL.BackgroundTasks
{
    /// <summary>
    /// Update the efforts in the DevOps
    /// </summary>
    /// <seealso cref="IBackgroundTask" />
    public sealed class NotificationActionBackgroundTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            List<TaskDto> activeTasks = new List<TaskDto>();
            var details = taskInstance.TriggerDetails as ToastNotificationActionTriggerDetail;

            foreach (var item in details.UserInput)
            {
                if (item.Value is string valStr && !string.IsNullOrEmpty(valStr) && int.TryParse(item.Key, out int taskId) && double.TryParse(valStr, out double completedWork))
                {
                    activeTasks.Add(new TaskDto { TaskId = taskId, CompletedWork = completedWork });
                }
            }
            Services.DevOpsSource.UpdateEfforts(activeTasks);
        }
    }
}

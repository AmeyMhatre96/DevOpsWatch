using DevOpsWatch.BL.AzureDevOpsSource.Models;
using DevOpsWatch.BL.Core;
using DevOpsWatch.BL.Core.Interfaces;
using DevOpsWatch.BL.Core.Models;

using System;
using System.Collections.Generic;
using System.Linq;

namespace DevOpsWatch.BL.AzureDevOpsSource
{
    public sealed class AzureDevOpsSource : IDevOpsSource
    {
        public AzureDevOpsClient AzureDevOps { get; set; }

        public AzureDevOpsSource(Settings settings)
        {
            AzureDevOps = new AzureDevOpsClient(settings);
        }

        public IEnumerable<TaskDto> GetEffortsUpdatedToday()
        {
            var allUserTasks = AzureDevOps.GetAllTasksAssignedToUserInCurrentIteration();

            List<TaskDto> todaysEffortUpdates = new List<TaskDto>();
            foreach (int taskId in allUserTasks.Select(t => t.Id))
            {
                WorkItemModel itemDetails = AzureDevOps.GetWorkItemById(taskId);
                // If the item is updated today,
                if (itemDetails.Fields.SystemChangedDate is DateTime changedDate && changedDate.Date == DateTime.Today)
                {
                    var updates = AzureDevOps.GetWorkItemUpdates(taskId);
                    // Find all the updates are updated today and contains updates to field Microsoft.VSTS.Scheduling.RemainingWork or Microsoft.VSTS.Scheduling.CompletedWork
                    var todayUpdates = updates
                        .Where(x => DateTime.Parse(x.Fields.SystemChangedDate.NewValue) >= DateTime.Today)
                        .Where(x => x.Fields.MicrosoftVSTSSchedulingCompletedWork != null).ToList();

                    // Get the total amount of completed work updated
                    double completedWorkUpdate = 0;
                    double remainingWorkUpdate = 0;
                    foreach (ValueModel taskCompletedWork in todayUpdates.Select(u => u.Fields.MicrosoftVSTSSchedulingCompletedWork))
                    {
                        if (taskCompletedWork != null)
                        {
                            var value = taskCompletedWork;
                            if (value.OldValue == null)
                            {
                                value.OldValue = "0";
                            }
                            if (double.TryParse(value.OldValue, out var oldValue) && double.TryParse(value.NewValue, out var newValue))
                            {
                                completedWorkUpdate += (newValue - oldValue);
                                remainingWorkUpdate += (newValue - oldValue);
                            }
                        }
                    }
                    if (completedWorkUpdate > 0 /*|| remainingWorkUpdate < 0*/)
                    {
                        todaysEffortUpdates.Add(new TaskDto(itemDetails.Id,
                            itemDetails.Fields.SystemTitle, completedWorkUpdate, remainingWorkUpdate));
                    }
                }

            }
            return todaysEffortUpdates;
        }


        public IEnumerable<TaskDto> GetActiveTasksAssignedTo()
        {
            var references = AzureDevOps.GetAllTasksAssignedToUserInCurrentIteration();
            List<TaskDto> workItemReferenceDtos = new List<TaskDto>();
            foreach (var reference in references)
            {
                TaskDto item = GetWorkDto(reference.Id);
                workItemReferenceDtos.Add(item);
            }
            return workItemReferenceDtos;
        }

        private TaskDto GetWorkDto(int id)
        {
            var detail = AzureDevOps.GetWorkItemById(id);
            double completedWork = detail.Fields.MicrosoftVstsSchedulingCompletedWork;
            double remainingWork = detail.Fields.MicrosoftVstsSchedulingRemainingWork;
            if (completedWork == 0 && remainingWork == 0)
            {
                remainingWork = detail.Fields.MicrosoftVstsSchedulingOriginalEstimate;
            }
            double originalEstimate = detail.Fields.MicrosoftVstsSchedulingOriginalEstimate;
            string state = detail.Fields.SystemState;

            TaskDto item = new TaskDto()
            {
                TaskId = id,
                TaskName = detail.Fields.SystemTitle.ToString(),
                CompletedWork = completedWork,
                RemainingWork = remainingWork,
                TotalWork = originalEstimate,
                State = state
            };
            return item;

        }

        public void UpdateEfforts(IEnumerable<TaskDto> todaysEffortUpdates)
        {
            foreach (var update in todaysEffortUpdates)
            {
                var detail = GetWorkDto(update.TaskId);

                if (update.CompletedWork < 1 || detail.RemainingWork < update.CompletedWork)
                {
                    continue;
                }
                if (update.RemainingWork == -1)
                {
                    update.RemainingWork = detail.RemainingWork - update.CompletedWork;
                    update.CompletedWork = detail.CompletedWork + update.CompletedWork;
                }
                AzureDevOps.UpdateCompletedAndRemainingWork(update.TaskId, update.CompletedWork, update.RemainingWork, detail.State);
            }
        }
    }
}

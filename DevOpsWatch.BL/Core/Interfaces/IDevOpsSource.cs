using DevOpsWatch.BL.Core.Models;

using System.Collections.Generic;

namespace DevOpsWatch.BL.Core.Interfaces
{
    /// <summary>
    /// The dev ops source.
    /// </summary>
    public interface IDevOpsSource
    {
        /// <summary>
        /// Gets the active tasks assigned to.
        /// </summary>
        /// <returns>A list of TaskDtos.</returns>
        IEnumerable<TaskDto> GetActiveTasksAssignedTo();

        /// <summary>
        /// Gets the efforts updated today.
        /// </summary>
        /// <returns>A list of TaskDtos.</returns>
        IEnumerable<TaskDto> GetEffortsUpdatedToday();

        /// <summary>
        /// Updates the efforts.
        /// </summary>
        /// <param name="todaysEffortUpdates">The todays effort updates.</param>
        void UpdateEfforts(IEnumerable<TaskDto> todaysEffortUpdates);
    }
}
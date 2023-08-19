namespace DevOpsWatch.BL.Core.Models
{
    /// <summary>
    /// The task dto.
    /// </summary>
    public sealed class TaskDto
    {
        /// <summary>
        /// Gets or sets the task id.
        /// </summary>
        public int TaskId { get; set; }
        /// <summary>
        /// Gets or sets the task name.
        /// </summary>
        public string TaskName { get; set; } = "-";
        /// <summary>
        /// Gets or sets the completed work.
        /// </summary>
        public double CompletedWork { get; set; }
        /// <summary>
        /// Gets or sets the total work.
        /// </summary>
        public double TotalWork { get; set; }
        /// <summary>
        /// Gets or sets the remaining work.
        /// </summary>
        public double RemainingWork { get; set; } = -1;
        public string State { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskDto"/> class.
        /// </summary>
        public TaskDto()
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskDto"/> class.
        /// </summary>
        /// <param name="taskId">The task id.</param>
        /// <param name="taskName">The task name.</param>
        /// <param name="completedWork">The completed work.</param>
        /// <param name="remainingWork">The remaining work.</param>
        public TaskDto(int taskId, string taskName, double completedWork, double remainingWork)
        {
            TaskId = taskId;
            TaskName = taskName;
            CompletedWork = completedWork;
            RemainingWork = remainingWork;
        }
    }
}

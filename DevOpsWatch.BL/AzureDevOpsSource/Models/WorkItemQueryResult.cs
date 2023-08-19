using System.Collections.Generic;

namespace DevOpsWatch.BL.AzureDevOpsSource.Models
{
    public sealed class WorkItemQueryResult
    {
        public IEnumerable<WorkItemReference> WorkItems { get; set; }
    }

    public sealed class WorkItemReference
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string State { get; set; }
    }
}

using Newtonsoft.Json;

using System;
using System.Collections.Generic;

namespace DevOpsWatch.BL.AzureDevOpsSource.Models
{
    public sealed class WorkItemUpdateRoot
    {
        public int Count { get; set; }
        public IEnumerable<WorkItemUpdate> Value { get; set; }
    }


    public sealed class WorkItemUpdate
    {
        public DateTimeOffset RevisedDate { get; set; }

        public UpdateFieldModel Fields { get; set; } 
    }

    public sealed class UpdateFieldModel
    {
        [JsonProperty("Microsoft.VSTS.Scheduling.CompletedWork")]
        public ValueModel MicrosoftVSTSSchedulingCompletedWork { get; set; }

        [JsonProperty("Microsoft.VSTS.Scheduling.RemainingWork")]
        public ValueModel MicrosoftVSTSSchedulingRemainingWork { get; set; }

        [JsonProperty("System.ChangedDate")]
        public ValueModel SystemChangedDate { get; set; }
    }

    public sealed class ValueModel
    {
        public string NewValue { get; set; }
        public string OldValue { get; set; }
    }
}

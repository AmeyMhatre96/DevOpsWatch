using Newtonsoft.Json;

using System;

namespace DevOpsWatch.BL.AzureDevOpsSource.Models
{
    public sealed class WorkItemModel
    {
        public int Id { get; set; }
        public int Rev { get; set; }
        public FieldsModel Fields { get; set; }
        public string Url { get; set; }
    }

    public sealed class FieldsModel
    {
        public string SystemAreaPath { get; set; }
        public string SystemTeamProject { get; set; }
        public string SystemIterationPath { get; set; }
        public string SystemWorkItemType { get; set; }
        [JsonProperty("System.State")]
        public string SystemState { get; set; }
        public string SystemReason { get; set; }
        public AssignedToModel SystemAssignedTo { get; set; }
        public string SystemCreatedDate { get; set; }
        public CreatedByModel SystemCreatedBy { get; set; }

        [JsonProperty("System.ChangedDate")]
        internal DateTime SystemChangedDate { get; set; }
        public ChangedByModel SystemChangedBy { get; set; }
        public int SystemCommentCount { get; set; }
        [JsonProperty("System.Title")]
        public string SystemTitle { get; set; }

        [JsonProperty("Microsoft.VSTS.Scheduling.OriginalEstimate")]
        public double MicrosoftVstsSchedulingOriginalEstimate { get; set; }

        [JsonProperty("Microsoft.VSTS.Scheduling.CompletedWork")]
        public double MicrosoftVstsSchedulingCompletedWork { get; set; }

        [JsonProperty("Microsoft.VSTS.Scheduling.RemainingWork")]
        public double MicrosoftVstsSchedulingRemainingWork { get; set; }

        internal DateTime MicrosoftVstsCommonStateChangeDate { get; set; }
        public string MicrosoftVstsCommonActivatedDate { get; set; }
        public ActivatedByModel MicrosoftVstsCommonActivatedBy { get; set; }
        public string MicrosoftVstsCommonClosedDate { get; set; }
        public int MicrosoftVstsCommonPriority { get; set; }
    }

    public sealed class AssignedToModel
    {
        public string DisplayName { get; set; }
        public string Url { get; set; }
        public string Id { get; set; }
        public string UniqueName { get; set; }
        public string ImageUrl { get; set; }
        public string Descriptor { get; set; }
    }

    public sealed class CreatedByModel
    {
        public string DisplayName { get; set; }
        public string Url { get; set; }
        public string Id { get; set; }
        public string UniqueName { get; set; }
        public string ImageUrl { get; set; }
        public string Descriptor { get; set; }
    }

    public sealed class ChangedByModel
    {
        public string DisplayName { get; set; }
        public string Url { get; set; }
        public string Id { get; set; }
        public string UniqueName { get; set; }
        public string ImageUrl { get; set; }
        public string Descriptor { get; set; }
    }

    public sealed class ActivatedByModel
    {
        public string DisplayName { get; set; }
        public string Url { get; set; }
        public string Id { get; set; }
        public string UniqueName { get; set; }
        public string ImageUrl { get; set; }
        public string Descriptor { get; set; }
    }
}
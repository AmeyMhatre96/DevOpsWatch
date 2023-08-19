namespace DevOpsWatch.BL.AzureDevOpsSource.Models
{
    internal class PatchOperation
    {
        public string op { get; set; }
        public string path { get; set; }
        public string value { get; set; }
    }
}

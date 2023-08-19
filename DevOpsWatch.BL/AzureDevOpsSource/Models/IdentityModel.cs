namespace DevOpsWatch.BL.AzureDevOpsSource.Models
{
    internal class UserModel
    {
        public string DisplayName { get; set; }
        public string PublicAlias { get; set; }
        public string EmailAddress { get; set; }
        public long CoreRevision { get; set; }
        public string Id { get; set; }
        public long Revision { get; set; }
    }

}

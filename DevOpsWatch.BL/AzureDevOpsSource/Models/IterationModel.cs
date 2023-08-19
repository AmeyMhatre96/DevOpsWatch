using System;
using System.Collections.Generic;

namespace DevOpsWatch.BL.AzureDevOpsSource.Models
{
    internal class IterationModel
    {
        public int count { get; set; }

        public List<Iteration> value { get; set; }
    }

    internal class Iteration
    {
        public string id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public IterationAttributes attributes { get; set; }
        public string url { get; set; }
    }

    internal class IterationAttributes
    {
        public DateTime? startDate { get; set; }
        public DateTime? finishDate { get; set; }
        public string timeFrame { get; set; }
    }
}

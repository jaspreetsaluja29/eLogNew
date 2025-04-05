    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

namespace eLog.Models.ORB1
{
    public class CodeFModel
    {
        public int UserId { get; set; }
        public DateTime EntryDate { get; set; }
        public TimeSpan? TimeFailure { get; set; }
        public TimeSpan? TimeOperational { get; set; }
        public string ReasonFailure { get; set; }
    }
}

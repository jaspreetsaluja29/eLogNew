namespace eLog.ViewModels.ORB1
{
    public class CodeFViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime EntryDate { get; set; }
        public TimeSpan? TimeFailure { get; set; }
        public TimeSpan? TimeOperational { get; set; }
        public string ReasonFailure { get; set; }
        public string StatusName { get; set; }
        public string ApprovedBy { get; set; }
        public string Comments { get; set; }
    }
}
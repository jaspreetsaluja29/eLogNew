namespace eLog.ViewModels.ORB1
{
    public class CodeEViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime EntryDate { get; set; }
        public string AutomaticDischargeType { get; set; }
        public string OverboardPositionShipStart { get; set; }
        public TimeSpan? OverboardTimeSwitching { get; set; }
        public TimeSpan? TransferTimeSwitching { get; set; }
        public string TransferTankfrom { get; set; }
        public string TransferTankTo { get; set; }
        public TimeSpan? TimeBackToManual { get; set; }
        public string StatusName { get; set; }
        public string ApprovedBy { get; set; }
        public string Comments { get; set; }
    }
}